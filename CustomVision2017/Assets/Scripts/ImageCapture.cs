using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.WSA.Input;
using UnityEngine.XR.WSA.WebCam;

//Capturing an image using the HoloLens camera and storing it in the App folder.
//Handling Tap gestures from the user.

public class ImageCapture : MonoBehaviour
{

    //singleton 
    public static ImageCapture Instance;

    //keep track of taps 
    private int captureCount = 0;

    //Photo object 
    private PhotoCapture photoCaptureObject;

    //allows gestures recognition in hololens 
    //Manager class for handling the clicking of objects
    private GestureRecognizer recognizer;

    //flagging if the capture loop is running 
    internal bool captureIsActive;

    //file of current analyzed photo 
    internal string filePath = string.Empty;

    // Use this for initialization
    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Start()
    {
        DirectoryInfo info = new DirectoryInfo(Application.persistentDataPath);
        var fileInfo = info.GetFiles();
        foreach (var file in fileInfo)
        {
            try
            {
                file.Delete();
            }
            catch (Exception)
            {
                Debug.LogFormat("Cannot Delete File: ", file.Name);
            }
        }

        recognizer = new GestureRecognizer();
        //sets what getsures are wanting to be recognized 
        recognizer.SetRecognizableGestures(GestureSettings.Tap);
        //initializes the microsoft tapped thing to also call the tap handler
        recognizer.Tapped += TapHandler;
        //sets the recognizer in loop 
        recognizer.StartCapturingGestures();
    }

    //Respond Tap Input 
    private void TapHandler(TappedEventArgs obj)
    {
        if (!captureIsActive)
        {
            captureIsActive = true;


            //static class to the singletone of scene organizer and its cursor to turn red
            SceneOrganiser.Instance.cursor.GetComponent<Renderer>().material.color = Color.red;

            Invoke("ExecuteImageCaptureAndAnalysis", 0);

        }
    }

    /// <summary>
    /// Begin process of image capturing and send to Azure Custom Vision Service.
    /// </summary>
    private void ExecuteImageCaptureAndAnalysis()
    {
        // Create a label in world space using the ResultsLabel class 
        // Invisible at this point but correctly positioned where the image was taken
        SceneOrganiser.Instance.PlaceAnalysisLabel();

        // Set the camera resolution to be the highest possible
        Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending
            ((res) => res.width * res.height).First();
        Texture2D targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);

        // Begin capture process, set the image format
        PhotoCapture.CreateAsync(true, delegate (PhotoCapture captureObject)
        {
            photoCaptureObject = captureObject;

            CameraParameters camParameters = new CameraParameters
            {
                hologramOpacity = 1.0f,
                cameraResolutionWidth = targetTexture.width,
                cameraResolutionHeight = targetTexture.height,
                pixelFormat = CapturePixelFormat.BGRA32
            };

            // Capture the image from the camera and save it in the App internal folder
            captureObject.StartPhotoModeAsync(camParameters, delegate (PhotoCapture.PhotoCaptureResult result)
            {
                string filename = string.Format(@"CapturedImage{0}.jpg", captureCount);
                filePath = Path.Combine(Application.persistentDataPath, filename);
                captureCount++;
                photoCaptureObject.TakePhotoAsync(filePath, PhotoCaptureFileOutputFormat.JPG, OnCapturedPhotoToDisk);
            });
        });
    }

    /// <summary>
    /// Register the full execution of the Photo Capture. 
    /// </summary>
    void OnCapturedPhotoToDisk(PhotoCapture.PhotoCaptureResult result)
    {
        try
        {
            // Call StopPhotoMode once the image has successfully captured
            photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
        }
        catch (Exception e)
        {
            Debug.LogFormat("Exception capturing photo to disk: {0}", e.Message);
        }
    }

    /// <summary>
    /// The camera photo mode has stopped after the capture.
    /// Begin the image analysis process.
    /// </summary>
    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        Debug.LogFormat("Stopped Photo Mode");

        // Dispose from the object in memory and request the image analysis 
        photoCaptureObject.Dispose();
        photoCaptureObject = null;

        // Call the image analysis
        StartCoroutine(CustomVisionAnalyser.Instance.AnalyseLastImageCaptured(filePath));
    }

    /// <summary>
    /// Stops all capture pending actions
    /// </summary>
    internal void ResetImageCapture()
    {
        captureIsActive = false;

        // Set the cursor color to green
        SceneOrganiser.Instance.cursor.GetComponent<Renderer>().material.color = Color.green;

        // Stop the capture loop if active
        CancelInvoke();
    }
}
