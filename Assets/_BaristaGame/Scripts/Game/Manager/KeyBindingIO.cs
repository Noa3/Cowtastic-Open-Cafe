using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Progress;

public class KeyBindingIO : MonoBehaviour
{
    [SerializeField]
    private Color buttonSelectedColor;
    [SerializeField]
    private Color buttonDeselectedColor;
    [SerializeField]
    private KeyBindingManager.BindableActions[] bindableActionOrder;
    [SerializeField]
    private GameObject[] KeyBindingUIRows;

    private Dictionary<KeyBindingManager.BindableActions, UnityEngine.UI.Image> keybindingImages;
    private Dictionary<KeyBindingManager.BindableActions, TextMeshProUGUI> keybindingButtonTexts;

    private KeyBindingManager.BindableActions selectedBind;
    private UnityEngine.UI.Image selectedImage = null;
    private TextMeshProUGUI selectedText = null;

    // Start is called before the first frame update
    void Start()
    {
        keybindingButtonTexts = new Dictionary<KeyBindingManager.BindableActions, TextMeshProUGUI>();
        keybindingImages = new Dictionary<KeyBindingManager.BindableActions, UnityEngine.UI.Image>();

        for(int i = 0; i < KeyBindingUIRows.Length; i++)
        {
            var button = KeyBindingUIRows[i].transform.Find("Button").GetComponent<UnityEngine.UI.Button>();
            var image = button.GetComponent<UnityEngine.UI.Image>();
            var textBox = button.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            keybindingImages.Add(bindableActionOrder[i], image);
            keybindingButtonTexts.Add(bindableActionOrder[i], textBox);
            int nonCapturedIntValue = i;
            button.onClick.AddListener(() => SetButtonClicked(bindableActionOrder[nonCapturedIntValue]));
            image.color = buttonDeselectedColor;
            var playerPrefKeyForAction = "KeyBind:" + bindableActionOrder[i].ToString();
            if (PlayerPrefs.HasKey(playerPrefKeyForAction))
            {
                string keyCodeText = PlayerPrefs.GetString(playerPrefKeyForAction).ToString();
                textBox.text = keyCodeText;
            }
        }
    }

    private void DeselectButton()
    {
        if (selectedImage != null)
        {
            selectedImage.color = buttonDeselectedColor;
            selectedImage = null;
            selectedText = null;
        }
    }

    public void ResetAll()
    {
        DeselectButton();
        foreach(var item in keybindingButtonTexts)
        {
            item.Value.text = "";
            var bindableActionString = item.Key.ToString();
            if (PlayerPrefs.HasKey("KeyBind:" + bindableActionString))
            {
                PlayerPrefs.SetString("KeyBind:" + bindableActionString, "");
            }
        }
        if (KeyBindingManager.instance != null)
        {
            KeyBindingManager.instance.LoadKeyBindings();
        }
    }

    public void SetButtonClicked(KeyBindingManager.BindableActions bind)
    {
        DeselectButton();
        selectedBind = bind;
        selectedImage = keybindingImages[bind];
        selectedText = keybindingButtonTexts[bind];
        selectedImage.color = buttonSelectedColor;
    }

    public void SetBinding(KeyCode code)
    {
        var bindableActionString = selectedBind.ToString();
        var keyCodeString = code.ToString();
        PlayerPrefs.SetString("KeyBind:" + bindableActionString, keyCodeString);
        selectedText.text = code.ToString();
        DeselectButton();
        if (KeyBindingManager.instance != null)
        {
            KeyBindingManager.instance.LoadKeyBindings();
        }
    }

    private void Update()
    {
        if(selectedImage != null)
        {
            if (Input.anyKeyDown)
            {
                foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(key))
                    {
                        SetBinding(key);
                        break;
                    }
                }
            }
        }
    }
}
