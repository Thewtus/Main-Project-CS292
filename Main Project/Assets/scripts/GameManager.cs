using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] float p1Hp = 100;
    [SerializeField] float p2Hp = 100;


    // Start is called before the first frame update
    void Start()
    {
        EventManager.instance.e_hit.AddListener(Damage);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Damage(float i)
    {
        p1Hp -= i;
        Debug.Log(p1Hp);
    }
}
