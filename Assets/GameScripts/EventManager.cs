using UnityEngine;
using System.Collections;

//Script to manage the shooting event.
//Use TriggerEvent to begin a shooting event. This should be called upon collision with an event hotspot. TestObject has a hotkey of E assigned to test event functionality on Lake 1.
//Once an event is triggered, the event should take care of itself. Based on the results, the control will either be returned to the player or the game will enter "Game Over" mode.
using System.Collections.Generic;

public class EventManager : MonoBehaviour 
{
	public GameManager gameManager;

	public float maxSpeed;
	public GameObject firstPersonController;
	public GameObject mainCamera;
	public GameObject shotgun;
	public GameObject emptyGun;
	public GameObject dogPrefab;
	public Transform duckPrefabEasy;
	public Transform duckPrefabHard;
    public Transform clayPrefab;
    public Transform duckCallPrefab;
    public Transform duckDecoyPrefab;
	public Transform fadeOut;
	public GUIText requirements;
	public GUIText countdownTimer;
	public GUIText flyAway;
	public GUIText flyAwayTimer;
	public GUIText roundResults;
	public GUIText eventResults;
	public GUIText hudScore;
	public GUIText hudHighScore;
	public GUITexture crosshair;
	public GUITexture shotgunShell;
	public GUITexture trophy;
	public GUITexture deadDuck;
    public GUITexture duckCallTexture;
    public GUITexture duckDecoyTexture;

	//Flags for different stages of events.
	private enum EventStage
	{
		Null, TransitionIn, ShowRequirements, CountdownToEvent, EventActive, ShowRoundResults, ShowEventResults, TransitionOut
	}

	private EventStage stage = EventStage.Null;

	private Transform shootingPosition;
	private Transform targetSpawn;
	private float timer;
	private int roundNumber;
	private int targetsShotTotal;
	private int targetsShotRound;
	private bool targetsInPlay;
	private List<Transform> targetsInPlayList;
    private int numDecoys; //how many decoys the player has
    private int numCalls; //how many duck calls the player has
    private float decoyTime;
	private int shells;
	private int score;
	private int highScore;

	// Use this for initialization
	public void Start () 
	{
		targetsInPlayList = new List<Transform> ();
        numDecoys = 1;
        numCalls = 1;
		score = 0;
		highScore = PlayerPrefs.GetInt ("ThatDuckingGame_HighScore", 0);
		hudScore.text = "Score: " + score;
		hudHighScore.text = "High: " + highScore;
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
        {
            checkKeys();
            UpdateEvent();
        }
		if (stage == EventStage.ShowRoundResults)
			ShowRoundResults ();
		if (stage == EventStage.ShowEventResults)
			ShowEventResults ();        
	}

    private void checkKeys()
    {
        if (targetSpawn.GetComponent<TargetSpawn>().targetType != TargetSpawn.TargetType.Clay)
        {
            if (Input.GetKeyUp(KeyCode.C)) //spawn a duck call
            {
                if (numCalls > 0 && roundNumber > 1)
                {
                    numCalls--;
                    DuckCall callScript = duckCallPrefab.gameObject.GetComponent<DuckCall>();
                    callScript.startDuckCall();
                    roundNumber--; //crude way to implement our desired functionality. Removes one round so we can get an extra one
                }
            }
            if (Input.GetKeyUp(KeyCode.V)) //spawn a decoy
            {
                if (numDecoys > 0)
                {
                    numDecoys--;
                    Transform decoy = (Instantiate(duckDecoyPrefab, Vector3.zero, Quaternion.identity) as Transform);
                    decoyTime = Time.time + decoy.GetComponent<Decoy>().decoyLife;
                    Debug.Log("Decoy Time" + decoyTime.ToString());
                }
            }
        }
    }

	public void OnGUI()
	{
		if (stage == EventStage.CountdownToEvent || stage == EventStage.EventActive)
			UpdateEventOnGUI ();
	}

