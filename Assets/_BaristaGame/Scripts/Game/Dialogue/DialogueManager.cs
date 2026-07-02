/// <summary>
/// Manages dialogue interactions between customers and barista in the Unity cafe game.
/// Handles text display, audio playback, flustered levels, and tooltip generation.
/// Optimized for Unity 6.1 with Burst compilation where applicable.
/// </summary>
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text;
using System.Threading.Tasks;
using _BaristaGame.Scripts.AddressablesScripts;
using Unity.Burst;
using System.Linq; // Added for dynamic dialogue construction
using System.Text.RegularExpressions; // Added for sentence normalization

public class DialogueManager : MonoBehaviour
{
    // --- Variation Phrase Pools (Customer) ---
    private static readonly string[] VAR_Mostly = { "mostly", "mainly", "primarily", "largely" };
    private static readonly string[] VAR_MostlyWithSome = { "mostly {0} with some {1}", "mainly {0} with a bit of {1}", "primarily {0} plus some {1}", "largely {0} with some {1}" };
    private static readonly string[] VAR_MostlyWithSomeAndTouch = { "mostly {0} with some {1} and a touch of {2}", "mainly {0} with a little {1} and just a hint of {2}", "primarily {0} plus some {1} and a dash of {2}", "largely {0} with some {1} and a trace of {2}" };
    private static readonly string[] VAR_Hint = { "with a hint of", "with just a hint of", "with a touch of", "with a trace of" };
    private static readonly string[] VAR_HalfHalf = { "a half-and-half mix of", "an even split between", "a balanced mix of", "an even blend of" };
    private static readonly string[] VAR_Blend = { "a blend of", "a mixture of", "a mix of", "a combo of" };
    private static readonly string[] VAR_MixedDrinkWith = { "a mixed drink with", "something mixed with", "a drink that combines", "a concoction featuring" };
    private static readonly string[] VAR_SomeWith = { "some {0} with {1}", "{0} with some {1}", "{0} and a bit of {1}", "{0} plus some {1}" };
    private static readonly string[] VAR_IdLike = { "I'd like {0}", "Can I get {0}", "I'll take {0}", "Let me have {0}" };
    private static readonly string[] VAR_Simple = { "I'd like something simple", "just something simple", "keep it simple", "something plain" };
    private static readonly string[] VAR_NoExtras = { "No extras please.", "Nothing extra, thanks.", "No toppings or extras.", "That's all, no extras.", "Skip the extras." };
    private static readonly string[] VAR_ComplexThanks = { "Thanks – I know that's quite specific!", "Thanks – kind of a detailed order!", "Appreciate it – that's a lot, I know!", "Thanks, I realize that's a picky order!" };

    // Descriptor adjective variation pools
    private static readonly string[] VAR_StrongLow = { "strong", "bold", "robust", "punchy" };
    private static readonly string[] VAR_StrongHigh = { "extra strong", "very bold", "super strong", "intense" };
    private static readonly string[] VAR_CreamLow = { "creamy", "smooth", "silky", "velvety" };
    private static readonly string[] VAR_CreamHigh = { "very creamy", "extra silky", "super smooth", "rich and creamy" };
    private static readonly string[] VAR_SweetLow = { "lightly sweet", "mildly sweet", "just a little sweet", "slightly sweet" };
    private static readonly string[] VAR_SweetHigh = { "sweet", "nice and sweet", "pretty sweet", "fairly sweet" };
    private static readonly string[] VAR_ChocolateLow = { "chocolatey", "with chocolate notes", "a bit chocolatey", "lightly chocolatey" };
    private static readonly string[] VAR_ChocolateHigh = { "richly chocolatey", "very chocolatey", "deep chocolate flavor", "boldly chocolatey" };

    // Helper to pick random variation
    private static string Pick(string[] pool)
    {
        if (pool == null || pool.Length == 0) return string.Empty;
        return pool[UnityEngine.Random.Range(0, pool.Length)];
    }

    [Header("References Customer")]
    public GameObject DialogBox;
    public GameObject DialogBoxVisibilityButton;
    public GameObject ButtonContionueText;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    public DialogOrderCup OrderCup;
    public TooltipTrigger TooltipCup;

    public TooltipTrigger TooltipFlustered;
    public Image CharacterBackgroundImage;

    [Header("References Barista")]
    public GameObject DialogBoxBarista;
    public TextMeshProUGUI dialogueTextBarista;

    public SoundEffectVariation SoundVariationBaristaDialoge;

