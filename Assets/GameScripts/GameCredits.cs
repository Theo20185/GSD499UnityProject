using UnityEngine;
using System.Collections;

public class GameCredits : MonoBehaviour {

	public GUISkin customSkin;
	public GameObject MainMenu;
	public GUITexture crosshair;
	private bool showCredits = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		//if (Input.GetKeyDown ("m")) {
			//CreditScreen();
		//}
	
	}

	public void CreditScreen(){
		showCredits = !showCredits;
		StartCoroutine(playAudio ());
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

			if (GUI.Button (new Rect (445, 545, 150, 50), "Main Menu")) {
				audio.Stop ();
				showCredits = false;
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
