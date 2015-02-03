using UnityEngine;
using System.Collections;

public class GameOver : MonoBehaviour 
{
	public GUIText gameOverText;
	private float timer;

	// Use this for initialization
	void Start () 
	{
		Color gameOverColor = gameOverText.font.material.color;
		gameOverColor.a = 0;
		Debug.Log ("GameOverText.color.a start: " + gameOverText.font.material.color.a);
		timer = 0;
	}
	
	// Update is called once per frame
	void Update () 
	{
		timer += Time.smoothDeltaTime;
	
		Debug.Log (gameOverText.color.a.ToString ());

		Color gameOverColor = gameOverText.font.material.color;
		gameOverColor.a = (timer / 3f);

		if (timer >= 5f)
			Application.LoadLevel ("MainMenu");
	}
}
