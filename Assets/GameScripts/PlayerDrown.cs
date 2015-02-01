using UnityEngine;
using System.Collections;

//checks to see if player entered the drink and teleports out of it
public class PlayerDrown : MonoBehaviour {
    public GameObject fadeIn;
    public GameObject fadeOut;

    private float startTime;
    private bool isDrowning;
    private string warpToName;

    public void Start()
    {
        isDrowning = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("Lake")) //whoops... entered the water
        {
            warpToName = other.name + "ShootingPos";
            startTime = Time.time;
            isDrowning = true;
            Instantiate(fadeOut);
        }
    }

    public void Update()
    {
        if (isDrowning)
        {
            if (Time.time > startTime + 1f)
            {
                //Instantiate(fadeIn);
                Debug.Log(warpToName);
                GameObject warpTo = GameObject.Find(warpToName);
                transform.position = warpTo.transform.position;
                isDrowning = false;
            }
        }
    }
}
