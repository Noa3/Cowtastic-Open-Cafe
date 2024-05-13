using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text;
using Unity.Burst;

public class DialogueManager : MonoBehaviour
{
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
    [Range(0.01f,0.5f)]
    public float TalkSpeedCustomer = 0.1f;
    [Range(0.01f, 0.5f)]
    public float TalkSpeedBarista = 0.1f;

    [Header("FlusteredFaces")]
    [Range(-1,4)]
    public int FlusteredLevel = -1;

    public GameObject FlusteredImageHolder;
    public GameObject FlusteredNormal;
    public GameObject FlusteredLevel1;
    public GameObject FlusteredLevel2;
    public GameObject FlusteredLevel3;
    public GameObject FlusteredLevel4;

    public Color FlusteredColorNormal = Color.white; // 0
    public Color FlusteredColorLevel1 = Color.white; // 1
    public Color FlusteredColorLevel2 = Color.white; // 2
    public Color FlusteredColorLevel3 = Color.white; // 3
    public Color FlusteredColorLevel4 = Color.white; // 4

    //public List<float> OrderCupValues = new List<float>();


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


    void Awake()
    {
        instance = this;

        SetDialogBoxVisibility(false);
        DialogBoxVisibilityButton.SetActive(false);

        FlusteredNormalImage = FlusteredNormal.GetComponent<Image>(); // 0
        FlusteredLevel1Iamge = FlusteredLevel1.GetComponent<Image>(); // 1
        FlusteredLevel2Iamge = FlusteredLevel2.GetComponent<Image>(); // 2
        FlusteredLevel3Iamge = FlusteredLevel3.GetComponent<Image>(); // 3
        FlusteredLevel4Iamge = FlusteredLevel4.GetComponent<Image>(); // 4

        SetDialogBoxBaristaVisibility(false);
    }

    private void Start()
    {
        baristaController = BaristaController.instance;
    }

    #region Customer

    [BurstCompile]
    public void StartDialogue(Customers customers, List<float> OrderValues, bool needToBeClosedExternal = true)
    {
        OrderCup.gameObject.SetActive(true);
        OrderCup.SetValues(OrderValues);
        ButtonContionueText.SetActive(false);

        if (TooltipCup != null)
        {
            TooltipCup.isActive = true;
            TooltipCup.content = BuildToolTipText(OrderValues);
        }


        FlusteredLevel =  -1;
        SetAvatarPicture(FlusteredLevel,customers);
        if (TooltipFlustered != null)
        {
            TooltipFlustered.isActive = false;
        }


        StartDialogue(customers.DialogeTextForOrder, needToBeClosedExternal);
    }

    [BurstCompile]
    public void StartDialogue(Customers customers, List<float> OrderValues , int FlusteredLevel = 0,  bool needToBeClosedExternal = true)
    {
        OrderCup.gameObject.SetActive(true);
        OrderCup.SetValues(OrderValues);
        ButtonContionueText.SetActive(false);

        if (TooltipCup != null)
        {
            TooltipCup.isActive = true;
            TooltipCup.content = BuildToolTipText(OrderValues);
        }

        SetFlusteredLevel(customers, FlusteredLevel);
        if (TooltipFlustered != null)
        {
            TooltipFlustered.isActive = true;
        }

        StartDialogue(customers.DialogeTextForOrder, needToBeClosedExternal);
    }

