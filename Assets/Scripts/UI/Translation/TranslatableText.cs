using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[AddComponentMenu("Game UI/Translatable text/Simple")]
[RequireComponent(typeof(Text))]
public class TranslatableText : MonoBehaviour
{
    protected Text textComponent;
    public List<string> staticTranslatableWords;
    protected List<string> outputText;

    protected void Start()
    {
        textComponent = GetComponent<Text>();
        outputText = new List<string>();
        TranslateWords();
        RefreshText();
        Translation.LanguageChange += OnLanguageChange;
    }

    protected virtual void TranslateWords()
    {
        string output = "";
        foreach (string word in staticTranslatableWords)
        {
            output += Translation.wordDictionary[word]+' ';
        }
        if (outputText.Count < 1) outputText.Add(output);
        else outputText[0] = output;
    }

    protected virtual void RefreshText()
    {
        textComponent.text = outputText[0];
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
