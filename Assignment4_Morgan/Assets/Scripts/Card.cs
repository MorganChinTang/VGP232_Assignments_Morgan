using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Card : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Name;
    [SerializeField] private TextMeshProUGUI Description;
    [SerializeField] private Image TypeDef;
    [SerializeField] private Image TypeAtt;

    [SerializeField] private TextMeshProUGUI Cost;
    [SerializeField] private TextMeshProUGUI Att;
    [SerializeField] private TextMeshProUGUI Def;
    [SerializeField] private Image Image;

    [SerializeField] private Sprite Type1;
    [SerializeField] private Sprite Type2;
    [SerializeField] private Sprite Type3;

    [SerializeField] private Button cardButton;

    private CardData currentData;
    private GameManager gameManager;

    public CardData CurrentData => currentData;

    private void Awake()
    {
        if (cardButton == null)
        {
            cardButton = GetComponent<Button>();
        }
    }

    private void OnEnable()
    {
        if (cardButton != null)
        {
            cardButton.onClick.AddListener(HandleCardClick);
        }
    }

    private void OnDisable()
    {
        if (cardButton != null)
        {
            cardButton.onClick.RemoveListener(HandleCardClick);
        }
    }

    public void SetData(CardData data)
    {
        if (data == null)
        {
            return;
        }

        currentData = data;

        SetLocalizedText(data.cardname, data.description);

        if (Cost != null)
        {
            Cost.text = "$" + data.cost.ToString();
        }

        if (Att != null)
        {
            Att.text = data.att.ToString();
        }

        if (Def != null)
        {
            Def.text = data.def.ToString();
        }

        if (Image != null)
        {
            Image.sprite = data.image;
        }

        if (TypeAtt != null)
        {
            TypeAtt.sprite = GetTypeSprite(data.type);
        }

        if (TypeDef != null)
        {
            TypeDef.sprite = GetTypeSprite(data.type);
        }

        LanguageManager languageManager = FindFirstObjectByType<LanguageManager>();
        if (languageManager != null)
        {
            languageManager.ApplyLocalizationToCard(this);
        }
    }

    public void SetLocalizedText(string localizedName, string localizedDescription)
    {
        if (Name != null)
        {
            Name.text = localizedName;
        }

        if (Description != null)
        {
            Description.text = localizedDescription;
        }
    }

    private Sprite GetTypeSprite(int type)
    {
        switch (type)
        {
            case 1:
                return Type1;
            case 2:
                return Type2;
            case 3:
                return Type3;
            default:
                return null;
        }
    }

    private void HandleCardClick()
    {
        if (currentData == null)
        {
            return;
        }

        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager == null) {Debug.LogError("Game Manager not found.");}
        }

        if (gameManager != null)
        {
            gameManager.ShowCardDetail(currentData);
        }
    }
}
