using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using TMPro;
using System.Collections;
using System.Linq;

public class LanguageSelector : MonoBehaviour
{
    public TMP_Dropdown languageDropdown;

    void Start()
    {
        StartCoroutine(InitializeDropdown());
    }

    IEnumerator InitializeDropdown()
    {
        // Esperar a que LocalizationSettings esté listo
        yield return LocalizationSettings.InitializationOperation;

        var locales = LocalizationSettings.AvailableLocales.Locales;
        languageDropdown.ClearOptions();
        var opciones = locales.Select(locale => Mayusculas(locale.Identifier.CultureInfo.NativeName.ToLower())).ToList();
        languageDropdown.AddOptions(opciones);

        // Detectar idioma del sistema
        var systemLocale = LocalizationSettings.SelectedLocale;
        var matchedLocale = locales.FirstOrDefault(locale =>
            locale.Identifier.Code == systemLocale.Identifier.Code);

        int seleccionIndex = 0;
        if (matchedLocale != null)
        {
            LocalizationSettings.SelectedLocale = matchedLocale;
            seleccionIndex = locales.IndexOf(matchedLocale);
        }
        else
        {
            var fallbackLocale = locales.FirstOrDefault(l => l.Identifier.Code.StartsWith("es")) ?? locales[0];
            LocalizationSettings.SelectedLocale = fallbackLocale;
            seleccionIndex = locales.IndexOf(fallbackLocale);
        }

        // Actualizar selección del dropdown
        languageDropdown.value = seleccionIndex;
        languageDropdown.RefreshShownValue();

        // Detectar cambios del usuario
        languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
    }

    public void OnLanguageChanged(int index)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
    }


    string Mayusculas(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return input;
        return char.ToUpper(input[0]) + input.Substring(1);
    }
}
