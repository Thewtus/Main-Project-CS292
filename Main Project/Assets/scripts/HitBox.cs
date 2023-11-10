using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    public float damage = 1;
    private Player parent;
    private int cancelLevel;
    [SerializeField] int ID;
    bool isGrabBox = false; //grab
    bool isKnockBox; //knocks down (like a sweep)

    public HitBox(float dmg, int l, int w)
    {
        damage = dmg;
        transform.localScale = new Vector3(l, w, 1);
    }
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        //collision.gameObject.GetComponentInParent<Player>();
        if (collision.gameObject.tag == "HurtBox" && collision.gameObject.GetComponentInParent<Player>().getPlayerID() != ID)
        {
            if (isGrabBox)
            {
                //grab, can only work if other player is actionable or attacking
                if (collision.gameObject.GetComponentInParent<Player>().IsGrabbable())
                {
                    parent.grab(damage, collision.gameObject);
                    Destroy(gameObject);
                }
            }
            else
            {
                //hit
                collision.gameObject.GetComponentInParent<Player>().get_hit(damage);
                parent.setCancelLevel(cancelLevel);
                Destroy(gameObject);
                if(isKnockBox && !collision.gameObject.GetComponentInParent<Player>().getIsBlocking())
                { //only knock down & cancel if proper hit
                    collision.gameObject.GetComponentInParent<Player>().knockDown();
                }
                else if (isKnockBox)
                {
                    parent.setCancelLevel(9);
                }
            }
        }
    }

    public void setDamage(float f)
    {
        damage = f;
    }

    public void setDimension(float l, float w)
    {
        transform.localScale = new Vector3(l, w, 1);
    }
    public void createBox(float d, float l, float w, Player p, int c)
    {
        //create the whole hitbox
        damage = d;
        transform.localScale = new Vector3(l, w, 1);
        parent = p;
        ID = parent.getPlayerID();
        cancelLevel = c;
    }

    public void createBoxKnock(float d, float l, float w, Player p, int c)
    {
        damage = d;
        transform.localScale = new Vector3(l, w, 1);
        parent = p;
        ID = parent.getPlayerID();
        cancelLevel = c;
        isKnockBox = true;
    }

    public void createGrabBox(float d, float l, float w, Player p)
    {
        damage = d;
        transform.localScale = new Vector3(l, w, 1);
        parent = p;
        ID = parent.getPlayerID();
        isGrabBox = true;
    }
}
