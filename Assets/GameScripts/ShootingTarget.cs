using UnityEngine;
using System.Collections;

//A single duck. Knows only about itself - just a script upon the duck object
//Instantiate duck objects and they'll take care of themselves
//DO NOT INSTANTIATE DUCKS OFF OF THE SCREEN. DONT DO IT! I MEAN IT!

public class ShootingTarget : MonoBehaviour {
    //these next three can be modified to change the difficulty of this duck type.
    public Color duckColor; //red, green, black, white, doesn't matter. God loves them all the same.
    public float escapeTime; //how long will duck fly around before escaping?
    public float decisionTiming; //how often this sort of duck will update its decisions
    public float duckSpeed; //how quick does this duck fly?

    protected float startTime; //when was this duck spawned into being?
    protected float lastActionTime; //last time we made a decision
    protected float inhibitTime; //keep some path modification code from executing for a bit

    private bool isEscaping; //sweet freedom!
    private bool isAscending; //false would be descending
    private bool isDead; //I think you know what this means for our poor duck
    private float deadTime; //time we started to die

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
    private AudioSource[] sounds;
    private AudioSource flapping;
    private AudioSource quack;

    private GameObject decoy;

    private Quaternion targetVector; //the travel vector we want to achieve.
    private Vector3 originalPos; //our starting point

    public void Start()
    {
        sounds = GetComponents<AudioSource>();
        flapping = sounds[0];
        quack = sounds[1];
        camera = GameObject.Find("Main Camera");
        isAscending = true;
        isEscaping = false;
        this.animation.Play("fly");
        startTime = Time.time;
        lastActionTime = startTime;
        inhibitTime = Time.time - 1.0f;
        originalPos = transform.position;
        Renderer thisRenderer = this.GetComponentInChildren<Renderer>();
        thisRenderer.material.color = duckColor;
        setNewDirection(true); //immediately set direction with no interpolation
    }

    public void Update()
    {
        //we have a choice to make at every update.
        //We can continue along our current vector.
        //We can escape (if the elapsed time is past the threshold)
        //We can change course

        if (isDead) //do special processing if we're dead
        {
            deadUpdate();
            return;
        }

        float distanceToGround = 0.0f;

        //interpolate the rotation vector smoothly - but quickly
        transform.rotation = Quaternion.Lerp(transform.rotation, targetVector, Time.deltaTime * 2);

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

        //Make sure the duck has not gone off the left or right of the screen
        //these are a bit before the edge of the screen so that the duck has time to turn around
        if (vPoint.x < 0.075)
        {
            Debug.Log("Hit left edge of screen");
            targetOriginalPos();           
        }

        if (vPoint.x > 0.925)
        {
            Debug.Log("Hit right edge of screen");
            targetOriginalPos();
        }

        //distance of duck from camera
        //Debug.Log(vPoint.z.ToString());
        if (vPoint.z < 20f)
        {            
            Debug.Log("Too close to player");
            targetOriginalPos();
            //setNewDirection();
        }

        //Also don't let duck leave top of screen unless it is escaping
        if ((vPoint.y > 0.90 || distanceToGround > 20.0f) && !isEscaping)
        {
            Debug.Log("Duck is too high");
            //isAscending = false; //got to go down!
            //setNewDirection();
            targetOriginalPos();
            inhibitTime = Time.time + 1.0f;
        }


        //Escape if it is time, we're not already escaping, and there are no decoys
        if (Time.time > (startTime + escapeTime) && !isEscaping && (decoy == null))
        {
            Debug.Log("Escape!");
            isEscaping = true; //inhibits calling this code more than once
            //this is a pretty stiff incline upward
            targetVector = Quaternion.Euler(new Vector3(-50f, Random.Range(0, 360), 0));
            duckSpeed = 50; //FAST!
            quack.Play();
            //must update GUI to show that the duck got away. A signal of some sort must be sent here.
        }

        //if it was time to escape, we're not currently escaping, but there is a decoy
        if (Time.time > (startTime + escapeTime) && !isEscaping && !(decoy == null))
        {
            escapeTime += 2; //give us more time
        }

        if (Time.time > (startTime + escapeTime + 2)) //trust me... the escape wont take long.
        {
            this.animation.Stop();
            DestroyObject(this.gameObject); //DIE! DIE! DIE!
        }

        //time to contemplate a decision now so long as we're not making a break for it
        if (Time.time > (lastActionTime + decisionTiming) && !isEscaping && Time.time > inhibitTime)
        {
            quack.Play();
            Debug.Log("Decision");
            lastActionTime = Time.time;
            if (Random.value > 0.5f)
            {
                isAscending = false;
                Debug.Log("Going Down!");
            }
            else
            {
                isAscending = true;
                Debug.Log("Headin' Up!");
            }
            setNewDirection();
        }

        //finally, if we got here then try to move in the current direction
        transform.Translate(Vector3.forward * Time.deltaTime * duckSpeed);
    }

    private void setNewDirection(bool immediate = false)
    {
        Quaternion temp = Quaternion.identity;
        if (isAscending) targetVector = Quaternion.Euler(new Vector3(Random.Range(-40f, -10f), Random.Range(0f, 360f), 0));
        else
        {
            if (decoy == null)
            {
                targetVector = Quaternion.Euler(new Vector3(Random.Range(10f, 40f), Random.Range(0, 360), 0));
            }
            else //if we're headed downward and there is a decoy then shoot for it
            {
                Debug.Log("Aiming for decoy");
                temp = transform.rotation; //remember current rotation                            
                transform.LookAt(decoy.transform.position); //calculate a new one to look at decoy
                targetVector = transform.rotation; //store that as our target
                transform.rotation = temp; //restore current rotation
            }
        }
        if (immediate) transform.rotation = targetVector;
    }

    //used when we hit edge of screen. A rotation of 180 degrees in Y turns us around.
    private void reflectDirection()
    {
        Vector3 oldDirection = transform.rotation.eulerAngles;
        oldDirection.y += 180;
        targetVector = Quaternion.Euler(oldDirection);
        inhibitTime = Time.time + 1.0f;
    }

    private void targetOriginalPos()
    {
        Quaternion temp = transform.rotation; //remember current rotation                            
        transform.LookAt(originalPos); //calculate a new one to look at decoy
        targetVector = transform.rotation; //store that as our target
        transform.rotation = temp; //restore current rotation
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        //if the duck hit a trigger it probably means we hit the ground or lake so make sure to go up!
        isAscending = true;
        //and recalculate path
        setNewDirection();
    }

    public void Die()
    {
        isDead = true;
        deadState = DeadState.DYING;
        deadTime = Time.time;
        animation.Play("inAirDeath");
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
            transform.Translate(0f, -Time.deltaTime * 10f, 0f);
            if (Physics.Raycast(transform.position, -Vector3.up, out hit))
                distanceToGround = hit.distance;
            if (distanceToGround < 0.5f)
            {
                deadState = DeadState.HITGROUND;
                animation.Play("FallingToHitTheFloor");
                deadTime = Time.time;
            }
        }
        if (deadState == DeadState.HITGROUND && Time.time > deadTime + 0.4f)
        {
            Destroy(this.gameObject);
        }
    }

	public bool IsDead
	{
		get { return isDead; }
	}

}
