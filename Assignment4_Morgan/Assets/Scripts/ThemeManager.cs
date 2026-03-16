using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ThemeManager : MonoBehaviour
{
    public enum ThemeSwitch
    {
        Regular,
        Special
    }

    // Default to Regular Theme
    private static ThemeSwitch currentTheme = ThemeSwitch.Regular;

    [SerializeField] private Theme theme;

    public List<Button> buttons = new List<Button>();
    public List<TMP_Text> texts = new List<TMP_Text>();

    private TMP_FontAsset regularFontAsset;
    private TMP_FontAsset specialFontAsset;

    private void OnEnable()
    {
        ApplyCurrentTheme();
    }

    public void ApplyCurrentTheme()
    {
        if (currentTheme == ThemeSwitch.Special)
        {
            ApplySpecialFont();
        }
        else
        {
            ApplyRegularFont();
        }
    }

    public void SetRegularFont()
    {
        currentTheme = ThemeSwitch.Regular;
        ApplyRegularFont();
    }

    public void SetSpecialFont()
    {
        currentTheme = ThemeSwitch.Special;
        ApplySpecialFont();
    }

    private void ApplyRegularFont()
    {
        if (theme == null)
        {
            return;
        }

        ApplyTheme(theme.regularFontType, theme.regularFontColor, theme.regularButtonStyle, ref regularFontAsset);
    }

    private void ApplySpecialFont()
    {
        if (theme == null)
        {
            return;
        }

        ApplyTheme(theme.specialFontType, theme.specialFontColor, theme.specialFontStyle, ref specialFontAsset);
    }

    private void ApplyTheme(Font currentFont, Color color, Sprite buttonSprite, ref TMP_FontAsset NewFont)
    {
        if (currentFont != null && (NewFont == null || NewFont.sourceFontFile != currentFont))
        {
            NewFont = TMP_FontAsset.CreateFontAsset(currentFont);
        }

        foreach (TMP_Text text in texts)
        {
            if (text == null)
            {
                continue;
            }

            if (NewFont != null)
            {
                text.font = NewFont;
            }

            text.color = color;
        }

        foreach (Button button in buttons)
        {
            if (button == null || button.image == null)
            {
                continue;
            }

            if (buttonSprite != null)
            {
                button.image.sprite = buttonSprite;
            }
        }
    }
}
