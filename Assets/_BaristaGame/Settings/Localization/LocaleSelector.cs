using System.Collections;
using UnityEngine;

using UnityEngine.Localization.Settings;

public class LocaleSelector : MonoBehaviour
{
    private bool active = false;

    public void ChangeLocale(int localeID)
    {
        if (active == false)
        {
            StartCoroutine(SetLocale(localeID));
        }
    }

    private IEnumerator SetLocale(int _LocaleID)
    {
        active = true;
        //yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_LocaleID];
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
