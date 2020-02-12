using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class UIHelper
{
    public static IEnumerator ColorChanger(MaskableGraphic component, Color targetColor)
    {
        Color baseColor = component.color;
        for (float f = 0.02f; f <= 1; f += 0.02f)
        {
            component.color = Vector4.Lerp(baseColor, targetColor, f * (2 - f));
            yield return null;
        }
    }
}
