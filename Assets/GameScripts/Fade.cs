using UnityEngine;
using System.Collections;

public class Fade : MonoBehaviour {

    public Color startColor;
    public Color endColor;
    public float duration;
    public Texture labelTexture;

    private Color currentColor;
    private float currentTime;

	// Use this for initialization
	void Start () {
        currentTime = Time.time;
	    currentColor = startColor;
        Destroy(gameObject, duration + 0.1f);
	}
	
	// Update is called once per frame
	void OnGUI()
    {
	    GUI.depth = 0;
        GUI.color = currentColor;

        GUI.DrawTexture(new Rect(0, 0, 2048, 2048), labelTexture);
	}

    public void FixedUpdate()
    {
        currentColor = Color.Lerp(startColor, endColor, (Time.time - currentTime) / duration);
    }
}
    


