using UnityEngine;
using System.Collections;

public class SphereSoundManager : MonoBehaviour {

    GameObject myCamera;
    GameObject sphereManager;
    AudioSource sound = null;
    bool playing = false;
    

	// Use this for initialization
	void Start () {
        myCamera = Camera.main.gameObject;
        sound = GetComponent<AudioSource>();
        sphereManager = GameObject.Find("SphereCollection");
	}
	
	// Update is called once per frame
	void Update () {
	    if (Vector3.Distance(sphereManager.GetComponent<SpaceCollectionManager>().findNearestSphere(),myCamera.transform.position) 
            == Vector3.Distance(transform.position,myCamera.transform.position)) {
            if (!playing) {
                playing = true;
                sound.Play();
            }
        } else {
            if (playing) {
                playing = false;
                sound.Stop();
            }
        }
	}
}
