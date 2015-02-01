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

    private Transform lastDuck;

	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyUp(KeyCode.C)) //spawn a duck call
        {
            DuckCall callScript = duckcallPrefab.gameObject.GetComponent<DuckCall>();
            callScript.startDuckCall();
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
        if (Input.GetKeyUp(KeyCode.Q))
        {
            lastDuck.SendMessage("Die");
        }
	}
}
