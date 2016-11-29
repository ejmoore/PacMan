using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour
{

    
    public float speed = .01f;

    private Camera CameraToFollow;
    private BoxCollider CameraCollider;

    // Use this for initialization
    void Start()
    {
        CameraToFollow = Camera.main;
        CameraCollider = CameraToFollow.GetComponent<BoxCollider>();   
    }

    // Update is called once per frame
    void Update()
    {
        //Sets a rotation so that the cube is always facing the CameraToFollow
        //The slerp forces the cube to rotate slowly so the player actually sees a 3d object following them. 
        Quaternion newRotation = Quaternion.LookRotation(CameraToFollow.transform.position - transform.position) * Quaternion.Euler(0, 0, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * .5f);

        //Slowly moves the cube towards the camera at a velocity equal to speed * Time.deltaTime
        transform.position = Vector3.MoveTowards(transform.position, CameraToFollow.transform.position, speed * Time.deltaTime);

       //Sets the camera color to red if the CameraToFollow is within 1 unity of this object otherwise sets the color to nothing
        if(Vector3.Distance(transform.position, CameraToFollow.transform.position) < (2 + CameraCollider.size.x))
        {
            Color cColor;

            if (Vector3.Distance(transform.position, CameraToFollow.transform.position) > .5f)
                cColor = new Color(.5f / Vector3.Distance(transform.position, CameraToFollow.transform.position), 0, 0, .2f);
            else
                cColor = new Color(1f, 0, 0, .1f);

            CameraToFollow.backgroundColor = cColor;
        }
        else 
        {
            CameraToFollow.backgroundColor = new Color(0,0,0,0);
        }

        //CameraToFollow.backgroundColor = new Color(1/Vector3.Distance(transform.position, CameraToFollow.transform.position), 0, 0, .1f);

    }
}
