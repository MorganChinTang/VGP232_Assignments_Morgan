using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Card More Info UI")]
    [SerializeField] private GameObject CardDetailUI;
    [SerializeField] private GameObject CardDetailPrefab;
    public void OnPlayHit()
    {
        SceneManager.LoadScene("Play");
    }

    public void OnReturnMenuHit()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnQuitHit()
    {
        Application.Quit();
        EditorApplication.isPlaying = false;
    }

    public void ShowCardDetail(CardData data)
    {
        if (CardDetailUI == null || CardDetailPrefab == null || data == null)
        {
            return;
        }

        if (!CardDetailUI.activeSelf)
        {
            CardDetailUI.SetActive(true);
        }

        Card detailCard = CardDetailPrefab.GetComponent<Card>();
        if (detailCard != null)
        {
            detailCard.SetData(data);
        }
    }
}
