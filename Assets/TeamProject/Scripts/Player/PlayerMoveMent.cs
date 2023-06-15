using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveMent : MonoBehaviour
{
    int maxHealth = 100;
    int layer = 0;
    public float hp = 100f;
    public float oxygen = 100f;
    public float curOxygen = 0.5f;
    public float oxygenUp = 0.1f;

    Vector2 afpos;
    Vector3 dirvec;

    Rigidbody2D rigid;

    float jumpTime = 0;

    public bool isTutorial = false;
    public bool isDead = false;
    public bool isDamaged = false;
    public bool isDrop = false;
    public bool isStun = false;
    public bool isJump = false;
    public bool isGetTime = false;
    bool isWalk = false;
    bool closeDeath = false;

    bool isJumpSound = false;
    bool isDropSound = false;

    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float jumpPower = 25f;
    [SerializeField] private float dropPower = 5f;
    //[SerializeField] private VirtualJoystick virtualJoystick;

    Animator anim;
    BoxCollider2D boxCol;
    CapsuleCollider2D capCol;

    IEnumerator breath;
    IEnumerator recovery;

    public CameraMove followCamera;

    private void Awake()
    {
        boxCol = GetComponent<BoxCollider2D>();
        capCol = GetComponent<CapsuleCollider2D>();
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        followCamera = FindObjectOfType<CameraMove>();
    }

    private void Start()
    {
        layer = (1 << LayerMask.NameToLayer("Block")) + (1 << LayerMask.NameToLayer("Monster"));
    }

    private void FixedUpdate()
    {
        if (isDead)
        {
            rigid.velocity = Vector2.zero;
            capCol.enabled = false;
            boxCol.enabled = false;
            UIManager.Instance.ClearCountSpeechBubble();
            return;
        }

        AttackAnim();
        DropCheck();

        if (isDamaged || isStun || isDrop || isGetTime)
        {
            if (anim.GetBool("isWalk"))
                anim.SetBool("isWalk", false);

            if (!isDrop)
                rigid.velocity = Vector2.zero;
            dirvec = Vector3.zero;
            jumpTime = 0f;
            return;
        }else if (InputManager.Instance.isAttack)
        {
            jumpTime = 0f;
        }

        Move();
    }

    void AttackAnim()
    {
        anim.SetBool("Attack", InputManager.Instance.isAttack);
    }
    void DropCheck()
    {
        RaycastHit2D rayhitDown = Physics2D.Raycast(transform.position, Vector2.down, 0.585f, layer);
        Debug.DrawRay(transform.position, Vector2.down * 0.585f, Color.green);

        if (isJump)
            return;

        if (rayhitDown && !isJump)
        {
            if (isWalk || isDrop) //걷다가 아래가 빈 공간 없이 바로 몬스터 머리를 밟는 경우 << isDrop이 true가 아닌 상태에서 위 if문이 실행되어 문제가 있다.
            {
                if (rayhitDown.collider.CompareTag("Monster"))
                {
                    Vector2 hitPos = rayhitDown.collider.transform.position;
                    Vector2 playerPos = this.transform.position;
                    //아래에 몬스터가 있는 경우
                    if (playerPos.y - hitPos.y > 0.1f)
                    {
                        DropOn();
                        if (rayhitDown.collider.GetComponent<Monster>())
                            GameManager.Instance.GetScore(GameManager.BreakType.Monster);

                        //SoundManager.Instance.SfxPlay(SoundManager.Sfx.AttackMonster, false);
                        rayhitDown.collider.GetComponent<Block>().OnDamaged(100);
                    }
                }
                else
                    DropOff();
            }
        }
        else if (!rayhitDown && !isJump)
        {
            DropOn();
        }
    }


    void DropOn()
    {
        if (!isDrop && !isDamaged)
        {
            isDrop = true;
            anim.SetBool("Drop", isDrop);
        }
        rigid.velocity = Vector3.zero;
        transform.position = transform.position + Vector3.down * dropPower * Time.deltaTime;
    }

    void DropOff()
    {
        rigid.velocity = Vector3.zero;
        if (isDrop && !isDamaged)
        {
            if(!isDropSound && !isJumpSound)
                isDropSound = SoundManager.Instance.SfxPlay(SoundManager.Sfx.Down, false);
            anim.SetBool("Drop", false);
            anim.SetBool("Idle", true);
            Invoke("DownOff", 0.25f);
        }
    }

    void DownOff()
    {
        isDrop = false;
        if (isDropSound)
            isDropSound = false;
    }

    public void PlayerStun()
    {
        SoundManager.Instance.SfxPlay(SoundManager.Sfx.Stun, false);
        isStun = true;
        Invoke("PlayerAwake", 1f);
    }

    void PlayerAwake()
    {
        isStun = false;
    }

    private void Move()
    {
        if (isTutorial)
            return;

        Vector2 curVec = InputManager.Instance.dirVec;
        bool filpX = dirvec.x != curVec.x; //방향 전환이 되었는가?

        if (!isJump)
            dirvec = curVec;
        //InputManager.Instance.DragEnter(dirvec);
        Vector2 horizontal = new Vector2(dirvec.x, 0);

        if (horizontal.x != 0)
        {
            isWalk = true;

            if (!anim.GetBool("isWalk"))
                anim.SetBool("isWalk", isWalk);

            //방향 전환, jumpTime 초기화
            if (filpX)
            {
                gameObject.GetComponent<SpriteRenderer>().flipX = horizontal.x > 0 ? true : false;
                jumpTime = 0f;
            }
        }
        else
        {
            isWalk = false;
            if (anim.GetBool("isWalk"))
                anim.SetBool("isWalk", isWalk);
            anim.SetBool("Idle", true);
        }

        rigid.velocity = new Vector2(horizontal.x * moveSpeed, rigid.velocity.y);

        JumpCheck(horizontal);
    }

    void JumpCheck(Vector3 horizontal)
    {
        RaycastHit2D rayhit = Physics2D.Raycast(transform.position, horizontal, 0.8f, LayerMask.GetMask("Block"));
        if (rayhit && (rayhit.collider.CompareTag("Block") || rayhit.collider.CompareTag("Glass") || rayhit.collider.CompareTag("Item")) && !isJump)
        {
            RaycastHit2D uprayhit = Physics2D.Raycast(transform.position + Vector3.up, horizontal, 0.8f, LayerMask.GetMask("Block"));
            jumpTime += Time.deltaTime; //점프 시간 측정

            if ((!uprayhit || uprayhit.collider.CompareTag("Monster") || uprayhit.collider.CompareTag("Item")) && jumpTime >= 0.4f) //점프 위치가 빈 경우, 그리고 점프 위치 블록이 몬스터인 경우
            {
                isJump = true;
                anim.SetBool("Jump", isJump);

                if(!isJumpSound)
                    isJumpSound = SoundManager.Instance.SfxPlay(SoundManager.Sfx.Jump, false);
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
        rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.25f);
        isJump = false;
        anim.SetBool("Jump", isJump);

        yield return new WaitForSeconds(0.3f);

        if (isJumpSound)
            isJumpSound = false;
    }


    public void Breathe(int stageLevel)
    {
        oxygen -= curOxygen + (stageLevel * oxygenUp);

        if (oxygen <= 0)
        {
            oxygen = 0;

            if (closeDeath)
            {
                closeDeath = false;
            }
            OnDamaged(maxHealth + 1, false);
        }
        else if (oxygen <= 30)
        {
            //10이하
            if (oxygen <= 10)
            {
                SoundManager.Instance.SpeedUpSound((int)SoundManager.Sfx.CloseDeath); // 사운드 빠르게
                UIManager.Instance.SettingCountSpeechBubble(oxygen);
            }
            else
            {
                //SoundManager.Instance.SpeedDownSound((int)SoundManager.Sfx.CloseDeath); // 사운드 느리게
                UIManager.Instance.ClearCountSpeechBubble();
            }

            if (closeDeath)
                return;
            closeDeath = true;
            //스프라이트 교체
            anim.SetLayerWeight(1, 1);

            //사운드 출력
            SoundManager.Instance.SfxPlay(SoundManager.Sfx.CloseDeath, true);
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
        while (!isDead && !isDamaged && !GameManager.Instance.clear)
        {
            Breathe(GameManager.Instance.stageLevel);
            yield return new WaitForSeconds(0.5f);
        }

    }

    public void RealeseAir()
    {
        oxygen -= 20 + GameManager.Instance.stageLevel * 0.5f;
        UIManager.Instance.SettingAirImage(0); // type == 0 -air ;
        SoundManager.Instance.SfxPlay(SoundManager.Sfx.LoseAir, false);
    }

    public void ChangeFilp(bool isRight)
    {
        gameObject.GetComponent<SpriteRenderer>().flipX = isRight;
    }

    public void OnDamaged(int damage, bool isBlock)
    {
        hp -= damage;

        if (hp <= 0)
        {
            hp = 0;
            isDead = true;
            GameManager.Instance.GameExit();

            if(isBlock)
                SoundManager.Instance.SfxPlay(SoundManager.Sfx.BlockDamaged, false);
            else
                SoundManager.Instance.SfxPlay(SoundManager.Sfx.MonsterDamaged, false);
            anim.SetTrigger("Die");
        }
        else
        {
            StartCoroutine(DamageRoutine(isBlock));
        }
    }

    IEnumerator DamageRoutine(bool isBlock)
    {
        isDamaged = true;

        anim.SetBool("Idle", false);
        anim.SetBool("Block", isBlock);

        anim.SetTrigger("OnDamaged");
        rigid.velocity = Vector2.zero;
        capCol.enabled = false;
        boxCol.enabled = false;

        StopCoroutine(BreatheRoutine());

        if (!isBlock)
        {
            SoundManager.Instance.SfxPlay(SoundManager.Sfx.MonsterDamaged, false);
            yield return new WaitForSeconds(0.5f);
            anim.SetBool("Idle", true);

            isDamaged = false;
            capCol.enabled = true;
            boxCol.enabled = true;
            StartCoroutine(BreatheRoutine());

            yield break;
        }
        SoundManager.Instance.SfxPlay(SoundManager.Sfx.BlockDamaged, false);

        yield return new WaitForSeconds(3.5f);
        anim.SetBool("Idle", true);

        if(isBlock)
            ClearOverBlock();

        yield return new WaitForSeconds(0.5f);
        isDamaged = false;
        capCol.enabled = true;
        boxCol.enabled = true;
        StartCoroutine(BreatheRoutine());
    }

    void ClearOverBlock()
    {
        RaycastHit2D[] rayhits = Physics2D.RaycastAll(transform.position, Vector2.up, 100, layer);
        RaycastHit2D[] leftrayhits = Physics2D.RaycastAll(transform.position + Vector3.left, Vector2.up, 100, layer);
        RaycastHit2D[] rightrayhits = Physics2D.RaycastAll(transform.position + Vector3.right, Vector2.up, 100, layer);

        if (rayhits != null)
        {
            for (int i = 0; i < rayhits.Length; i++)
            {
                Block block = rayhits[i].collider.GetComponent<Block>();

                block.OnDamaged(100);

                //foreach (Block member in block.group)
                //{
                //    if (member == block)
                //        continue;
                //    member.RemoveGroup(block.group.blockManager);
                //}
            }
        }
        if (leftrayhits != null)
        {
            for (int i = 0; i < leftrayhits.Length; i++)
            {
                Block leftBlock = leftrayhits[i].collider.GetComponent<Block>();

                leftBlock.OnDamaged(100);
                //foreach (Block member in leftBlock.group)
                //{
                //    if (member == leftBlock)
                //        continue;
                //    member.RemoveGroup(leftBlock.group.blockManager);
                //}
            }
        }
        if (rightrayhits != null)
        {
            for (int i = 0; i < rightrayhits.Length; i++)
            {
                Block rightBlock = rightrayhits[i].collider.GetComponent<Block>();

                rightBlock.OnDamaged(100);
                //foreach (Block member in rightBlock.group)
                //{
                //    if (member == rightBlock)
                //        continue;
                //    member.RemoveGroup(rightBlock.group.blockManager);
                //}
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
        if (collision.gameObject.CompareTag("Manager"))
        {
            if (breath == null)
            {
                breath = BreatheRoutine();
                StartCoroutine(breath);
            }
            if (recovery == null)
            {
                recovery = RecoveryRoutine();
                StartCoroutine(recovery);
            }
        }

        if (collision.CompareTag("Block") && !isDamaged && !isDead)
        {
            anim.SetBool("Drop", false);

            Barrier barrier = GetComponentInChildren<Barrier>();
            if(barrier != null)
            {
                barrier.Use(collision.gameObject);
                return;
            }

            OnDamaged(collision.GetComponent<Block>().attackDamage, true);
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

    public void ClearPos()
    {
        anim.SetTrigger("Clear");
    }
}
