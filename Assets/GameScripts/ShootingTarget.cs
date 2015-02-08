using UnityEngine;
using System.Collections;

//A single target. Knows only about itself - just a script upon the target object
//Instantiate objects with this script on them and it'll take care of the flight
//DO NOT INSTANTIATE THINGS OFF OF THE SCREEN. DONT DO IT! I MEAN IT!

public class ShootingTarget : MonoBehaviour {
    //these next four can be modified to change the difficulty of this duck type.
    public Color duckColor; //red, green, black, white, doesn't matter. God loves them all the same.
    public float escapeTime; //how long will duck fly around before escaping?
    public float decisionTiming; //how often this sort of duck will update its decisions
    public float duckSpeed; //how quick does this duck fly?
    public Transform splashFX; //link to prefab that gives us a splash

    protected float startTime; //when was this thing spawned into being?
    protected float lastActionTime; //last time we made a decision - if we're a duck

    //more duck specific stuff
    private bool isEscaping; //sweet freedom!
    private bool isDead; //I think you know what this means for our poor duck
    private float deadTime; //time we started to die
    private bool didInitial; //did we do our initial placement already?
    private bool goingForDecoy; //are we targetting the decoy currently?

    //clay specific stuff
    private bool isClay; //is this a simple clay target instead?
    private bool didShoot; //have we launched the clay yet?

    private enum DeadState
    {
        NOTDEAD,
        DYING,
        FALLING,
        HITGROUND
    }

    private DeadState deadState;

    //These are automatically filled in and cached
    private GameObject camera;
    private GameObject player;
    private AudioSource[] sounds;
    private AudioSource flapping;
    private AudioSource quack;

    private GameObject decoy;

    private Quaternion targetVector; //the travel vector we want to achieve.
    private Vector3 targetPos; //where we are heading toward

    public void Start()
    {
        isClay = false;
        EventManager eventMgr = (EventManager)GameObject.Find("EventManager").GetComponent<EventManager>();
        if (eventMgr.GetTargetType() == "Clays") isClay = true;

        camera = GameObject.Find("Main Camera");
        player = GameObject.Find("First Person Controller");


        if (!isClay)
        {
            sounds = GetComponents<AudioSource>();
            flapping = sounds[0];
            quack = sounds[1];
            isEscaping = false;
            this.animation.Play("fly");
            startTime = Time.time;
            lastActionTime = startTime - 10f;
            Renderer thisRenderer = this.GetComponentInChildren<Renderer>();
            thisRenderer.material.color = duckColor;
            didInitial = false;
            goingForDecoy = false;
        }
        else
        {
            //clay position should be properly set by event manager so don't try to move it here.
            //Vector3 clayPos = player.transform.position;
            startTime = Time.time;
            isDead = false;     
            //Vector3 forwardplayer = player.transform.forward;
            //clayPos += (forwardplayer * 2); //start decoy just a short distance in front of the player.
            //clayPos.y -= 1; //also down a bit so it appears that it launched from below somewhere
            //transform.position = clayPos;
        }
    }

    public void Update()
    {
        if (!isClay)
        {
            if (isDead) //do special processing if we're dead
            {
                deadUpdate();
                return;
            }

            float distanceToGround = 0.0f;

            //Find out if a decoy exists - this is pretty wasteful. It would be more efficient
            //to have external code notify us of added and subtracted decoys
            decoy = GameObject.FindGameObjectWithTag("DuckDecoy");

            //Fire a ray at the ground to see how high up we are
            RaycastHit hit;
            if (Physics.Raycast(transform.position, -Vector3.up, out hit))
                distanceToGround = hit.distance;
            //Debug.Log(distanceToGround.ToString());

            //Get the duck's position within the viewport
            //the bottom-left of the camera is (0,0); the top-right is (1,1)
            Vector3 vPoint = camera.camera.WorldToViewportPoint(transform.position);

            if (!isEscaping)
            {

                //time to contemplate a decision now so long as we're not making a break for it                
                if (Time.time > (lastActionTime + decisionTiming))
                {
                    quack.Play();
                    Debug.Log("Decision");
                    lastActionTime = Time.time;
                    if (didInitial)
                    {
                        SetNewHeading(false);
                    }
                    else //causes an immediate course change
                    {
                        SetNewHeading(true);
                        didInitial = true;
                    }
                }

                //how far away from the target are we?
                Vector3 offsetError = targetPos - transform.position;
                //Debug.Log(offsetError.sqrMagnitude.ToString());

                //we got close enough to target
                if (offsetError.sqrMagnitude < 5f)
                {
                    Debug.Log("Got to where I was going");
                    SetNewHeading();
                }

                //Escape if it is time, we're not already escaping, and there are no decoys
                if (Time.time > (startTime + escapeTime) && (decoy == null))
                {
                    Debug.Log("Escape!");
                    isEscaping = true; //inhibits calling this code more than once
                    //this is a pretty stiff incline upward
                    targetVector = Quaternion.Euler(new Vector3(-50f, Random.Range(-180f, 180), 0));
                    duckSpeed = 50; //FAST!
                    quack.Play();
                    //must update GUI to show that the duck got away. A signal of some sort must be sent here.
                }

                //if it was time to escape, we're not currently escaping, but there is a decoy
                if (Time.time > (startTime + escapeTime) && !(decoy == null))
                {
                    escapeTime += 2; //give us more time
                }
            }

            if (Time.time > (startTime + escapeTime + 2)) //trust me... the escape wont take long.
            {
                this.animation.Stop();
                DestroyObject(this.gameObject); //DIE! DIE! DIE!
            }

            //interpolate the rotation vector smoothly - but quickly
            UpdateTargetVector();
            transform.rotation = Quaternion.Lerp(transform.rotation, targetVector, Time.deltaTime * 2);

            //finally, move in the current direction
            transform.Translate(Vector3.forward * Time.deltaTime * duckSpeed);
        }
        else //clay shot
        {
            if (!didShoot)
            {
                //Vector3 forwardplayer = player.transform.forward;
                Rigidbody rigidbody = GetComponent<Rigidbody>();
                //We're about to do a single push so it has to be pretty hard
                Vector3 forceShoot = player.transform.forward * 450; //push in front of us hard
                forceShoot.y += 500; //add some upward motion too.
                rigidbody.AddForce(forceShoot);
                didShoot = true; //no mo' shoving
            }
            if (Time.time > (startTime + escapeTime))
            {
                Debug.Log("Escape!");
                isEscaping = true; //inhibits calling this code more than once
                DestroyObject(this.gameObject); //clay is no more
            }
        }
    }

