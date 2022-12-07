using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class VoiceRecognizer : MonoBehaviour {


    //for singleton
    public static VoiceRecognizer Instance;

    //recognizer object (windows input stuff)
    private KeywordRecognizer keywordRecognizer;

    //dictionary used for mapping 
    public Dictionary<string, Action> actions = new Dictionary<string, Action>();


    // Use this for initialization

    void Awake()
    {
        Instance = this;
    }
    void Start () {

        bool capture = ImageCapture.Instance.captureIsActive;

        //adds text to the dictionary to be recognized with its coresponding function to be called
        actions.Add("next", Next);
        actions.Add("reset", ResetState);
        actions.Add("back", Back);
        actions.Add("complete", Complete);

        //takes the keys of the dictionary and starts listening to the user
        keywordRecognizer = new KeywordRecognizer(actions.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += RecgonizedSpeech;
        keywordRecognizer.Start();
    
	}

    void RecgonizedSpeech(PhraseRecognizedEventArgs speech)
    {
        //invoke will take the text that is recognized and call the function with the same text
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
        }
        else if(capture)
        {
            //alerts if capture is happening so cannot make change 
            Alert.Instance.changeContent("Error", "Cannot remove object while capture is active");

        }
        else
        {
            //alerts if the state is not correct for this function
            Alert.Instance.changeContent("Error", "cannot complete shape while in Shape mode");
        }

    }

    void ResetState()
    {
        string state = GameState.Instance.state;

        bool capture = ImageCapture.Instance.captureIsActive;

        if (!capture && state=="slot")
        {
            GameState.Instance.resetShapeAndTimer();
            
        }
        else if (capture)
        {
            //alerts if capture is happening so cannot make change 
            Alert.Instance.changeContent("Error", "Cannot reset while capture is active");
        }
        else
        {
            //alerts if the state is not right
            Alert.Instance.changeContent("Error", "Cannot remove shape while in Shape state");
        }
    }


    //This function will try and remove the object that was last entered in the system
    void Back()
    {
        string state = GameState.Instance.state;

        bool capture = ImageCapture.Instance.captureIsActive;

        if (!capture)
        {
            bool success = GameState.Instance.removeLast();
            Debug.LogFormat("removal of object" + success);
        }
        else
        {
            //alerts if capture is happening so cannot make change 
            Alert.Instance.changeContent("Error", "Cannot remove object while capture is active");
        }
    }

    void Complete()
    {
        GameState.Instance.completeGame();
    }
}
