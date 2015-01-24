using UnityEngine;
using System.Collections;

public class Decoy : MonoBehaviour {

    private GameObject player;
    private bool didPush;
    private float startTime;
    public float decoyLife; //how long do decoys last?

	// Use this for initialization
	void Start () {
        didPush = false;
        player = GameObject.Find("First Person Controller");
        Vector3 decoyPos = player.transform.position;
        Vector3 forwardplayer = player.transform.forward;
        decoyPos += (forwardplayer * 2); //start decoy just a short distance in front of the player.
        decoyPos.y -= 1; //also down a bit so it appears that the player threw it from their hands
        transform.position = decoyPos;
        startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
        //it appears that physics forces must be added after Start so you have to do it in update here.
        if (!didPush)
        {
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            //We're about to do a single push so it has to be pretty hard
            Vector3 forceDecoy = player.transform.forward * 500; //push in front of us hard
            forceDecoy.y += 400; //add some upward motion too.
            rigidbody.AddForce(forceDecoy);
            didPush = true; //no mo' shoving
        }
        if (Time.time > (startTime + decoyLife))
        {
            Destroy(gameObject); //bye!
        }
	}

    //some explantion for why this is an on trigger that then turns off isTrigger. We trigger once
    //to get the splash sound but leaving it as a trigger allows it to fall right through the lake
    //so turning isTrigger off lets it stay on the lake after the initial sound effect.
    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Splash!");
        audio.Play(); //play our plop sound when we hit the water.
        rigidbody.collider.isTrigger = false;
    }
}
