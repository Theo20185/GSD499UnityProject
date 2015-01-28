using UnityEngine;
using System.Collections;

public class MainMenuManager : MonoBehaviour {

	public GUISkin customSkin;
	public GUITexture crosshair;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI(){

		GUI.depth = 0;

		GUI.skin = customSkin;

		//GUILayout.Label("That Ducking Game");
		GUI.BeginGroup(new Rect(Screen.width / 2 - 300, Screen.height / 2 - 300, 600, 600));
		GUI.Box(new Rect(0, 0, 600, 600), "That\nDucking\nGame");

		if (GUI.Button (new Rect (250, 450, 100, 50), "Start")) {
			Application.LoadLevel("MainScene");
		}

		GUI.Label(new Rect(100, 500, 400, 100), "Top Score: 120,000");
		GUI.EndGroup();

		GUI.depth = 1;

		//cursor
		float x = Input.mousePosition.x - (crosshair.texture.width / 2);
		float y = Screen.height - Input.mousePosition.y - (crosshair.texture.height / 2);
		GUI.DrawTexture (new Rect (x, y, crosshair.texture.width, crosshair.texture.height), crosshair.texture);

	}
}
