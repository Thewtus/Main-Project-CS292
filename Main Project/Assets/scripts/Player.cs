using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] protected float speed = 5;
    [SerializeField] int playerID;
    [SerializeField] GameObject hitbox;
    [SerializeField] GameManager gameManager;
    protected float xMin, xMax;
    //states
    protected bool isActionable = true;
    protected bool isAttacking, isBlocking, isStun, isRecover, isDown;

    private Animator anim;
    float xD;

    GameObject atkBox1;

    float stunTimer;
    // Start is called before the first frame update
    void Start()
    {
        //maximum boundaries for ship movement
        xMin = Camera.main.ViewportToWorldPoint(new Vector3(.05f, 0, 0)).x;
        xMax = Camera.main.ViewportToWorldPoint(new Vector3(.95f, 0, 0)).x;

        anim = gameObject.GetComponent<Animator>();

        EventManager.instance.e_hit.AddListener(get_hit);

        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        float xDirection = Input.GetAxis("Horizontal");
        //movement:
        if(isActionable)
            transform.position += new Vector3(xDirection, 0, 0) * Time.deltaTime * speed;
        
        if (transform.position.x < xMin || transform.position.x > xMax)
        { //collision with sides
            transform.position -= new Vector3(xDirection, 0, 0) * Time.deltaTime * speed;
        }
        
        
        isBlocking = xDirection <= 0; //checking for blocking, if moving backwards, block
        xD = xDirection;
        //Debug.Log(isBlocking + " " + xDirection);
        if(Input.GetButtonDown("Fire0") && isActionable)
        {
            isAttacking = true;
            isActionable = false;
            anim.Play("atk0");
        }
        if(Input.GetButtonDown("Fire1") && isActionable)
        {
            isAttacking = true;
            isActionable = false;
            anim.Play("atk1");
            //Instantiate(atkBox1, new Vector3(transform.position.x + .1f, transform.position.y - .5f, transform.position.z), Quaternion.identity);
            
        }
        if (Input.GetKeyDown(KeyCode.Space))
            anim.Play("block");

        if(stunTimer > 0)
        {
            stunTimer -= Time.deltaTime;
        } else if (isStun)
        {
            isStun = false;
            resetState();
            anim.SetBool("exit", true);
        }
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
        atkBox1 = Instantiate(hitbox, new Vector3(transform.position.x + .7f, transform.position.y - .5f, transform.position.z), Quaternion.identity);
        atkBox1.GetComponent<HitBox>().createBox(2, 1, 1.35f, this);
    }

    void makeAtk1()
    {
        atkBox1 = Instantiate(hitbox, new Vector3(transform.position.x + 1f, transform.position.y -.6f, transform.position.z), Quaternion.identity); //transform to make it a child
        atkBox1.GetComponent<HitBox>().createBox(10, 3, 1.65f, this);
    }

    void endAttack()
    {
        isAttacking = false;
        Destroy(atkBox1 );
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
        stunTimer = dmg / 10;
    }

    public void get_hit(float dmg)
    {
        Debug.Log(isBlocking + " When got hit " + xD);
        if (isBlocking)
        {
            dmg *= 0.7f;
            gameManager.Damage(dmg, playerID);
            block(dmg);
        }
        else
        {
            gameManager.Damage(dmg, playerID);
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
        anim.Play("block");
        //anim.Play("block")
    }

    void resetState()
    {
        isActionable = true;
        isAttacking = false;
        isBlocking = false;
        isStun = false;
    }
}
