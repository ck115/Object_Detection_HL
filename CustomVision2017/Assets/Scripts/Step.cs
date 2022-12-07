using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Step : MonoBehaviour
{

    [SerializeField] Text header;

    [SerializeField] Text content;

    string contentString;

    string headerString;

    public static Step Instance;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

        headerString = "Game State: Shapes";

        contentString = "Use \"click\" command to find shape";
        
    }

    // Update is called once per frame
    void Update()
    {
        header.text = headerString;
        content.text = contentString;
    }

    public void changeContent(string state, string content)
    {
        headerString = "Game State: " + state;

        contentString = content;
    }
}