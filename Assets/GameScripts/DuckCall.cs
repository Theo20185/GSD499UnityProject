using UnityEngine;
using System.Collections;

public class DuckCall : MonoBehaviour {
    private float startTime;
    private bool isRunning; //are we doing anything?
    MeshRenderer thisRenderer;


	// Use this for initialization
	void Start () {
        isRunning = false;
        thisRenderer = gameObject.GetComponentInChildren<MeshRenderer>();
        thisRenderer.enabled = false;
    }   
	
	// Update is called once per frame
	void Update () {
        if (!isRunning) return;
        if (Time.time > (startTime + 2.0))
        {
            thisRenderer.enabled = false;            
        }
	}

    public void startDuckCall()
    {
        audio.Play();
        startTime = Time.time;
        isRunning = true;
        thisRenderer.enabled = true;
    }
}
