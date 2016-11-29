using UnityEngine;
using System.Collections;

public class occlude : MonoBehaviour {

    private Camera camera;
    private Color initialColor;

	// Use this for initialization
	void Start () {
        camera = Camera.main;
        initialColor = GetComponent<Renderer>().material.color;

    }
	
    //returns true if camera is hit
    bool castRay(Vector3 translatedPos)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, camera.transform.position - transform.position, out hit))
            return hit.transform == camera.transform;

        return false;
    }

	// Update is called once per frame
	void Update () {
        if(castRay(new Vector3()))
            GetComponent<Renderer>().material.color = initialColor;
        else  
            GetComponent<Renderer>().material.color = Color.clear;
	}
}
