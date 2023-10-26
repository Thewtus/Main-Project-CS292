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
        //EventManager.instance.e_hit.AddListener(test);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Damage(float f, int i)
    {
        if(i == 1)
        {
            p1Hp -= f;
            Debug.Log("P1HP: " +  p1Hp);
        }
        else
        {
            p2Hp -= f;
            Debug.Log("P2HP" + p2Hp);
        }
    }

    public void test(float f)
    {
        Debug.Log("Yes");
    }
}
