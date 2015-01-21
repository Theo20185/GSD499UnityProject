using UnityEngine;
using System.Collections;

//A single duck. Knows only about itself - just a script upon the duck object
//Instantiate duck objects with DuckManager

public class Duck : MonoBehaviour {
    protected float startTime; //when was this duck spawned into being?
    protected float lastActionTime; //last time we made a decision

    //these next three can be modified to change the difficulty of this duck type.
    public Color duckColor;
    public float escapeTime; //how long will duck fly around before escaping?
    public float decisionTiming; //how often this sort of duck will update its decisions
    public float duckSpeed;

    public void Start()
    {
        this.animation.Play("fly");
        startTime = Time.time;
        lastActionTime = startTime;
        //this.renderer.material.color = duckColor;
        //this.transform.rotation = Random.rotation;
    }

    public void Update()
    {
        //we have a choice to make at every update.
        //We can continue along our current vector.
        //We can escape (if the elapsed time is past the threshold)
        //We can change course

        if (Time.time > (startTime + escapeTime))
        {
            Debug.Log("Escape!");
            //must update GUI to show that the duck got away. A signal of some sort must be sent here.
            this.animation.Stop();
            DestroyObject(this); //DIE! DIE! DIE!
        }

        //time to contemplate a decision now
        if (Time.time > (lastActionTime + decisionTiming))
        {
            Debug.Log("Decision");
            lastActionTime = Time.time;
            //this.transform.rotation = Random.rotation;
        }

        //finally, if we got here then try to move in the current direction
        //Debug.Log(transform.rotation.eulerAngles.ToString());

        Vector3 movement = this.transform.rotation.eulerAngles;
        movement.z += 270;
        //this.transform.position += movement * Time.deltaTime * duckSpeed * 0.001f;
    }
}
