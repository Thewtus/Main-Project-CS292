using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    public float damage = 1;
    private Player parent;
    [SerializeField] int ID;

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
            //Debug.Log("INVOKE");
            //EventManager.instance.e_hit.Invoke(damage);
            collision.gameObject.GetComponentInParent<Player>().get_hit(damage);
            Destroy(gameObject);
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
    public void createBox(float d, float l, float w, Player p)
    {
        //create the whole hitbox
        damage = d;
        transform.localScale = new Vector3(l, w, 1);
        parent = p;
        ID = parent.getPlayerID();
    }
}
