using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowCardMenu : MonoBehaviour
{
    [SerializeField] private GameObject ShowCardMenuParent;
    [SerializeField] private HorizontalLayoutGroup CardLayoutGroup;
    [SerializeField] private Card cardPrefab;
    [SerializeField] private List<CardData> cardDataList = new List<CardData>();

    private void OnEnable()
    {
        BuildCards();
    }

    public void BuildCards()
    {
        if (CardLayoutGroup == null || cardPrefab == null)
        {
            return;
        }

        Transform layoutTransform = CardLayoutGroup.transform;
        for (int i = layoutTransform.childCount - 1; i >= 0; i--)
        {
            Destroy(layoutTransform.GetChild(i).gameObject);
        }

        foreach (CardData data in cardDataList)
        {
            if (data == null)
            {
                continue;
            }

            Card cardInstance = Instantiate(cardPrefab, layoutTransform);
            cardInstance.SetData(data);
        }
    }
}
