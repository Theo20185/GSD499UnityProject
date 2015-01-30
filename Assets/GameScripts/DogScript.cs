﻿using UnityEngine;
using System.Collections;

public class DogScript : MonoBehaviour {

	public GameObject firstPersonController;
	public AudioClip dogSingleBark;
	public AudioClip dogBark;
	public AudioClip dogLaughter;

	private Transform target;			//final target to travel to
	private float travelSpeed;			//final travel speed
	private float walkingSpeed = 1.5f;	//set walking speed
	private float runningSpeed = 7.0f;	//set running speed

	private bool dogWalking = false;	//true if dog is to walk
	private bool dogRunning = false;	//true if dog is to run
	private bool dogJumping = false;	//true if dog is to jump
	private bool dogLaughing = false;	//true if dog is to laugh

	private float timeSet;				//use for timing

	private Vector3 tempTarget;	

	private GameObject temp1;
	private GameObject temp2;
	private GameObject temp3;
	private GameObject temp4;
	private GameObject temp5;
	private GameObject temp6;

	private GameObject jumpTarget1;
	private GameObject jumpTarget2;
	private GameObject jumpTarget3;
	private GameObject jumpTarget4;
	private GameObject jumpTarget5;
	private GameObject jumpTarget6;
	private GameObject nearestTent;
	
	private SkinnedMeshRenderer dogMeshRender;

	public int nextStage;	//need to change to one when done

	private float distanceFromFPC;
	private float distanceNearestToTent;
	private float timeTracker;
	
	private bool animationStart = false;
	private bool atTarget = false;
	private bool dogToPlayer = false;
	private bool dogToTent = false;
	private bool playerMoving = false;
	private bool eventStarted = false;
	private bool returningPlayer = false;
	private bool runnigBackToPlayer = false;
	private bool walkingBackToPlayer = false;
	private bool dogLayingDown = false;

	// Use this for initialization
	private void Start () {

		dogMeshRender = GameObject.Find ("dalmatynczyk1").GetComponent<SkinnedMeshRenderer>();

		temp1 = GameObject.Find ("DogTarget1");
		temp2 = GameObject.Find ("DogTarget2");
		temp3 = GameObject.Find ("DogTarget3");
		temp4 = GameObject.Find ("DogTarget4");
		temp5 = GameObject.Find ("DogTarget5");
		temp6 = GameObject.Find ("DogTarget6");

		jumpTarget1 = GameObject.Find ("TargetJump1");
		jumpTarget2 = GameObject.Find ("TargetJump2");
		jumpTarget3 = GameObject.Find ("TargetJump3");
		jumpTarget4 = GameObject.Find ("TargetJump4");
		jumpTarget5 = GameObject.Find ("TargetJump5");
		jumpTarget6 = GameObject.Find ("TargetJump6");

		timeTracker = Time.time;
	}
	
	// Update is called once per frame
	private void Update () {

		distanceFPCtoDog ();			//update distance between dog and Player
		distanceFPCtoNearestTent ();	//update disatnce between Player and nearest tent

		if (Input.GetAxis ("Vertical") == 0) {
			playerMoving = false;
		} 
		else {
			playerMoving = true;
			timeTracker = Time.time;
			returningPlayer = false;
			runnigBackToPlayer = false;
			walkingBackToPlayer = false;
			dogLayingDown = false;
		}

		if (!playerMoving && !eventStarted) {
			//print ("player not moving");
			if (Time.time > (timeTracker + 5)) {

				returningPlayer = true;
				//print ("player not moving");

				if (distanceFromFPC <= 5) {
					dogRunning = false;
					dogWalking = false;
					atTarget = false;

					if(!dogLayingDown){
						dogLayingDown = true;
						animation.Play ("Lie Down", PlayMode.StopAll);
					}
					else if(animation.isPlaying == false){
						animation.Play ("Rest", PlayMode.StopAll);
					}
				}

				//walk
				if (distanceFromFPC < 10 && distanceFromFPC > 5 && !walkingBackToPlayer) {
					dogRunning = false;
					dogWalking = false;
					atTarget = false;
					dogWalk (firstPersonController);
					walkingBackToPlayer = true;
				}

				//run
				if (distanceFromFPC > 10 && !runnigBackToPlayer) {
					dogRunning = false;
					dogWalking = false;
					atTarget = false;
					dogRun (firstPersonController);
					runnigBackToPlayer = true;
				}
	
			}
		}

		if(!returningPlayer)
		{
			//print ("player moving");
			//if less than 30 meters from nearest tent
			if (distanceNearestToTent < 30 && !dogRunning && !atTarget) {
				dogRun(nearestTent);
				dogToTent = true;
			}

			//if more than 30 meters from nearest tent
			if (distanceNearestToTent >= 30 && !dogRunning && !dogWalking && !atTarget) {

				//if more than 20 meters from Player
				if(distanceFromFPC >= 20){
					dogRun(firstPersonController);
					dogToPlayer = true;
				}

			}

			//start walking when near to player
			if(distanceFromFPC < 10 && distanceFromFPC > 3 && !dogRunning  && !dogWalking && !atTarget){
				dogWalk(firstPersonController);
				dogToPlayer = true;
			}

			//moving towards tent
			if (dogToTent) {

				//player is 30 meters or more from nearest tent
				if (distanceNearestToTent >= 30) {
					dogRunning = false;
					dogWalking = false;
					dogToTent = false;
					atTarget = false;
				}

			} 

			if (dogToPlayer) {

				//player is less than 30 meters nearest tent
				if (distanceNearestToTent < 30) {
					dogRunning = false;
					dogWalking = false;
					dogToPlayer = false;
					atTarget = false;
				}

				//start walking when near to player
				if(distanceFromFPC < 10){
					dogRunning = false;
					dogWalking = false;
					atTarget = false;
					dogWalk(firstPersonController);
				}
				
				//stop walking when near to player
				if(distanceFromFPC <= 3){
					dogRunning = false;
					dogWalking = false;
					dogToPlayer = false;
					atTarget = false;
				}
			}
		}


		//dog animations
		if (!dogWalking && !dogRunning && !dogJumping && !dogLaughing && !returningPlayer) {
				animation.Play ("Idled");
		} else {
			//look at code
			tempTarget.x = target.position.x;		//use target's x and z
			tempTarget.z = target.position.z;
			tempTarget.y = transform.position.y;	//use this object's y value
			transform.LookAt(tempTarget);
		}
	
		if (dogWalking) {
			float step = travelSpeed * Time.deltaTime;
			transform.position = Vector3.MoveTowards (transform.position, target.position, step);
			animation.Play ("Walk", PlayMode.StopAll);
			if(transform.position == target.position){
				//print ("reached target");
				dogWalking = false;
				atTarget = true;
			}
		}

		if (dogRunning) {
			float step = travelSpeed * Time.deltaTime;
			transform.position = Vector3.MoveTowards (transform.position, target.position, step);
			animation.Play ("Run", PlayMode.StopAll);
			if(transform.position == target.position){
				dogRunning = false;
				atTarget = true;
				//print ("reached target");
			}
		}

		if (dogJumping) {
			float step = travelSpeed * Time.deltaTime;
			transform.position = Vector3.MoveTowards (transform.position, target.position, step);
			animation.Play ("Jump Low", PlayMode.StopAll);
			if(transform.position == target.position){
				dogJumping = false;
				dogMeshRender.enabled = false;
				//print ("reached target");
			}
		}

		if (dogLaughing) {
			animation["Hit Front"].speed = 6f;
			animation.Play ("Hit Front", PlayMode.StopAll);
			transform.LookAt(firstPersonController.transform);
			if(Time.time >= (timeSet + 1)){
				dogLaughing = false;
				animationStart = false;
			 }
		}

	}

