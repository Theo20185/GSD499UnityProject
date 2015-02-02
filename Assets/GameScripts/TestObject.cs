using UnityEngine;
using System.Collections;

//nothing to see here anymore.
public class TestObject : MonoBehaviour {
    public Transform player;
    public Transform duckcallPrefab; 
    public Transform decoyPrefab;
    public Transform duckPrefab;
    public Transform spawnBasis;
    public Vector3 spawnSpan;

	public EventManager eventManager;
	public Transform shootingPosition;
	public Transform cameraFocusOn;

    private Transform lastDuck;
	
	// Update is called once per frame
	void Update () {

	}
}