	public void TriggerEvent(Transform shootingPosition, Transform duckSpawn)
	{
		this.shootingPosition = shootingPosition;
		this.targetSpawn = duckSpawn;
		targetsShotTotal = 0;
		roundNumber = 0;

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
        Vector3 lookAtPos;
        float magnitude = (delta.x * delta.x) + (delta.z * delta.z);

		if (magnitude > 0.04f) 
		{
			//Move the FPC to look at the Focus target.
            lookAtPos = targetSpawn.position;
            //clays have the spawn position right by the player so we have to fudge the view position a bit.
            if (targetSpawn.GetComponent<TargetSpawn>().targetType == TargetSpawn.TargetType.Clay)
            {
                lookAtPos += firstPersonController.transform.forward * 50;
                lookAtPos.y += 2; //look up a bit
            }
            else
            {
                lookAtPos.y += 10; //look up a bit - need more lift because these targets are farther out
            }

            

		    firstPersonController.transform.LookAt (lookAtPos);

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

    public string GetTargetType()
    {
        string target = targetSpawn.GetComponent<TargetSpawn>().targetType == TargetSpawn.TargetType.Clay ? "Clays" : "Ducks";
        return target;
    }

	private void ShowRequirements()
	{
		timer = timer - Time.deltaTime;
        string target = GetTargetType();

		if (timer < 6f && timer >= 4f) 
		{
			requirements.text = string.Concat ("Event Start!","\n",
			                                   "Rounds: ", targetSpawn.GetComponent<TargetSpawn>().spawnRounds.ToString ("n0"));
		} 
		else if (timer < 4f && timer >= 2f) 
		{
			requirements.text = string.Concat ("Event Start!","\n",
			                                   "Rounds: ", targetSpawn.GetComponent<TargetSpawn>().spawnRounds.ToString("n0"), "\n", 
			                                   target, " Needed: ", targetSpawn.GetComponent<TargetSpawn>().targetsNeeded.ToString ("n0"));
		} 
		else if (timer < 2f) 
		{
			requirements.text = string.Concat ("Event Start!","\n",
			                                   "Rounds: ", targetSpawn.GetComponent<TargetSpawn>().spawnRounds.ToString ("n0"), "\n", 
			                                   target, " Needed: ", targetSpawn.GetComponent<TargetSpawn>().targetsNeeded.ToString ("n0"), "\n",
			                                   target, " at Event: ", (targetSpawn.GetComponent<TargetSpawn>().spawnRounds * targetSpawn.GetComponent<TargetSpawn>().targetsPerSpawn).ToString ("n0"));
		} 
		else 
			requirements.text = "Event Start!";

		if (timer < 0)
			InitializeCountdownToEvent ();
	}
	
	private void InitializeCountdownToEvent()
	{
		roundNumber++;
		requirements.enabled = false;
		roundResults.enabled = false;
		countdownTimer.enabled = true;
		timer = 3f;
		shells = 3;
		stage = EventStage.CountdownToEvent;
	}

	private void CountDownToEvent()
	{
		flyAway.enabled = false;
		flyAwayTimer.enabled = false;
		timer = timer - Time.deltaTime;
		countdownTimer.text = timer > 0 ? "Round " + roundNumber + "\n" + timer.ToString ("N2") : "Round " + roundNumber + "\nGO!";

		//call the dog jump animation
		dogPrefab.GetComponent<DogScript>().dogReady ();

		if (timer < -2)
			InitializeUpdateEvent ();
	}

	private void InitializeUpdateEvent()
	{
		countdownTimer.enabled = false;
		timer = 0.5f;
		targetsInPlay = false;
		targetsShotRound = 0;
		stage = EventStage.EventActive;
	}

	private void UpdateEvent()
	{
        float dTime = Time.deltaTime;
		timer = timer - dTime;

		if (timer <= 0 && targetsInPlay == false)
			SpawnTargets ();

		if (Input.GetMouseButtonDown (0) && flyAway.enabled == false)
			FireGun ();

		bool allTargetsAreDead = true;

		for (int targetsInPlayIndex = 0; targetsInPlayIndex < targetsInPlayList.Count; targetsInPlayIndex++) 
		{
			if (targetsInPlayList [targetsInPlayIndex] == null)
				targetsInPlayList.RemoveAt (targetsInPlayIndex);
			else
			{
				if (targetsInPlayList[targetsInPlayIndex].GetComponent<ShootingTarget>().CanDestroy == true)
				{
					targetsInPlayList[targetsInPlayIndex].GetComponent<ShootingTarget>().Destroy();
					targetsInPlayList.RemoveAt (targetsInPlayIndex);
				}
				else
				{
					if (targetsInPlayList[targetsInPlayIndex].GetComponent<ShootingTarget>().IsDead == false)
						allTargetsAreDead = false;
				}
			}
		}

        if (targetSpawn.GetComponent<TargetSpawn> ().targetType != TargetSpawn.TargetType.Clay) 
 		{
		    float flyAwayTime = (targetSpawn.GetComponent<TargetSpawn> ().escapeTime - ((-1 * timer) + 0.5f));
            float dtLeft = (decoyTime - Time.time);

            if (dtLeft > flyAwayTime) flyAwayTime = dtLeft;

			if (flyAwayTime < 0) 
			{
				Debug.Log ("Fly Away Time: " + flyAwayTime.ToString ("N2"));

				flyAway.enabled = true;
				flyAwayTimer.text = "Time: 0.00";

				//call the dog jump laugh
				dogPrefab.GetComponent<DogScript> ().dogLaugh ();
			} 
			else if (targetsInPlay == true && allTargetsAreDead == true)
					flyAwayTimer.text = "Time: 0.00";
			else
					flyAwayTimer.text = "Time: " + flyAwayTime.ToString ("N2");
		}

		if (targetsInPlayList.Count == 0 && targetsInPlay == true) 
		{
			InitializeShowRoundResults ();
		}
	}

	private void UpdateEventOnGUI()
	{
		if (flyAway.enabled == false) 
		{
			float x = Input.mousePosition.x - (crosshair.texture.width / 2);
			float y = Screen.height - Input.mousePosition.y - (crosshair.texture.height / 2);
			GUI.DrawTexture (new Rect (x, y, crosshair.texture.width, crosshair.texture.height), crosshair.texture);
		}

		for (int guiShellsIndex = 0; guiShellsIndex < shells; guiShellsIndex++)
			GUI.DrawTexture (new Rect(10 + (guiShellsIndex * 50), Screen.height - 110, shotgunShell.texture.width, shotgunShell.texture.height), shotgunShell.texture);

		GUI.TextArea (new Rect ((Screen.width / 2) - (deadDuck.texture.width / 2) - 55, Screen.height - 110, deadDuck.texture.width, deadDuck.texture.height), targetsShotTotal.ToString ("N0"));
		GUI.DrawTexture (new Rect((Screen.width / 2) - (deadDuck.texture.width / 2) - 55, Screen.height - 110, deadDuck.texture.width, deadDuck.texture.height), deadDuck.texture);

		GUI.TextArea (new Rect ((Screen.width / 2) + (trophy.texture.width / 2) + 55, Screen.height - 110, trophy.texture.width, trophy.texture.height), targetSpawn.GetComponent<TargetSpawn> ().targetsNeeded.ToString ("N0"));
		GUI.DrawTexture (new Rect((Screen.width / 2) + (trophy.texture.width / 2) + 55, Screen.height - 110, trophy.texture.width, trophy.texture.height), trophy.texture);

        GUI.TextArea(new Rect((Screen.width / 2) + (duckCallTexture.texture.width / 2) + 255, Screen.height - 110, duckCallTexture.texture.width, duckCallTexture.texture.height), numCalls.ToString("N0"));
        GUI.DrawTexture(new Rect((Screen.width / 2) + (duckCallTexture.texture.width / 2) + 255, Screen.height - 110, duckCallTexture.texture.width, duckCallTexture.texture.height), duckCallTexture.texture);

        GUI.TextArea(new Rect((Screen.width / 2) + (duckDecoyTexture.texture.width / 2) - 355, Screen.height - 110, duckDecoyTexture.texture.width, duckDecoyTexture.texture.height), numDecoys.ToString("N0"));
        GUI.DrawTexture(new Rect((Screen.width / 2) + (duckDecoyTexture.texture.width / 2) - 355, Screen.height - 110, duckDecoyTexture.texture.width, duckDecoyTexture.texture.height), duckDecoyTexture.texture);        
	}


	private void InitializeShowRoundResults()
	{
		flyAway.enabled = false;
		flyAwayTimer.enabled = false;
		roundResults.enabled = true;
		roundResults.text = "Round " + roundNumber + ": Bagged " + targetsShotRound + "!";
		stage = EventStage.ShowRoundResults;
		timer = 2f;

		//call the dog jump laugh
		if(targetsShotRound != 0 ) dogPrefab.GetComponent<DogScript>().dogCaptureDuck ();
	}

	private void ShowRoundResults()
	{
		timer = timer - Time.deltaTime;

		if (timer <= 0) 
		{
			if (roundNumber == targetSpawn.GetComponent<TargetSpawn>().spawnRounds)
				InitializeShowEventResults ();
			else
				InitializeCountdownToEvent ();
		}
	}

	private void InitializeShowEventResults()
	{
		roundResults.enabled = false;
		eventResults.enabled = true;
		stage = EventStage.ShowEventResults;
		timer = 6f;
	}

	private void ShowEventResults()
	{
		timer = timer - Time.deltaTime;

		bool passed = targetsShotTotal >= targetSpawn.GetComponent<TargetSpawn> ().targetsNeeded;

		if (timer < 4f && timer >= 2f) 
		{
			eventResults.text = string.Concat ("Event Complete!","\n",
			                                   "Bagged: ", targetsShotTotal);
		} 
		else if (timer < 2f) 
		{
			eventResults.text = string.Concat ("Event Complete!","\n",
			                                   "Bagged: ", targetsShotTotal, "\n",
			                                   "Result: ", passed ? "Passed!" : "Failed!");		
		} 
		else 
			eventResults.text = "Event Complete!";

		if (timer <= 0)
			TransitionOut ();

		dogPrefab.GetComponent<DogScript>().moveToNextStage ();
	}

	private void TransitionOut()
	{
		eventResults.enabled = false;
		stage = EventStage.TransitionOut;

		if (targetsShotTotal < targetSpawn.GetComponent<TargetSpawn> ().targetsNeeded) 
		{
			//Event Failed
			PlayerPrefs.SetInt ("ThatDuckingGame_HighScore", highScore);
			Instantiate (fadeOut);
			Application.LoadLevel ("GameOver");
		} 
		else 
		{
			//Event Passed
			ReturnControl();
		}
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
		flyAwayTimer.enabled = true;

		targetsInPlay = true;
		targetsInPlayList = new List<Transform> ();

		for (int targets = 0; targets < targetSpawn.GetComponent<TargetSpawn>().targetsPerSpawn; targets++) 
		{
			Transform target = duckPrefabEasy;

			if (targetSpawn.GetComponent<TargetSpawn> ().targetType == TargetSpawn.TargetType.HardDuck)
				target = duckPrefabHard;

            if (targetSpawn.GetComponent<TargetSpawn>().targetType == TargetSpawn.TargetType.Clay)
                target = clayPrefab;

			target.GetComponent<ShootingTarget> ().escapeTime = targetSpawn.GetComponent<TargetSpawn> ().escapeTime;

            //positions are in world coordinates but the player can turn all around and blinds 
            //are not aligned to world axes. So, the spawn X,Y,Z should be in local player coordinates
            //based on the player's view. This'll look odd but trust me on this one, it works.
            //first get our absolute offsets in X, Y, Z
            float x = (float)Random.Range(0, targetSpawn.GetComponent<TargetSpawn>().spawnSpan.x) - (targetSpawn.GetComponent<TargetSpawn>().spawnSpan.x / 2f);
            float y = (float)Random.Range(0, targetSpawn.GetComponent<TargetSpawn>().spawnSpan.y) - (targetSpawn.GetComponent<TargetSpawn>().spawnSpan.y / 2f);
            float z = (float)Random.Range(0, targetSpawn.GetComponent<TargetSpawn>().spawnSpan.z) - (targetSpawn.GetComponent<TargetSpawn>().spawnSpan.z / 2f);

            //Then transform them based on local X, Y, Z from the point of view of the player
            Vector3 xVector = x * firstPersonController.transform.right;
            Vector3 yVector = y * firstPersonController.transform.up;
            Vector3 zVector = z * firstPersonController.transform.forward;
            //Then combine all those vectors together to get the total X, Y, Z offset in world coordinates
            Vector3 combinedVector = xVector + yVector + zVector;

            //and apply the offset, once again this is now in world coordinates
            x = combinedVector.x + targetSpawn.GetComponent<TargetSpawn>().transform.position.x;
            y = combinedVector.y + targetSpawn.GetComponent<TargetSpawn>().transform.position.y;
            z = combinedVector.z + targetSpawn.GetComponent<TargetSpawn>().transform.position.z;

			targetsInPlayList.Add (Instantiate (target, new Vector3 (x, y, z), Quaternion.identity) as Transform);
		}

		for (int targetsInPlayIndex = 0; targetsInPlayIndex < targetsInPlayList.Count; targetsInPlayIndex++) 
			targetsInPlayList [targetsInPlayIndex].name = targetsInPlayList [targetsInPlayIndex].name + targetsInPlayIndex.ToString ();
	}

	private void FireGun()
	{
		if (shells > 0) 
		{
			shotgun.audio.Play ();
			shells--;

			Ray ray = mainCamera.camera.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast (ray, out hit))
			{
				for (int targetsInPlayIndex = 0; targetsInPlayIndex < targetsInPlayList.Count; targetsInPlayIndex++)
				{
					if (hit.transform.name == targetsInPlayList[targetsInPlayIndex].name && targetsInPlayList[targetsInPlayIndex].GetComponent<ShootingTarget>().IsDead == false)
					{
						targetsInPlayList[targetsInPlayIndex].GetComponent<ShootingTarget>().Die ();
						targetsShotTotal++;
						targetsShotRound++;
						score += targetSpawn.GetComponent<TargetSpawn>().targetScore;
						hudScore.text = "Score: " + score;

						if (score > highScore)
						{
							highScore = score;
							hudHighScore.text = "High: " + highScore;
						}
					}
				}
			}
		} 
		else 
		{
			Debug.Log ("Out of shells!");
			emptyGun.audio.Play ();
		}
	}
}
