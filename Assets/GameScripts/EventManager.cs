﻿using UnityEngine;
using System.Collections;

//Script to manage the shooting event.
//Use TriggerEvent to begin a shooting event. This should be called upon collision with an event hotspot. TestObject has a hotkey of E assigned to test event functionality on Lake 1.
//Once an event is triggered, the event should take care of itself. Based on the results, the control will either be returned to the player or the game will enter "Game Over" mode.

public class EventManager : MonoBehaviour 
{
	public float maxSpeed;
	public GameObject firstPersonController;
	public GameObject mainCamera;
	public Transform duckPrefabEasy;
	public Transform duckPrefabHard;
	public GUIText requirements;
	public GUIText countdownTimer;
	public GUITexture crosshair;
	public GUITexture shotgunShell;

	//Flags for different stages of events.
	private enum EventStage
	{
		Null, TransitionIn, ShowRequirements, CountdownToEvent, EventActive, ShowResults, TransitionOut
	}

	private EventStage stage = EventStage.Null;

	private Transform shootingPosition;
	private Transform targetSpawn;
	private float timer;
	private int targetsShot;
	private int targetsSpawned;
	private bool targetsInPlay;
	private int shells;

	// Use this for initialization
	public void Start () 
	{
	}
	
	// Update is called once per frame
	public void Update () 
	{
		if (stage == EventStage.TransitionIn)
			TransitionIn ();
		if (stage == EventStage.ShowRequirements)
			ShowRequirements ();
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

	public void TriggerEvent(Transform shootingPosition, Transform duckSpawn)
	{
		this.shootingPosition = shootingPosition;
		this.targetSpawn = duckSpawn;
		targetsShot = 0;
		targetsSpawned = 0;

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
<<<<<<< HEAD
			mainCamera.transform.LookAt (targetSpawn.position);
=======
		    firstPersonController.transform.LookAt (duckSpawn.position);
>>>>>>> origin/master

			//Move the player to specified shooting position.
			Vector3 moveDirection = shootingPosition.position - fpcPosition;
			moveDirection.y = 0;
			Vector3 velocity = Vector3.ClampMagnitude (moveDirection * maxSpeed, maxSpeed);
			firstPersonController.GetComponent<CharacterController> ().SimpleMove (velocity);
		}
		else 
			InitializeShowRequirements ();
	}

	private void InitializeShowRequirements()
	{
		timer = 8f;
		requirements.enabled = true;
		stage = EventStage.ShowRequirements;
	}

	private void ShowRequirements()
	{
		timer = timer - Time.deltaTime;
		string target = targetSpawn.GetComponent<TargetSpawn> ().targetType == TargetSpawn.TargetType.Clay ? "Clays" : "Ducks";

		if (timer < 6f && timer >= 4f) 
		{
			requirements.text = string.Concat ("Event!","\n",
			                                   "Rounds: ", targetSpawn.GetComponent<TargetSpawn>().spawnRounds.ToString ("n0"));
		} 
		else if (timer < 4f && timer >= 2f) 
		{
			requirements.text = string.Concat ("Event!","\n",
			                                   "Rounds: ", targetSpawn.GetComponent<TargetSpawn>().spawnRounds.ToString("n0"), "\n", 
			                                   target, " Needed: ", targetSpawn.GetComponent<TargetSpawn>().targetsNeeded.ToString ("n0"));
		} 
		else if (timer < 2f) 
		{
			requirements.text = string.Concat ("Event!","\n",
			                                   "Rounds: ", targetSpawn.GetComponent<TargetSpawn>().spawnRounds.ToString ("n0"), "\n", 
			                                   target, " Needed: ", targetSpawn.GetComponent<TargetSpawn>().targetsNeeded.ToString ("n0"), "\n",
			                                   target, " at Event: ", (targetSpawn.GetComponent<TargetSpawn>().spawnRounds * targetSpawn.GetComponent<TargetSpawn>().targetsPerSpawn).ToString ("n0"));
		} 
		else 
			requirements.text = "Event!";

		if (timer < 0)
			InitializeCountdownToEvent ();
	}
	
	private void InitializeCountdownToEvent()
	{
		requirements.enabled = false;
		countdownTimer.enabled = true;
		timer = 3f;
		shells = 3;
		stage = EventStage.CountdownToEvent;
	}

	private void CountDownToEvent()
	{
		timer = timer - Time.deltaTime;
		countdownTimer.text = timer > 0 ? timer.ToString ("N2") : "GO!";

		if (timer < -2)
			InitializeUpdateEvent ();
	}

	private void InitializeUpdateEvent()
	{
		countdownTimer.enabled = false;
		timer = 0.5f;
		targetsInPlay = false;
		stage = EventStage.EventActive;
	}

	private void UpdateEvent()
	{
		timer = timer - Time.deltaTime;

		if (timer <= 0 && targetsInPlay == false)
			SpawnTargets ();
	}

	private void UpdateEventOnGUI()
	{
		float x = Input.mousePosition.x - (crosshair.texture.width / 2);
		float y = Screen.height - Input.mousePosition.y - (crosshair.texture.height / 2);
		GUI.DrawTexture (new Rect (x, y, crosshair.texture.width, crosshair.texture.height), crosshair.texture);

		for (int guiShellsIndex = 0; guiShellsIndex < shells; guiShellsIndex++)
		{
			GUI.DrawTexture (new Rect(10 + (guiShellsIndex * 50), Screen.height - 110, shotgunShell.texture.width, shotgunShell.texture.height), shotgunShell.texture);
		}
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

	private void SpawnTargets()
	{
		targetsInPlay = true;

		for (int targets = 0; targets < targetSpawn.GetComponent<TargetSpawn>().targetsPerSpawn; targets++) 
		{
			Transform target = duckPrefabEasy;

			if (targetSpawn.GetComponent<TargetSpawn> ().targetType == TargetSpawn.TargetType.HardDuck)
				target = duckPrefabHard;
			//TODO: Set target to Clay object if the spawn is for Clays.

			target.GetComponent<Duck> ().escapeTime = targetSpawn.GetComponent<TargetSpawn> ().escapeTime;

			float x = targetSpawn.GetComponent<TargetSpawn> ().transform.position.x + (float)Random.Range (0, 2 * targetSpawn.GetComponent<TargetSpawn> ().spawnSpan.x - targetSpawn.GetComponent<TargetSpawn> ().spawnSpan.x);
			float y = targetSpawn.GetComponent<TargetSpawn> ().transform.position.y + (float)Random.Range (0, 2 * targetSpawn.GetComponent<TargetSpawn> ().spawnSpan.y - targetSpawn.GetComponent<TargetSpawn> ().spawnSpan.y);
			float z = targetSpawn.GetComponent<TargetSpawn> ().transform.position.z + (float)Random.Range (0, 2 * targetSpawn.GetComponent<TargetSpawn> ().spawnSpan.z - targetSpawn.GetComponent<TargetSpawn> ().spawnSpan.z);

			Instantiate (target, new Vector3 (x, y, z), Quaternion.identity);
		}
	}
}