    //Find view frustum and set a new random point to head to within a reasonable space in that frustum
    //We're interested in targetting the duck anywhere on the screen and within a certain distance
    //of the camera.
    //But, if there is a decoy we alternate targetting that and flying around
    private void SetNewHeading(bool immediate = false)
    {
        if (decoy == null || goingForDecoy == true)
        {
            float Z = Random.Range(15f, 40f);
            float X = Random.Range(100, camera.camera.pixelWidth - 100);
            float Y = Random.Range((camera.camera.pixelHeight * 0.66f), camera.camera.pixelHeight - 110);
            targetPos.x = X;
            targetPos.y = Y;
            targetPos.z = Z;
            Debug.Log("Before: " + targetPos.ToString());
            targetPos = camera.camera.ScreenToWorldPoint(targetPos);
            Debug.Log("After: " + targetPos.ToString());
            goingForDecoy = false;
        }
        else //would only get here is there was a decoy and we were not previously going for it
        {
            targetPos = decoy.transform.position;
            targetPos.y += 1.5f; //aim for 1.5 above the decoy
            goingForDecoy = true;
        }

        UpdateTargetVector();
        if (immediate) transform.rotation = targetVector;
    }

    private void UpdateTargetVector()
    {
        Quaternion temp = transform.rotation; //remember current rotation
        transform.rotation = Quaternion.identity;
        transform.LookAt(targetPos); //calculate a new one to look at decoy
        targetVector = transform.rotation; //store that as our target
        transform.rotation = temp; //restore current rotation
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if (!isClay)
        {
            if ((deadState == DeadState.FALLING || deadState == DeadState.DYING))
            {
                if (other.name.Contains("Lake"))
                { //splash if on a lake
                    Instantiate(splashFX, transform.position, Quaternion.identity);
                }
                deadState = DeadState.HITGROUND;
            }
            else SetNewHeading(); //if we hit the water or land and aren't dying then the path is messed up
        }

    }

    public void Die()
    {
        isDead = true;
        if (!isClay)
        {
            flapping.Stop(); //if we were playing a flapping sound then cancel it. We're dead.
            deadState = DeadState.DYING;
            deadTime = Time.time;
            animation.Play("inAirDeath");
        }
    }

    private void deadUpdate()
    {
        RaycastHit hit;
        float distanceToGround = 40f;

        if (deadState == DeadState.DYING && Time.time > deadTime + 0.5f)
        {
            //transition to falling animation
            deadState = DeadState.FALLING;
            animation.Play("falling");
        }

        if (deadState == DeadState.FALLING)
        {
            transform.rotation = Quaternion.identity;
            transform.Translate(0f, -Time.deltaTime * 10f, 0f);
            //if (Physics.Raycast(transform.position, -Vector3.up, out hit))
            //    distanceToGround = hit.distance;
            //Debug.Log(distanceToGround.ToString());
            //if (distanceToGround < 0.5f)
            //{
            //    deadState = DeadState.HITGROUND;
            //    animation.Play("FallingToHitTheFloor");
            //    deadTime = Time.time;
           // }
        }
        if (deadState == DeadState.HITGROUND && Time.time > deadTime + 0.4f)
        {
            animation.Stop();                       
        }
    }

    public void Destroy()
    {
        Destroy(this.gameObject); 
    }

	public bool IsDead
	{
		get 
        {
			return isDead;
        }
	}

	public bool CanDestroy
	{
		get
		{
            if (!isClay)
                return deadState == DeadState.HITGROUND && Time.time > deadTime + 0.4f;
            else return isDead;
		}
	}

}