    [Header("Settings")]
    [Range(0.01f, 0.5f)]
    public float TalkSpeedCustomer = 0.1f;
    [Range(0.01f, 0.5f)]
    public float TalkSpeedBarista = 0.1f;

    [Header("FlusteredFaces")]
    [Range(-1, 4)]
    public int FlusteredLevel = -1;

    public GameObject FlusteredImageHolder;
    public GameObject FlusteredNormal;
    public GameObject FlusteredLevel1;
    public GameObject FlusteredLevel2;
    public GameObject FlusteredLevel3;
    public GameObject FlusteredLevel4;

    public Color FlusteredColorNormal = Color.white; // Level <= 0
    public Color FlusteredColorLevel1 = Color.white; // Level 1
    public Color FlusteredColorLevel2 = Color.white; // Level 2
    public Color FlusteredColorLevel3 = Color.white; // Level 3
    public Color FlusteredColorLevel4 = Color.white; // Level >= 4

    [Header("Etc.")]
    public bool ShowDialogeBox = true;
    [ReadOnly]
    public bool DialogNeedToBeClosedExtern = false;

    public Queue<string> Sentences = new Queue<string>();

    public static DialogueManager instance;

    private Image FlusteredNormalImage; // 0
    private Image FlusteredLevel1Iamge; // 1
    private Image FlusteredLevel2Iamge; // 2
    private Image FlusteredLevel3Iamge; // 3
    private Image FlusteredLevel4Iamge; // 4

    private BaristaController baristaController;

    private Coroutine LastDialogCorutine;
    private Coroutine LastBaristaDialogCorutine;

    // Recent sentence memory to avoid repetition (normalized) for next 2 sentences
    private readonly Queue<string> recentCustomerSentences = new Queue<string>(2);
    private readonly Queue<string> recentBaristaSentences = new Queue<string>(2);

    void Awake()
    {
        InitializeSingleton();
        InitializeComponents();
        InitializeDialogSettings();
    }

    private void Start()
    {
        InitializeDependencies();
    }

    private void InitializeSingleton()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Statics.LogWarningSafe("Multiple DialogueManager instances detected. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }
    }

    private void InitializeComponents()
    {
        // Cache flustered face Image components safely
        FlusteredNormalImage = Statics.GetComponentSafe<Image>(FlusteredNormal);
        FlusteredLevel1Iamge = Statics.GetComponentSafe<Image>(FlusteredLevel1);
        FlusteredLevel2Iamge = Statics.GetComponentSafe<Image>(FlusteredLevel2);
        FlusteredLevel3Iamge = Statics.GetComponentSafe<Image>(FlusteredLevel3);
        FlusteredLevel4Iamge = Statics.GetComponentSafe<Image>(FlusteredLevel4);
    }

    private void InitializeDialogSettings()
    {
        SetDialogBoxVisibility(false);
        SetDialogBoxBaristaVisibility(false);

        if (DialogBoxVisibilityButton != null)
        {
            DialogBoxVisibilityButton.SetActive(false);
        }
    }

    private void InitializeDependencies()
    {
        baristaController = Statics.FindObjectOfTypeSafe<BaristaController>();
        if (baristaController == null)
        {
            Statics.LogErrorSafe("BaristaController instance not found in scene");
        }
    }

    #region Customer Dialogue

    [BurstCompile]
    public void StartDialogue(Customers customers, List<float> OrderValues, bool needToBeClosedExternal = true)
    {
        if (!ValidateCustomerInput(customers, OrderValues)) return;

        SetupOrderDisplay(OrderValues);
        ConfigureFlusteredState(customers, Consts.DialogueSystem.DefaultFlusteredLevel);
        var orderDialogue = GetOrderDialogue(customers, OrderValues, customers.DialogeTextForOrder);
        StartDialogue(orderDialogue, needToBeClosedExternal);
    }

    [BurstCompile]
    public void StartDialogue(Customers customers, List<float> OrderValues, int FlusteredLevel = 0, bool needToBeClosedExternal = true)
    {
        if (!ValidateCustomerInput(customers, OrderValues)) return;

        SetupOrderDisplay(OrderValues);
        SetFlusteredLevel(customers, FlusteredLevel);

        if (TooltipFlustered != null)
        {
            TooltipFlustered.isActive = true;
        }

        var orderDialogue = GetOrderDialogue(customers, OrderValues, customers.DialogeTextForOrder);
        StartDialogue(orderDialogue, needToBeClosedExternal);
    }

