using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardBlock : Block
{
    bool isDestory;

    int hitCount = 0;

    public override void Awake()
    {
        base.Awake();
    }

    public override void OnEnable()
    {
        base.OnEnable();

        boxCol.enabled = true;

        health = 50f;
        hitCount = 0;
        isDestory = false;
    }

    public override void OnDamaged(int damage)
    {
        sprite.sprite = sprites[hitCount + 1 < sprites.Length ? ++hitCount : hitCount];
        health -= damage;

        if (health <= 0)
        {
            isDestory = true;
            boxCol.enabled = false;

            if (!group.isSoundPlay)
                group.FirstMemberSoundPlay(SoundManager.Sfx.BreakHard);

            if (gameObject.activeInHierarchy)
                StartCoroutine(DestroyRoutine());
        }
    }

    IEnumerator DestroyRoutine()
    {
        effect = ObjectManager.Instance.GetEffect((int)ObjectManager.effect.Hard);
        effect.transform.position = this.transform.position;
        sprite.color = Color.clear;
        effect.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        ObjectManager.Instance.ReturnEffect(effect, (int)ObjectManager.effect.Hard);

        yield return new WaitForSeconds(0.05f);

        group.blockManager.RemovePos(this);

        foreach (Block member in this.group)
        {
            if (this == member)
                continue;
            member.RemoveGroup(group.blockManager);
            member.group.CheckGroupUnbalance();
        }

        gameObject.SetActive(false);
    }

    public bool isDestroy()
    {
        return isDestory;
    }
}
