using UnityEngine;
using UnityEngine.XR.WSA;

//This class will set the Spatial Mapping Collider in the scene so to be able to detect 
//collisions between virtual objects and real objects.

public class SpatialMapping : MonoBehaviour {

    //singleton 
    public static SpatialMapping Instance;

    //property use by the Gaze Cursor 
    public static int PhysicsRaycastMask;

    //Layer used for spatial mapping colisions 
    internal int physicsLayer = 31;

    //creates enviorment colliders 
    private SpatialMappingCollider spatialMappingCollider;

    void Awake()
    {
        //establish singleton
        Instance = this;
    }

	// Use this for initialization
	void Start () {

        //allows holographic stuff to interact with components 
        spatialMappingCollider = gameObject.GetComponent<SpatialMappingCollider>();
        //select the parent where the mapping inherits stuff 
        spatialMappingCollider.surfaceParent = this.gameObject;
        //queries the spatial mapping data for surface changes in physical space, freeze prevents this 
        spatialMappingCollider.freezeUpdates = false;
        //no quite sure what this does 
        spatialMappingCollider.layer = physicsLayer;

        //the << shifts the bits to the left. Not sure why you would do it this way 
        PhysicsRaycastMask = 1 << physicsLayer;

        //set object as active 
        gameObject.SetActive(true);
	}	

}
