using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour 
{
	public EventManager eventManager;
	public GameObject firstPersonController;

	public enum GameState
	{
		MainMenu, Active, GameOver
	}

	private GameState gameState;

	void Start () 
	{

	}

	void Update () 
	{
	
	}
}
