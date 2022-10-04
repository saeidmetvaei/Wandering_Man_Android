using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using static SpeechRecognizerPlugin;

public class SpeechRecognizer : MonoBehaviour, SpeechRecognizerPlugin.ISpeechRecognizerPlugin
{
    //[SerializeField] private Button startListeningBtn = null;
    //[SerializeField] private Button stopListeningBtn = null;
    //[SerializeField] private Toggle continuousListeningTgle = null;
    //[SerializeField] private TMP_Dropdown languageDropdown = null;
    //[SerializeField] private TMP_InputField maxResultsInputField = null;
    //[SerializeField] private Text resultsTxt;
    //[SerializeField] private Text errorsTxt;
    //[SerializeField] private Text Commandtxt;

    private SpeechRecognizerPlugin plugin = null;


    private void Start()
    {
        plugin = SpeechRecognizerPlugin.GetPlatformPluginVersion(this.gameObject.name);

        int maxResults = 10;
        plugin.SetMaxResultsForNextRecognition(maxResults);

        plugin.SetLanguageForNextRecognition("en-US");


        //startListeningBtn.onClick.AddListener(StartListening);
        //stopListeningBtn.onClick.AddListener(StopListening);
        //continuousListeningTgle.onValueChanged.AddListener(SetContinuousListening);
        //languageDropdown.onValueChanged.AddListener(SetLanguage);
        //maxResultsInputField.onEndEdit.AddListener(SetMaxResults);
    }


    private void Update()
    {
        StartListening();
    }

    private void StartListening()
    {
        plugin.StartListening();

       // resultsTxt.text = "listening is started";
    }

    private void StopListening()
    {
        plugin.StopListening();
    }

    //private void SetContinuousListening(bool isContinuous)
    //{
    //    plugin.SetContinuousListening(isContinuous);
    //}

    //private void SetLanguage()
    //{
    // //   string newLanguage = languageDropdown.options[dropdownValue].text;
    //    plugin.SetLanguageForNextRecognition("en-US");

    //}

    //private void SetMaxResults(string inputValue)
    //{
    //    if (string.IsNullOrEmpty(inputValue))
    //        return;

    //    int maxResults = int.Parse(inputValue);
    //    plugin.SetMaxResultsForNextRecognition(maxResults);
    //}

    public void OnResult(string recognizedResult)
    {
        char[] delimiterChars = { '~' };
        string[] result = recognizedResult.Split(delimiterChars);

       // resultsTxt.text = "";
        for (int i = 0; i < result.Length; i++)
        {
           // resultsTxt.text += result[i] + '\n';
        }


        // check for action
        foreach (var item in result)
        {
            if (item == "change")
            {
                if (SceneManager.GetActiveScene().buildIndex == 0)
                {
                    SceneManager.LoadScene(1);
                }
                else
                {
                    SceneManager.LoadScene(0);
                }

                //Commandtxt.text = "Command Recognized";
            }
        }
    }

    public void OnError(string recognizedError)
    {
        SpeechRecognizerPlugin.ERROR error = (SpeechRecognizerPlugin.ERROR)int.Parse(recognizedError);
        switch (error)
        {
            case SpeechRecognizerPlugin.ERROR.UNKNOWN:
                Debug.Log("<b>ERROR: </b> Unknown");
               // errorsTxt.text += "Unknown";
                break;
            case SpeechRecognizerPlugin.ERROR.INVALID_LANGUAGE_FORMAT:
                Debug.Log("<b>ERROR: </b> Language format is not valid");
               // errorsTxt.text += "Language format is not valid";
                break;
            default:
                break;
        }
    }
}
