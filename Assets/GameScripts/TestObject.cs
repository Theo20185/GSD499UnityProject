using UnityEngine;
using System.Collections;

//temporarry object that allows for doing stuff as a test
public class TestObject : MonoBehaviour {
    public Transform duckPrefab;
    public Transform spawnBasis;
    public Vector3 spawnSpan;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyUp(KeyCode.X))
        {
            Vector3 duckPos = spawnBasis.position;
            duckPos.x += (float)(Random.Range(0, (float)spawnSpan.x) - (spawnSpan.x / 2.0));
            duckPos.y += (float)(Random.Range(0, (float)spawnSpan.y) - (spawnSpan.y / 2.0));
            duckPos.z += (float)(Random.Range(0, (float)spawnSpan.z) - (spawnSpan.z / 2.0));
            Instantiate(duckPrefab, duckPos, Quaternion.identity);
        }
	}
}
