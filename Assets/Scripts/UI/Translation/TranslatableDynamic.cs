using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("Game UI/Translatable text/Dynamic")]
public class TranslatableDynamic : TranslatableStatic
{
    public List<string> dynamicTranslatableWords;
    public List<string> dynamicNonTranslatableWords;
    protected string outputSignature;

    protected override void TranslateWords()
    {
        string output = "";
        outputText.Clear();
        outputSignature = "";
        int stw = 0, // staticTranslatableWords Index
            sntw = 0, // staticNonTranslatableWords Index
            dtw = 0, // dynamicTranslatableWords Index
            dntw = 0, // dynamicNonTranslatableWords Index
            s = 0, // signature Index
            o = 0; // output Counter
        while (s < signature.Length)
        {
            switch (signature[s])
            {
                case 't':
                    {
                        output += Translation.wordDictionary[staticTranslatableWords[stw]] + ' ';
                        stw++;
                        o++;
                        break;
                    }
                case 'n':
                    {
                        output += Translation.wordDictionary[staticNonTranslatableWords[sntw]] + ' ';
                        sntw++;
                        o++;
                        break;
                    }
                case 'T':
                    {
                        if (o > 0)
                        {
                            AddToOutput(ref output);
                            output = "";
                            o = 0;
                        }
                        outputText.Add(Translation.wordDictionary[dynamicTranslatableWords[dtw]] + ' ');
                        dtw++;
                        outputSignature += 'T';
                        break;
                    }
                case 'N':
                    {
                        if (o > 0)
                        {
                            AddToOutput(ref output);
                            output = "";
                            o = 0;
                        }
                        outputText.Add(Translation.wordDictionary[dynamicNonTranslatableWords[dntw]] + ' ');
                        dntw++;
                        outputSignature += 'N';
                        break;
                    }
            }
            s++;
        }
        if (o > 0) AddToOutput(ref output);
    }

    private void AddToOutput(ref string output)
    {
        outputText.Add(output);
        outputSignature += 'o';
    }

    protected override void RefreshText()
    {
        //
    }

    public virtual void UpdateDynamicWords(
        string addSignature,    // Набор символов, определяющий, обновить слово из переводимого или непереводимого динамического списка
                                // Длина этой строки и длина передаваемых массивов должны совпадать
                                // Если длина массивов больше, значения, для которых не определен символ, будут проигнорированы
        int[] wordIndex,        // Каждый элемент - индекс элемента списка, который необходимо заменить
        string[] word           // Каждый элемент - слово, которое будет записано в определенный ключом список с индексом из предыдущего массива
        )
    {
        //
    }
}
