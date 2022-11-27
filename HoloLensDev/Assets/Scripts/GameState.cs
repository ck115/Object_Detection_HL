using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;

//just a basic structure to represent value of stuff 
public class Shape
{
    public string name;
    public double timeCompleted; 

    public Shape(string shapeName,double time)
    {
        name = shapeName;
        timeCompleted = time;
    }
}

public class GameState : MonoBehaviour {

    public static GameState Instance;

    private Stopwatch stopwatch = new Stopwatch();

    private string state = "shape"; 
    public string State
    {
        get { return state; }
        set { state = value; }
    }

    private string slotURL;

    private string slotKey;

    //these are not right but not big deal yet
    private string shapeURL = "https://northcentralus.api.cognitive.microsoft.com/customvision/v3.0/Prediction/9325afe6-4969-4791-ac6f-cf1520fe4ac2/detect/iterations/Watch%20Test/image";

    private string shapeKey = "bf691dd332aa404c8c5d05a0523d8299";

    private Shape shape = null;

    private int count = 0;

    List<Shape> completed = new List<Shape>();
    

    //Instantiates as a singleton 
    void Awake()
    {
        Instance = this; 
    }

    //based on the state that is passed the shape and slot URL is returned
    public string getURL (string state)
    {
        if(state == "slot")
        {
            return slotURL;
        }
        else
        {
            return shapeURL;
        }
    }
    

    //based on the state that is passed the respective shape or slot key is returend
    public string getKey(string state)
    {
        if(state == "slot")
        {
            return slotKey;
        }
        else
        {
            return shapeKey;
        }
    }

    public void setShape(string name)
    {
        //need to implment timer here to start 

        shape = new Shape(name, 0);
        stopwatch.Start();
    }

    public void addShape()
    {
        //ends stopwatch and gets the time
        stopwatch.Stop();
        TimeSpan timeState = stopwatch.Elapsed;

        //adds time completed in seconds 
        shape.timeCompleted = timeState.TotalSeconds;
        completed.Add(shape);
        count++;

        //resets stuff
        shape = null;
        stopwatch.Reset();

        
    }

    //will only be able used during the shape state
    //can use get in the voice recognizer area
    public void resetShapeAndTimer()
    {
        stopwatch.Stop();
        stopwatch.Reset();
        shape = null;
        state = "slot";
    }

    //removes the last shape that was completed 
    public bool removeLast()
    {
        if (count != 0)
        {
            completed.RemoveAt(count);
            return true;
        }
        else
        {
            return false;
        }
  
    }

    

}
