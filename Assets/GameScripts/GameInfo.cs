using UnityEngine;
using System.Collections;

public class GameInfo : MonoBehaviour {

	public GUISkin customSkin;
	public GameObject MainMenu;
	public GUITexture crosshair;
	private bool showInfo = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		//if (Input.GetKeyDown ("m")) {
			//CreditScreen();
		//}
	
	}

	public void InfoScreen(){
		showInfo = !showInfo;
		StartCoroutine(playAudio ());
	}

	void OnGUI(){
		
		GUI.depth = 1;
		
		GUI.skin = customSkin;

		if (showInfo) {
			//GUILayout.Label("That Ducking Game");
			GUI.BeginGroup (new Rect (Screen.width / 2 - 300, Screen.height / 2 - 300, 600, 600));
			//add lables here
			int rectY = 0;
			int addCount = 25;
			GUI.Box (new Rect (0, rectY, 600, 600), "Game Info");
			rectY += addCount;
			GUI.Label (new Rect (0, rectY + 50, 600, 600), "Use W/A/S/D keyboard keys to move player.\nUse Mouse to look around.\nUse Left Mouse button to shoot in Events.");
			rectY += addCount;
			rectY += addCount;
			rectY += addCount;
			rectY += addCount;
			GUI.Label (new Rect (290, (rectY + 20) + 50, 250, 250), "The Dog will guide player to nearest Shotting Event.");
			rectY += addCount;
			rectY += addCount;
			rectY += addCount;
			rectY += addCount;
			rectY += addCount;
			rectY += addCount;
			GUI.Label (new Rect (120, rectY + 60, 250, 250), "Green Tent\nEasy Event");
			GUI.Label (new Rect (350, rectY + 30, 250, 250), "Items hidden in\nthe game world:");
			GUI.Label (new Rect (390, rectY + 90, 250, 250), "Duck Decoy\nPress V");
			rectY += addCount;
			rectY += addCount;
			rectY += addCount;
			rectY += addCount;
			GUI.Label (new Rect (120, rectY + 50, 250, 250), "Red Tent\nHard Event");
			GUI.Label (new Rect (390, rectY + 80, 250, 250), "Duck Call\nPress C");
			rectY += addCount;
			rectY += addCount;
			rectY += addCount;
			rectY += addCount;
			GUI.Label (new Rect (120, rectY + 40, 250, 250), "Striped Tent\nClay Event");

			if (GUI.Button (new Rect (445, 545, 150, 50), "Main Menu")) {
				audio.Stop ();
				showInfo = false;
				MainMenu.GetComponent<MainMenuManager> ().ShowMainMenu ();
			}

			GUI.EndGroup();
			
			GUI.depth = 1;
			
			//cursor
			float x = Input.mousePosition.x - (crosshair.texture.width / 2);
			float y = Screen.height - Input.mousePosition.y - (crosshair.texture.height / 2);
			GUI.DrawTexture (new Rect (x, y, crosshair.texture.width, crosshair.texture.height), crosshair.texture);
		}
		
	}
	
	private IEnumerator playAudio(){
		yield return new WaitForSeconds (.5f);
		audio.Play ();
	}

}
