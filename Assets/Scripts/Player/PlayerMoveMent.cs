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
    bool isJump = false;
    bool closeDeath = false;


    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float jumpPower = 5f;
    [SerializeField] private float dropPower = 5f;
    [SerializeField] private VirtualJoystick virtualJoystick;

    Animator anim;

    private void Awake()
    {
       // UIManager.Instance.UpdateHpText(hp);
       // UIManager.Instance.UpdateOxygenText(oxygen);
    }

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

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

        RaycastHit2D rayhitDown = Physics2D.Raycast(transform.position + (Vector3.down * 0.51f), Vector2.down, 0.1f, LayerMask.GetMask("Block"));

        if (!rayhitDown && !isJump)
        {
            DropOn();
            return;
        }
        else if (rayhitDown)
        {
            if (isDrop && rayhitDown.collider.tag == "Monster") //drop 상태에서 monster를 밟는 경우
            {
                DropOn();
                rayhitDown.collider.GetComponent<Block>().OnDamaged(100);
                return;
            }
            else if (rayhitDown.collider.tag == "Monster") //걷다가 아래가 빈 공간 없이 바로 몬스터 머리를 밟는 경우 << isDrop이 true가 아닌 상태에서 위 if문이 실행되어 문제가 있다.
            {
                DropOn();
                return;
            }
            else
                DropOff();
        }

        if (isDamaged)
            return;

        Move();
    }

    void DropOn()
    {
        isDrop = true;
        rigid.velocity = new Vector3(rigid.velocity.x, 0, 0);
        transform.position = transform.position + Vector3.down * dropPower * Time.deltaTime;
        anim.SetBool("isDrop", true);

    }

    void DropOff()
    {
        isDrop = false;
        rigid.velocity = Vector3.zero;
        anim.SetBool("isDrop", false);
        anim.SetBool("isDown", true);
        Invoke("DownOff", 0.2f);
    }

    void DownOff()
    {
        anim.SetBool("isDown", false);
    }

    private void Move()
    {
        //Vector2 horizontal = new Vector2(InputManager.Instance.dirVec.x, 0);
        dirvec = new Vector2(virtualJoystick.Horizontal, virtualJoystick.Vertical).normalized;
        InputManager.Instance.DragEnter(dirvec);
        Vector2 horizontal = new Vector2(virtualJoystick.Horizontal, 0);

        if(horizontal.x != 0)
        {
            anim.SetBool("isWalk", true);
        }
        else
        {
            anim.SetBool("isWalk", false);
        }

        if (horizontal.x > 0)
        {
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
        }
        else if (horizontal.x < 0)
        {
            gameObject.GetComponent<SpriteRenderer>().flipX = false;
        }

       
        rigid.velocity = horizontal * moveSpeed;

        RaycastHit2D rayhit = Physics2D.Raycast(transform.position, horizontal, 0.8f, LayerMask.GetMask("Block"));
        if (rayhit && rayhit.collider.tag == "Block" && !isJump)
        {
            RaycastHit2D uprayhit = Physics2D.Raycast(transform.position + Vector3.up, horizontal, 0.8f, LayerMask.GetMask("Block"));
            jumpTime += Time.deltaTime; //점프 시간 측정

            if ((!uprayhit || uprayhit.collider.tag == "Monster" || uprayhit.collider.tag == "Item") && jumpTime >= 0.4f) //점프 위치가 빈 경우, 그리고 점프 위치 블록이 몬스터인 경우
            {
                anim.SetTrigger("isJump");
                afpos = horizontal;
                Jump();
                jumpTime = 0;
            }
            else if (uprayhit && uprayhit.collider.tag != "Monster" && uprayhit.collider.tag != "Item") //몬스터의 경우 측정되야함, 즉 리셋 하지 않음
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
        rigid.AddForce(Vector2.up * jumpPower * 5, ForceMode2D.Impulse);
        //rigid.velocity = new Vector2(0, jumpPower * 5);
        yield return new WaitForSeconds(0.1f);
        rigid.velocity = afpos * moveSpeed;
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
        anim.SetTrigger("OnDamaged");
        anim.SetBool("OnDamage", true);
        rigid.velocity = Vector2.zero;
        this.GetComponent<CapsuleCollider2D>().enabled = false;

        hp -= damage;

        //UIManager.Instance.UpdateHpText(hp);

        if (hp <= 0)
        {
            hp = 0;
            //UIManager.Instance.UpdateHpText(hp);
            isDead = true;
            anim.SetTrigger("Die");
        }
    }

    IEnumerator DamageRoutine(GameObject block)
    {
        isDamaged = true;

        ClearOverBlock();
        StopCoroutine(BreatheRoutine());
        OnDamaged(block.GetComponent<Block>().attackDamage);

        yield return new WaitForSeconds(3.5f);

        anim.SetBool("OnDamage", false);
        this.GetComponent<CapsuleCollider2D>().enabled = true;

        yield return new WaitForSeconds(0.5f);
        isDamaged = false;
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
                    //member.group.GroupUnbalance();
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
                    member.group.GroupUnbalance();
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
                    member.group.GroupUnbalance();
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

    private void OnCollisionEnter2D(Collision2D collision)
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Block" && !isDamaged && !isDead)
        {
            StartCoroutine(DamageRoutine(collision.gameObject));
        }

        if (collision.gameObject.tag == "Item" && !isDead)
        {
            IItem item = collision.GetComponent<IItem>();
            if (item != null)
            {
                item.Use(gameObject);
            }
        }
    }
}