    /// <summary>
    /// Returns either the predefined dialogue or generates a dynamic one from order values.
    /// If predefined dialogue has only one short sentence, we prefer dynamic generation.
    /// </summary>
    private Dialogue GetOrderDialogue(Customers customer, List<float> orderValues, Dialogue predefined)
    {
        bool predefinedValid = predefined != null && !predefined.GetIsDialougeNullOrEmpty();
        if (predefinedValid)
        {
            // Heuristic: if only one short sentence -> auto generate full order description.
            if (predefined.sentences.Length == 1 && predefined.sentences[0].Length < 25)
            {
                return BuildDynamicOrderDialogue(customer, orderValues);
            }
            return predefined;
        }
        return BuildDynamicOrderDialogue(customer, orderValues);
    }

    /// <summary>
    /// Dynamically builds order dialogue sentences based on all ingredient values.
    /// Considers base liquids, modifiers (sugar/cream), binary extras and toppings, producing a natural multi-part order.
    /// </summary>
    private Dialogue BuildDynamicOrderDialogue(Customers customer, List<float> orderValues)
    {
        var threshold = Consts.DialogueSystem.MinimumIngredientThreshold;
        var sentences = new List<string>();

        string greeting = "Hi";
        if (!Statics.IsArrayNullOrEmpty(Statics.CustomerDialogStartGreetings) &&
            Statics.CustomerDialogStartGreetings[0] != "None")
        {
            greeting = Statics.GetRandomFromArray(Statics.CustomerDialogStartGreetings);
        }

        // Percentage based core (liquids / bases)
        var baseIndices = new (int idx, string name, IngredientCategory cat)[] {
            (Consts.DialogueSystem.IngredientIndices.Milk, SanitizeName(Statics.Milk), IngredientCategory.Base),
            (Consts.DialogueSystem.IngredientIndices.BreastMilk, SanitizeName(Statics.BreastMilk), IngredientCategory.Base),
            (Consts.DialogueSystem.IngredientIndices.Coffee, SanitizeName(Statics.Coffee), IngredientCategory.Base),
            (Consts.DialogueSystem.IngredientIndices.Espresso, SanitizeName(Statics.Espresso), IngredientCategory.Strength),
            (Consts.DialogueSystem.IngredientIndices.Tea, SanitizeName(Statics.Tea), IngredientCategory.Base),
            (Consts.DialogueSystem.IngredientIndices.Chocolate, SanitizeName(Statics.Chocolate), IngredientCategory.Flavor),
            (Consts.DialogueSystem.IngredientIndices.Cream, SanitizeName(Statics.Cream), IngredientCategory.Texture),
            (Consts.DialogueSystem.IngredientIndices.Sugar, SanitizeName(Statics.Sugar), IngredientCategory.Sweetener)
        };

        var portions = baseIndices
            .Select(b => new IngredientPortion(b.name, orderValues[b.idx], b.cat))
            .Where(p => p.Name != null && p.Value > threshold)
            .ToList();

        float total = portions.Sum(p => p.Value);
        if (total > 0)
        {
            foreach (var p in portions) p.Normalized = (p.Value / total) * 100f;
        }

        // Build primary composition sentence
        sentences.Add(GenerateCompositionSentence(greeting, portions));

        // Derive descriptor sentence (strength / sweetness / texture) if applicable
        string descriptor = BuildDescriptorSentence(portions);

        // Extras (binary, but still have float value 0/1) - treat as structural or visual modifiers
        bool hasBoba = orderValues[Consts.DialogueSystem.IngredientIndices.Boba] > threshold;
        bool hasIce = orderValues[Consts.DialogueSystem.IngredientIndices.Ice] > threshold;

        // Toppings
        bool toppingCream = orderValues[Consts.DialogueSystem.IngredientIndices.WhippedCream] > threshold;
        bool toppingCaramel = orderValues[Consts.DialogueSystem.IngredientIndices.CaramelSauce] > threshold;
        bool toppingChocoSauce = orderValues[Consts.DialogueSystem.IngredientIndices.ChocolateSauce] > threshold;
        bool toppingSprinkles = orderValues[Consts.DialogueSystem.IngredientIndices.Sprinkles] > threshold;

        var toppingList = new List<string>();
        if (toppingCream) toppingList.Add(SanitizeName(Statics.WhippedCream));
        if (toppingCaramel) toppingList.Add(SanitizeName(Statics.CaramelSauce));
        if (toppingChocoSauce) toppingList.Add(SanitizeName(Statics.ChocolateSauce));
        if (toppingSprinkles) toppingList.Add(SanitizeName(Statics.Sprinkles));

        // Compose extras & toppings phrase
        var extraFragments = new List<string>();
        if (hasIce) extraFragments.Add("iced");
        if (hasBoba) extraFragments.Add("with boba pearls");

        if (toppingList.Count > 0)
        {
            // Group sauces vs others for nicer phrasing
            var sauces = toppingList.Where(t => t != null && (t.Contains("Caramel") || t.Contains("Chocolate") || t.Contains("Sauce"))).ToList();
            var nonSauces = toppingList.Except(sauces).ToList();
            if (nonSauces.Count > 0)
            {
                extraFragments.Add("topped with " + JoinList(nonSauces));
            }
            if (sauces.Count > 0)
            {
                extraFragments.Add((nonSauces.Count == 0 ? "with " : "and ") + JoinList(sauces.Select(s => s.Replace(" Sauce", " drizzle")).ToList()));
            }
        }

        if (!string.IsNullOrEmpty(descriptor))
        {
            extraFragments.Insert(0, descriptor); // descriptor first
        }

        if (extraFragments.Count > 0)
        {
            sentences.Add(CapitalizeFirst(string.Join(" ", extraFragments)).TrimEnd('.') + ".");
        }
        // Replace the block that adds fallback "No extras please." with variation
        // (Find marker comment above when editing)

        // Closing optional sentence if order is complex
        int complexity = portions.Count + (hasBoba?1:0) + (hasIce?1:0) + toppingList.Count;
        if (complexity >= 6)
        {
            sentences.Add(Pick(VAR_ComplexThanks));
        }

        var dialogue = new Dialogue(sentences.ToArray()) { name = customer != null ? customer.name : string.Empty };
        return dialogue;
    }

