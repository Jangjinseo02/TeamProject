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

    public bool[] inputBtn;
    public bool isControl;

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

        if (!isAttack && !player.isDrop && !player.isJump)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    IEnumerator AttackRoutine()
    {
        int layer = (1 << LayerMask.NameToLayer("Block")) + (1 << LayerMask.NameToLayer("Monster"));
        RaycastHit2D rayhit = Physics2D.Raycast(player.transform.position, dirVec == Vector3.zero ? Vector3.down : dirVec, 0.65f, layer);
        Block block = rayhit.collider != null && rayhit.collider.tag != "Item" ? rayhit.collider.GetComponent<Block>() : null;

        if (block != null)
        {
            isAttack = true;

            HardBlock hard = block.GetComponent<HardBlock>();
            ClearBlock clear = block.GetComponent<ClearBlock>();
            Monster monster = block.GetComponent<Monster>();
            Slim slim = block.GetComponent<Slim>();

            if (dirVec == Vector3.up)
                playeranim.SetTrigger("UpCheck");
            else if (dirVec == Vector3.right || dirVec == Vector3.left)
                playeranim.SetTrigger("LRCheck");
            else
                playeranim.SetTrigger("DownCheck");

            //Attack 사운드 출력
            if (hard)
                SoundManager.Instance.SfxPlay(SoundManager.Sfx.AttackHard, false);
            else if (block)
                SoundManager.Instance.SfxPlay(SoundManager.Sfx.Attack, false);
            else if (clear)
                SoundManager.Instance.SfxPlay(SoundManager.Sfx.LevelClear, false);


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

        yield return new WaitForSeconds(0.1f);
        isAttack = false;
    }

    //public void DragEnter(Vector2 dirvec)
    //{
    //    ExitEnter();

    //    dirVec = dirvec;

    //    if (dirvec.x > 0)
    //        RightEnter();
    //    else if (dirvec.x < 0)
    //        LeftEnter();
    //    if (dirvec.y > 0)
    //        UpEnter();
    //    else if (dirvec.y < 0)
    //        UnderEnter();
    //}

    public void PointerEnter(int type)
    {
        for(int i = 0; i < inputBtn.Length; i++)
        {
            inputBtn[i] = i == type;
        }

        if (inputBtn[0] && isControl)
            UpEnter();
        else if (inputBtn[1] && isControl)
            LeftEnter();
        else if (inputBtn[2] && isControl)
            RightEnter();
        else if (inputBtn[3] && isControl)
            UnderEnter();
        else
            ExitEnter();
    }

    public void PointerDown()
    {
        isControl = true;

        if (inputBtn[0] && isControl)
            UpEnter();
        else if (inputBtn[1] && isControl)
            LeftEnter();
        else if (inputBtn[2] && isControl)
            RightEnter();
        else if (inputBtn[3] && isControl)
            UnderEnter();
        else
            ExitEnter();
    }

    public void PointerUp()
    {
        isControl = false;
        ExitEnter();
    }


    void RightEnter()
    {
        dirVec = Vector3.right;
        buttonImage[2].color = Color.gray;
    }

    void LeftEnter()
    {
        dirVec = Vector3.left;
        buttonImage[1].color = Color.gray;
    }

    void UpEnter()
    {
        dirVec = Vector3.up;
        buttonImage[0].color = Color.gray;
    }

    void UnderEnter()
    {
        dirVec = Vector3.down;
        buttonImage[3].color = Color.gray;
    }

    public void ExitEnter()
    {
        dirVec = Vector3.zero;
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
