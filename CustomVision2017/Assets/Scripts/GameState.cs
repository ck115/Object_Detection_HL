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
    private Stopwatch stopwatch = new Stopwatch();

    public string state = "shape";

    private string slotURL = "https://northcentralus.api.cognitive.microsoft.com/customvision/v3.0/Prediction/1bd693cc-7d07-4b9c-9e64-262f96e2c6d8/detect/iterations/Slots1/image";

    private string slotKey = "bf691dd332aa404c8c5d05a0523d8299";

    //these are not right but not big deal yet
    private string shapeURL = "https://southcentralus.api.cognitive.microsoft.com/customvision/v3.0/Prediction/042f48c8-3d70-4771-a0c5-6403eb08c680/detect/iterations/Block-detection/image";

    private string shapeKey = "0f949d22085b434baeff5f670f4247d0";

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
        //should call message system here 
    }

    //removes the last shape that was completed 
    public bool removeLast()
    {
        if (count != 0)
        {
            Shape toRemove = completed[count];
            UnityEngine.Debug.LogFormat(toRemove.name);
            completed.RemoveAt(count);
            count--;
            return true;
            //call message system with toRemove being removed and successul 
        }
        else
        {
            return false;
            //call mesage system saying there is no items to remove
        }
  
    }

    void completeGame()
    {
        StringBuilder sb = new StringBuilder();
        foreach(var i in completed)
        {
            string line = string.Format("{0},{1}\n", i.name, i.timeCompleted);
            sb.Append(line);
        }

        File.WriteAllText("", sb.ToString());
    }

}
