using System.Collections;
using UnityEngine;
[AddComponentMenu("Game UI/UI Movement")]
[RequireComponent(typeof(RectTransform))]
public class UiMovement : MonoBehaviour
{
    [Header("Moving At Start")] // Необходимо, чтобы возвращать отлаженный UI элемент на необходимую позицию
    public bool moveAtStart = false;
    public Vector2 startPosition;
    public enum lines { Top, Right, Local_Center, Bottom_Right, Top_Right };
    public enum functions { Linear, Square, Cubic };
    private delegate float movingFunc(float value);
    //---------------------------------
    [Header("Translation")]
    public lines line;
    public functions function;
    public float stepSize;

    private bool moved = false; // If moved, makes a step back
    private Vector2 defaultPosition;
    private Vector2 newPosition;
    private Vector2 bias;
    private movingFunc movingFunction;
    private IEnumerator activeMovement;
    //---------------------------------
    private void Awake()
    {
        if (moveAtStart) GetComponent<RectTransform>().anchoredPosition = startPosition;
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
                    movingFunction = Linear;
                    break;
                }
            case functions.Square:
                {
                    movingFunction = Square;
                    break;
                }
            case functions.Cubic:
                {
                    movingFunction = Cubic;
                    break;
                }
        }
        activeMovement = SmoothMove(newPosition, defaultPosition, movingFunction);
    }

    private float Linear(float f)
    {
        return f;
    }
    private float Square(float f)
    {
        return f * f;
    }
    private float Cubic(float f)
    {
        return f * f * f;
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
        for (float f = 1; f > 0; f -= 0.03125f)
        {
            GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(endPosition, startPosition, func(f));
            yield return new WaitForSecondsRealtime(0.03125f);
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
        Gizmos.color = new Color(0,1,1,1);
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(bias.x*transform.lossyScale.x,bias.y * transform.lossyScale.y, 0));
    }
}
