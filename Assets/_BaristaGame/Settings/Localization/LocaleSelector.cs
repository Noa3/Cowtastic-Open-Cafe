using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.Localization.Settings;
using UnityEngine.Localization;

public class LocaleSelector : MonoBehaviour
{
    private bool active = false;
    private static bool runtimeButtonsInitialized = false;

    private readonly struct LocaleButtonConfig
    {
        public readonly string Name;
        public readonly string LocaleCode;
        public readonly string FlagResourceName;

        public LocaleButtonConfig(string name, string localeCode, string flagResourceName)
        {
            Name = name;
            LocaleCode = localeCode;
            FlagResourceName = flagResourceName;
        }
    }

    private static readonly LocaleButtonConfig[] LocaleButtons =
    {
        new("English", "en", "Flag_Eng"),
        new("French", "fr", "Flag_Fra"),
        new("German", "de", "Flag_Deu"),
        new("Russian", "ru", "Flag_Rus"),
        new("Chinese", "zh", "Flag_Chn"),
        new("Greek", "el", "Flag_Gre"),
        new("Indonesian", "id", "Flag_Ina"),
        new("Italian", "it", "Flag_Ita"),
        new("Japanese", "ja", "Flag_Jpn"),
        new("Korean", "ko", "Flag_Kor"),
        new("Polish", "pl", "Flag_Pol"),
        new("Portoguese", "pt", "Flag_Prt"),
        new("Espaniola", "es-ES", "Flag_Esp"),
        new("Swedish", "sv", "Flag_Swe"),
        new("Thai", "th", "Flag_Tha"),
        new("Turkish", "tr", "Flag_Tur"),
        new("Ukrainian", "uk", "Flag_Ukr"),
    };

    // Preserve legacy button indices used by existing 4-language and extended language button layouts.
    private static readonly System.Collections.Generic.Dictionary<int, string> LegacyLocaleCodeByIndex = new()
    {
        { 0, "en" },    // Open menu English button
        { 1, "fr" },    // Open menu French button
        { 2, "de" },    // Open menu German button
        { 3, "ru" },    // Open menu Russian button
        // Extended layout compatibility (closed project style)
        { 4, "el" },
        { 5, "id" },
        { 6, "it" },
        { 7, "ja" },
        { 8, "ko" },
        { 9, "pl" },
        { 10, "pt" },
        { 11, "ru" },
        { 12, "es-ES" },
        { 13, "sv" },
        { 14, "th" },
        { 15, "tr" },
        { 16, "uk" },
        { 17, "uk" }
    };

    private static readonly HashSet<string> DynamicManagedButtonNames = new()
    {
        "English", "French", "German", "Russian", "Chinese", "Greek", "Indonesian",
        "Italian", "Japanese", "Korean", "Polish", "Portoguese", "Espaniola",
        "Swedish", "Thai", "Turkish", "Ukrainian"
    };

    private void Start()
    {
        EnsureLanguageButtons();
    }

    public void ChangeLocale(int localeID)
    {
        if (active == false)
        {
            StartCoroutine(SetLocaleByIndex(localeID));
        }
    }

    public void ChangeLocaleByCode(string localeCode)
    {
        if (active == false)
        {
            StartCoroutine(SetLocaleByCode(localeCode));
        }
    }

    private void EnsureLanguageButtons()
    {
        if (runtimeButtonsInitialized)
            return;

        var parentTransform = transform.parent;
        if (parentTransform == null)
            return;

        var templateButton = GetTemplateButton(parentTransform);
        if (templateButton == null)
        {
            Debug.LogWarning("LocaleSelector: Could not find template language button.");
            return;
        }

        foreach (var config in LocaleButtons)
        {
            var button = GetOrCreateLanguageButton(parentTransform, templateButton, config.Name);
            if (button == null)
                continue;

            ConfigureButton(button, config.LocaleCode, config.FlagResourceName);
        }

        runtimeButtonsInitialized = true;
    }

    private static Button GetTemplateButton(Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            var child = parent.GetChild(i);
            if (!DynamicManagedButtonNames.Contains(child.name))
                continue;

            var button = child.GetComponent<Button>();
            if (button != null)
                return button;
        }

        return null;
    }

    private static Button GetOrCreateLanguageButton(Transform parent, Button templateButton, string buttonName)
    {
        var existing = parent.Find(buttonName);
        if (existing != null)
            return existing.GetComponent<Button>();

        var createdObject = Instantiate(templateButton.gameObject, parent);
        createdObject.name = buttonName;
        return createdObject.GetComponent<Button>();
    }

    private void ConfigureButton(Button button, string localeCode, string flagResourceName)
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => ChangeLocaleByCode(localeCode));

        var image = button.GetComponent<Image>();
        if (image == null)
            return;

        var flagSprite = Resources.Load<Sprite>($"Flags/{flagResourceName}");
        if (flagSprite != null)
        {
            image.sprite = flagSprite;
        }
    }

    private IEnumerator SetLocaleByIndex(int localeId)
    {
        active = true;
        yield return LocalizationSettings.InitializationOperation;

        if (LegacyLocaleCodeByIndex.TryGetValue(localeId, out var localeCode))
        {
            var mappedLocale = LocalizationSettings.AvailableLocales.GetLocale(localeCode);
            if (mappedLocale != null)
            {
                LocalizationSettings.SelectedLocale = mappedLocale;
                active = false;
                yield break;
            }
        }

        var locales = LocalizationSettings.AvailableLocales.Locales;
        if (localeId >= 0 && localeId < locales.Count)
        {
            LocalizationSettings.SelectedLocale = locales[localeId];
        }
        else
        {
            Debug.LogError($"LocaleSelector: Invalid locale index {localeId}. No matching locale found.");
        }

        active = false;
        yield return null;
    }

    private IEnumerator SetLocaleByCode(string localeCode)
    {
        active = true;
        yield return LocalizationSettings.InitializationOperation;

        var locale = LocalizationSettings.AvailableLocales.GetLocale(localeCode);
        if (locale != null)
        {
            LocalizationSettings.SelectedLocale = locale;
        }
        else
        {
            Debug.LogError($"LocaleSelector: Locale code '{localeCode}' not found.");
        }

        active = false;
        yield return null;
    }

    //public Dropdown dropdown;
    //IEnumerator Start()
    //{
    //    // Wait for the localization system to initialize
    //    yield return LocalizationSettings.InitializationOperation;

    //    // Generate list of available Locales
    //    var options = new List<Dropdown.OptionData>();
    //    int selected = 0;
    //    for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
    //    {
    //        var locale = LocalizationSettings.AvailableLocales.Locales[i];
    //        if (LocalizationSettings.SelectedLocale == locale)
    //            selected = i;
    //        options.Add(new Dropdown.OptionData(locale.name));
    //    }
    //    dropdown.options = options;

    //    dropdown.value = selected;
    //    dropdown.onValueChanged.AddListener(LocaleSelected);
    //}

    //static void LocaleSelected(int index)
    //{
    //    LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
    //}

}
