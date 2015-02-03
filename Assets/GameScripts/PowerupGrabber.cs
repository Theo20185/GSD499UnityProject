using UnityEngine;
using System.Collections;

public class PowerupGrabber : MonoBehaviour {

    public EventManager eventManager;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnTriggerEnter(Collider other)
    {
        if (other.name == "First Person Controller")
        {
            gameObject.GetComponent<AudioSource>().Play();
            if (name.Contains("Call")) eventManager.addDuckCall();
            if (name.Contains("Decoy")) eventManager.addDecoy();
            Destroy(this.gameObject,1);
        }
    }
}
