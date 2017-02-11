using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RemoveAndIncScore : MonoBehaviour
{

    Text scoreText;
    Text endOfGameText;
    GameObject SpaceCollection;

    // Use this for initialization
    void Start()
    {
        GameObject tempScore = GameObject.Find("ScoreNum");
        //GameObject tempScore = GameObject.FindGameObjectWithTag("Score");
        scoreText = tempScore.GetComponent<Text>();
        //endOfGameText = GameObject.Find("EndOfGameText").GetComponent<Text>();
        //endOfGameText.text = "";

        SpaceCollection = GameObject.Find("SphereCollection");
    }

    // Update is called once per frame
    void Update()
    {
        //updateText.text = score.ToString();
        float maxDistance = 20;

        RaycastHit hit;
        Physics.Raycast(Camera.main.transform.position, (transform.position - Camera.main.transform.position), out hit, maxDistance);

        if (hit.transform == transform)
        {
            GetComponent<Renderer>().material.color = Color.yellow;
        }
        else
        {
            GetComponent<Renderer>().material.color = Color.clear;
        }

        if (Vector3.Distance(transform.position, Camera.main.transform.position) < (Camera.main.GetComponent<BoxCollider>().size.x))
        {
            int score = 0;
            Camera.main.GetComponent<AudioSource>().Play();
            if (scoreText != null)
            {
                score = int.Parse(scoreText.text.ToCharArray()[0] + "");
                score++;
                scoreText.text = score.ToString() + " / " + SpaceCollection.GetComponent<SpaceCollectionManager>().placedSpheres;

                if (SpaceCollection.GetComponent<SpaceCollectionManager>().placedSpheres == score &&
                SpaceCollection.GetComponent<SpaceCollectionManager>().placedSpheres != 0)
                {
                    //endOfGameText.text = "You Win";
                }
            }
            Debug.Log("hit sphere " + score);
            this.gameObject.SetActive(false);
            SpaceCollection.GetComponent<SpaceCollectionManager>().removeSphere(this.gameObject.transform.position);
        }
    }
}
