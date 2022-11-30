using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class VoiceRecognizer : MonoBehaviour {

    public static VoiceRecognizer Instance;

    private KeywordRecognizer keywordRecognizer;

    public Dictionary<string, Action> actions = new Dictionary<string, Action>();



    // Use this for initialization

    void Awake()
    {
        Instance = this;
    }
    void Start () {

        actions.Add("next", Next);
        actions.Add("reset", ResetState);
        actions.Add("back", Back);

        keywordRecognizer = new KeywordRecognizer(actions.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += RecgonizedSpeech;

    
	}

    void RecgonizedSpeech(PhraseRecognizedEventArgs speech)
    {
        Debug.Log(speech.text);
        actions[speech.text].Invoke();
    }

    //this is to handle when a object has been placed in the whole
    //should prolly debug first as well
    void Next()
    {
        string state = GameState.Instance.State;

        bool capture = ImageCapture.Instance.captureIsActive;

        if(state == "slot" && capture)
        {
            GameState.Instance.addShape();

            GameState.Instance.State = "shape";

            //should call to the UI area to display success or whatever
            //might show what state the game is in (slot will have object as well)
        }

    }

    void ResetState()
    {
        //Will reset all the objects and stuff

        //check to see if image capture 
    }

    void Back()
    {

        bool capture = ImageCapture.Instance.captureIsActive;

        if(capture)
        {
            bool success = GameState.Instance.removeLast();
        }
    }


    // Update is called once per frame
}