    /// <summary>
    /// Generate composition phrase focusing on major proportions.
    /// </summary>
    private string GenerateCompositionSentence(string greeting, List<IngredientPortion> portions)
    {
        if (portions == null || portions.Count == 0)
            return $"{greeting}, {Pick(VAR_Simple)}.";

        var ordered = portions.OrderByDescending(p => p.Normalized).ToList();
        var top = ordered[0];
        IngredientPortion second = ordered.Count > 1 ? ordered[1] : null;
        IngredientPortion third = ordered.Count > 2 ? ordered[2] : null;

        if (top.Normalized >= 70f)
        {
            if (second != null && second.Normalized >= 12f)
                return $"{greeting}, {Pick(VAR_Mostly)} {top.Name} {Pick(VAR_Hint)} {second.Name}.";
            return $"{greeting}, {Pick(VAR_Mostly)} {top.Name} please.";
        }

        if (second != null && Math.Abs(top.Normalized - second.Normalized) < 15f && top.Normalized >= 35f && second.Normalized >= 30f)
        {
            return $"{greeting}, {Pick(VAR_HalfHalf)} {top.Name} and {second.Name}.";
        }

        if (second != null && top.Normalized >= 55f && second.Normalized >= 18f)
        {
            if (third != null && third.Normalized >= 12f)
                return $"{greeting}, {string.Format(Pick(VAR_MostlyWithSomeAndTouch), top.Name, second.Name, third.Name)}.";
            return $"{greeting}, {string.Format(Pick(VAR_MostlyWithSome), top.Name, second.Name)}.";
        }

        if (third != null && top.Normalized >= 34f && third.Normalized >= 18f)
        {
            return $"{greeting}, {Pick(VAR_Blend)} {top.Name}, {second.Name} and {third.Name}.";
        }

        if (ordered.Count >= 4)
        {
            var names = ordered.Take(4).Select(p => p.Name).ToList();
            return $"{greeting}, {Pick(VAR_MixedDrinkWith)} {JoinList(names)}.";
        }

        if (ordered.Count == 3)
            return $"{greeting}, {Pick(VAR_Blend)} {top.Name}, {second.Name} and {third.Name}.";
        if (ordered.Count == 2)
            return $"{greeting}, {string.Format(Pick(VAR_SomeWith), top.Name, second.Name)}.";

        return $"{greeting}, {string.Format(Pick(VAR_IdLike), top.Name)}.";
    }

