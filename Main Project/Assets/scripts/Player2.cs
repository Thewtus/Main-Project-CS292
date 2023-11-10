using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player2 : Player
{
    // Start is called before the first frame update
    void Start()
    {
        //maximum boundaries for ship movement
        xMin = Camera.main.ViewportToWorldPoint(new Vector3(.05f, 0, 0)).x;
        xMax = Camera.main.ViewportToWorldPoint(new Vector3(.95f, 0, 0)).x;

        anim = gameObject.GetComponent<Animator>();

        EventManager.instance.e_hit.AddListener(get_hit);

        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        facing = -1;
    }

    // Update is called once per frame
    void Update()
    {
        float xDirection = Input.GetAxis("2PHorizontal");
        //movement:
        if (isActionable)
        {
            cancelLevel = 9;
            if (xDirection * facing > 0)
            {
                transform.position += new Vector3(xDirection, 0, 0) * Time.deltaTime * speed;
            }
            else
                transform.position += new Vector3(xDirection, 0, 0) * Time.deltaTime * (speed * 0.85f);
        }


        /**if (transform.position.x < xMin || transform.position.x > xMax)
        { //collision with sides
            transform.position -= new Vector3(xDirection, 0, 0) * Time.deltaTime * speed;
        }**/

        if (transform.position.x < xMin)
            transform.position = new Vector3(xMin, transform.position.y, transform.position.z);
        if (transform.position.x > xMax)
            transform.position = new Vector3(xMax, transform.position.y, transform.position.z);


        isBlocking = (xDirection * facing) < 0; //checking for blocking, if moving backwards, block
        if (isStun || isAttacking)
        {
            isBlocking = false; //don't allow blocking while stunned or attacking
        }
        //Debug.Log(isBlocking + " " + xDirection);
        if (Input.GetButtonDown("2PFire0") && (isActionable || cancelLevel < 2))
        {
            isAttacking = true;
            isActionable = false;
            if (facing == 1)
                anim.Play("atk0");
            else
                anim.Play("atk0_right");
            //anim.Play("atk0");
        }
        if (Input.GetButtonDown("2PFire1") && xDirection * facing <= 0 && (isActionable || cancelLevel < 3))
        {
            isAttacking = true;
            isActionable = false;
            anim.Play("atk1");
            cancelLevel = 9;
        }
        if (Input.GetButtonDown("2PFire1") && xDirection * facing > 0 && (isActionable || cancelLevel < 3))
        {
            isAttacking = true;
            isActionable = false;
            anim.Play("atk11");
            cancelLevel = 9;
        }

        if (Input.GetButtonDown("2PFire2") && (isActionable || cancelLevel < 4))
        {
            isAttacking = true;
            isActionable = false;
            anim.Play("atk2");
            cancelLevel = 9;
        }
        else if (target != Vector3.zero && !isStun)
        {
            //Movement during atk2
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime * 2.2f); //hopefully moves?
            if (atkBox1 != null)
            {
                atkBox1.transform.position = Vector3.MoveTowards(atkBox1.transform.position, target, speed * Time.deltaTime * 2.2f);
            }
        }
        if (Input.GetKeyDown(KeyCode.KeypadEnter) && isActionable)
        {
            isActionable = false;
            anim.Play("grab");
        }

        if (stunTimer > 0)
        {
            stunTimer -= Time.deltaTime;
        }
        else if (isStun)
        {
            isStun = false;
            resetState();
            anim.SetBool("exit", true);
        }
        if (isStun && target != Vector3.zero)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime * 0.9f); //hopefully moves?
        }
        if (isSwitchQueue && isActionable)
        {
            switchSides();
        }
    }
}
