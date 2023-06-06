using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorBlock : Block
{
    public override void Awake()
    {
        base.Awake();
    }

    public override void OnEnable()
    {
        base.OnEnable();
    }

    //운석 블록은 그룹에 혼자만 존재
    public void OnlyOne()
    {
        group.AddGroup(this);
    }

    public override void OnDamaged(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            if (!group.isSoundPlay)
                group.FirstMemberSoundPlay(SoundManager.Sfx.BreakMeteor);

            if (gameObject.activeInHierarchy)
                StartCoroutine(DestroyRoutine());
        }
    }
    IEnumerator DestroyRoutine()
    {
        effect = ObjectManager.Instance.GetEffect((int)ObjectManager.effect.Meteor);
        effect.transform.position = this.transform.position;
        sprite.color = Color.clear;
        effect.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        ObjectManager.Instance.ReturnEffect(effect, (int)ObjectManager.effect.Meteor);
        yield return new WaitForSeconds(0.05f);

        group.blockManager.RemovePos(this);
        gameObject.SetActive(false);
    }
}