    [BurstCompile]
    public void SetFlusteredLevel(Customers customers, int FlusteredLevel = -1)
    {
        this.FlusteredLevel = FlusteredLevel;
        SetAvatarPicture(FlusteredLevel, customers);

        if (TooltipFlustered != null)
        {
            //Set new tooltip
            switch (FlusteredLevel)
            {
                case -1:
                    TooltipFlustered.isActive = false;
                    TooltipFlustered.header = Statics.TooltipFlusteredNormal;
                    break;
                case 0:
                    TooltipFlustered.isActive = false;
                    TooltipFlustered.header = Statics.TooltipFlusteredNormal;
                    break;
                case 1:
                    TooltipFlustered.isActive = true;
                    TooltipFlustered.header = Statics.TooltipFlusteredLevel1;
                    break;
                case 2:
                    TooltipFlustered.isActive = true;
                    TooltipFlustered.header = Statics.TooltipFlusteredLevel2;
                    break;
                case 3:
                    TooltipFlustered.isActive = true;
                    TooltipFlustered.header = Statics.TooltipFlusteredLevel3;
                    break;
                case 4:
                    TooltipFlustered.isActive = true;
                    TooltipFlustered.header = Statics.TooltipFlusteredLevel4;
                    break;
            }
        }
    }

    [BurstCompile]
    public void SetFlusteredLevel(Customers customers, int FlusteredLevel, List<float> OrderValues)
    {
        this.FlusteredLevel = FlusteredLevel;
        OrderCup.SetValues(OrderValues);
        SetAvatarPicture(FlusteredLevel, customers);

        if (TooltipCup != null)
        {
            TooltipCup.isActive = true;
            TooltipCup.content = BuildToolTipText(OrderValues);
        }

        if (TooltipFlustered != null)
        {
            //Set new tooltip
            switch (FlusteredLevel)
            {
                case -1:
                    TooltipFlustered.isActive = false;
                    TooltipFlustered.header = Statics.TooltipFlusteredNormal;
                    break;
                case 0:
                    TooltipFlustered.isActive = false;
                    TooltipFlustered.header = Statics.TooltipFlusteredNormal;
                    break;
                case 1:
                    TooltipFlustered.isActive = true;
                    TooltipFlustered.header = Statics.TooltipFlusteredLevel1;
                    break;
                case 2:
                    TooltipFlustered.isActive = true;
                    TooltipFlustered.header = Statics.TooltipFlusteredLevel2;
                    break;
                case 3:
                    TooltipFlustered.isActive = true;
                    TooltipFlustered.header = Statics.TooltipFlusteredLevel3;
                    break;
                case 4:
                    TooltipFlustered.isActive = true;
                    TooltipFlustered.header = Statics.TooltipFlusteredLevel4;
                    break;
            }
        }
    }

    [BurstCompile]
    public void StartDialogueSuccess(Customers customers, bool needToBeClosedExternal = true)
    {
        OrderCup.gameObject.SetActive(false);
        if (TooltipCup != null)
        {
            TooltipCup.isActive = false;
        }


        if (TooltipFlustered != null)
        {
            TooltipFlustered.isActive = false;
        }

        ButtonContionueText.SetActive(true);


        if (customers.DialogeTextForOrderSuccess == null || customers.DialogeTextForOrderSuccess.GetIsDialougeNullOrEmpty())
        {
            Dialogue diag = new Dialogue(Statics.CustomerDialogSucces);
            diag.name = customers.name;
            StartDialogue(diag);
        }
        else
        {
            StartDialogue(customers.DialogeTextForOrderSuccess, false);
        }
    }

    [BurstCompile]
    public void StartDialogueFail(Customers customers, bool needToBeClosedExternal = true)
    {
        OrderCup.gameObject.SetActive(false);

        if (TooltipCup != null)
        {
            TooltipCup.isActive = false;
        }

        if (TooltipFlustered != null)
        {
            TooltipFlustered.isActive = false;
        }
        ButtonContionueText.SetActive(true);

        if (customers.DialogeTextForOrderFail == null || customers.DialogeTextForOrderFail.GetIsDialougeNullOrEmpty())
        {
            Dialogue diag = new Dialogue(Statics.CustomerDialogFailed);
            if (string.IsNullOrWhiteSpace(customers.name))
            {
                diag.name = "";
            }
            else
            {
                diag.name = customers.name;
            }
            StartDialogue(diag);
        }
        else
        {
            StartDialogue(customers.DialogeTextForOrderFail, false);
        }
    }

