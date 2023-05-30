using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveMent : MonoBehaviour
{
    int maxHealth = 100;
    public float hp = 100f;
    public float oxygen = 100f;

    Vector2 afpos;
    Vector3 dirvec;

    Rigidbody2D rigid;

    float jumpTime = 0;

    public bool isDead = false;
    public bool isDamaged = false;
    public bool isDrop = false;
    public bool isStun = false;
    public bool isJump = false;
    public bool isGetTime = false;
    bool isWalk = false;
    bool closeDeath = false;

    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float jumpPower = 25f;
    [SerializeField] private float dropPower = 5f;
    [SerializeField] private VirtualJoystick virtualJoystick;

    Animator anim;
    BoxCollider2D boxCol;
    CapsuleCollider2D capCol;

    public CameraMove followCamera;

    private void Awake()
    {
        boxCol = GetComponent<BoxCollider2D>();
        capCol = GetComponent<CapsuleCollider2D>();
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        followCamera = FindObjectOfType<CameraMove>();
    }

    void Start()
    {
        StartCoroutine(BreatheRoutine());
        StartCoroutine(RecoveryRoutine());
    }

    private void FixedUpdate()
    {
        if (isDead)
        {
            rigid.velocity = Vector2.zero;
            UIManager.Instance.ClearCountSpeechBubble();
            GameManager.Instance.PlayerDead();
            return;
        }

        DropCheck();

        if (isDamaged || isStun || isDrop || isGetTime)
        {
            if (anim.GetBool("isWalk"))
                anim.SetBool("isWalk", false);

            dirvec = Vector3.zero;
            jumpTime = 0f;
            return;
        }else if (InputManager.Instance.isAttack)
        {
            jumpTime = 0f;
        }

        Move();
    }

    void DropCheck()
    {
        RaycastHit2D rayhitDown = Physics2D.Raycast(transform.position, Vector2.down, 0.585f, LayerMask.GetMask("Block"));
        Debug.DrawRay(transform.position, Vector2.down * 0.585f, Color.green);

        if (rayhitDown && !isJump)
        {
            if (isWalk && !isDrop) //걷다가 아래가 빈 공간 없이 바로 몬스터 머리를 밟는 경우 << isDrop이 true가 아닌 상태에서 위 if문이 실행되어 문제가 있다.
            {
                RaycastHit2D rayhitFront = Physics2D.Raycast(transform.position, dirvec, 0.6f, LayerMask.GetMask("Block"));

                if (rayhitFront && !rayhitFront.collider.CompareTag("Monster") && rayhitDown.collider.CompareTag("Monster"))
                    DropOn();
            }
            else if (isDrop && rayhitDown.collider.CompareTag("Monster")) //drop 상태에서 monster를 밟는 경우
            {
                DropOn();
                rayhitDown.collider.GetComponent<Block>().OnDamaged(100);
            }
            else
                DropOff();
        }
        else if (!rayhitDown && !isJump)
        {
            DropOn();
        }
    }


    void DropOn()
    {
        if (!isDrop)
        {
            Debug.Log("Drop");
            isDrop = true;
            anim.SetTrigger("IsDrop");
            anim.SetBool("isDrop", true);
        }
        rigid.velocity = Vector3.zero;
        transform.position = transform.position + Vector3.down * dropPower * Time.deltaTime;
    }

    void DropOff()
    {
        rigid.velocity = Vector3.zero;
        if (isDrop)
        {
            anim.SetBool("isDrop", false);
            Invoke("DownOff", 0.25f);
        }
    }

    void DownOff()
    {
        isDrop = false;
    }

    public void PlayerStun()
    {
        isStun = true;
        Invoke("PlayerAwake", 1f);
    }

    void PlayerAwake()
    {
        isStun = false;
    }

    private void Move()
    {
        Vector2 curVec = new Vector2(virtualJoystick.Horizontal, virtualJoystick.Vertical).normalized;
        bool filpX = dirvec.x != curVec.x; //방향 전환이 되었는가?

        if (!isJump)
            dirvec = curVec;
        InputManager.Instance.DragEnter(dirvec);
        Vector2 horizontal = new Vector2(dirvec.x, 0);

        if (horizontal.x != 0)
        {
            if (!anim.GetBool("isWalk"))
                anim.SetBool("isWalk", true);

            //방향 전환, jumpTime 초기화
            if (filpX)
            {
                gameObject.GetComponent<SpriteRenderer>().flipX = horizontal.x > 0 ? true : false;
                jumpTime = 0f;
            }
            isWalk = true;
        }
        else
        {
            if (anim.GetBool("isWalk"))
                anim.SetBool("isWalk", false);
            isWalk = false;
        }

        rigid.velocity = new Vector2(horizontal.x * moveSpeed, rigid.velocity.y);

        JumpCheck(horizontal);
    }

    void JumpCheck(Vector3 horizontal)
    {
        RaycastHit2D rayhit = Physics2D.Raycast(transform.position, horizontal, 0.8f, LayerMask.GetMask("Block"));
        if (rayhit && (rayhit.collider.CompareTag("Block") || rayhit.collider.CompareTag("Glass") || rayhit.collider.CompareTag("Monster") || rayhit.collider.CompareTag("Item")) && !isJump)
        {
            RaycastHit2D uprayhit = Physics2D.Raycast(transform.position + Vector3.up, horizontal, 0.8f, LayerMask.GetMask("Block"));
            jumpTime += Time.deltaTime; //점프 시간 측정

            if ((!uprayhit || uprayhit.collider.CompareTag("Monster") || uprayhit.collider.CompareTag("Item")) && jumpTime >= 0.4f) //점프 위치가 빈 경우, 그리고 점프 위치 블록이 몬스터인 경우
            {
                anim.SetTrigger("isJump");
                Jump();
                jumpTime = 0;
            }
            else if (uprayhit && !uprayhit.collider.CompareTag("Monster") && uprayhit.collider.CompareTag("Item")) //몬스터의 경우 측정되야함, 즉 리셋 하지 않음
            {
                jumpTime = 0;
            }
        }
    }

    void Jump()
    {
        StartCoroutine(JumpRoutine());
    }

    IEnumerator JumpRoutine()
    {
        isJump = true;
        //followCamera.JumpCameraMove(); //카메라 움직임 변경

        rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.25f);
        isJump = false;
    }


    public void Breathe(int stageLevel)
    {
        oxygen -= 1f + (stageLevel * 0.2f);

        //UIManager.Instance.UpdateOxygenText(oxygen);

        if (oxygen <= 0)
        {
            oxygen = 0;
            //UIManager.Instance.UpdateOxygenText(oxygen);
            //   UIManager.Instance.UpdateOxygenText(oxygen);
            if (closeDeath)
            {
                closeDeath = false;
                SoundManager.Instance.SfxAllStop();
            }
            OnDamaged(maxHealth);
        }
        else if (oxygen <= 30)
        {
            //10이하
            if (oxygen <= 10)
            {
                Debug.Log("Oxygen <= 10");
                UIManager.Instance.SettingCountSpeechBubble(oxygen);
            }
            else
            {
                UIManager.Instance.ClearCountSpeechBubble();
            }

            if (closeDeath)
                return;
            closeDeath = true;
            //스프라이트 교체
            anim.SetLayerWeight(1, 1);

            //사운드 출력
            SoundManager.Instance.SfxPlay(SoundManager.Sfx.CloseDeath);
        }
        else
        {
            closeDeath = false;
            anim.SetLayerWeight(1, 0);
            SoundManager.Instance.SfxAllStop();
            UIManager.Instance.ClearCountSpeechBubble();
        }
    }

    IEnumerator BreatheRoutine()
    {
        while (!isDead && !isDamaged)
        {
            Breathe(GameManager.Instance.stageLevel);
            yield return new WaitForSeconds(0.5f);
        }

    }

    public void RealeseAir()
    {
        oxygen -= 20 + GameManager.Instance.stageLevel * 0.5f;
        UIManager.Instance.SettingAirImage(0); // type == 0 -air ;
    }

    public void OnDamaged(int damage)
    {
        StartCoroutine(DamageRoutine());
        hp -= damage;

        if (hp <= 0)
        {
            hp = 0;
            isDead = true;
            anim.SetTrigger("Die");
        }
    }

    IEnumerator DamageRoutine()
    {
        isDamaged = true;

        anim.SetTrigger("OnDamaged");
        anim.SetBool("OnDamage", true);
        rigid.velocity = Vector2.zero;
        capCol.enabled = false;
        boxCol.enabled = false;

        StopCoroutine(BreatheRoutine());

        yield return new WaitForSeconds(3.5f);
        anim.SetBool("OnDamage", false);
        ClearOverBlock();

        yield return new WaitForSeconds(0.5f);
        isDamaged = false;
        capCol.enabled = true;
        boxCol.enabled = true;
        StartCoroutine(BreatheRoutine());
    }

    void ClearOverBlock()
    {
        RaycastHit2D[] rayhits = Physics2D.RaycastAll(transform.position, Vector2.up, 100, LayerMask.GetMask("Block"));
        RaycastHit2D[] leftrayhits = Physics2D.RaycastAll(transform.position + Vector3.left, Vector2.up, 100, LayerMask.GetMask("Block"));
        RaycastHit2D[] rightrayhits = Physics2D.RaycastAll(transform.position + Vector3.right, Vector2.up, 100, LayerMask.GetMask("Block"));

        if (rayhits != null)
        {
            for (int i = 0; i < rayhits.Length; i++)
            {
                Block block = rayhits[i].collider.GetComponent<Block>();

                block.OnDamaged(100);

                foreach (Block member in block.group)
                {
                    member.RemoveGroup(block.group.blockManager);
                    //member.group.CheckGroupUnbalance();
                }
            }
        }
        if (leftrayhits != null)
        {
            for (int i = 0; i < leftrayhits.Length; i++)
            {

                Block leftBlock = leftrayhits[i].collider.GetComponent<Block>();

                leftBlock.OnDamaged(100);
                foreach (Block member in leftBlock.group)
                {
                    member.RemoveGroup(leftBlock.group.blockManager);
                    member.group.CheckGroupUnbalance();
                }
            }
        }
        if (rightrayhits != null)
        {
            for (int i = 0; i < rightrayhits.Length; i++)
            {
                Block rightBlock = rightrayhits[i].collider.GetComponent<Block>();

                rightBlock.OnDamaged(100);
                foreach (Block member in rightBlock.group)
                {
                    member.RemoveGroup(rightBlock.group.blockManager);
                    member.group.CheckGroupUnbalance();
                }
            }
        }
    }

    void Recovery()
    {
        if (hp < maxHealth)
        {
            hp += 1;
           // UIManager.Instance.UpdateHpText(hp);
        }
    }

    IEnumerator RecoveryRoutine()
    {
        while (!isDead)
        {
            Recovery();
            yield return new WaitForSeconds(10f + (GameManager.Instance.stageLevel * 0.5f));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Block") && !isDamaged && !isDead)
        {
            Barrier barrier = GetComponentInChildren<Barrier>();
            if(barrier != null)
            {
                barrier.Use(collision.gameObject);
                return;
            }

            OnDamaged(collision.GetComponent<Block>().attackDamage);
        }

        if (collision.CompareTag("Item") && !isDead && !isGetTime)
        {
            IItem item = collision.GetComponent<IItem>();
            if (item != null)
            {
                isGetTime = true;
                StartCoroutine(UsingRoutine(item));
            }
        }
    }

    IEnumerator UsingRoutine(IItem item)
    {
        anim.SetTrigger("GetItem");
        item.Use(gameObject);
        yield return new WaitForSeconds(0.5f);
        isGetTime = false;
    }
}
