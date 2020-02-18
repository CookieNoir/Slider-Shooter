using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[AddComponentMenu("Game UI/Translatable text/Simple")]
[RequireComponent(typeof(Text))]
public class TranslatableText : MonoBehaviour
{
    private Text textComponent;
    public List<string> translatableWords;
    private List<string> outputText;

    protected virtual void Start()
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
        foreach (string word in translatableWords)
        {
            output += Translation.wordDictionary[word]+' ';
        }
        outputText.Add(output);
        
    }

    protected virtual void RefreshText()
    {
        textComponent.text = outputText[0];
    }

    protected virtual void OnLanguageChange()
    {
        TranslateWords();
        RefreshText();
    }

    protected void OnDestroy()
    {
        Translation.LanguageChange -= OnLanguageChange;
    }
}
