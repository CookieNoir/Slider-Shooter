using System.Collections;
using UnityEngine;
using UnityEngine.UI;
[AddComponentMenu("Game UI/Alpha Changer")]
[RequireComponent(typeof(MaskableGraphic))]
public class AlphaChanger : MonoBehaviour
{
    public float decreasingDuration = 1f;
    public float increasingDuration = 1f;
    public float lowerAlpha = 0f;
    public float higherAlpha = 1f;
    public bool increase = false; // Если Increase = false, то Альфа будет уменьшаться, иначе - увеличиваться

    private MaskableGraphic mg;
    private float decreasingDurationInversed;
    private float increasingDurationInversed;
    private Color lowerColor;
    private Color higherColor;

    private void Awake()
    {
        mg = GetComponent<MaskableGraphic>();
        decreasingDurationInversed = 1f / decreasingDuration;
        increasingDurationInversed = 1f / increasingDuration;
        lowerColor = new Color(mg.color.r, mg.color.g, mg.color.b, lowerAlpha);
        higherColor = new Color(mg.color.r, mg.color.g, mg.color.b, higherAlpha);
    }

    public IEnumerator ChangeAlpha()
    {
        Color c = mg.color;
        increase = !increase;
        if (increase)
        {
            for (float i = 0; i < decreasingDuration; i += Time.deltaTime)
            {
                mg.color = Vector4.Lerp(c, lowerColor, Mathf.Sqrt(i * decreasingDurationInversed));
                yield return null;
            }
            mg.color = lowerColor;
        }
        else
        {
            for (float i = 0; i < increasingDuration; i += Time.deltaTime)
            {
                mg.color = Vector4.Lerp(c, higherColor, Mathf.Sqrt(i * increasingDurationInversed));
                yield return null;
            }
            mg.color = higherColor;
        }
    }
}
