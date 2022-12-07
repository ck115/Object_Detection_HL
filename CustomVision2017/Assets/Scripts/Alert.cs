using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Alert : MonoBehaviour
{
    //both should be referencing the header and content fields of the alert box
    [SerializeField] Text header;

    [SerializeField] Text content;

    private string contentString;

    private string headerString;

    public static Alert Instance;


    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

        headerString = "Alert: Game Initialized";

        contentString = "ready to recieve user input";
    }

    // Update is called once per frame
    void Update()
    {
        header.text = headerString;
        content.text = contentString;
    }

    public void changeContent(string alert, string content)
    {
        headerString = "Alert: " + alert;

        contentString = content;
    }


}
