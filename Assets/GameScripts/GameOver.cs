using UnityEngine;
using System.Collections;

public class GameOver : MonoBehaviour 
{
	public GUIText gameOverText;
	private float timer;

	// Use this for initialization
	void Start () 
	{
		gameOverText.color = new Color (1, 1, 1, 0);
		timer = 0;
	}
	
	// Update is called once per frame
	void Update () 
	{
		timer += Time.smoothDeltaTime;

		gameOverText.color = new Color (1, 1, 1, timer / 3f);

		if (timer >= 5f)
			Application.LoadLevel ("MainMenu");
	}
}