    [BurstCompile]
    public void StartDialogue(Dialogue dialogue,bool needToBeClosedExternal = false)
    {
        //Debug.Log("Start Conversation with" + dialogue.name);

        //SetFlusteredLevel(FlusteredLevel);

        DialogNeedToBeClosedExtern = needToBeClosedExternal;

        SetDialogBoxVisibility(true);
        DialogBoxVisibilityButton.SetActive(true);

        nameText.text = dialogue.name;

        Sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            Sentences.Enqueue(sentence);
        }

        DislayNextSentence();
    }

    [BurstCompile]
    public void DislayNextSentence()
    {
        if (Sentences.Count == 0 && DialogNeedToBeClosedExtern == true)
        {
            return;
        }
        else if (Sentences.Count == 0 && DialogNeedToBeClosedExtern == false)
        {
            EndDialogue();
            return;
        }

        string sentence = Sentences.Dequeue();
        //dialogueText.text = sentence;
        if (LastDialogCorutine != null)
        {
            StopCoroutine(LastDialogCorutine);
        }
        LastDialogCorutine = StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(TalkSpeedCustomer);
        }
    }

    [BurstCompile]
    public void EndDialogue()
    {
        //Debug.Log("End of conversation.");
        SetDialogBoxVisibility(false);
        DialogBoxVisibilityButton.SetActive(false);
    }

    [BurstCompile]
    public void SwitchDialogBoxVisibility()
    {
        ShowDialogeBox = !ShowDialogeBox;
        DialogBox.SetActive(ShowDialogeBox);
    }

    [BurstCompile]
    public void SetDialogBoxVisibility(bool b)
    {
        ShowDialogeBox = b;
        DialogBox.SetActive(b);
    }

    [BurstCompile]
    public string BuildToolTipText(List<float> values)
    {
        #region Order of Fillings
        //Chocolate
        //Milk
        //Tea
        //Cream
        //Espresso
        //Sugar
        //Coffee
        //Boba
        //Ice
        //WhipedCream
        //ChocolateSauce
        //CaramelSauce
        //Sprinkles
        //BreastMilk
        #endregion

        string RetVal = "Tooltip content text test";

        StringBuilder tooltipContent = new StringBuilder();

        //bool isFirstPercentage = true;

        if (values[13] > 0.01)
        {
            //if (isFirstPercentage == false)
            //{
            //    tooltipContent.Append(" ,");
            //}
            tooltipContent.AppendLine(Statics.BreastMilk + ": " + (values[13]) + "%");
            //isFirstPercentage = false;
        }

        if (values[0] > 0.01)
        {
            //if (isFirstPercentage == false)
            //{
            //    tooltipContent.Append(" ,");
            //}
            tooltipContent.AppendLine(Statics.Chocolate + ": " + (values[0]) + "%");
            //isFirstPercentage = false;
        }

        if (values[1] > 0.01)
        {
            //if (isFirstPercentage == false)
            //{
            //    tooltipContent.Append(" ,");
            //}
            tooltipContent.AppendLine(Statics.Milk + ": " + (values[1]) + "%");
            //isFirstPercentage = false;
        }

        if (values[2] > 0.01)
        {
            //if (isFirstPercentage == false)
            //{
            //    tooltipContent.Append(" ,");
            //}
            tooltipContent.AppendLine(Statics.Tea + ": " + (values[2]) + "%");
            //isFirstPercentage = false;
        }

        if (values[3] > 0.01)
        {
            //if (isFirstPercentage == false)
            //{
            //    tooltipContent.Append(" ,");
            //}
            tooltipContent.AppendLine(Statics.Cream + ": " + (values[3]) + "%");
            //isFirstPercentage = false;
        }

        if (values[4] > 0.01)
        {
            //if (isFirstPercentage == false)
            //{
            //    tooltipContent.Append(" ,");
            //}
            tooltipContent.AppendLine(Statics.Espresso + ": " + (values[4]) + "%");
            //isFirstPercentage = false;
        }

        if (values[5] > 0.01)
        {
            //if (isFirstPercentage == false)
            //{
            //    tooltipContent.Append(" ,");
            //}
            tooltipContent.AppendLine(Statics.Sugar + ": " + (values[5]) + "%");
            //isFirstPercentage = false;
        }

        if (values[6] > 0.01)
        {
            //if (isFirstPercentage == false)
            //{
            //    tooltipContent.Append(" ,");
            //}
            tooltipContent.AppendLine(Statics.Coffee + ": " + (values[6]) + "%");
            //isFirstPercentage = false;
        }

        if (values[7] > 0.01)
        {
            //if (isFirstPercentage == false)
            //{
            //    tooltipContent.Append(" ,");
            //}
            tooltipContent.AppendLine(Statics.Boba);
            //isFirstPercentage = false;
        }

        if (values[8] > 0.01)
        {
            //if (isFirstPercentage == false)
            //{
            //    tooltipContent.Append(" ,");
            //}
            tooltipContent.AppendLine(Statics.Ice);
            //isFirstPercentage = false;
        }

        if (values[9] > 0.01)
        {
            //if (isFirstPercentage == false)
            //{
            //    tooltipContent.Append(" ,");
            //}
            tooltipContent.AppendLine(Statics.WhippedCream);
            //isFirstPercentage = false;
        }

        if (values[10] > 0.01)
        {
            //if (isFirstPercentage == false)
            //{
            //    tooltipContent.Append(" ,");
            //}
            tooltipContent.AppendLine(Statics.ChocolateSauce);
            //isFirstPercentage = false;
        }

        if (values[11] > 0.01)
        {
            //if (isFirstPercentage == false)
            //{
            //    tooltipContent.Append(" ,");
            //}
            tooltipContent.AppendLine(Statics.CaramelSauce);
            //isFirstPercentage = false;
        }

        if (values[12] > 0.01)
        {
            //if (isFirstPercentage == false)
            //{
            //    tooltipContent.Append(" ,");
            //}
            tooltipContent.AppendLine(Statics.Sprinkles);
            //isFirstPercentage = false;
        }

        RetVal = tooltipContent.ToString();
        return RetVal;
    }

    [BurstCompile]
    private void SetAvatarPicture(int flusteredLevel,Customers customer)
    {
        DeactivateAllFlusteredImages();

        FlusteredImageHolder.SetActive(true);

        if (flusteredLevel <= 0)
        {
            if (customer != null && customer.Avatar.Normal != null)
            {
                FlusteredNormalImage.sprite = customer.Avatar.Normal;
                FlusteredNormal.SetActive(true);
                CharacterBackgroundImage.color = FlusteredColorNormal;
            }
            return;
        }
        else if (flusteredLevel == 1)
        {
            if (customer != null && customer.Avatar.Level1 != null)
            {
                FlusteredLevel1Iamge.sprite = customer.Avatar.Level1;
                FlusteredLevel1.SetActive(true);
                CharacterBackgroundImage.color = FlusteredColorLevel1;
            }
            return;
        }
        else if (flusteredLevel == 2)
        {
            if (customer != null && customer.Avatar.Level2 != null)
            {
                FlusteredLevel2Iamge.sprite = customer.Avatar.Level2;
                FlusteredLevel2.SetActive(true);
                CharacterBackgroundImage.color = FlusteredColorLevel2;
            }
            return;
        }
        else if (flusteredLevel == 3)
        {
            if (customer != null && customer.Avatar.Level3 != null)
            {
                FlusteredLevel3Iamge.sprite = customer.Avatar.Level3;
                FlusteredLevel3.SetActive(true);
                CharacterBackgroundImage.color = FlusteredColorLevel3;
            }
            return;
        }
        else if (flusteredLevel >= 4)
        {
            if (customer != null && customer.Avatar.Level4 != null)
            {
                FlusteredLevel4Iamge.sprite = customer.Avatar.Level4;
                FlusteredLevel4.SetActive(true);
                CharacterBackgroundImage.color = FlusteredColorLevel4;
            }
            return;
        }
    }

    [BurstCompile]
    public void DeactivateAllFlusteredImages()
    {
        FlusteredImageHolder.SetActive(false);
        FlusteredNormal.SetActive(false);
        FlusteredLevel1.SetActive(false);
        FlusteredLevel2.SetActive(false);
        FlusteredLevel3.SetActive(false);
        FlusteredLevel4.SetActive(false);
    }

    #endregion

    #region Barista

    /// <summary>
    /// Let the Barista talk
    /// </summary>
    /// <param name="sentence"></param>
    /// <param name="stopOffset"></param>
    public void StartDialoguebarista(DialogSentence sentence, float stopOffset)
    {
        if (gameObject.activeSelf == false) // prevents to do anystuff while beeing inactive
        {
            return;
        }

        SetDialogBoxBaristaVisibility(true);
        DislayBaristaSentence(sentence.sentence , stopOffset);
        PlayAudioBarista(sentence.AudioName);
    }

    public void DislayBaristaSentence(string sentence, float stopOffset)
    {
        if (LastBaristaDialogCorutine != null)
        {
            StopCoroutine(LastBaristaDialogCorutine);
        }
        LastBaristaDialogCorutine = StartCoroutine(TypeSentenceBarista(sentence, stopOffset));
    }

    IEnumerator TypeSentenceBarista(string sentence, float stopOffset)
    {
        dialogueTextBarista.text = "";
        baristaController.Talking = true;
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueTextBarista.text += letter;
            yield return new WaitForSeconds(TalkSpeedBarista);
        }
        baristaController.Talking = false;
        StartCoroutine(StoptBaristaTalk(stopOffset));
    }

    private IEnumerator StoptBaristaTalk(float stopOffset)
    {   //Stopoffset
        yield return new WaitForSeconds(stopOffset);
        EndDialogueBarista();
    }

    public void EndDialogueBarista()
    {
        //Debug.Log("End of conversation.");
        SetDialogBoxBaristaVisibility(false);
    }

    public void SetDialogBoxBaristaVisibility(bool b)
    {
        DialogBoxBarista.SetActive(b);
    }

    const string AudioFolderName = "Audio/VoiceLines/";
    private void PlayAudioBarista(string AudioName)
    {
        if (SoundVariationBaristaDialoge != null && string.IsNullOrWhiteSpace(AudioName) == false)
        {
            //AudioClip myAudioClip = Resources.Load<AudioClip>(AudioFolderName + AudioName);
            //if (myAudioClip != null)
            //{
            //    SoundVariationBaristaDialoge.PlayRandomOneShot(myAudioClip);
            //}

            List<AudioClip> myAudioClip = GetResourceAudioClips(AudioName);

            if (myAudioClip != null && myAudioClip.Count >= 1)
            {
                myAudioClip.Shuffle();
                SoundVariationBaristaDialoge.PlayRandomOneShot(myAudioClip[0]);
            }
        }
    }

    private List<AudioClip> GetResourceAudioClips(string AudioName)
    {
        List<AudioClip> RetVal = new List<AudioClip>();
        bool LookForFiles = true;
        int currentFile = 0;

        while (LookForFiles)
        {
            currentFile++;
            string name = AudioFolderName + AudioName + "_" + currentFile.ToString("00");
            AudioClip cur = Resources.Load<AudioClip>( name );

            if (cur != null) 
            {
                RetVal.Add(cur);
            }
            else
            {
                LookForFiles = false;
            }
        }

        return RetVal;
    }


    #endregion
}

public class DialogSentence
{
    public string sentence;
    public string AudioName;

    public DialogSentence(string sentence,string AudioName)
    {
        this.sentence = sentence;
        this.AudioName = AudioName;
    }

    public DialogSentence(string sentence)
    {
        this.sentence = sentence;
        //AudioName = null;
    }
}
