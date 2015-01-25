using UnityEngine;
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

	public int nextStage = 1;	//need to change to one when done
	private bool moveToNextSpot = false;

	// Use this for initialization
	void Start () {
		temp1 = GameObject.Find ("DogTarget1");
		temp2 = GameObject.Find ("DogTarget2");
		temp3 = GameObject.Find ("DogTarget3");
		temp4 = GameObject.Find ("DogTarget4");
		temp5 = GameObject.Find ("DogTarget5");
		temp6 = GameObject.Find ("DogTarget6");

		StartCoroutine(startDog ());
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown ("h")) {
			nextStage++;
			moveToNextSpot = true;
		}
		
		if (Input.GetKeyDown ("t")) {
			dogWalk (temp1);
		}
		
		if (Input.GetKeyDown ("y")) {
			dogRun (temp2);
		}
		
		if (Input.GetKeyDown ("u")) {
			dogJump (temp3);
		}
		
		if (Input.GetKeyDown ("i")) {
			dogLaugh ();
		}

		if (moveToNextSpot) {
				switch (nextStage) {
				case 1:
						dogRun (temp1);
						moveToNextSpot = false;
						break;
				case 2:
						dogRun (temp2);
						moveToNextSpot = false;
						break;
				case 3:
						dogRun (temp3);
						moveToNextSpot = false;
						nextStage = 99;
						break;
				case 4:
						dogRun (temp4);
						moveToNextSpot = false;
						nextStage = 99;
						break;
				case 5:
						dogRun (temp5);
						moveToNextSpot = false;
						nextStage = 99;
						break;
				case 6:
						dogRun (temp6);
						moveToNextSpot = false;
						nextStage = 99;
						break;
				}
		}

		if (!dogWalking && !dogRunning && !dogJumping && !dogLaughing) {
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
			}
		}

		if (dogRunning) {
			float step = travelSpeed * Time.deltaTime;
			transform.position = Vector3.MoveTowards (transform.position, target.position, step);
			animation.Play ("Run", PlayMode.StopAll);
			if(transform.position == target.position){
				dogRunning = false;
				//print ("reached target");
			}
		}

		if (dogJumping) {
			float step = travelSpeed * Time.deltaTime;
			transform.position = Vector3.MoveTowards (transform.position, target.position, step);
			animation.Play ("Jump High", PlayMode.StopAll);
			if(transform.position == target.position){
				dogJumping = false;
				//print ("reached target");
			}
		}

		if (dogLaughing) {
			animation["Hit Front"].speed = 6f;
			animation.Play ("Hit Front", PlayMode.StopAll);
			if(Time.time >= (timeSet + 1)){
				dogLaughing = false;
			 }
		}

	}

	void dogWalk(GameObject t){
		target = t.transform;
		travelSpeed = walkingSpeed;
		dogWalking = true;
	}
	
	void dogRun(GameObject t){
		target = t.transform;
		travelSpeed = runningSpeed;
		dogRunning = true;
		StartCoroutine (playBarking ());
	}
	
	void dogJump(GameObject t){
		target = t.transform;
		travelSpeed = 5;
		dogJumping = true;
		StartCoroutine (playSingleBark ());
	}

	void dogLaugh(){
		timeSet = Time.time;
		dogLaughing = true;
		StartCoroutine (playLaugh ());
	}
	
	IEnumerator startDog(){
		dogWalk (firstPersonController);
		yield return new WaitForSeconds (2f);
		dogWalking = false;
		moveToNextSpot = true;
	}

	IEnumerator playSingleBark(){
		yield return new WaitForSeconds (.5f);
		audio.clip = dogSingleBark;
		audio.Play ();
	}
	
	IEnumerator playBarking(){
		yield return new WaitForSeconds (.5f);
		audio.clip = dogBark;
		audio.Play ();
	}

	IEnumerator playLaugh(){
		yield return new WaitForSeconds (.5f);
		audio.clip = dogLaughter;
		audio.Play ();
	}
}
