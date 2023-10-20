using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Player : MonoBehaviour
{
    enum ThisPlayer
    {
        player1,
        player2
    }
    [SerializeField] float speed = 5;
    [SerializeField] int TEMP_defThisPlayer;
    [SerializeField] GameObject hitbox;

    ThisPlayer thePlayer;
    float xMin, xMax;
    //states
    bool isActionable = true;
    bool isAttacking, isBlocking, isStun, isRecover, isDown, isGrounded;

    private float atkTimer = 0;
    // Start is called before the first frame update
    void Start()
    {
        //maximum boundaries for ship movement
        xMin = Camera.main.ViewportToWorldPoint(new Vector3(.05f, 0, 0)).x;
        xMax = Camera.main.ViewportToWorldPoint(new Vector3(.95f, 0, 0)).x;
        setPlayer(TEMP_defThisPlayer);
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


        if(Input.GetButtonDown("Fire1") && isActionable)
        {
            isAttacking = true;
            isActionable = false;
            atkTimer = 3;
            GameObject atkBox1 = Instantiate(hitbox, new Vector3(transform.position.x + .1f, transform.position.y - .5f, transform.position.z), Quaternion.identity); //transform to make it a child
            
            atkBox1.GetComponent<HitBox>().createBox(10, 2, 1.2f, this);
           
            
            //Instantiate(atkBox1, new Vector3(transform.position.x + .1f, transform.position.y - .5f, transform.position.z), Quaternion.identity);
            
        }
        if(isAttacking)
        {
            atkTimer -= Time.deltaTime;
        }
        if(atkTimer < 2)
        {
            atkTimer = 0;
            isAttacking = false;
            isActionable = true;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }
        if(collision.gameObject.tag == "Player")
        {
            //collision with other player

        }
    }

    void setPlayer(int input)
    {
        if(input ==1)
        {
            thePlayer = ThisPlayer.player1;
        }
        else
        {
            thePlayer = ThisPlayer.player2;
        }
    }

    void makeHitBox()
    {

    }
}
