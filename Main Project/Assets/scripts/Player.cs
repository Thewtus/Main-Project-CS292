using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Player : MonoBehaviour
{
    //Parent function for the two player classes (which only differ in the controls in the update function)

    [SerializeField] protected float speed = 5;
    [SerializeField] protected int playerID;
    [SerializeField] protected GameObject hitbox;
    [SerializeField] protected GameManager gameManager;
    protected float xMin, xMax;
    //states
    protected bool isActionable = true;
    protected bool isAttacking, isBlocking, isStun, isGrab, isEndLag;
    protected int cancelLevel = 9; //levels for attack cancelling, keeping track of what the player can and can't cancel into
    protected bool isSwitchQueue = false;
    public Animator anim;

    //objects
    protected GameObject atkBox1;
    GameObject grabbed_player;

    protected float stunTimer;
    protected Vector3 target = Vector3.zero;
    protected int facing = 1;
    // Start is called before the first frame update
    void Start()
    {
        //maximum boundaries for ship movement
        xMin = Camera.main.ViewportToWorldPoint(new Vector3(.05f, 0, 0)).x;
        xMax = Camera.main.ViewportToWorldPoint(new Vector3(.95f, 0, 0)).x;

        anim = gameObject.GetComponent<Animator>();

        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //exists in subclasses
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            //behavior for colliding players (if any)
        }
    }

    void setPlayer(int input)
    {
        playerID = input;
    }

    void makeAtk0()
    { 
        float xOff = 0.7f; //x offset
        float yOff = -0.5f; //y offset
        atkBox1 = Instantiate(hitbox, new Vector3(transform.position.x + (xOff * facing), transform.position.y + (yOff), transform.position.z), Quaternion.identity);
        atkBox1.GetComponent<HitBox>().createBox(3, 1, 1.35f, this, 1);
    }

    void makeAtk1()
    {
        float xOff = 1f; //x offset
        float yOff = -0.6f; //y offset
        atkBox1 = Instantiate(hitbox, new Vector3(transform.position.x + (xOff * facing), transform.position.y + (yOff), transform.position.z), Quaternion.identity);
        atkBox1.GetComponent<HitBox>().createBox(10, 3, 1.65f, this, 2);
    }

    void makeAtk11()
    {
        //called atk11 because it still uses the fire2 key
        float xOff = 1f; //x offset
        float yOff = -1f; //y offset
        atkBox1 = Instantiate(hitbox, new Vector3(transform.position.x + (xOff * facing), transform.position.y + (yOff), transform.position.z), Quaternion.identity);
        atkBox1.GetComponent<HitBox>().createBoxKnock(11, 1.5f, 1, this, 2);
    }

    void setupAtk2()
    {
        target = new Vector3(transform.position.x + (4.5f * facing), transform.position.y, transform.position.z);
    }
    void makeAtk2()
    {
        float xOff = 1f;
        float yOff = -.6f;
        atkBox1 = Instantiate(hitbox, new Vector3(transform.position.x + (xOff * facing), transform.position.y + (yOff), transform.position.z), Quaternion.identity);
        atkBox1.GetComponent<HitBox>().createBox(16, 2.7f, 1.65f, this, 9);
        //target = new Vector3(transform.position.x + (5 * facing), transform.position.y, transform.position.z);
        //Debug.Log("ran makeAtk2: " + target.ToString());
    }

    void makeGrabAtk()
    {
        float xOff = 0.25f;
        float yOff = -0.5f;
        atkBox1 = Instantiate(hitbox, new Vector3(transform.position.x + (xOff * facing), transform.position.y + (yOff), transform.position.z), Quaternion.identity);
        atkBox1.GetComponent<HitBox>().createGrabBox(9, 1, 1.35f, this);
    }

    void endAttack()
    {
        isAttacking = false;
        Destroy(atkBox1);
        isEndLag = true;
    }
    void makeAction()
    { //makes a player actionable again
        isActionable = true;
        isEndLag = false;
        target = Vector3.zero;
    }

    public int getPlayerID()
    {
        return playerID;
    }

    public void stun(float dmg)
    {
        resetState();
        isActionable = false;
        isStun = true;
        //cancelLevel = 9;
        anim.SetBool("exit", false);
        anim.Play("stun");
        stunTimer = (.3f - dmg / 100) + (Mathf.Pow(dmg, 2) / 1000);
        target = new Vector3(transform.position.x + (dmg / 10 * facing * -1), transform.position.y, transform.position.z);
    }

    public void get_hit(float dmg)
    {
        if(isAttacking)
        {
            endAttack();
            dmg *= 1.15f; //counterhit
            isBlocking = false;
        }
            
        if (isBlocking)
        {
            dmg *= 0.6f;
            gameManager.Damage(dmg, playerID);
            if(!gameManager.getRoundEnd())
                block(dmg);
        }
        else
        {
            gameManager.Damage(dmg, playerID);
            if(!gameManager.getRoundEnd())
                stun(dmg);
        }
    }

    public void grab(float dmg, GameObject otherPlayer)
    {
        anim.Play("grab_finish");
        otherPlayer.GetComponentInParent<Animator>().SetBool("exit", false);
        otherPlayer.GetComponentInParent<Animator>().Play("stun");
        otherPlayer.GetComponentInParent<Player>().setActionable(false);
        grabbed_player = otherPlayer;
    }

    public void grab_finish(float dmg)
    {
        //needs to work like this for the animations
        gameManager.Damage(dmg, grabbed_player.GetComponentInParent<Player>().getPlayerID());
        grabbed_player.GetComponentInParent<Player>().knockDown();
        /**Transform pfb = grabbed_player.GetComponentInParent<Transform>();
        Vector3 t = new Vector3(pfb.position.x + (.6f * facing), pfb.position.y, pfb.position.z);
        grabbed_player.GetComponentInParent<Player>().setTarget(t);**/
    }

    void block(float dmg)
    {
        resetState();
        isActionable = false;
        isBlocking = true;
        isStun = true; //blockstun
        stunTimer = (.3f - dmg / 80) + (Mathf.Pow(dmg, 2) / 1000);
        anim.SetBool("exit", false);
        if (facing == 1)
            anim.Play("block");
        else
            anim.Play("block_right");

        //float knockback = dmg / 10 * facing * -1;
        target = new Vector3(transform.position.x + (dmg / 12 * facing * -1), transform.position.y, transform.position.z);
        //transform.position += new Vector3(knockback, 0, 0);
        /**if (transform.position.x < xMin || transform.position.x > xMax)
        { //collision with sides
            transform.position -= new Vector3(knockback, 0, 0);
        }**/
        
    }

    public void knockDown()
    {
        resetState();
        isActionable = false;
        if(!gameManager.getRoundEnd())
            anim.Play("knockdown");
    }

    protected void resetState()
    {
        isActionable = true;
        isAttacking = false;
        isBlocking = false;
        isEndLag = false;
        isGrab = false;
        isStun = false;
        Destroy(atkBox1);
        target = Vector3.zero;
        cancelLevel = 9;
        anim.SetBool("exit", true);
    }

    public void switchSides()
    {
        if(!isActionable)
        {
            isSwitchQueue = !isSwitchQueue;
        }
        else
        {
            facing *= -1;

            foreach (SpriteRenderer child in gameObject.GetComponentsInChildren<SpriteRenderer>())
            {
                child.flipX = !child.flipX;
            }
            isSwitchQueue = false;
        }
        
    }

    public void setActionable(bool a)
    {
        //resetState();
        isActionable = a;
    }
    public void setTarget(Vector3 v)
    {
        target = v;
    }
    public bool getIsActionable()
    {
        return isActionable;
    }

    public bool getIsAttacking()
    {
        return isAttacking;
    }

    public bool getIsBlocking()
    {
        return isBlocking;
    }

    public bool IsGrabbable()
    {
        //returns if the player meets conditions for being grabbed
        return isActionable || isAttacking || isEndLag;
    }

    public void disableHitBoxes()
    {
        isActionable = false;
        isStun = false;
        foreach(GameObject hBox in GameObject.FindGameObjectsWithTag("HurtBox"))
        {
            hBox.SetActive(false);
        }
    }

    public void enableHitBoxes()
    {
        isActionable = true;
        foreach (GameObject hBox in GameObject.FindGameObjectsWithTag("HurtBox"))
        {
            hBox.SetActive(true);
        }
        
    }

    public int getCancelLevel()
    {
        return cancelLevel;
    }
    public void setCancelLevel(int i)
    {
        cancelLevel = i;
    }
}
