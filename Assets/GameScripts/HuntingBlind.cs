using UnityEngine;
using System.Collections;

public class HuntingBlind : MonoBehaviour {
    public Color tentColor;

	// Use this for initialization
	void Start () {
        Renderer thisRenderer = this.GetComponentInChildren<Renderer>();
        thisRenderer.material.color = tentColor;	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
