using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
[AddComponentMenu("Game UI/Translation")]
public class Translation : MonoBehaviour
{
    public static Dictionary<string, string> wordDictionary;

    public delegate void OnLanguageChange();
    public static event OnLanguageChange LanguageChange;

    private static Translation instance;

    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            wordDictionary = new Dictionary<string, string>();
            SetLanguage(PlayerPrefs.GetString("Language", "Localization/Russian"));
            instance = this as Translation;
        }
        else Destroy(this);
    }

    public static void SetLanguage(string path)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(path);
        string text = textAsset.text;
        string[] textLines = Regex.Split(text, "\n|\r|\r\n");

        for (int i = 2; i < textLines.Length; i += 2)
        {
            int index = textLines[i].IndexOf(' ');
            string id = textLines[i].Substring(0, index);
            string word = textLines[i].Substring(index + 1, textLines[i].Length - index - 1);
            wordDictionary[id] = word;
        }

        PlayerPrefs.SetString("Language", path);

        LanguageChange?.Invoke();
    }
}
