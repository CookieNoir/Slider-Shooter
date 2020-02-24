using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Text))]
public class TranslatableText : MonoBehaviour
{
    [HideInInspector] public Text textComponent;
    protected List<string> outputText;

    protected void Start()
    {
        textComponent = GetComponent<Text>();
        outputText = new List<string>();
        OnLanguageChange();
        Translation.LanguageChange += OnLanguageChange;
    }

    protected virtual void TranslateWords()
    {
    }

    protected virtual void RefreshText()
    {
    }

    protected void OnLanguageChange()
    {
        TranslateWords();
        RefreshText();
    }

    protected void OnDestroy()
    {
        Translation.LanguageChange -= OnLanguageChange;
    }
}
