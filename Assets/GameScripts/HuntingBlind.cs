using UnityEngine;
using System.Collections;

//right now clays dont work so the clay event is instead turned into an easy duck hunt

public class HuntingBlind : MonoBehaviour {
    public int tentType; //0 = clays, 1 = easy, 2 = hard
    public EventManager eventManager;
    public Transform spawnPoint;
    public Transform shootPoint;

    private float startTime;
    private bool isHidden = false;

	// Use this for initialization
	void Start () {
        Renderer thisRenderer = this.GetComponentInChildren<Renderer>();
        switch (tentType)
        {
            case 0:
                thisRenderer.material.color = Color.white;
                break;
            case 1:
                thisRenderer.material.color = Color.green;
                break;
            case 2:
                thisRenderer.material.color = Color.red;
                break;
        }
        
	}
	
	// Update is called once per frame
	void Update () {
        if ((Time.time > startTime + 10f) && isHidden)
        {
            //reenable collider and renderer once the player is in the event.
            //This way the player can trigger it again later if they want
            isHidden = false;
            collider.enabled = true; 
            renderer.enabled = true;
        }
	}

    void OnTriggerEnter(Collider other)
    {
        //if player hit this hunting blind then start an event!
        if (other.name == "First Person Controller")
        {
            collider.enabled = false; //turn it off while in event;
            renderer.enabled = false; //turn off gfx for tent too
            isHidden = true;
            if (tentType < 2)
                eventManager.TriggerEvent(shootPoint, spawnPoint);
            else
                eventManager.TriggerEvent(shootPoint, spawnPoint);
        }
    }
}
