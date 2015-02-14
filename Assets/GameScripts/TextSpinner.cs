using UnityEngine;
using System.Collections;

public class TextSpinner : MonoBehaviour {

    public float rotationSpeed;

    private TextMesh textMesh;

	// Use this for initialization
	void Start () {
        textMesh = GetComponent<TextMesh>();
        transform.eulerAngles = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
        float y = Time.deltaTime * rotationSpeed;
        transform.RotateAround(textMesh.renderer.bounds.center, new Vector3(0,1,0), y);
	}
}
