using UnityEngine;
[AddComponentMenu("Game UI/Translatable text/Single Word")]
public class TranslatableWord : TranslatableText
{
    public string key;

    protected override void TranslateWords()
    {
        if (outputText.Count == 0) outputText.Add(Translation.wordDictionary[key]);
        else outputText[0] = (Translation.wordDictionary[key]);
    }

    protected override void RefreshText()
    {
        textComponent.text = outputText[0];
    }
}
