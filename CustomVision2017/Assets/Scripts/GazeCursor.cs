using UnityEngine;

//This class is responsible for setting up the cursor in the correct location in real space, 
//by making use of the SpatialMappingCollider

public class GazeCursor : MonoBehaviour {

    //the cursors mesh  renderer
    private MeshRenderer meshRenderer;

	// Use this for initialization
	void Start () {
        //Grab the mesh renderer that is on the same object as this script 
        meshRenderer = gameObject.GetComponent<MeshRenderer>();

        //set the cursor reference 
        SceneOrganiser.Instance.cursor = gameObject;
        gameObject.GetComponent<Renderer>().material.color = Color.green;

        //this is where the size of the cursor is defined
        //vector 3 is a area inside of 3d space 
        gameObject.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
	}
	
	// Update is called once per frame
	void Update () {

        //Raycast is when a ray is passed into the enviornment and returns true of false if something is hit 
        //do a racast based on the users head position and orientation 
        Vector3 headPosition = Camera.main.transform.position;
        Vector3 gazeDirection = Camera.main.transform.forward;

        //information about object hit 
        RaycastHit gazeHitInfo;
        if (Physics.Raycast(headPosition,gazeDirection, out gazeHitInfo, 30.0f, SpatialMapping.PhysicsRaycastMask))
        {
            meshRenderer.enabled= true;

            //point is the point in 3d space
            transform.position = gazeHitInfo.point;

            //object used to represent rotation 
            transform.rotation = Quaternion.FromToRotation(Vector3.up, gazeHitInfo.normal);

        }
        else
        {
            meshRenderer.enabled = false;
        }
	}
}
