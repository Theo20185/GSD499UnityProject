using UnityEngine;
using System.Collections;

//Script to manage the shooting event.

public class EventManager : MonoBehaviour 
{
	//This is from the First Person Controller component Character Motor. This is a Javascript file, and I cannot access it here. I can only access MouseLook since that is also in C#.
	private float maxSpeed = 6f;

	public GameObject firstPersonController;
	public GameObject mainCamera;
	public Texture2D cursor;

	//Flags for different stages of events.
	private enum EventStage
	{
		Null, TransitionIn, CountdownToEvent, EventActive, ShowResults, Return
	}

	private EventStage stage = EventStage.Null;

	private Transform shootingPosition;
	private Transform focusCameraOn;

	public Transform ShootingPosition
	{
		get { return shootingPosition; }
		set { shootingPosition = value; }
	}
	
	public Transform FocusCameraOn
	{
		get { return focusCameraOn; }
		set { focusCameraOn = value; }
	}

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

	public void TriggerEvent()
	{
		firstPersonController.GetComponent<MouseLook> ().enabled = false;
		GameObject.Find ("First Person Controller").GetComponent ("FPSInputController").SendMessage ("DisableControl");
		stage = EventStage.TransitionIn;
	}

	public void TriggerEvent(Transform shootingPosition, Transform focusCameraOn)
	{

		TriggerEvent ();
		this.shootingPosition = shootingPosition;
		this.focusCameraOn = focusCameraOn;
	}

	private void TransitionIn()
	{
		Vector3 fpcPosition = firstPersonController.GetComponent<Transform> ().position;

		if (fpcPosition.x != ShootingPosition.position.x || fpcPosition.z != ShootingPosition.position.z) 
		{
			//Move the camera to look at the Focus target.
			mainCamera.transform.LookAt (FocusCameraOn.position);

			//Move the player to specified shooting position.
			Vector3 moveDirection = ShootingPosition.position - fpcPosition;
			moveDirection.y = 0;
			Vector3 velocity = Vector3.ClampMagnitude (moveDirection * maxSpeed, maxSpeed);
			firstPersonController.GetComponent<CharacterController>().SimpleMove (velocity);
		} 
		else 
		{
			stage = EventStage.CountdownToEvent;
		}
	}

	private void CountDownToEvent()
	{

	}

	private void UpdateEvent()
	{
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
