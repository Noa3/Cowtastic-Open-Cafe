using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Manages random events during gameplay, including timing, conditions, and effects
/// Optimized for Unity 6.1 with Burst compilation
/// </summary>
public class EventManager : MonoBehaviour
{
        [Header("References")]
        [Tooltip("The game object on which the icons for the events should be spawned")]
        [FormerlySerializedAs("IconHolder")]
        public Transform eventIconContainer;

        [FormerlySerializedAs("PossibileEvents")]
        public EventBase[] availableEvents;

        [Header("Event Activation Conditions")]
        [Tooltip("Minimal time needed to play, until the first event can appear")]
        [FormerlySerializedAs("minTimeForFirstEvent")]
        public float minimumTimeBeforeFirstEvent = 30f;

        [Tooltip("Minimal MaxSize of the Barista to start the events")]
        [Min(0)]
        [FormerlySerializedAs("minMaxBustStartEvents")]
        public float minimumBustSizeForEvents = 20f;

        [Min(0)]
        [FormerlySerializedAs("CompletedCupsNeeded")]
        public int requiredCompletedCups = 10;

        [Header("Event Timing")]
        [Tooltip("Minimum time between events in seconds")]
        [Min(0)]
        [FormerlySerializedAs("MinTime")]
        public float minimumTimeBetweenEvents = 60f;

        [Tooltip("Maximum time between events in seconds")]
        [Min(1)]
        [FormerlySerializedAs("MaxTime")]
        public float maximumTimeBetweenEvents = 120f;

        [Header("Event Limits")]
        [Min(0)]
        [FormerlySerializedAs("MaxEventsOnSameTime")]
        public int maximumConcurrentEvents = 10;

        [Header("Current Event State")]
        [FormerlySerializedAs("CurrentEvents")]
        public List<EventBase> activeEvents = new List<EventBase>();

        [ReadOnly]
        [Tooltip("Multiplier for milk production speed from active events")]
        [FormerlySerializedAs("EventFillSpeedMultipler")]
        public float eventMilkFillSpeedMultiplier = 1f;

        [ReadOnly]
        [Tooltip("Multiplier for sell value from active events")]
        [FormerlySerializedAs("EventSellMoreValueMultipler")]
        public float eventSellValueMultiplier = 1f;

        [ReadOnly]
        [Tooltip("Additional ingredients from active events")]
        [FormerlySerializedAs("moreIngreedients")]
        public int additionalIngredients = 0;

        [Header("Debug")]
        [FormerlySerializedAs("ForceEventStart")]
        public bool forceStartEvent = false;

        [HideInInspector]
        public static EventManager instance;

        // Private cached references
        private OrderManager orderManager;
        private BaseGameMode gameMode;
        private BestTimeManager bestTimeManager;

        // Event timing tracking
        private float timeOfLastEventEnd = 0f;
        private float nextEventTime = 0f;
        private int completedCupsAtLastEvent = 0;

        // Optimized collections for event management
        private readonly List<EventBase> eventsToRemove = new List<EventBase>();
        private readonly Dictionary<System.Type, int> eventTypeCounts = new Dictionary<System.Type, int>();
        private bool isInitialized = false;

        #region Unity Lifecycle

        public void Awake()
        {
            instance = this;
        }

        void Start()
        {
            timeOfLastEventEnd = Time.timeSinceLevelLoad;
            CacheManagerReferences();
            isInitialized = true;
        }

        private void FixedUpdate()
        {
            if (isInitialized)
            {
                ProcessEventLogic();
            }
        }

        void OnDestroy()
        {
            // Clean up any remaining events
            EndAllActiveEvents();
        }

        #endregion

        #region Initialization

        private void CacheManagerReferences()
        {
            orderManager = OrderManager.instance;
            gameMode = BaseGameMode.instance;
            bestTimeManager = BestTimeManager.instance;
        }

        #endregion

        #region Event Processing

        [BurstCompile]
        private void ProcessEventLogic()
        {
            CheckForNewEventTrigger();
            UpdateActiveEventEffects();
        }

        [BurstCompile]
        private void CheckForNewEventTrigger()
        {
            if (!CanTriggerEvents()) return;

            bool timeConditionMet = Time.timeSinceLevelLoad > nextEventTime || forceStartEvent;
            bool cupConditionMet = orderManager.CompletedCups >= (completedCupsAtLastEvent + requiredCompletedCups) || forceStartEvent;

            if (timeConditionMet && cupConditionMet)
            {
                TriggerRandomEvent();
                forceStartEvent = false;
            }
        }

