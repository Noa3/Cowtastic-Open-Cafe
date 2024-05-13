using UnityEngine;

/// <summary>
/// This Script sits on a object and trigger a new dialog
/// </summary>

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;

    public void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }

}
