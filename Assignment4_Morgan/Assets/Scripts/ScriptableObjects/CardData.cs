using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Scriptable Objects/CardData")]
public class CardData : ScriptableObject
{
    public string cardname = "Card Name";
    public string description = "Card Description";
    public int type;
    public int cost;
    public int att;
    public int def;
    public Sprite image;
}
