using UnityEngine;
using System.Collections;

//temporarry object that allows for doing stuff as a test
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

	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyUp(KeyCode.X)) //spawn a duck
        {
            Vector3 duckPos = spawnBasis.position;
            duckPos.x += (float)(Random.Range(0, (float)spawnSpan.x) - (spawnSpan.x / 2.0));
            duckPos.y += (float)(Random.Range(0, (float)spawnSpan.y) - (spawnSpan.y / 2.0));
            duckPos.z += (float)(Random.Range(0, (float)spawnSpan.z) - (spawnSpan.z / 2.0));
            Instantiate(duckPrefab, duckPos, Quaternion.identity);
        }
        if (Input.GetKeyUp(KeyCode.C)) //spawn a duck call
        {
            //no need to set position or rotation.
            Instantiate(duckcallPrefab, Vector3.zero, Quaternion.identity);
        }
        if (Input.GetKeyUp(KeyCode.V)) //spawn a decoy
        {
            //The decoy can be instantiated without needing to worry about rotation or position. Its scripts handle it all
            Instantiate(decoyPrefab, Vector3.zero, Quaternion.identity);
        }

		if (Input.GetKeyUp(KeyCode.E)) 
		{
			eventManager.TriggerEvent (shootingPosition, cameraFocusOn);
		}
	}
}
