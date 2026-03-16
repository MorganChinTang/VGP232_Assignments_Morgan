using UnityEngine;

[CreateAssetMenu(fileName = "Theme", menuName = "Scriptable Objects/Theme")]
public class Theme : ScriptableObject
{
    public Font regularFontType;
    public Color regularFontColor;
    public Sprite regularButtonStyle;

    public Font specialFontType;
    public Color specialFontColor;
    public Sprite specialFontStyle;
}
