using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("Game UI/Translatable text/Words List")]
public class TranslatableWords : TranslatableText
{
    public List<string> keys;

    protected override void TranslateWords()
    {
        string output = string.Empty;
        int index = 0;
        while (index < keys.Count)
        {
            output += Translation.wordDictionary[keys[index]] + ' ';
            index++;
        }
        if (outputText.Count == 0) outputText.Add(output);
        else outputText[0] = output;
    }

    protected override void RefreshText()
    {
        textComponent.text = outputText[0];
    }
}