    /// <summary>
    /// Builds a descriptor sentence capturing sweetness, creaminess, strength, chocolate notes.
    /// </summary>
    private string BuildDescriptorSentence(List<IngredientPortion> portions)
    {
        if (portions == null || portions.Count == 0) return string.Empty;

        var sweet = portions.FirstOrDefault(p => p.Category == IngredientCategory.Sweetener)?.Normalized ?? 0f;
        var cream = portions.FirstOrDefault(p => p.Category == IngredientCategory.Texture)?.Normalized ?? 0f;
        var strong = portions.FirstOrDefault(p => p.Category == IngredientCategory.Strength)?.Normalized ?? 0f;
        var chocolate = portions.FirstOrDefault(p => p.Category == IngredientCategory.Flavor)?.Normalized ?? 0f;

        var words = new List<string>();
        if (strong >= 20f) words.Add(strong >= 40f ? Pick(VAR_StrongHigh) : Pick(VAR_StrongLow));
        if (cream >= 15f) words.Add(cream >= 30f ? Pick(VAR_CreamHigh) : Pick(VAR_CreamLow));
        if (sweet >= 12f) words.Add(sweet >= 25f ? Pick(VAR_SweetHigh) : Pick(VAR_SweetLow));
        if (chocolate >= 18f) words.Add(chocolate >= 35f ? Pick(VAR_ChocolateHigh) : Pick(VAR_ChocolateLow));

        if (words.Count == 0) return string.Empty;
        return string.Join(", ", words.Select(CapitalizeFirst));
    }

    /// <summary>
    /// Joins a list with commas and an 'and' before the last element.
    /// </summary>
    private string JoinList(IList<string> items)
    {
        if (items == null || items.Count == 0) return string.Empty;
        if (items.Count == 1) return items[0];
        if (items.Count == 2) return $"{items[0]} and {items[1]}";
        return string.Join(", ", items.Take(items.Count - 1)) + " and " + items.Last();
    }