	private void distanceFPCtoDog(){
		float dist = Vector3.Distance(firstPersonController.transform.position, transform.position);
		//print("Distance to FPC: " + dist);
		distanceFromFPC = dist;
	}
	
	private void distanceFPCtoNearestTent(){
		GameObject location = temp1;
		nextStage = 1;
		float distFinal = Vector3.Distance(temp1.transform.position, firstPersonController.transform.position);

		float dist = Vector3.Distance(temp2.transform.position, firstPersonController.transform.position);
		if (dist < distFinal) {
			distFinal = dist;
			location = temp2;
			nextStage = 2;
		}

		dist = Vector3.Distance(temp3.transform.position, firstPersonController.transform.position);
		if (dist < distFinal) {
			distFinal = dist;
			location = temp3;
			nextStage = 3;
		}

		dist = Vector3.Distance(temp4.transform.position, firstPersonController.transform.position);
		if (dist < distFinal) {
			distFinal = dist;
			location = temp4;
			nextStage = 4;
		}

		dist = Vector3.Distance(temp5.transform.position, firstPersonController.transform.position);
		if (dist < distFinal) {
			distFinal = dist;
			location = temp5;
			nextStage = 5;
		}

		dist = Vector3.Distance(temp6.transform.position, firstPersonController.transform.position);
		if (dist < distFinal) {
			distFinal = dist;
			location = temp6;
			nextStage = 6;
		}

		//print("Distance to Neartes Tent number " + location + ": " + distFinal);
		distanceNearestToTent = distFinal;
		nearestTent = location;
	}

	public void moveToNextStage(){
		if (eventStarted) {
			eventStarted = false;
			dogMeshRender.enabled = true;
			dogRunning = false;
			dogWalking = false;
			dogToPlayer = false;
			atTarget = false;
		}
	}

	public void dogWalk(GameObject t){
		target = t.transform;
		travelSpeed = walkingSpeed;
		dogWalking = true;
	}
	
	public void dogRun(GameObject t){
		target = t.transform;
		travelSpeed = runningSpeed;
		dogRunning = true;
		StartCoroutine (playBarking ());
	}
	
	public void dogJump(){

		if (animationStart) return;

		animationStart = true;

		switch (nextStage) {
		case 1:
			target = jumpTarget1.transform;
			break;
		case 2:
			target = jumpTarget2.transform;
			break;
		case 3:
			target = jumpTarget3.transform;
			break;
		case 4:
			target = jumpTarget4.transform;
			break;
		case 5:
			target = jumpTarget5.transform;
			break;
		case 6:
			target = jumpTarget6.transform;
			break;
		}

		travelSpeed = 5;
		dogJumping = true;
		eventStarted = true;
		StartCoroutine (playSingleBark ());
	}

	public void dogLaugh(){

		if (dogMeshRender.enabled == false) animationStart = false;

		if (animationStart) return;
		animationStart = true;

		dogMeshRender.enabled = true;
		timeSet = Time.time;
		dogLaughing = true;
		StartCoroutine (playLaugh ());
	}

	private IEnumerator playSingleBark(){
		yield return new WaitForSeconds (.5f);
		//audio.pitch = 1f;
		audio.clip = dogSingleBark;
		audio.Play ();
	}
	
	private IEnumerator playBarking(){
		yield return new WaitForSeconds (.5f);
		audio.clip = dogBark;
		audio.Play ();
	}

	private IEnumerator playLaugh(){
		yield return new WaitForSeconds (.5f);
		//audio.pitch = 1.5f;
		audio.clip = dogLaughter;
		audio.Play ();
	}
}
