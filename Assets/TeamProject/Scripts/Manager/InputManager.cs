using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputManager : SingleTon<InputManager>
{
    [SerializeField] private GameObject Panel;
    [SerializeField] PlayerMoveMent player;
    [SerializeField] Image[] buttonImage;
    public Vector3 dirVec;

    Vector3 panelPos;
    Animator playeranim;

    public bool isAttack;

    private void Awake()
    {
        playeranim = player.GetComponent<Animator>();
    }
    private void Update()
    {
        //컴퓨터 확인용
        if (Input.GetButtonDown("Jump"))
            Attack();
    }

    public void PanelOn()
    {
        panelPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        panelPos.z = 0;
        Panel.SetActive(true);
        Panel.transform.position = panelPos;
    }

    public void Attack()
    {
        //player.isDamaged 추가
        if (player.isDead || player.isDamaged || player.isStun || player.isDrop || player.isJump)
            return;

        if (!isAttack && !player.isDrop)
        {
            isAttack = true;
            StartCoroutine(AttackRoutine());
        }
    }

    IEnumerator AttackRoutine()
    {
        RaycastHit2D rayhit = Physics2D.Raycast(player.transform.position, dirVec == Vector3.zero ? Vector3.down : dirVec, 0.65f, LayerMask.GetMask("Block"));
        Block block = rayhit.collider != null && rayhit.collider.tag != "Item" ? rayhit.collider.GetComponent<Block>() : null;

        if (block != null)
        {
            HardBlock hard = block.GetComponent<HardBlock>();
            ClearBlock clear = block.GetComponent<ClearBlock>();
            Monster monster = block.GetComponent<Monster>();
            Slim slim = block.GetComponent<Slim>();

            if (dirVec == Vector3.up)
                playeranim.SetTrigger("upAttack");
            else if (dirVec == Vector3.right || dirVec == Vector3.left)
                playeranim.SetTrigger("lrAttack");
            else
                playeranim.SetTrigger("downAttack");

            //Attack 사운드 출력
            if (hard)
                SoundManager.Instance.SfxPlay(SoundManager.Sfx.AttackHard, false);
            else if (monster || slim)
                SoundManager.Instance.SfxPlay(SoundManager.Sfx.AttackMonster, false);
            else if (block)
                SoundManager.Instance.SfxPlay(SoundManager.Sfx.Attack, false);


            if (hard)
                hard.OnDamaged(10);
            else if (block.group != null)
            {
                block.group.isSoundPlay = true;

                foreach (Block member in block.group)
                {
                    //if (block.type == 0 && firstMember == null) //type == 0 : ClearBlock
                    //    firstMember = member;
                    member.OnDamaged(10);
                    if (block.group.Count >= 4)
                        GameManager.Instance.GetScore(GameManager.BreakType.Multi);
                    else
                        GameManager.Instance.GetScore(GameManager.BreakType.Single);
                }
            }

            if (hard)
                if (hard.isDestroy())
                {
                    player.RealeseAir();
                    GameManager.Instance.GetScore(GameManager.BreakType.Single);
                }
            if (clear)
                block.StageClear();
            if (monster)
                GameManager.Instance.GetScore(GameManager.BreakType.Monster);

        }

        yield return new WaitForSeconds(0.2f);
        isAttack = false;
    }

    public void DragEnter(Vector2 dirvec)
    {
        ExitEnter();

        dirVec = dirvec;

        if (dirvec.x > 0)
            RightEnter();
        else if (dirvec.x < 0)
            LeftEnter();
        if (dirvec.y > 0)
            UpEnter();
        else if (dirvec.y < 0)
            UnderEnter();
    }

    void RightEnter()
    {
        //dirVec = Vector3.right;
        buttonImage[3].color = Color.black;
    }

    void LeftEnter()
    {
        //dirVec = Vector3.left;
        buttonImage[2].color = Color.black;
    }

    void UpEnter()
    {
        //dirVec = Vector3.up;
        buttonImage[0].color = Color.black;
    }

    void UnderEnter()
    {
        //dirVec = Vector3.down;
        buttonImage[1].color = Color.black;
    }

    public void ExitEnter()
    {
        //dirVec = Vector3.zero;
        for (int i = 0; i < buttonImage.Length; i++)
        {
            buttonImage[i].color = Color.white;
        }
    }

    public void Select()
    {
        //UIManager.Instance.SelectBackGroundPopdown();
        isAttack = false;
    }

}
