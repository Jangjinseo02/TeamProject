using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    MonsterA monster;

    PlayerMoveMent target;
    BoxCollider2D boxCol;

    private void Awake()
    {
        monster = GetComponentInParent<MonsterA>();
        boxCol = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        boxCol.enabled = false;
    }

    public void TargetClear()
    {
        boxCol.enabled = false;
        target = null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && target == null && !monster.isDead)
        {
            target = collision.GetComponent<PlayerMoveMent>();
            target.OnDamaged(monster.attackDamage, false);
        }
    }
}
