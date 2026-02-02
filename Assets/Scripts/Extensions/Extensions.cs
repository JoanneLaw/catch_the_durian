using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class Extensions
{
    #region  Graphic
    public static void SetAlpha(this Graphic graphic, float alpha)
    {
        Color color = graphic.color;
        color.a = alpha;
        graphic.color = color;
    }

    public static void SetColor(this Graphic graphic, string hexColor)
    {
        Color color;
        if (ColorUtility.TryParseHtmlString(hexColor, out color))
        {
            graphic.color = color;
        }
    }
    #endregion

    public static void SetActiveWithCheck(this GameObject gameObject, bool value)
    {
        if (gameObject.activeSelf != value)
        {
            gameObject.SetActive(value);
        }
    }
}

public static class DateTimeEx
{
    public static string ConvertDuration(TimeSpan timeSpan)
    {
        return string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
    }
}
