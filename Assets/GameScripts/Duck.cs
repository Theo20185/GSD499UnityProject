using UnityEngine;
using System.Collections;

//A single duck. Knows only about itself - just a script upon the duck object
//Instantiate duck objects and they'll take care of themselves
//DO NOT INSTANTIATE DUCKS OFF OF THE SCREEN. DONT DO IT! I MEAN IT!

//This code immediately changes direction vectors which makes the duck motion
//kind of jerky. In the future it should use smooth interpolation

public class Duck : MonoBehaviour {
    protected float startTime; //when was this duck spawned into being?
    protected float lastActionTime; //last time we made a decision

    //these next three can be modified to change the difficulty of this duck type.
    public Color duckColor; //red, green, black, white, doesn't matter. God loves them all the same.
    public float escapeTime; //how long will duck fly around before escaping?
    public float decisionTiming; //how often this sort of duck will update its decisions
    public float duckSpeed; //how quick does this duck fly?

    private bool isEscaping;
    private bool isAscending; //false would be descending

    //These are automatically filled in and cached
    private GameObject camera;
    private AudioSource[] sounds;
    private AudioSource flapping;
    private AudioSource quack;

    private GameObject decoy;

    private Quaternion targetVector; //the travel vector we want to achieve.

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

        //interpolate the rotation vector smoothly
        transform.rotation = Quaternion.Lerp(transform.rotation, targetVector, Time.deltaTime);

        //Find out if a decoy exists - this is pretty wasteful. It would be more efficient
        //to have external code notify us of added and subtracted decoys
        decoy = GameObject.FindGameObjectWithTag("DuckDecoy");

        //Get the duck's position within the viewport
        //the bottom-left of the camera is (0,0); the top-right is (1,1)
        Vector3 vPoint = camera.camera.WorldToViewportPoint(transform.position);

        //Make sure the duck has not gone off the left or right of the screen
        if (vPoint.x < 0.08)
        {
            //setNewDirection(200f, 250f);
            reflectDirection();
        }

        if (vPoint.x > 0.92)
        {
            //setNewDirection(50f, 130f);
            reflectDirection();
        }


        //Also don't let duck leave top of screen unless it is escaping
        if (vPoint.y > 0.90 && !isEscaping)
        {
            isAscending = false; //got to go down!
            setNewDirection();
        }


        //Escape if it is time, we're not already escaping, and there are no decoys
        if (Time.time > (startTime + escapeTime) && !isEscaping && (decoy == null))
        {
            Debug.Log("Escape!");
            isEscaping = true; //inhibits calling this code more than once
            //this is a pretty stiff incline upward
            this.transform.rotation = Quaternion.Euler(new Vector3(-50f, Random.Range(0, 360), 0));
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
        if (Time.time > (lastActionTime + decisionTiming) && !isEscaping)
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
                transform.LookAt(decoy.transform); //calculate a new one to look at decoy
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
        transform.rotation = Quaternion.Euler(oldDirection);
    }


    public void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        //if the duck hit a trigger it probably means we hit the ground or lake so make sure to go up!
        isAscending = true;
        //and recalculate path
        setNewDirection();
    }


}
