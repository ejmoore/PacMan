using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UpdateScore : MonoBehaviour {

    int score = 0;
    GUIText txt;

	// Use this for initialization
	void Start () {
        GameObject tempScore = GameObject.FindGameObjectWithTag("Score");
        txt = tempScore.GetComponent<GUIText>();
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    void Score () {
        score++;
        txt.text = "" + score;
    }
}
