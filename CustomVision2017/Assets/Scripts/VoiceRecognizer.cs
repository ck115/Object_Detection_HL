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

        bool capture = ImageCapture.Instance.captureIsActive;

        actions.Add("next", Next);
        actions.Add("reset", ResetState);
        actions.Add("back", Back);

        keywordRecognizer = new KeywordRecognizer(actions.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += RecgonizedSpeech;
        keywordRecognizer.Start();
    
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
        string state = GameState.Instance.state;

        bool capture = ImageCapture.Instance.captureIsActive; 

        if (state == "slot" && !capture)
        {
            GameState.Instance.addShape();

            GameState.Instance.state = "shape";

            //should call to the UI area to display success or whatever
            //might show what state the game is in (slot will have object as well)
        }
        else
        {
            //should make a call out to the message for captuer 

            //should make a call out to the message for slot 
        }

    }

    void ResetState()
    {
        string state = GameState.Instance.state;

        bool capture = ImageCapture.Instance.captureIsActive;

        if (!capture && state=="shape")
        {
            GameState.Instance.resetShapeAndTimer();

            //call to ui to alert change state and stuff
        }
        else
        {
            //make call if is capturing for message 

            //make calll if is in wrong state 
        }
    }

    void Back()
    {
        string state = GameState.Instance.state;

        bool capture = ImageCapture.Instance.captureIsActive;

        if (!capture)
        {
            bool success = GameState.Instance.removeLast();
        }
        else
        {
            // should make alert that system cannot while capture is active
        }
    }


    // Update is called once per frame
}
