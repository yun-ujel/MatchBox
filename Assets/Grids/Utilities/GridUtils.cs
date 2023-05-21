using TMPro;
using UnityEngine;

namespace Grids.Utilities
{
    public static class GridUtils
    {
        public static TextMeshPro CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default, float fontSize = 2f, Color? color = null, TextAlignmentOptions alignment = TextAlignmentOptions.Midline)
        {
            if (color == null) { color = Color.white; }
            return CreateWorldText(text, parent, localPosition, fontSize, (Color)color, alignment, out GameObject _);
        }
        public static TextMeshPro CreateWorldText(string text, Transform parent, Vector3 localPosition, float fontSize, Color color, TextAlignmentOptions alignment, out GameObject gameObject)
        {
            gameObject = new GameObject("WorldText", typeof(TextMeshPro));

            RectTransform transform = gameObject.GetComponent<RectTransform>();
            transform.SetParent(parent, false);
            transform.localPosition = localPosition;
            transform.sizeDelta = new Vector2(text.Length, 1f);

            TextMeshPro tMPro = gameObject.GetComponent<TextMeshPro>();
            tMPro.alignment = alignment;
            tMPro.text = text;
            tMPro.fontSize = fontSize;
            tMPro.color = color;

            return tMPro;
        }
    }
}
