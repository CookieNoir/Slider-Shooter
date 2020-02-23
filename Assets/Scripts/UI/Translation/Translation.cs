using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
[AddComponentMenu("Game UI/Translation")]
public class Translation : MonoBehaviour
{
    public static Dictionary<string, string> sentenceDictionary;
    public static Dictionary<string, string> wordDictionary;

    public delegate void OnLanguageChange();
    public static event OnLanguageChange LanguageChange;

    private static Translation instance;

    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);

            sentenceDictionary = new Dictionary<string, string>();
            wordDictionary = new Dictionary<string, string>();

            SetLanguage(PlayerPrefs.GetString("Language", "Localization/Russian"));

            instance = this as Translation;
        }
        else Destroy(this);
    }

    public static void SetLanguage(string path)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(path);
        string fileText = textAsset.text;

        int index = 0, endIndex = 0, length = fileText.Length;
        string keyString, textString;
        while (index < length)
        {
            switch (fileText[index])
            {
                case '#':   // Метка предложения
                    {
                        index++;
                        endIndex = fileText.IndexOf(' ', index);
                        if (endIndex != -1)
                        {
                            keyString = fileText.Substring(index, endIndex - index);
                            index = endIndex + 1;
                            endIndex = IndexOfNewLine(fileText, index);
                            if (endIndex == -1)
                                endIndex = length;
                            textString = fileText.Substring(index, endIndex - index);

                            sentenceDictionary[keyString] = textString;
                            //Debug.Log("Sentence added: " + keyString + " = " + textString);
                            index = endIndex;
                        }
                        else
                            index = length;
                        break;
                    }
                case '@':   // Метка стандартного слова
                case '$':   // Метка дополнительного слова
                    {
                        index++;
                        endIndex = fileText.IndexOf(' ', index);
                        if (endIndex != -1)
                        {
                            keyString = fileText.Substring(index, endIndex - index);
                            index = endIndex + 1;
                            endIndex = IndexOfNewLine(fileText, index);
                            textString = fileText.Substring(index, endIndex - index);

                            wordDictionary[keyString] = textString;
                            //Debug.Log("Word added: "+keyString+" = "+textString);
                            index = endIndex;
                        }
                        else
                            index = length;
                        break;
                    }
                case '\r':
                case '\n':
                    {
                        index++;
                        break;
                    }
                default:
                    {
                        index = IndexOfNewLine(fileText, index);
                        break;
                    }
            }
        }

        PlayerPrefs.SetString("Language", path);

        LanguageChange?.Invoke();
    }

    private static int IndexOfNewLine(string textString, int startIndex)
    {
        int i = startIndex, length = textString.Length;
        while (i < length && textString[i] != '\r' && textString[i] != '\n')
        {
            i++;
        }
        return i;
    }
}
