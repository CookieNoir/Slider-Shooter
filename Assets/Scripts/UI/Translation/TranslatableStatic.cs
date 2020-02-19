using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[AddComponentMenu("Game UI/Translatable text/Static")]
public class TranslatableStatic : TranslatableText
{
    public List<string> staticNonTranslatableWords;
    public string signature;

    protected override void TranslateWords()
    {
        string output = "";
        int tw = 0, // translatableWords Index
            ntw = 0, // nonTranslatableWords Index
            s = 0; // signature Index
        while (s < signature.Length)
        {
            switch (signature[s])
            {
                case 't':
                    {
                        output += Translation.wordDictionary[staticTranslatableWords[tw]] + ' ';
                        tw++;
                        break;
                    }
                case 'n':
                    {
                        output += Translation.wordDictionary[staticNonTranslatableWords[ntw]] + ' ';
                        ntw++;
                        break;
                    }
            }
            s++;
        }
        if (outputText.Count < 1) outputText.Add(output);
        else outputText[0] = output;
    }

    protected override void RefreshText()
    {
        textComponent.text = outputText[0];
    }
}
