using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    [System.Serializable]
    private class LocalizedText
    {
        public string locKey;
        public TMP_Text text;
    }

    private enum LanguageMode
    {
        English,
        Spanish,
        French
    }

    [SerializeField] private Language languageData;
    [SerializeField] private List<CardData> cardDataList = new List<CardData>();
    [SerializeField] private List<LocalizedText> buttonTexts = new List<LocalizedText>();

    private LanguageMode currentLanguage = LanguageMode.English;
    private readonly Dictionary<string, LocData> locMap = new Dictionary<string, LocData>();

    private void Awake()
    {
        CacheLocalizationData();
        ApplyLocalization();
    }

    public void OnEnglishHit()
    {
        currentLanguage = LanguageMode.English;
        ApplyLocalization();
    }

    public void OnSpanishHit()
    {
        currentLanguage = LanguageMode.Spanish;
        ApplyLocalization();
    }

    public void OnFrenchHit()
    {
        currentLanguage = LanguageMode.French;
        ApplyLocalization();
    }

    public void ApplyLocalizationToCard(Card card)
    {
        if (card == null || card.CurrentData == null)
        {
            return;
        }

        CardData data = card.CurrentData;
        card.SetLocalizedText(GetLocalizedValue(data.cardname), GetLocalizedValue(data.description));
    }

    private void ApplyLocalization()
    {
        Card[] cards = FindObjectsByType<Card>(FindObjectsSortMode.None);
        foreach (Card card in cards)
        {
            ApplyLocalizationToCard(card);
        }

        foreach (LocalizedText entry in buttonTexts)
        {
            if (entry == null || entry.text == null)
            {
                continue;
            }

            entry.text.text = GetLocalizedValue(entry.locKey);
        }
    }

    private void CacheLocalizationData()
    {
        locMap.Clear();

        if (languageData != null && languageData.locData != null)
        {
            foreach (LocData entry in languageData.locData)
            {
                if (entry == null || string.IsNullOrWhiteSpace(entry.locKey))
                {
                    continue;
                }

                locMap[entry.locKey] = entry;
            }
        }

        foreach (LocalizedText entry in buttonTexts)
        {
            if (entry == null)
            {
                continue;
            }

            if (string.IsNullOrWhiteSpace(entry.locKey) && entry.text != null)
            {
                entry.locKey = entry.text.text;
            }
        }

        WarnIfMissingKeys();
    }

    private void WarnIfMissingKeys()
    {
        foreach (CardData data in cardDataList)
        {
            if (data == null)
            {
                continue;
            }

            if (!locMap.ContainsKey(data.cardname))
            {
                Debug.LogWarning($"Missing localization entry for card name: {data.cardname}");
            }

            if (!locMap.ContainsKey(data.description))
            {
                Debug.LogWarning($"Missing localization entry for card description: {data.description}");
            }
        }

        foreach (LocalizedText entry in buttonTexts)
        {
            if (entry == null || string.IsNullOrWhiteSpace(entry.locKey))
            {
                continue;
            }

            if (!locMap.ContainsKey(entry.locKey))
            {
                Debug.LogWarning($"Missing localization entry for UI text: {entry.locKey}");
            }
        }
    }

    private string GetLocalizedValue(string locKey)
    {
        if (string.IsNullOrWhiteSpace(locKey))
        {
            return locKey;
        }

        if (!locMap.TryGetValue(locKey, out LocData entry))
        {
            return locKey;
        }

        switch (currentLanguage)
        {
            case LanguageMode.French:
                return entry.fr;
            case LanguageMode.Spanish:
                return entry.sp;
            default:
                return entry.en;
        }
    }
}
