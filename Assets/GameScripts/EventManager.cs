using UnityEngine;
using System.Collections;

//Script to manage the shooting event.
//Use TriggerEvent to begin a shooting event. This should be called upon collision with an event hotspot. TestObject has a hotkey of E assigned to test event functionality on Lake 1.
//Once an event is triggered, the event should take care of itself. Based on the results, the control will either be returned to the player or the game will enter "Game Over" mode.

public class EventManager : MonoBehaviour 
{
	public float maxSpeed;
	public GameObject firstPersonController;
	public GameObject mainCamera;
	public GUIText countdownTimer;
	public GUITexture crosshair;

	//Flags for different stages of events.
	private enum EventStage
	{
		Null, TransitionIn, CountdownToEvent, EventActive, ShowResults, TransitionOut
	}

	private EventStage stage = EventStage.Null;

	private Transform shootingPosition;
	private Transform duckSpawn;
	private float countdown;

	// Use this for initialization
	public void Start () 
	{
	}
	
	// Update is called once per frame
	public void Update () 
	{
		if (stage == EventStage.TransitionIn)
			TransitionIn ();
		if (stage == EventStage.CountdownToEvent)
			CountDownToEvent ();
		if (stage == EventStage.EventActive)
			UpdateEvent ();
		if (stage == EventStage.ShowResults)
			ShowResults ();
	}

	public void OnGUI()
	{
		if (stage == EventStage.EventActive)
			UpdateEventOnGUI ();

	}

	public void TriggerEvent(Transform shootingPosition, Transform duckSpawn, int ducksSpawned = 10, int ducksNeeded = 5, bool useEasyDucks = true)
	{
		this.shootingPosition = shootingPosition;
		this.duckSpawn = duckSpawn;
	
		InitializeTransitionIn ();
	}

	private void InitializeTransitionIn()
	{
		//Screen.showCursor = false;
		DisableControl ();
		stage = EventStage.TransitionIn;
	}

	private void TransitionIn()
	{        
		Vector3 fpcPosition = firstPersonController.GetComponent<Transform> ().position;
        Vector3 delta = fpcPosition - shootingPosition.position;
        float magnitude = (delta.x * delta.x) + (delta.z * delta.z);

		if (magnitude > 0.04f)
		{
			//Move the camera to look at the Focus target.
		    firstPersonController.transform.LookAt (duckSpawn.position);

			//Move the player to specified shooting position.
			Vector3 moveDirection = shootingPosition.position - fpcPosition;
			moveDirection.y = 0;
			Vector3 velocity = Vector3.ClampMagnitude (moveDirection * maxSpeed, maxSpeed);
			firstPersonController.GetComponent<CharacterController> ().SimpleMove (velocity);
		} 
		else 
			InitializeCountdownToEvent ();
	}

	private void InitializeCountdownToEvent()
	{
		firstPersonController.GetComponent<CharacterController> ().SimpleMove (Vector3.zero);
		countdownTimer.enabled = true;
		countdown = 3.5f;
		stage = EventStage.CountdownToEvent;
	}

	private void CountDownToEvent()
	{
		countdown = countdown - Time.deltaTime;
		countdownTimer.text = countdown > 0 ? countdown.ToString ("N2") : "GO!";

		if (countdown < -2)
			InitializeUpdateEvent ();
	}

	private void InitializeUpdateEvent()
	{
		countdownTimer.enabled = false;
		stage = EventStage.EventActive;
	}

	private void UpdateEvent()
	{
	}

	private void UpdateEventOnGUI()
	{
		float x = Input.mousePosition.x - (crosshair.texture.width / 2);
		float y = Screen.height - Input.mousePosition.y - (crosshair.texture.height / 2);
		GUI.DrawTexture (new Rect (x, y, crosshair.texture.width, crosshair.texture.height), crosshair.texture);
	}

	private void ShowResults()
	{
	}

	private void DisableControl()
	{
		firstPersonController.GetComponent<MouseLook> ().enabled = false;
		GameObject.Find ("First Person Controller").GetComponent ("FPSInputController").SendMessage ("DisableControl");
	}

	private void ReturnControl()
	{
		firstPersonController.GetComponent<MouseLook> ().enabled = true;
		GameObject.Find ("First Person Controller").GetComponent ("FPSInputController").SendMessage ("EnableControl");
	}
}