    private static string SanitizeName(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw) || raw == "None") return null;
        return raw.Trim();
    }

    private static string CapitalizeFirst(string txt)
    {
        if (string.IsNullOrEmpty(txt)) return txt;
        if (char.IsUpper(txt[0])) return txt;
        return char.ToUpperInvariant(txt[0]) + txt.Substring(1);
    }

    private class IngredientPortion
    {
        public string Name;
        public float Value;     // Raw absolute value
        public float Normalized; // Relative to sum of considered bases
        public IngredientCategory Category;
        public IngredientPortion(string name, float value, IngredientCategory cat)
        {
            Name = name;
            Value = value;
            Category = cat;
        }
    }

    private enum IngredientCategory
    {
        Base,
        Strength,
        Flavor,
        Sweetener,
        Texture
    }

    [BurstCompile]
    private bool ValidateCustomerInput(Customers customers, List<float> OrderValues)
    {
        if (customers == null)
        {
            Statics.LogErrorSafe("Customer parameter is null");
            return false;
        }

        if (Statics.IsListNullOrEmpty(OrderValues))
        {
            Statics.LogErrorSafe("OrderValues list is null or empty");
            return false;
        }

        if (OrderValues.Count != Consts.DialogueSystem.ExpectedOrderValuesCount)
        {
            Debug.LogWarning($"OrderValues count mismatch. Expected: {Consts.DialogueSystem.ExpectedOrderValuesCount}, Actual: {OrderValues.Count}");
        }

        return true;
    }

    [BurstCompile]
    private void SetupOrderDisplay(List<float> OrderValues)
    {
        if (OrderCup != null)
        {
            OrderCup.gameObject.SetActive(true);
            OrderCup.SetValues(OrderValues);
        }

        if (ButtonContionueText != null)
        {
            ButtonContionueText.SetActive(false);
        }

        if (TooltipCup != null)
        {
            TooltipCup.isActive = true;
            TooltipCup.content = Statics.BuildTooltipTextFromValues(OrderValues);
        }
    }

    [BurstCompile]
    private void ConfigureFlusteredState(Customers customers, int flusteredLevel)
    {
        this.FlusteredLevel = flusteredLevel;
        SetAvatarPicture(flusteredLevel, customers);

        if (TooltipFlustered != null)
        {
            TooltipFlustered.isActive = false;
        }
    }

    [BurstCompile]
    public void SetFlusteredLevel(Customers customers, int FlusteredLevel = -1)
    {
        this.FlusteredLevel = FlusteredLevel;
        SetAvatarPicture(FlusteredLevel, customers);
        UpdateFlusteredTooltip(FlusteredLevel);
    }

    [BurstCompile]
    public void SetFlusteredLevel(Customers customers, int FlusteredLevel, List<float> OrderValues)
    {
        this.FlusteredLevel = FlusteredLevel;

        if (OrderCup != null)
        {
            OrderCup.SetValues(OrderValues);
        }

        SetAvatarPicture(FlusteredLevel, customers);

        if (TooltipCup != null)
        {
            TooltipCup.isActive = true;
            TooltipCup.content = Statics.BuildTooltipTextFromValues(OrderValues);
        }

        UpdateFlusteredTooltip(FlusteredLevel);
    }

    [BurstCompile]
    private void UpdateFlusteredTooltip(int flusteredLevel)
    {
        if (TooltipFlustered == null) return;

        string tooltipHeader = Statics.GetFlusteredTooltipText(flusteredLevel);
        bool shouldShowTooltip = flusteredLevel > 0;

        TooltipFlustered.isActive = shouldShowTooltip;
        TooltipFlustered.header = tooltipHeader;
    }

    [BurstCompile]
    public void StartDialogueSuccess(Customers customers, bool needToBeClosedExternal = true)
    {
        HideOrderDisplay();
        ButtonContionueText?.SetActive(true);

        var dialogueToUse = GetDialogueOrFallback(
            customers.DialogeTextForOrderSuccess,
            () => Statics.CreateFallbackDialogue(customers.name, Statics.CustomerDialogSucces)
        );

        StartDialogue(dialogueToUse, false);
    }

    [BurstCompile]
    public void StartDialogueFail(Customers customers, bool needToBeClosedExternal = true)
    {
        HideOrderDisplay();
        ButtonContionueText?.SetActive(true);

        var dialogueToUse = GetDialogueOrFallback(
            customers.DialogeTextForOrderFail,
            () => Statics.CreateFallbackDialogue(customers.name, Statics.CustomerDialogFailed)
        );

        StartDialogue(dialogueToUse, false);
    }

    [BurstCompile]
    private void HideOrderDisplay()
    {
        if (OrderCup != null)
        {
            OrderCup.gameObject.SetActive(false);
        }

        if (TooltipCup != null)
        {
            TooltipCup.isActive = false;
        }

        if (TooltipFlustered != null)
        {
            TooltipFlustered.isActive = false;
        }
    }

    [BurstCompile]
    private Dialogue GetDialogueOrFallback(Dialogue primaryDialogue, System.Func<Dialogue> fallbackCreator)
    {
        if (primaryDialogue != null && !primaryDialogue.GetIsDialougeNullOrEmpty())
        {
            return primaryDialogue;
        }
        return fallbackCreator();
    }

    [BurstCompile]
    public void StartDialogue(Dialogue dialogue, bool needToBeClosedExternal = false)
    {
        if (dialogue == null)
        {
            Statics.LogErrorSafe("Dialogue parameter is null");
            return;
        }

        DialogNeedToBeClosedExtern = needToBeClosedExternal;

        SetDialogBoxVisibility(true);
        if (DialogBoxVisibilityButton != null)
        {
            DialogBoxVisibilityButton.SetActive(true);
        }

        if (nameText != null)
        {
            nameText.text = dialogue.name ?? string.Empty;
        }

        PopulateSentenceQueue(dialogue.sentences);
        DislayNextSentence();
    }

    [BurstCompile]
    private void PopulateSentenceQueue(string[] sentences)
    {
        Sentences.Clear();

        if (sentences != null)
        {
            // Maintain local recent list to avoid duplicates inside same dialogue batch
            var lastAdded = new Queue<string>(2);
            foreach (string sentence in sentences)
            {
                if (string.IsNullOrWhiteSpace(sentence)) continue;
                string norm = NormalizeSentence(sentence);
                bool isRecentInBatch = lastAdded.Any(s => s == norm);
                bool isRecentGlobal = recentCustomerSentences.Any(s => s == norm);
                if (isRecentInBatch || isRecentGlobal) continue; // skip duplicates within last 2 sentences overall
                Sentences.Enqueue(sentence);
                lastAdded.Enqueue(norm);
                if (lastAdded.Count > 2) lastAdded.Dequeue();
            }
        }
    }

    [BurstCompile]
    public void DislayNextSentence()
    {
        while (Sentences.Count > 0)
        {
            string sentence = Sentences.Dequeue();
            string norm = NormalizeSentence(sentence);
            if (recentCustomerSentences.Any(s => s == norm))
            {
                // Skip and continue to next sentence
                continue;
            }

            StopPreviousDialogCoroutine();
            LastDialogCorutine = StartCoroutine(TypeSentence(sentence, norm));
            return;
        }

        // No sentences left
        if (Sentences.Count == 0)
        {
            if (DialogNeedToBeClosedExtern)
            {
                return;
            }
            else
            {
                EndDialogue();
                return;
            }
        }
    }

    [BurstCompile]
    private void StopPreviousDialogCoroutine()
    {
        if (LastDialogCorutine != null)
        {
            StopCoroutine(LastDialogCorutine);
            LastDialogCorutine = null;
        }
    }

    IEnumerator TypeSentence(string sentence, string normalizedCache)
    {
        if (dialogueText == null) yield break;

        dialogueText.text = string.Empty;

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(TalkSpeedCustomer);
        }

        RememberRecent(recentCustomerSentences, normalizedCache);
    }

    // Backwards compatibility overload (if any other code still calls original signature)
    IEnumerator TypeSentence(string sentence)
    {
        string norm = NormalizeSentence(sentence);
        yield return TypeSentence(sentence, norm);
    }

    [BurstCompile]
    public void EndDialogue()
    {
        SetDialogBoxVisibility(false);
        if (DialogBoxVisibilityButton != null)
        {
            DialogBoxVisibilityButton.SetActive(false);
        }
    }

    [BurstCompile]
    public void SwitchDialogBoxVisibility()
    {
        ShowDialogeBox = !ShowDialogeBox;
        if (DialogBox != null)
        {
            DialogBox.SetActive(ShowDialogeBox);
        }
    }

    [BurstCompile]
    public void SetDialogBoxVisibility(bool visible)
    {
        ShowDialogeBox = visible;
        if (DialogBox != null)
        {
            DialogBox.SetActive(visible);
        }
    }

    [BurstCompile]
    private void SetAvatarPicture(int flusteredLevel, Customers customer)
    {
        DeactivateAllFlusteredImages();

        if (FlusteredImageHolder != null)
        {
            FlusteredImageHolder.SetActive(true);
        }

        var (activeImage, targetSprite, backgroundColor) = GetFlusteredLevelAssets(flusteredLevel, customer);

        if (activeImage != null && targetSprite != null)
        {
            activeImage.sprite = targetSprite;
            activeImage.gameObject.SetActive(true);

            if (CharacterBackgroundImage != null)
            {
                CharacterBackgroundImage.color = backgroundColor;
            }
        }
    }

    [BurstCompile]
    private (Image activeImage, Sprite targetSprite, Color backgroundColor) GetFlusteredLevelAssets(int flusteredLevel, Customers customer)
    {
        if (customer?.Avatar == null) return (null, null, Color.white);

        return flusteredLevel switch
        {
            <= 0 => (FlusteredNormalImage, customer.Avatar.Normal, FlusteredColorNormal),
            1 => (FlusteredLevel1Iamge, customer.Avatar.Level1, FlusteredColorLevel1),
            2 => (FlusteredLevel2Iamge, customer.Avatar.Level2, FlusteredColorLevel2),
            3 => (FlusteredLevel3Iamge, customer.Avatar.Level3, FlusteredColorLevel3),
            >= 4 => (FlusteredLevel4Iamge, customer.Avatar.Level4, FlusteredColorLevel4),
        };
    }

    [BurstCompile]
    public void DeactivateAllFlusteredImages()
    {
        var flusteredObjects = new[] { FlusteredImageHolder, FlusteredNormal, FlusteredLevel1, FlusteredLevel2, FlusteredLevel3, FlusteredLevel4 };

        foreach (var obj in flusteredObjects)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
    }

    #endregion

    #region Barista Dialogue

    /// <summary>
    /// Initiates barista dialogue with optional audio playback.
    /// </summary>
    public async Task StartDialoguebarista(DialogSentence sentence, float stopOffset)
    {
        if (!ValidateBaristaDialogueInput(sentence)) return;

        string norm = NormalizeSentence(sentence.sentence);
        if (recentBaristaSentences.Any(s => s == norm))
        {
            // Prevent repeat; do not display or play audio
            return;
        }

        SetDialogBoxBaristaVisibility(true);
        DislayBaristaSentence(sentence.sentence, stopOffset);

        await PlayAudioBaristaAsync(sentence.AudioName);
    }

    [BurstCompile]
    private bool ValidateBaristaDialogueInput(DialogSentence sentence)
    {
        if (!gameObject.activeSelf)
        {
            Statics.LogWarningSafe("DialogueManager is inactive, cannot start barista dialogue");
            return false;
        }

        if (sentence == null)
        {
            Statics.LogErrorSafe("DialogSentence parameter is null");
            return false;
        }

        return true;
    }

    public void DislayBaristaSentence(string sentence, float stopOffset)
    {
        if (string.IsNullOrEmpty(sentence)) return;

        string norm = NormalizeSentence(sentence);
        if (recentBaristaSentences.Any(s => s == norm))
        {
            return; // skip repeat
        }

        StopPreviousBaristaDialogCoroutine();
        LastBaristaDialogCorutine = StartCoroutine(TypeSentenceBarista(sentence, stopOffset, norm));
    }

    [BurstCompile]
    private void StopPreviousDialogCoroutineBarista() { StopPreviousBaristaDialogCoroutine(); }

    [BurstCompile]
    private void StopPreviousBaristaDialogCoroutine()
    {
        if (LastBaristaDialogCorutine != null)
        {
            StopCoroutine(LastBaristaDialogCorutine);
            LastBaristaDialogCorutine = null;
        }
    }

    IEnumerator TypeSentenceBarista(string sentence, float stopOffset, string normalizedCache)
    {
        if (dialogueTextBarista == null || baristaController == null) yield break;

        dialogueTextBarista.text = string.Empty;
        baristaController.Talking = true;

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueTextBarista.text += letter;
            yield return new WaitForSeconds(TalkSpeedBarista);
        }

        baristaController.Talking = false;
        RememberRecent(recentBaristaSentences, normalizedCache);
        StartCoroutine(StoptBaristaTalk(stopOffset));
    }

    // Backwards compatibility old signature
    IEnumerator TypeSentenceBarista(string sentence, float stopOffset)
    {
        string norm = NormalizeSentence(sentence);
        yield return TypeSentenceBarista(sentence, stopOffset, norm);
    }

    private IEnumerator StoptBaristaTalk(float stopOffset)
    {
        yield return new WaitForSeconds(Mathf.Max(0, stopOffset));
        EndDialogueBarista();
    }

    public void EndDialogueBarista()
    {
        SetDialogBoxBaristaVisibility(false);
    }

    public void SetDialogBoxBaristaVisibility(bool visible)
    {
        if (DialogBoxBarista != null)
        {
            DialogBoxBarista.SetActive(visible);
        }
    }

    public LocalAudioLoader LocalAudioLoader;

    private async Task PlayAudioBaristaAsync(string audioName)
    {
        if (SoundVariationBaristaDialoge == null || string.IsNullOrWhiteSpace(audioName))
        {
            return;
        }

        try
        {
            LocalAudioLoader = new LocalAudioLoader();
            List<AudioClip> audioClips = await GetAddressablesAudioClipsAsync(audioName);

            if (!Statics.IsListNullOrEmpty(audioClips))
            {
                audioClips.Shuffle();
                SoundVariationBaristaDialoge.PlayRandomOneShot(audioClips[0]);
            }
        }
        catch (Exception ex)
        {
            Statics.LogErrorSafe($"Failed to play barista audio '{audioName}': {ex.Message}");
        }
    }

    private async Task<List<AudioClip>> GetAddressablesAudioClipsAsync(string generalAudioName)
    {
        List<AudioClip> audioClips = new List<AudioClip>();
        bool lookForFiles = true;
        byte currentFile = 0;

        while (lookForFiles && currentFile < Consts.DialogueSystem.MaxAudioClipSearchAttempts)
        {
            currentFile++;
            string audioName = $"{generalAudioName}_{currentFile:0}";

            try
            {
                AudioClip clip = await GetAddressablesAudioClip(audioName);
                if (clip != null)
                {
                    audioClips.Add(clip);
                    LocalAudioLoader?.UnloadAudio();
                }
                else
                {
                    lookForFiles = false;
                }
            }
            catch (Exception ex)
            {
                Statics.LogWarningSafe($"Failed to load audio clip '{audioName}': {ex.Message}");
                lookForFiles = false;
            }
        }

        return audioClips;
    }

    private async Task<AudioClip> GetAddressablesAudioClip(string audioName)
    {
        return await LocalAudioLoader?.LoadAudioAsync(audioName);
    }

    #endregion

    #region Sentence Repeat Helpers

    private static string NormalizeSentence(string sentence)
    {
        if (string.IsNullOrWhiteSpace(sentence)) return string.Empty;
        string lower = sentence.ToLowerInvariant();
        lower = Regex.Replace(lower, "[\\p{P}-[']]+", " "); // Remove punctuation except apostrophes
        lower = Regex.Replace(lower, "\\s+", " ").Trim();
        return lower;
    }

    private static void RememberRecent(Queue<string> queue, string normalized)
    {
        if (string.IsNullOrEmpty(normalized)) return;
        queue.Enqueue(normalized);
        while (queue.Count > 2) // keep last 2
        {
            queue.Dequeue();
        }
    }

    #endregion
}

/// <summary>
/// Represents a dialogue sentence with optional audio playback
/// </summary>
public class DialogSentence
{
    public string sentence;
    public string AudioName;

    public DialogSentence(string sentence, string AudioName)
    {
        this.sentence = sentence ?? string.Empty;
        this.AudioName = AudioName;
    }

    public DialogSentence(string sentence)
    {
        this.sentence = sentence ?? string.Empty;
        this.AudioName = null;
    }
}