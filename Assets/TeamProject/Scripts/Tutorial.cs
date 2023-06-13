using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public enum TutorialType { One, Two, Three, Four, Five }
    public TutorialType Type;

    PlayerMoveMent player;
    Animator playeranim;

    private void Start()
    {
        player = GameManager.Instance.player.GetComponent<PlayerMoveMent>();
        playeranim = player.GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Type == TutorialType.One)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                playeranim.SetBool("Drop", false);
                playeranim.SetBool("Idle", false);
                player.isDamaged = true;

                playeranim.SetBool("Block", true);
                playeranim.SetTrigger("OnDamaged");

                player.isTutorial = true;
                InputManager.Instance.isAttack = true;

                gameObject.GetComponent<BoxCollider2D>().enabled = false;

                Invoke("Off", 0.4f);

                //스크립트 출력 이후 끝나는 시점에  InputManager.Instance.isAttack = false;
            }
        }

        if (Type == TutorialType.Two)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                player.isTutorial = true;
                InputManager.Instance.isAttack = true;

                gameObject.GetComponent<BoxCollider2D>().enabled = false;

                Invoke("Off", 0.4f);

                //스크립트 끝나는 시점에 player.isTutorial = false, InputManager.Instance.isAttack = false;
            }
        }

        if (Type == TutorialType.Four)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                player.isTutorial = true;
                InputManager.Instance.isAttack = true;

                playeranim.SetBool("isWalk", false);
                playeranim.SetBool("Idle", true);

                gameObject.GetComponent<BoxCollider2D>().enabled = false;
                gameObject.transform.GetChild(0).gameObject.SetActive(false);

                Invoke("Off", 0.4f);

                //스크립트 끝나는 시점에 player.isTutorial = false, InputManager.Instance.isAttack = false;
            }
        }

        if (Type == TutorialType.Five)
        {
            if (collision.gameObject.CompareTag("Player"))
            {

                player.isTutorial = true;
                InputManager.Instance.isAttack = true;

                playeranim.SetBool("isWalk", false);
                playeranim.SetBool("Idle", true);

                gameObject.GetComponent<BoxCollider2D>().enabled = false;

                Invoke("Off", 0.4f);

                //스크립트 끝나는 시점에 player.isTutorial = false, InputManager.Instance.isAttack = false;
            }
        }
    }


    public void On()
    {
        player.isTutorial = false;
        InputManager.Instance.isAttack = false;
    }

    void Off()
    {
        if (Type.Equals(TutorialType.One))
        {
            player.isDamaged = false;
            playeranim.SetBool("Idle", true);
        }
    }
}
