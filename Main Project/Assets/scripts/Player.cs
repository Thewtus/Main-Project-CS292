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
    [SerializeField] protected float speed = 5;
    [SerializeField] protected int playerID;
    [SerializeField] protected GameObject hitbox;
    [SerializeField] protected GameManager gameManager;
    protected float xMin, xMax;
    //states
    protected bool isActionable = true;
    protected bool isAttacking, isBlocking, isStun, isRecover, isDown;

    public Animator anim;

    protected GameObject atkBox1;

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
        /**float xDirection = Input.GetAxis("Horizontal");
        //movement:
        if(isActionable)
            transform.position += new Vector3(xDirection, 0, 0) * Time.deltaTime * speed;
        
        if (transform.position.x < xMin || transform.position.x > xMax)
        { //collision with sides
            transform.position -= new Vector3(xDirection, 0, 0) * Time.deltaTime * speed;
        }
        
        
        isBlocking = (xDirection * facing) < 0; //checking for blocking, if moving backwards, block
        //Debug.Log(isBlocking + " " + xDirection);
        if(Input.GetButtonDown("Fire0") && isActionable)
        {
            isAttacking = true;
            isActionable = false;
            if (facing < 0)
                anim.Play("atk0");
            else
                anim.Play("atk0_right");
        }
        if(Input.GetButtonDown("Fire1") && isActionable)
        {
            isAttacking = true;
            isActionable = false;
            anim.Play("atk1");
            //Instantiate(atkBox1, new Vector3(transform.position.x + .1f, transform.position.y - .5f, transform.position.z), Quaternion.identity);
            
        }
        if (Input.GetKeyDown(KeyCode.Space))
            switchSides();

        if(stunTimer > 0)
        {
            stunTimer -= Time.deltaTime;
        } else if (isStun)
        {
            isStun = false;
            resetState();
            anim.SetBool("exit", true);
        }**/
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            //collision with other player

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
        atkBox1.GetComponent<HitBox>().createBox(3, 1, 1.35f, this);
    }

    void makeAtk1()
    {
        float xOff = 1f; //x offset
        float yOff = -0.6f; //y offset
        atkBox1 = Instantiate(hitbox, new Vector3(transform.position.x + (xOff * facing), transform.position.y + (yOff), transform.position.z), Quaternion.identity);
        atkBox1.GetComponent<HitBox>().createBox(10, 3, 1.65f, this);
    }

    void makeAtk2()
    {
        float xOff = 1f;
        float yOff = -.6f;
        atkBox1 = Instantiate(hitbox, new Vector3(transform.position.x + (xOff * facing), transform.position.y + (yOff), transform.position.z), Quaternion.identity);
        atkBox1.GetComponent<HitBox>().createBox(16, 3, 1.65f, this);
        target = new Vector3(transform.position.x + (5 * facing), transform.position.y, transform.position.z);
        //Debug.Log("ran makeAtk2: " + target.ToString());
    }

    void endAttack()
    {
        isAttacking = false;
        Destroy(atkBox1 );
        target = Vector3.zero;
        //Debug.Log("ended attack");
    }
    void makeAction()
    { //makes a player actionable again
        isActionable = true;
    }

    public int getPlayerID()
    {
        return playerID;
    }

    void stun(float dmg)
    {
        resetState();
        isActionable = false;
        isStun = true;

        anim.SetBool("exit", false);
        anim.Play("stun");
        stunTimer = (.3f - dmg / 100) + (Mathf.Pow(dmg, 2) / 1000);
        float knockback = dmg / 30 * facing * -1;
        transform.position += new Vector3(knockback, 0, 0);
        if (transform.position.x < xMin || transform.position.x > xMax)
        { //collision with sides
            transform.position -= new Vector3(knockback, 0, 0);
        }
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

    void block(float dmg)
    {
        resetState();
        isActionable = false;
        isBlocking = true;
        isStun = true; //blockstun
        stunTimer = dmg / 10;
        anim.SetBool("exit", false);
        if (facing == 1)
            anim.Play("block");
        else
            anim.Play("block_right");

        float knockback = dmg / 20 * facing * -1;
        transform.position += new Vector3(knockback, 0, 0);
        if (transform.position.x < xMin || transform.position.x > xMax)
        { //collision with sides
            transform.position -= new Vector3(knockback, 0, 0);
        }
        //anim.Play("block")
    }

    protected void resetState()
    {
        isActionable = true;
        isAttacking = false;
        isBlocking = false;
        isStun = false;
        Destroy(atkBox1);
        target = Vector3.zero;
    }

    public void switchSides()
    {
        facing *= -1;
        
        foreach (SpriteRenderer child in gameObject.GetComponentsInChildren<SpriteRenderer>())
        {
            child.flipX = !child.flipX;
        }
    }

    public void setActionable(bool a)
    {
        isActionable = a;
    }

    void disableHitBoxes()
    {
        isActionable = false;
        isStun = false;
        foreach(GameObject hBox in GameObject.FindGameObjectsWithTag("HurtBox"))
        {
            hBox.SetActive(false);
        }
    }
}
