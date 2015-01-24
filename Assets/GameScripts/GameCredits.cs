using UnityEngine;
using System.Collections;

public class GameCredits : MonoBehaviour {

	public GUISkin customSkin;
	private bool showCredits = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown ("m")) {
			CreditScreen();
		}
	
	}

	void CreditScreen(){
		showCredits = !showCredits;
	}

	void OnGUI(){
		
		GUI.depth = 1;
		
		GUI.skin = customSkin;

		if (showCredits) {
				//GUILayout.Label("That Ducking Game");
				GUI.BeginGroup (new Rect (Screen.width / 2 - 300, Screen.height / 2 - 300, 600, 600));
				//add lables here
				int rectY = 0;
				int addCount = 25;
				GUI.Box (new Rect (0, rectY, 600, 600), "Credits");
				rectY += addCount;
				GUI.Label (new Rect (0, rectY + 50, 600, 600), "Thanks for playing!");
				rectY += addCount;
				GUI.Label (new Rect (0, rectY + 50, 600, 600), "Developers: Brandon, Collin, and Yan");
				rectY += addCount;
				GUI.Label (new Rect (0, rectY + 50, 600, 600), "Assets: Unity Store (want to add more specifics here)");
				rectY += addCount;
				GUI.Label (new Rect (0, rectY + 50, 600, 600), "Thanks to....");
		}

		GUI.EndGroup();
		
	}

}
