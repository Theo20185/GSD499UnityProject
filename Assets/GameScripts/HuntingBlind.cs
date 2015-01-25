using UnityEngine;
using System.Collections;

//right now clays dont work so the clay event is instead turned into an easy duck hunt

public class HuntingBlind : MonoBehaviour {
    public int tentType; //0 = clays, 1 = easy, 2 = hard
    public EventManager eventManager;
    public Transform spawnPoint;
    public Transform shootPoint;

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
	
	}

    void OnTriggerEnter(Collider other)
    {
        //if player hit this hunting blind then start an event!
        if (other.name == "First Person Controller")
        {
            if (tentType < 2)
                eventManager.TriggerEvent(shootPoint, spawnPoint, 10, 5, true);
            else
                eventManager.TriggerEvent(shootPoint, spawnPoint, 10, 7, false);
        }
    }
}
