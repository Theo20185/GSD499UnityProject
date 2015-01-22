using UnityEngine;
using System.Collections;

public class DogScript : MonoBehaviour {

	public AudioClip dogSingleBark;
	public AudioClip dogBark;
	public AudioClip dogLaughter;

	private Transform target;			//final target to travel to
	private float travelSpeed;			//final travel speed
	private float walkingSpeed = 1.5f;	//set walking speed
	private float runningSpeed = 3.0f;	//set running speed

	private bool dogWalking = false;	//true if dog is to walk
	private bool dogRunning = false;	//true if dog is to run
	private bool dogJumping = false;	//true if dog is to jump
	private bool dogLaughing = false;	//true if dog is to laugh

	private float timeSet;				//use for timing

	// Use this for initialization
	void Start () {
		//GameObject temp = GameObject.Find ("Cube");
		//dogWalk (temp);
		//dogRun (temp);
		//dogJump (temp);
		//dogLaugh ();
	}
	
	// Update is called once per frame
	void Update () {

		if (!dogWalking && !dogRunning && !dogJumping && !dogLaughing) {
			animation.Play ("Idled");
		}
	
		if (dogWalking) {
			float step = travelSpeed * Time.deltaTime;
			transform.position = Vector3.MoveTowards (transform.position, target.position, step);
			animation.Play ("Walk");
			if(transform.position == target.position){
				dogWalking = false;
				//print ("reached target");
			}
		}

		if (dogRunning) {
			float step = travelSpeed * Time.deltaTime;
			transform.position = Vector3.MoveTowards (transform.position, target.position, step);
			animation.Play ("Run");
			if(transform.position == target.position){
				dogRunning = false;
				//print ("reached target");
			}
		}

		if (dogJumping) {
			float step = travelSpeed * Time.deltaTime;
			transform.position = Vector3.MoveTowards (transform.position, target.position, step);
			animation.Play ("Jump High");
			if(transform.position == target.position){
				dogJumping = false;
				//print ("reached target");
			}
		}

		if (dogLaughing) {
			animation["Hit Front"].speed = 6f;
			animation.Play ("Hit Front");
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
