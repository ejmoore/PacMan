using UnityEngine;
using System.Collections;
using UnityEngine.Windows.Speech;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;

public class VoiceManager : MonoBehaviour {

    KeywordRecognizer interpreter = null;
    Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();

	// Use this for initialization
	void Start () {
        keywords.Add("Audio", () =>
        {
            Debug.Log("Audio Keyword Recognized, Loading Audio Level");
            SceneManager.LoadScene("AudioLevel");
        });

        keywords.Add("Visual", () =>
        {
            SceneManager.LoadScene("VisualLevel");
        });

        interpreter = new KeywordRecognizer(keywords.Keys.ToArray());

        interpreter.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        interpreter.Start();
    }

    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args) {
        System.Action action;
        if (keywords.TryGetValue(args.text, out action)) {
            action.Invoke();
        }
        interpreter.Stop();
    }
}