        [BurstCompile]
        private bool CanTriggerEvents()
        {
            return gameMode.CurrentMaxSize >= minimumBustSizeForEvents &&
                   bestTimeManager.PlayTime > minimumTimeBeforeFirstEvent &&
                   activeEvents.Count < maximumConcurrentEvents;
        }

        [BurstCompile]
        private void UpdateActiveEventEffects()
        {
            if (activeEvents.Count == 0)
            {
                ResetEventEffects();
                return;
            }

            // Reset multipliers
            eventSellValueMultiplier = 1f;
            eventMilkFillSpeedMultiplier = 1f;
            additionalIngredients = 0;

            // Calculate combined effects from all active events
            for (int i = 0; i < activeEvents.Count; i++)
            {
                var currentEvent = activeEvents[i];
                if (currentEvent == null) continue;

                eventSellValueMultiplier *= currentEvent.MoreSellValue;
                eventMilkFillSpeedMultiplier *= currentEvent.MilkFillSpeedMultipler;

                // Handle specific event types
                if (currentEvent is Event_MoreIngreedients moreIngredientsEvent)
                {
                    additionalIngredients += moreIngredientsEvent.AdditionalIngreedients;
                }
            }

            // Apply effects to game systems
            ApplyEventEffectsToGameSystems();
        }

        [BurstCompile]
        private void ResetEventEffects()
        {
            eventSellValueMultiplier = 1f;
            eventMilkFillSpeedMultiplier = 1f;
            additionalIngredients = 0;
            ApplyEventEffectsToGameSystems();
        }

        [BurstCompile]
        private void ApplyEventEffectsToGameSystems()
        {
            if (orderManager != null)
            {
                orderManager.EventSellMoreValueMultipler = eventSellValueMultiplier;
                orderManager.ChangedIngreedientCount = additionalIngredients;
            }

            if (gameMode != null)
            {
                gameMode.EventFastMilkFillMultipler = eventMilkFillSpeedMultiplier;
            }
        }

        #endregion

        #region Event Management

        [BurstCompile]
        public void TriggerRandomEvent()
        {
            if (availableEvents == null || availableEvents.Length == 0)
            {
                Debug.LogWarning("No available events to trigger!");
                return;
            }

            EventBase selectedEvent = SelectRandomEvent();
            if (selectedEvent == null) return;

            if (!ValidateEventRequirements(selectedEvent)) return;
            if (!CheckEventConflicts(selectedEvent)) return;

            StartEvent(selectedEvent);
        }

        [BurstCompile]
        private EventBase SelectRandomEvent()
        {
            if (availableEvents.Length == 1)
            {
                return availableEvents[0];
            }

            int randomIndex = Statics.GetRandomRange(0, availableEvents.Length - 1, Statics.EventTypeRNG());
            return availableEvents[randomIndex];
        }

        [BurstCompile]
        private bool ValidateEventRequirements(EventBase eventToStart)
        {
            return eventToStart.MinMaxBust <= gameMode.CurrentMaxSize;
        }

        [BurstCompile]
        private bool CheckEventConflicts(EventBase eventToStart)
        {
            if (activeEvents.Count == 0) return true;

            var eventToStartType = eventToStart.GetType();

            for (int i = 0; i < activeEvents.Count; i++)
            {
                var activeEvent = activeEvents[i];
                if (activeEvent == null || activeEvent.PreventEventsWhileRunning == null) continue;

                for (int j = 0; j < activeEvent.PreventEventsWhileRunning.Length; j++)
                {
                    if (activeEvent.PreventEventsWhileRunning[j] == null) continue;

                    if (eventToStartType == activeEvent.PreventEventsWhileRunning[j].GetType())
                    {
                        return false; // Event conflict detected
                    }
                }
            }

            return true;
        }

        [BurstCompile]
        public EventBase StartEvent(EventBase eventPrefab, bool forceOverride = false)
        {
            if (eventPrefab == null)
            {
                Debug.LogError("Cannot start null event!");
                return null;
            }

            // Handle forced event override
            if (forceOverride)
            {
                RemoveExistingEventOfSameType(eventPrefab.GetType());
            }

            // Instantiate and configure the event
            EventBase newEvent = Instantiate(eventPrefab, eventIconContainer);
            if (newEvent == null)
            {
                Debug.LogError("Failed to instantiate event!");
                return null;
            }

            newEvent.eventManager = this;
            activeEvents.Add(newEvent);

            // Update timing for next event
            ScheduleNextEvent();
            completedCupsAtLastEvent = orderManager.CompletedCups;

            return newEvent;
        }

