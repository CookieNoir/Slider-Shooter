using System.Collections;
using UnityEngine;
[AddComponentMenu("Game UI/UI Movement")]
[RequireComponent(typeof(RectTransform))]
public class UiMovement : MonoBehaviour
{
    [Header("Editing At Start")] // Необходимо, чтобы возвращать отлаженный UI элемент на необходимую позицию
    public bool moveAtStart = false;
    public Vector2 startPosition;
    public bool fillEntireScreen = false;
    //---------------------------------
    public enum lines { Top, Right, Local_Center, Bottom_Right, Top_Right };
    public enum functions { Linear, Square, Cubic, SmoothStep, SquareRoot, SquareNegativePlusOneXY };
    private delegate float movingFunc(float value);
    [Header("Translation")]
    public lines line;
    public functions function;
    public float stepSize;
    public float translationDuration = 1f;
    public bool inversedInterpolation = true;
    public bool timeScaleIndependent = true;

    private bool moved = false; // If moved, makes a step back
    private float translationDurationInversed;
    private Vector2 defaultPosition;
    private Vector2 newPosition;
    private Vector2 bias;
    private movingFunc movingFunction;
    private IEnumerator activeMovement;
    //---------------------------------
    private void Awake()
    {
        if (moveAtStart) GetComponent<RectTransform>().anchoredPosition = startPosition;
        if (fillEntireScreen)
        {
            GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);
        }
        translationDurationInversed = 1f / translationDuration;
        defaultPosition = GetComponent<RectTransform>().anchoredPosition;
        switch (line)
        {
            case lines.Top:
                {
                    bias = new Vector2(0, stepSize);
                    break;
                }
            case lines.Right:
                {
                    bias = new Vector2(stepSize, 0);
                    break;
                }
            case lines.Local_Center:
                {
                    bias = defaultPosition.normalized * stepSize;
                    break;
                }
            case lines.Bottom_Right:
                {
                    bias = new Vector2(stepSize, -stepSize);
                    break;
                }
            case lines.Top_Right:
                {
                    bias = new Vector2(stepSize, stepSize);
                    break;
                }
        }
        newPosition = defaultPosition + bias;
        switch (function)
        {
            case functions.Linear:
                {
                    movingFunction = SimpleFunctions.Linear;
                    break;
                }
            case functions.Square:
                {
                    movingFunction = SimpleFunctions.Square;
                    break;
                }
            case functions.Cubic:
                {
                    movingFunction = SimpleFunctions.Cubic;
                    break;
                }
            case functions.SmoothStep:
                {
                    movingFunction = SimpleFunctions.SmoothStep;
                    break;
                }
            case functions.SquareRoot:
                {
                    movingFunction = SimpleFunctions.SquareRoot;
                    break;
                }
            case functions.SquareNegativePlusOneXY:
                {
                    movingFunction = SimpleFunctions.SquareNegativePlusOneXY;
                    break;
                }
        }
        activeMovement = SmoothMove(newPosition, defaultPosition, movingFunction);
    }

    public void Translate()
    {
        StopCoroutine(activeMovement);
        if (!moved)
        {
            activeMovement = SmoothMove(GetComponent<RectTransform>().anchoredPosition, newPosition, movingFunction);
        }
        else
        {
            activeMovement = SmoothMove(GetComponent<RectTransform>().anchoredPosition, defaultPosition, movingFunction);
        }
        StartCoroutine(activeMovement);
        moved = !moved;
    }

    private IEnumerator SmoothMove(Vector2 startPosition, Vector2 endPosition, movingFunc func)
    {
        if (timeScaleIndependent)
        {
            if (inversedInterpolation)
            {
                for (float f = translationDuration; f > 0; f -= 0.03125f) // 32 calls per second
                {
                    GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(endPosition, startPosition, func(f * translationDurationInversed));
                    yield return new WaitForSecondsRealtime(0.03125f);
                }
            }
            else
            {
                for (float f = 0; f < translationDuration; f += 0.03125f)
                {
                    GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(startPosition, endPosition, func(f * translationDurationInversed));
                    yield return new WaitForSecondsRealtime(0.03125f);
                }
            }
        }
        else
        {
            if (inversedInterpolation)
            {
                for (float f = translationDuration; f > 0; f -= Time.deltaTime)
                {
                    GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(endPosition, startPosition, func(f * translationDurationInversed));
                    yield return null;
                }
            }
            else
            {
                for (float f = 0; f < translationDuration; f += Time.deltaTime)
                {
                    GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(startPosition, endPosition, func(f * translationDurationInversed));
                    yield return null;
                }
            }
        }
        GetComponent<RectTransform>().anchoredPosition = endPosition;
    }

    private void OnDrawGizmosSelected()
    {
        switch (line)
        {
            case lines.Top:
                {
                    bias = new Vector2(0, stepSize);
                    break;
                }
            case lines.Right:
                {
                    bias = new Vector2(stepSize, 0);
                    break;
                }
            case lines.Local_Center:
                {
                    bias = defaultPosition.normalized * stepSize;
                    break;
                }
            case lines.Bottom_Right:
                {
                    bias = new Vector2(stepSize, -stepSize);
                    break;
                }
            case lines.Top_Right:
                {
                    bias = new Vector2(stepSize, stepSize);
                    break;
                }
        }
        Gizmos.color = new Color(0, 1, 1, 1);
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(bias.x * transform.lossyScale.x, bias.y * transform.lossyScale.y, 0));
    }
}
