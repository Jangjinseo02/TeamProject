using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tutorial : MonoBehaviour
{
    public enum TutorialType { One, Two, Three, Four, Five }
    public TutorialType Type;

    PlayerMoveMent player;
    Animator playeranim;

    [SerializeField] string[] setText = new string[7];
    [SerializeField] TMP_Text outText;

    bool typing = false;
    int index = 0;

    [SerializeField] GameObject tutorialcanvas;

    private void Start()
    {
        player = GameManager.Instance.player.GetComponent<PlayerMoveMent>();
        playeranim = player.GetComponent<Animator>();

        outText = tutorialcanvas.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Type == TutorialType.One)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                tutorialcanvas.SetActive(true);

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
                SetTextAndStartRoutine(Type);

            }
        }

        if (Type == TutorialType.Two)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                tutorialcanvas.SetActive(true);

                player.isTutorial = true;
                InputManager.Instance.isAttack = true;

                gameObject.GetComponent<BoxCollider2D>().enabled = false;

                Invoke("Off", 0.4f);

                //스크립트 끝나는 시점에 player.isTutorial = false, InputManager.Instance.isAttack = false;
                SetTextAndStartRoutine(this.Type);
            }
        }

        if (Type == TutorialType.Four)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                tutorialcanvas.SetActive(true);

                player.isTutorial = true;
                InputManager.Instance.isAttack = true;

                playeranim.SetBool("isWalk", false);
                playeranim.SetBool("Idle", true);

                gameObject.GetComponent<BoxCollider2D>().enabled = false;
                gameObject.transform.GetChild(0).gameObject.SetActive(false);

                Invoke("Off", 0.4f);

                //스크립트 끝나는 시점에 player.isTutorial = false, InputManager.Instance.isAttack = false;
                SetTextAndStartRoutine(this.Type);
            }
        }

        if (Type == TutorialType.Five)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                tutorialcanvas.SetActive(true);

                player.isTutorial = true;
                InputManager.Instance.isAttack = true;

                playeranim.SetBool("isWalk", false);
                playeranim.SetBool("Idle", true);

                gameObject.GetComponent<BoxCollider2D>().enabled = false;

                Invoke("Off", 0.4f);

                //스크립트 끝나는 시점에 player.isTutorial = false, InputManager.Instance.isAttack = false;
                SetTextAndStartRoutine(this.Type);
            }
        }
    }

    public void SetTextAndStartRoutine(TutorialType type)
    {
        switch (type)
        {
            case TutorialType.One:
                setText[0] = "혹시 오성전자가 외계인을 납치해서 기술력을 얻고 있다는 농담을 들어본적이 있나?";
                setText[1] = "세상을 바꿀 멋진 기술자들을 많이 데려와 주게나. 그게 자네 일이네.";
                setText[2] = "화면 오른쪽에 얼마나 외계인을 납치해야 하는지 적혀 있을 걸세.";
                setText[3] = "할당량을 다 채우면 복귀시켜줄테니 우선 내 말을 따르고.";
                setText[4] = "일단 드릴을 써보게. 화면 오른쪽을 터치하면 사용할 수 있네.";

                StartCoroutine(Diallog(5, false, true));
                break;
            case TutorialType.Two:
                setText[0] = "잘 했네! 드릴로는 다양한 색의 블록들을 부술 수 있지.";
                setText[1] = "하지만 드릴 뿐만이 아니네, 왼쪽을 한번 보게.";
                setText[2] = "자네의 몸 상태를 눈으로 볼 수 있게 한 것라네. 그 중에서도 파란 것을 설명하지.";
                setText[3] = "파란 게이지는, 자세의 남은 산소량이라네. 숨을 쉴 수 없다면 복귀해야겠지?";
                setText[4] = "설명하는 사이에 저기에 외계인이 보이는구만!";
                setText[5] = "오른쪽으로 이동하여 드릴로 외계인을 기절시키게!";

                StartCoroutine(Diallog(6, true, true));
                break;
            case TutorialType.Three:
                setText[0] = "외계인은 그렇게 기절시키면 될 거야.";
                setText[1] = "이제 아래로 내려가 보게나. 하지만 단단해 보이는 X 블록은 건들지 말게.";
                setText[2] = "그 블록을 부순다면 숨을 쉬기가 살짝 힘들걸세.";
                setText[3] = "어쨌든 빛이 나는 곳까지 내려가보게.";

                tutorialcanvas.SetActive(true);
                StartCoroutine(Diallog(4, true, true));
                break;
            case TutorialType.Four:
                setText[0] = "잠시 멈춰보게! 말해 줄게 있네.";
                setText[1] = "우선 블록은 아래가 비면 모두 아래로 떨어진다네, 깔리지 않게 조심하게.";
                setText[2] = "하지만 같은 종류의 블록은, 가까이 붙으면 그 블록과 붙어버려.";
                setText[3] = "그리고, 붙은 블록들이 4개 이상이 된다면 터지고 사라져 없어져버릴걸세.";
                setText[4] = "그건 이 단단한 블록들도 마찬가지지. 한번 해 보겠나?";
                setText[5] = "왼쪽의 블록을 파 보게.";

                StartCoroutine(Diallog(6, true, true));
                break;
            case TutorialType.Five:
                setText[0] = "잘 했네! 근데 깔려 버렸구만.";
                setText[1] = "왼쪽의 파란 게이지가 아니라 빨간 게이지가 줄어들었을 걸세.";
                setText[2] = "이건 자네의 몸이 다친 정도와, 얼마나 더 버틸 수 있을지 나타낸 것이네.";
                setText[3] = "이 두 게이지 중 하나가 다한다면 다시 복귀해야 할 거야.";
                setText[4] = "이제 아래 금속 블록을 부수어 보게.";
                setText[5] = "만약 외계인 할당량을 다 채웠다면, 그 자리에서 복귀할 수 있지.";
                setText[6] = "지금 할당량을 다 채웠으니, 아래 블록을 부수어 복귀하면 되겠구만, 복귀하게!";

                StartCoroutine(Diallog(7, true, true));
                break;
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

    IEnumerator Diallog(int index, bool move, bool attack)
    {
        int i = 0;
        while (i < index)
        {
            while (typing)
            {
                player.isTutorial = true;
                InputManager.Instance.isAttack = true;

                if (Input.GetMouseButtonDown(0))
                {
                    outText.text = null;
                    break;
                }
                yield return null;
            }

            yield return StartCoroutine(TextRoutine(i));

            i++;
        }

        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                tutorialcanvas.SetActive(false);
                break;
            }

            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        if (move)
            player.isTutorial = false;

        if (attack)
            InputManager.Instance.isAttack = false;

        outText.text = null;
        typing = false;

    }

    IEnumerator TextRoutine(int index)
    {
        int textIndex = 0;
        float typingSpeed = 0.1f;

        this.index = index;
        typing = false;

        StartCoroutine(SoundRoutine());

        while (textIndex < setText[index].Length && !typing)
        {
            //if (Input.GetMouseButtonDown(0) && !typing)
            //{
            //    outText.text = null;

            //    outText.text = setText[index];
            //    typing = true;

            //    break;
            //}
            outText.text += setText[index][textIndex++];

            yield return new WaitForSeconds(typingSpeed);
        }

        yield return null;
        SoundManager.Instance.SfxAllStop();
        typing = true;
    }

    IEnumerator SoundRoutine()
    {
        while (!typing)
        {
            yield return new WaitForSeconds(0.1f);
            SoundManager.Instance.SfxPlay(SoundManager.IntroSfx.DoctorText);
        }
    }
}