        [BurstCompile]
        private void RemoveExistingEventOfSameType(System.Type eventType)
        {
            eventsToRemove.Clear();

            // Find events of the same type
            for (int i = 0; i < activeEvents.Count; i++)
            {
                if (activeEvents[i] != null && activeEvents[i].GetType() == eventType)
                {
                    eventsToRemove.Add(activeEvents[i]);
                }
            }

            // Remove found events
            foreach (var eventToRemove in eventsToRemove)
            {
                StopEvent(eventToRemove);
            }
        }

        [BurstCompile]
        public void StopEvent(EventBase eventToStop)
        {
            if (eventToStop == null || !activeEvents.Contains(eventToStop))
            {
                return;
            }

            // Check if there are multiple events of the same type
            int sameTypeCount = CountEventsOfType(eventToStop.GetType());

            // Only disable the event state if this is the last one of its type
            if (sameTypeCount == 1)
            {
                eventToStop.SetEventState(false);
            }

            // Remove from active events and destroy
            activeEvents.Remove(eventToStop);
            if (eventToStop.gameObject != null)
            {
                Destroy(eventToStop.gameObject);
            }
        }

        [BurstCompile]
        private int CountEventsOfType(System.Type eventType)
        {
            int count = 0;
            for (int i = 0; i < activeEvents.Count; i++)
            {
                if (activeEvents[i] != null && activeEvents[i].GetType() == eventType)
                {
                    count++;
                }
            }
            return count;
        }

        [BurstCompile]
        private void ScheduleNextEvent()
        {
            nextEventTime = Time.timeSinceLevelLoad +
                           Statics.GetRandomRange(minimumTimeBetweenEvents, maximumTimeBetweenEvents, Statics.EventGapRNG());
        }

        #endregion

        #region Public API

        /// <summary>
        /// Ends all currently active events immediately
        /// </summary>
        public void EndAllActiveEvents()
        {
            // Create a copy to avoid modification during iteration
            eventsToRemove.Clear();
            eventsToRemove.AddRange(activeEvents);

            foreach (var eventBase in eventsToRemove)
            {
                StopEvent(eventBase);
            }

            eventsToRemove.Clear();
        }

        /// <summary>
        /// Gets the count of active events of a specific type
        /// </summary>
        public int GetActiveEventCount<T>() where T : EventBase
        {
            int count = 0;
            var targetType = typeof(T);

            for (int i = 0; i < activeEvents.Count; i++)
            {
                if (activeEvents[i] != null && activeEvents[i].GetType() == targetType)
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Forces a specific event to start, regardless of conditions
        /// </summary>
        public EventBase ForceStartSpecificEvent<T>() where T : EventBase
        {
            var targetType = typeof(T);
            for (int i = 0; i < availableEvents.Length; i++)
            {
                if (availableEvents[i] != null && availableEvents[i].GetType() == targetType)
                {
                    return StartEvent(availableEvents[i], true);
                }
            }

            Debug.LogWarning($"Event of type {targetType.Name} not found in available events!");
            return null;
        }

        /// <summary>
        /// Checks if a specific event type is currently active
        /// </summary>
        public bool IsEventActive<T>() where T : EventBase
        {
            return GetActiveEventCount<T>() > 0;
        }

        /// <summary>
        /// Gets all active events of a specific type
        /// </summary>
        public List<T> GetActiveEventsOfType<T>() where T : EventBase
        {
            var result = new List<T>();
            for (int i = 0; i < activeEvents.Count; i++)
            {
                if (activeEvents[i] is T eventOfType)
                {
                    result.Add(eventOfType);
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the time remaining until the next random event can trigger
        /// </summary>
        public float GetTimeUntilNextEvent()
        {
            return Mathf.Max(0f, nextEventTime - Time.timeSinceLevelLoad);
        }

        #endregion

        #region Debug Methods

        #region Debug Methods

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying) return;

            // Draw debug information in scene view
            UnityEditor.Handles.Label(transform.position, $"Active Events: {activeEvents.Count}\nNext Event In: {GetTimeUntilNextEvent():F1}s");
        }
#endif

        #endregion

        #endregion
    }
