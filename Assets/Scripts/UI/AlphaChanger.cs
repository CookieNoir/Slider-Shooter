using System.Collections;
using UnityEngine;
using UnityEngine.UI;
[AddComponentMenu("Game UI/Alpha Changer")]
[RequireComponent(typeof(MaskableGraphic))]
public class AlphaChanger : MonoBehaviour
{
    public int lowerSteps = 50;
    public int higherSteps = 50;
    public float lowerAlpha = 0f;
    public float higherAlpha = 1f;   
    public bool increase = false; // Если Increase = false, то Альфа будет уменьшаться, иначе - увеличиваться

    private MaskableGraphic mg;
    private float lowerStepWeight;
    private float higherStepWeight;
    private Color lowerColor;
    private Color higherColor;

    private void Awake()
    {
        mg = GetComponent<MaskableGraphic>();
        lowerStepWeight = 1f / lowerSteps;
        higherStepWeight = 1f / higherSteps;
        lowerColor = new Color(mg.color.r, mg.color.g, mg.color.b, lowerAlpha);
        higherColor = new Color(mg.color.r, mg.color.g, mg.color.b, higherAlpha);
    }

    public IEnumerator ChangeAlpha()
    {
        Color c = mg.color;
        increase = !increase;
        if (increase)
        {
            for (int i = 1; i <= lowerSteps; ++i)
            {
                yield return null;
                mg.color = Vector4.Lerp(c, lowerColor, Mathf.Sqrt(i * lowerStepWeight));
            }
        }
        else
        {
            for (int i = 1; i <= higherSteps; ++i)
            {
                yield return null;
                mg.color = Vector4.Lerp(c, higherColor, Mathf.Sqrt(i * higherStepWeight));
            }
        }
    }
}
