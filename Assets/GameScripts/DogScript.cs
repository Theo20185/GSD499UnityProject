using UnityEngine;
using System.Collections;

public class DogScript : MonoBehaviour {

	public AudioClip dogSingleBark;
	public AudioClip dogBark;
	public AudioClip dogLaughter;

	private Transform target;			//final target to travel to
	private float travelSpeed;			//final travel speed
	private float walkingSpeed = 1.5f;	//set walking speed
	private float runningSpeed = 5.0f;	//set running speed

	private bool dogWalking = false;	//true if dog is to walk
	private bool dogRunning = false;	//true if dog is to run
	private bool dogJumping = false;	//true if dog is to jump
	private bool dogLaughing = false;	//true if dog is to laugh

	private float timeSet;				//use for timing

	private Vector3 tempTarget;	

	private GameObject temp1;
	private GameObject temp2;
	private GameObject temp3;

	// Use this for initialization
	void Start () {
		temp1 = GameObject.Find ("CubeTarget1");
		temp2 = GameObject.Find ("CubeTarget2");
		temp3 = GameObject.Find ("CubeTarget3");
	}
	
	// Update is called once per frame
	void Update () {
		
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
				dogWalking = false;
				//print ("reached target");
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
