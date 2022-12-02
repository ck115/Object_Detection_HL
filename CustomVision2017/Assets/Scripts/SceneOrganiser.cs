
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//might need to pass values for determining if searching for shape or value 


//Set up the Main Camera by attaching the appropriate components to it.

//When an object is detected, it will be responsible for calculating its position in the real world
//and place a Tag Label near it with the appropriate Tag Name.

public class SceneOrganiser : MonoBehaviour {

    public static SceneOrganiser Instance;

    //internal limits access to the current assembly
    //cursor object attached to the Main Camera 
    internal GameObject cursor;

    //Label used to display the analysis on objects 
    public GameObject label;

    //Transform as a object is the property of an object with its movement and stuff 
    //Every object has a transform property and it is used to manipulate position and rotation
    //Reference to last label placed 
    internal Transform lastLabelPlaced;

    //text mesh contains properties of actual text 
    //reference to last label placed
    internal TextMesh lastLabelPlacedText;

    //Curent threshhold to displaying a label
    internal float probabilityThreshold = 0.3f;

    //The quad object hosting the imposed image captured 
    //It is our bounding box basically, it will appear around our objects based on what is returned from custom vision 
    private GameObject quad;

    //renderer of quad Object
    // makes what a object appears 
    internal Renderer quadRenderer;


    void Awake()
    {
        Instance = this;

        //add the image class to game object 
        gameObject.AddComponent<ImageCapture>();

        //add custom vision analyser to object 
        gameObject.AddComponent<CustomVisionAnalyser>();

        //add custiom vision object to object 
        gameObject.AddComponent<CustomVisionObjects>();

        gameObject.AddComponent<VoiceRecognizer>();

        gameObject.AddComponent<GameState>();

    }

    //Instantiate the label in the scene(which at this point is invisible to the user). 
    //It also places the quad(also invisible) where the image is placed, and overlaps with the real world.
    //This is important because the box coordinates retrieved from the Service 
    //after analysis are traced back into this quad to determined the approximate location of the object in the real world.

    public void PlaceAnalysisLabel()
    {
        //instantiate can be used to create a new object at run time 
        lastLabelPlaced = Instantiate(label.transform, cursor.transform.position, transform.rotation);
        lastLabelPlacedText = lastLabelPlaced.GetComponent<TextMesh>();
        lastLabelPlacedText.text = "";
        lastLabelPlaced.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);

        //create a game object to which the texture can be applied 
        quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quadRenderer = quad.GetComponent<Renderer>() as Renderer;


        Material m = Resources.Load("Transp", typeof(Material)) as Material;
        quadRenderer.material = m;

        //tranparency of the quad , the color is rgb the transpaerncy 
        float transparency = 0f;
        quadRenderer.material.color = new Color(1, 1, 1, transparency);

        //sets the position and scale of the quad depending on user position (tranform being the player i think)
        quad.transform.parent = transform;
        quad.transform.rotation = transform.rotation;

        //quad placed slightly in front of user 
        quad.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);

        // The quad scale as been set with the following value following experimentation,  
        // to allow the image on the quad to be as precisely imposed to the real world as possible
        quad.transform.localScale = new Vector3(3f, 1.65f, 1f);
        quad.transform.parent = null;
    }

    //Setting the Label text with the Tag of the Prediction with the highest confidence.
    //Calling the calculation of the Bounding Box on the quad object, positioned previously, and placing the label in the scene.
    //Adjusting the label depth by using a Raycast towards the Bounding Box, which should collide against the object in the real world.
    //Resetting the capture process to allow the user to capture another image.

    //Need to add param for the state that is currently being found (shape will recognize and return only what is found, slot makes cub)
    public string FinalizeLabel(AnalysisRootObject analysisObject,string gstate)
    {
        if (analysisObject != null)
        {
            lastLabelPlacedText = lastLabelPlaced.GetComponent<TextMesh>();
            //sort the predictions to locate the highest one 
            List<Prediction> sortedPredictions = new List<Prediction>();
            sortedPredictions = analysisObject.predictions.OrderBy(p => p.probability).ToList();
            Prediction bestPrediction = new Prediction();
            bestPrediction = sortedPredictions[sortedPredictions.Count - 1];
            /* 
             *  Will display the highets probablity, Controller should be able to handle which we are referencing. Might
             *  be easier to just make two models lol
             *  */
            if(bestPrediction.probability > probabilityThreshold)
            {
                if (gstate == "shape")
                {
                    return bestPrediction.tagName;
                }
                else
                {
                    quadRenderer = quad.GetComponent<Renderer>() as Renderer;
                    Bounds quadBounds = quadRenderer.bounds;

                    //Position of the label is placed where the quad is now.
                    //at this point depth is not considered 
                    lastLabelPlaced.transform.parent = quad.transform;

                    //this is calculating the box that will be placed on the label so this will need to be changed a bit
                    lastLabelPlaced.transform.localPosition = CalculateBoundingBoxPosition(quadBounds, bestPrediction.boundingBox);

                    lastLabelPlacedText.text = bestPrediction.tagName;

                    // Cast a ray from the user's head to the currently placed label, it should hit the object detected by the Service.
                    // At that point it will reposition the label where the ray HL sensor collides with the object,
                    // (using the HL spatial tracking)
                    //need to figure this out

                    Debug.Log("Repositioning Label");
                    Vector3 headPosition = Camera.main.transform.position;
                    RaycastHit objHitInfo;
                    Vector3 objDirection = lastLabelPlaced.position;
                    if (Physics.Raycast(headPosition, objDirection, out objHitInfo, 30.0f, SpatialMapping.PhysicsRaycastMask))
                    {
                        lastLabelPlaced.position = objHitInfo.point;
                    }


                    //reset color of cursor 
                    cursor.GetComponent<Renderer>().material.color = Color.green;

                    //stop the anylisis process 
                    ImageCapture.Instance.ResetImageCapture();

                    return bestPrediction.tagName;
                }

            }
            else
            {
                //reset color of cursor 
                cursor.GetComponent<Renderer>().material.color = Color.green;

                //stop the anylisis process 
                ImageCapture.Instance.ResetImageCapture();

                return "none";
            }


        }

        return "none";


    }

    // This method hosts a series of calculations to determine the position 
    // of the Bounding Box on the quad created in the real world
    /// by using the Bounding Box received back alongside the Best Prediction
    public Vector3 CalculateBoundingBoxPosition(Bounds b, BoundingBox boundingBox)
    {
        Debug.Log($"BB: left {boundingBox.left}, top {boundingBox.top}, width {boundingBox.width}, height {boundingBox.height}");

        double centerFromLeft = boundingBox.left + (boundingBox.width / 2);
        double centerFromTop = boundingBox.top + (boundingBox.height / 2);
        Debug.Log($"BB CenterFromLeft {centerFromLeft}, CenterFromTop {centerFromTop}");

        double quadWidth = b.size.normalized.x;
        double quadHeight = b.size.normalized.y;
        Debug.Log($"Quad Width {b.size.normalized.x}, Quad Height {b.size.normalized.y}");

        double normalisedPos_X = (quadWidth * centerFromLeft) - (quadWidth / 2);
        double normalisedPos_Y = (quadHeight * centerFromTop) - (quadHeight / 2);

        return new Vector3((float)normalisedPos_X, (float)normalisedPos_Y, 0);
    }


}
