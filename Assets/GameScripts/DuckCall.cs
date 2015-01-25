using UnityEngine;
using System.Collections;

public class DuckCall : MonoBehaviour {
    private GameObject player;
    private float startTime;


	// Use this for initialization
	void Start () {
        //rather than attach this object to the controller and hide/show we'll just 
        //instantiate when needed and place it in front of the player when used
        player = GameObject.Find("First Person Controller");
        Vector3 callPos = player.transform.position;
        Vector3 forwardplayer = player.transform.forward;
        callPos += (forwardplayer * 1.0f); //start decoy just a short distance in front of the player.
        callPos.y += 0.6f;
        transform.position = callPos;
        transform.rotation = Quaternion.Euler(0f, -90f, -80f); //model came in a bit rotated this is the lazy way to fix that
        startTime = Time.time;
        audio.Play();
    }   
	
	// Update is called once per frame
	void Update () {
        if (Time.time > (startTime + 2.0))
        {
            Destroy(this.gameObject);
        }
	}
}
