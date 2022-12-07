using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

//Loading the latest image captured as an array of bytes.

//Sending the byte array to your Azure Custom Vision Service instance for analysis.

//Receiving the response as a JSON string.

//Deserializing the response and passing the resulting Prediction to the SceneOrganiser class,
//which will take care of how the response should be displayed.


public class CustomVisionAnalyser : MonoBehaviour {

    //used to create a singleton of the this class 
    public static CustomVisionAnalyser Instance;

   
    //Byte array for the image for submission to the API ( hide in inspector prevents viewing in unity) 
    [HideInInspector] private byte[] byteImage;

    void Awake()
    {
        Instance = this;
    }

    //IEnumberator acts as an object ot parse through something 
    public IEnumerator AnalyseLastImageCaptured (string imagePath)
    {
        //calls to the debug functionalities of unity 
        Debug.Log("Anaylzing....");

        //gets the current state, endpoint, and key from gamestate
        string gstate = GameState.Instance.state;
        string predictionEndpoint = GameState.Instance.getURL(gstate);
        string predictionKey = GameState.Instance.getKey(gstate);

        //helper class to send post data to API 
        WWWForm webForm = new WWWForm();

        //UnityWEbREquest handles communication to web servers 
        using(UnityWebRequest unityWebRequest = UnityWebRequest.Post(predictionEndpoint, webForm))
        {

            byteImage = GetImageAsByteArray(imagePath);

            unityWebRequest.SetRequestHeader("Content-type", "application-type/octect-stream");
            unityWebRequest.SetRequestHeader("Prediction-Key", predictionKey);

            // The upload handler will help uploading the byte array with the request
            //uploadHandler formats the body data 
            unityWebRequest.uploadHandler = new UploadHandlerRaw(byteImage);
            unityWebRequest.uploadHandler.contentType = ("application-type/octect-stream");

            // The download handler will help receiving the analysis from Azure
            unityWebRequest.downloadHandler = new DownloadHandlerBuffer();

            //provides value to a enumerator object being the value given within a loop
            yield return unityWebRequest.SendWebRequest();

            //download handler just recieves the data in the response body of the request
            string jsonResponse = unityWebRequest.downloadHandler.text;

            Debug.Log("response:" + jsonResponse);

            //Create a texture. this is will be replaced by incoming image 
            Texture2D txt = new Texture2D(1, 1);
            txt.LoadImage(byteImage);
            if(GameState.Instance.state == "slot")
            {
                SceneOrganiser.Instance.quadRenderer.material.SetTexture("_MainTex", txt);
            }

            //respon will be in Json formt, therfore it will have to be deserialized
            AnalysisRootObject analysisRootObject = new AnalysisRootObject();
            analysisRootObject = JsonConvert.DeserializeObject<AnalysisRootObject>(jsonResponse);

            if(gstate == "shape")
            {
                Step.Instance.changeContent("Shape", "Analyzing picture taken, searching for shapes");
            }
            else
            {
                Step.Instance.changeContent("Slot", "Analyzing picture taken, searching for slots");
            }

            string shape = SceneOrganiser.Instance.FinalizeLabel(analysisRootObject, gstate);

            Debug.Log(shape);

            if (gstate == "shape")
            {

                if(shape != "none")
                {
                    GameState.Instance.state = "slot";

                    GameState.Instance.setShape(shape);
                }
                else
                {
                    Step.Instance.changeContent("Shape","No Object found, try to \"click\" again");
                }
            }
            else
            {
                if(shape == "none")
                {
                    Step.Instance.changeContent("Slot ", "No Object found, try to \"click\" again");
                }
            }


        }
    }

    public byte[] GetImageAsByteArray(string imagePath)
    {
        FileStream fileStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read);

        BinaryReader binaryReader = new BinaryReader(fileStream);

        //type casting the length to in and reading bytes of the file stream by length
        return binaryReader.ReadBytes((int)fileStream.Length);
    }

}
