using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;
using System.IO;
using System.Text;

//just a basic structure to represent value of stuff 
//name: the shape that to be found in the thingy 
//timeCompleted: the total time that it took to compelete this shape
public class Shape
{
    public string name;
    public double timeCompleted; 

    //basic constructor for the obj
    public Shape(string shapeName,double time)
    {
        name = shapeName;
        timeCompleted = time;
    }
}

public class GameState : MonoBehaviour {
    
    //instance of the game state as singleton 
    public static GameState Instance;

    //used to keep track of time
    private Stopwatch stopwatch;

    //string that keeps track of the state the game is in
    public string state;

    //respective string for the endpoint and key for the slot model
    private string slotURL = "https://northcentralus.api.cognitive.microsoft.com/customvision/v3.0/Prediction/1bd693cc-7d07-4b9c-9e64-262f96e2c6d8/detect/iterations/Slots1/image";
    private string slotKey = "bf691dd332aa404c8c5d05a0523d8299";

    //rspective strings for the endpoint and key for the shape model
    private string shapeURL = "https://southcentralus.api.cognitive.microsoft.com/customvision/v3.0/Prediction/042f48c8-3d70-4771-a0c5-6403eb08c680/detect/iterations/block/image";
    private string shapeKey = "ef9310649d5d4bc7941a40d42ff394b6";

    private Shape shape = null;

    //keeps track of how many shapes have been completed
    private int count;

    List<Shape> completed;
    

    //Instantiates as a singleton 
    void Awake()
    {
        //instantiates the singleton and other stuff
        Instance = this;

        stopwatch = new Stopwatch();

        completed = new List<Shape>();

        count = -1;

        state = "shape";

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
    
    //called by custom vision analyzer if object is detected while in shape state
    public void setShape(string name)
    {

        shape = new Shape(name, 0);
        stopwatch.Start();

        //out to alert system
        Alert.Instance.changeContent("Shape Found", "The shape " + name + "was found, look for slot");

        //out to step system
        Step.Instance.changeContent("Slot", "Use \"click\" command to find slot");

    }

    public Shape getShape()
    {
        return shape;
    }

    public void addShape()
    {


        //ends stopwatch and gets the time
        stopwatch.Stop();
        TimeSpan timeState = stopwatch.Elapsed;

        //adds time completed in seconds, then adds the shape to the end of the list 
        shape.timeCompleted = timeState.TotalSeconds;
        completed.Add(shape);
        count++;

        //creates an alert that notifies how long it took to complete the game 
        Alert.Instance.changeContent(shape.name + " completed ", shape.name + " took " + shape.timeCompleted + " to finish");

        //updates step for new in structions
        Step.Instance.changeContent("Shape", "Use \"click\" command to find shape");

        //resets stuff and will set state to shape
        shape = null;
        stopwatch.Reset();
        state = "shape";
    }

    //will only be able used during the slot state
    //will reset the state back to shape and remove the currently added shape
    public void resetShapeAndTimer()
    {
        //gets name of current shape being searched for on slots
        //resets timer and shape
        //converts state to shape
        string name = shape.name; 
        stopwatch.Stop();
        stopwatch.Reset();
        shape = null;
        state = "shape";

        //reaches out to alert and step ui to display change
        Alert.Instance.changeContent("Reset"  + name, name + " is no longer being searched for");
        Step.Instance.changeContent("Shape", "Use \"click\" command to find slot");

    }

    //removes the last shape that was completed 
    public bool removeLast()
    {
        //checks to see if there is any shapes completed to remove 
        if (count > -1)
        {
            //will remove the shape from the list 
            string toRemove = completed[count].name;
            UnityEngine.Debug.LogFormat(toRemove);
            completed.RemoveAt(count);
            count--;

            //calling out to the alert system 
            toRemove = toRemove + " no longer completed";
            Alert.Instance.changeContent(toRemove, "must locate and try again");

            return true;
        }
        else
        {
            Alert.Instance.changeContent("Error", "there are no shapes completed to remove");
            return false;
        }
  
    }

    public void completeGame()
    {
        StringBuilder sb = new StringBuilder();
        foreach(var i in completed)
        {
            string line = string.Format("{0},{1}\n", i.name, i.timeCompleted);
            sb.Append(line);
        }

        File.WriteAllText("C:/App/GamePerformance", sb.ToString());
    }

}
