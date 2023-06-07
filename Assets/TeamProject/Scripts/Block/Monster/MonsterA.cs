using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterA : Monster
{
    [SerializeField] AttackArea attackArea;

    PlayerMoveMent player;

    IEnumerator checkTarget;

    public override int attackDamage
    {
        get { return 10 + (GameManager.Instance.stageLevel * 1); }
    }

    public override void OnEnable()
    {
        health = 5 + GameManager.Instance.stageLevel * 1.25f;

        boxCol.enabled = true;
        isDead = false;
        inCamera = false;
    }

    public override void Awake()
    {
        base.Awake();

        attackArea = GetComponentInChildren<AttackArea>();
        anim = GetComponent<Animator>();
    }

    public override void CameraOut()
    {
        base.CameraOut();

        if (checkTarget != null)
        {
            StopCoroutine(checkTarget);
            checkTarget = null;
        }
    }

    public override void CheckTarget(HashSet<Block> blocks)
    {
        base.CheckTarget(blocks);
        Debug.Log("child");

        if (checkTarget == null)
        {
            checkTarget = CheckTargetPlayer();
            StartCoroutine(checkTarget);
        }
    }

    void AttackReady()
    {
        float x = gameObject.transform.localScale.x;
        float y = gameObject.transform.localScale.y;
        Vector2 dir = player.transform.position - transform.position;
        Vector2 areaPos = Vector2.zero;

        //공격 범위 위치 조절
        if (dir.x < 0)
        {
            areaPos = new Vector2(transform.position.x + (-0.5f), transform.position.y);
            x = gameObject.transform.localScale.x < 0 ? x * (-1) : x;
        }
        else if (dir.x > 0)
        {
            areaPos = new Vector2(transform.position.x + 0.5f, transform.position.y);
            x = gameObject.transform.localScale.x > 0 ? x * (-1) : x;
        }
        gameObject.transform.localScale = new Vector2(x, y);

        //공격 위치 지정
        attackArea.transform.position = areaPos;

    }

    public override void Attack()
    {
        base.Attack();

        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        //공격 애니메이션
        Debug.Log("anim");
        //대상 체력 감소
        Debug.Log("hp down");

        //공격 범위 활성화
        attackArea.gameObject.GetComponent<BoxCollider2D>().enabled = true;

        yield return new WaitForSecondsRealtime(0.5f);
        //재공격을 위한 다시 탐색(초기화)
        attackArea.TargetClear();
        player = null;
        if (checkTarget != null)
            CameraOut();
        //StartCoroutine(checkTarget);
    }

    IEnumerator CheckTargetPlayer()
    {
        while (player == null && !isDead && inCamera)
        {
            RaycastHit2D righthit = Physics2D.Raycast(transform.position + (Vector3.right * 0.3f), Vector2.right, 1f, LayerMask.GetMask("Player"));
            RaycastHit2D lefthit = Physics2D.Raycast(transform.position + (Vector3.left * 0.3f), Vector2.left, 1f, LayerMask.GetMask("Player"));
            Debug.DrawRay(transform.position + (Vector3.right * 0.2f), Vector2.right, Color.green);
            Debug.DrawRay(transform.position + (Vector3.left * 0.2f), Vector2.left, Color.green);

            if (righthit)
                player = righthit.collider.GetComponent<PlayerMoveMent>();
            else if (lefthit)
                player = lefthit.collider.GetComponent<PlayerMoveMent>();

            yield return new WaitForSecondsRealtime(0.35f);
        }
        if (isDead)
            yield break;
        anim.SetTrigger("CheckTarget");
        Debug.Log("공격 준비");
        AttackReady();
        yield return new WaitForSeconds(1f);
        Debug.Log("target Check");
        if (player)
            Attack();
    }
}
