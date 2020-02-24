using System;
using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("Game UI/Translatable text/Sentence")]
public class TranslatableSentence : TranslatableWord
{
    [Serializable]
    public class DynamicWords
    {
        public string value;
        public List<int> entries;
    }

    public List<DynamicWords> dynamicWords;
    public bool allowEndLines;

    protected override void TranslateWords()
    {
        outputText.Clear();
        string  sentence = Translation.sentenceDictionary[key],
                output = string.Empty, wordKey;
        int index = 0, endIndex = 0, outputIndex = 0,
            length = sentence.Length, dynamicNumber, dynamicIndex = 0;
        while (index < length)
        {
            switch (sentence[index])
            {
                case '@':
                case '$':
                    {
                        endIndex = sentence.IndexOf(' ', index);
                        index++;
                        if (endIndex == -1) endIndex = length;
                        wordKey = sentence.Substring(index, endIndex - index);
                        output += Translation.wordDictionary[wordKey] + ' ';
                        break;
                    }
                case '*':
                    {
                        outputText.Add(output);
                        output = string.Empty;
                        outputIndex++;

                        endIndex = sentence.IndexOf(' ', index);
                        index++;
                        if (endIndex == -1) endIndex = length;
                        if (index != endIndex)
                        {
                            wordKey = sentence.Substring(index, endIndex - index);
                            dynamicNumber = Convert.ToInt32(wordKey);
                            dynamicWords[dynamicNumber].entries.Add(outputIndex);
                        }
                        else
                        {
                            dynamicWords[dynamicIndex].entries.Add(outputIndex);
                            dynamicIndex++;
                        }
                        index = endIndex + 1;
                        outputText.Add(output);
                        outputIndex++;
                        break;
                    }
                default:
                    {
                        endIndex = sentence.IndexOf(' ', index);
                        if (endIndex == -1) endIndex = length;
                        output += sentence.Substring(index, endIndex - index) + ' ';
                        break;
                    }
            }
            index = endIndex + 1;
        }
        if (output.Length > 0)
        {
            outputText.Add(output);
        }
    }

    protected override void RefreshText()
    {
        string output = string.Empty;
        int dynamicWordIndex = 0,
            index = 0,
            count;
        while (dynamicWordIndex < dynamicWords.Count)
        {
            index = 0;
            count = dynamicWords[dynamicWordIndex].entries.Count;
            while (index < count)
            {
                outputText[dynamicWords[dynamicWordIndex].entries[index]] = dynamicWords[dynamicWordIndex].value + ' ';
                index++;
            }
            dynamicWordIndex++;
        }
        index = 0;
        while (index < outputText.Count)
        {
            output += outputText[index];
            index++;
        }
        if (allowEndLines) output = output.Replace('^', '\n');
        textComponent.text = output;
    }
}
