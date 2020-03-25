using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class UIHelper
{
    public delegate float function(float value);
    public static IEnumerator ColorChanger(MaskableGraphic component, Color targetColor)
    {
        Color baseColor = component.color;
        for (float f = 0; f < 1; f += Time.deltaTime)
        {
            component.color = Vector4.Lerp(baseColor, targetColor, f * (2 - f));
            yield return null;
        }
        component.color = targetColor;
    }

    public static IEnumerator ColorChanger(MaskableGraphic component, Color targetColor, float duration)
    {
        Color baseColor = component.color;
        float durationInversed = 1f / duration;
        for (float f = 0; f < duration; f += Time.deltaTime)
        {
            component.color = Vector4.Lerp(baseColor, targetColor, f * (2 - f));
            yield return null;
        }
        component.color = targetColor;
    }

    public static IEnumerator ColorChanger(MaskableGraphic component, Color targetColor, function func)
    {
        Color baseColor = component.color;
        for (float f = 0; f < 1; f += Time.deltaTime)
        {
            component.color = Vector4.Lerp(baseColor, targetColor, func(f));
            yield return null;
        }
        component.color = targetColor;
    }

    public static IEnumerator ColorChanger(MaskableGraphic component, Color targetColor, float duration, function func)
    {
        Color baseColor = component.color;
        float durationInversed = 1f / duration;
        for (float f = 0; f < duration; f += Time.deltaTime)
        {
            component.color = Vector4.Lerp(baseColor, targetColor, func(f * durationInversed));
            yield return null;
        }
        component.color = targetColor;
    }

    public static IEnumerator ColorChanger(MaskableGraphic component, Color targetColor, float duration, function func, bool inversedInterpolation, bool timeScaleIndependent = false)
    {
        Color baseColor = component.color;
        float durationInversed = 1f / duration;
        if (timeScaleIndependent)
        {
            if (inversedInterpolation)
            {
                for (float f = duration; f > 0; f += Time.deltaTime)
                {
                    component.color = Vector4.Lerp(targetColor, baseColor, func(f * durationInversed));
                    yield return new WaitForSecondsRealtime(0.03125f);
                }
            }
            else
            {
                for (float f = 0; f < duration; f += Time.deltaTime)
                {
                    component.color = Vector4.Lerp(baseColor, targetColor, func(f * durationInversed));
                    yield return new WaitForSecondsRealtime(0.03125f);
                }
            }
        }
        else
        {
            if (inversedInterpolation)
            {
                for (float f = duration; f > 0; f += Time.deltaTime)
                {
                    component.color = Vector4.Lerp(targetColor, baseColor, func(f * durationInversed));
                    yield return null;
                }
            }
            else
            {
                for (float f = 0; f < duration; f += Time.deltaTime)
                {
                    component.color = Vector4.Lerp(baseColor, targetColor, func(f * durationInversed));
                    yield return null;
                }
            }
        }
        component.color = targetColor;
    }
}
