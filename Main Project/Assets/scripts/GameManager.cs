using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] float p1Hp = 100;
    [SerializeField] float p2Hp = 100;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI KOText;
    [SerializeField] TextMeshProUGUI P1Wins;
    [SerializeField] TextMeshProUGUI P2Wins;
    [SerializeField] GameObject P1HpBar;
    [SerializeField] GameObject P2HpBar;



    [SerializeField] Player1 player1;
    [SerializeField] Player2 player2;
    float roundTimer = 99;
    float endTimer = 3;
    float startTimer = 2f;
    bool flipState = false; //state of how players or facing, false for defualt, true if swapped
    bool isRoundEnd, isRoundStart, isGameEnd;
    Slider p1Bar, p2Bar;
    static int p1RoundWins, p2RoundWins; //static as they need to be preserved between rounds

    // Start is called before the first frame update
    void Start()
    {
        p1Bar = P1HpBar.GetComponent<Slider>();
        p1Bar.maxValue = p1Hp;
        p1Bar.value = p1Hp;
        p2Bar = P2HpBar.GetComponent<Slider>();
        p2Bar.maxValue = p2Hp;
        p2Bar.value = p2Hp;

        isRoundStart = true;
        player2.setActionable(false);
        player1.setActionable(false);

        P1Wins.text = "Wins: " + p1RoundWins + "/2";
        P2Wins.text = "Wins: " + p2RoundWins + "/2";
    }

    // Update is called once per frame
    void Update()
    {
        //timers
        if(isRoundStart && startTimer > 0.5f)
        {
            KOText.text = "READY";
            KOText.gameObject.SetActive(true);
            startTimer -= Time.deltaTime;
        }
        else if (isRoundStart && startTimer > 0)
        {
            KOText.gameObject.SetActive(false);
            KOText.text = "GO";
            KOText.gameObject.SetActive(true);
            startTimer -= Time.deltaTime;
        }
        else if (isRoundStart && startTimer <= 0)
        {
            KOText.gameObject.SetActive(false);
            KOText.text = "K.O.";
            player1.setActionable(true);
            player2.setActionable(true);
            isRoundStart = false;
        }
        else if (roundTimer > 0)
        {
            roundTimer -= Time.deltaTime;
            timerText.text = (int)roundTimer + "";
        }
        if(roundTimer <=0 && !isRoundEnd)
        {
            timeOut();
        }

        if(isRoundEnd && endTimer > 0)
        {
            endTimer -= Time.deltaTime;
        } else if (isRoundEnd)
        {
            SceneManager.LoadScene(0);
        }

        //flipping players around
        if((!flipState && player1.gameObject.transform.position.x >= player2.gameObject.transform.position.x)  || (flipState && player1.gameObject.transform.position.x <= player2.gameObject.transform.position.x))
        {
            player2.switchSides();
            player1.switchSides();
            flipState = !flipState;
        }


    }

    public void Damage(float f, int i)
    {
        if(i == 1)
        {
            p1Hp -= f;
            p1Bar.value = (int)p1Hp;
            if(p1Hp <= 0)
            {
                endGame(2);
            }
        }
        else
        {
            p2Hp -= f;
            p2Bar.value = (int)p2Hp;
            if (p2Hp <= 0)
            {
                endGame(1);
            }
        }
    }

    void endGame(int p)
    {
        //ends the game, p is winning player
        player2.setActionable(false);
        player1.setActionable(false);
        if(p == 1)
        {
            player2.anim.Play("KO");
            p1RoundWins += 1;
            P1Wins.text = "Wins: " + p1RoundWins + "/2";
        }
        else
        {
            player1.anim.Play("KO");
            p2RoundWins += 1;
            P2Wins.text = "Wins: " + p2RoundWins + "/2";
        }
        if(p1RoundWins == 2)
        {
            KOText.color = Color.blue;
            KOText.text = "P1 WINS";
            isGameEnd = true;
        } else if (p2RoundWins == 2)
        {
            KOText.color = Color.blue;
            KOText.text = "P2 WINS";
            isGameEnd = true;
        } else
        {
            KOText.gameObject.SetActive(true);
            isRoundEnd = true;
        }
        
    }

    void timeOut()
    {
        KOText.text = "TIME OUT";
        player2.setActionable(false);
        player1.setActionable(false);
        if (p1Hp > p2Hp)
        {
            endGame(1);
        }
        else if (p2Hp > p1Hp) { 
            endGame(2);
        }
        else
        {
            KOText.gameObject.SetActive(true);
            isRoundEnd = true;
        }
    }

    public bool getRoundEnd()
    {
        return isRoundEnd;
    }
}
