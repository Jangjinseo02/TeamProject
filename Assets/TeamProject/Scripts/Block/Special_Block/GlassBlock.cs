using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassBlock : Block
{
    Camera mainCamera;
    CameraMove checkCamera;

    public bool inCamera = false;

    public override void Awake()
    {
        base.Awake();
        mainCamera = FindObjectOfType<Camera>();
        checkCamera = mainCamera.GetComponent<CameraMove>();
    }

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public void InCamera()
    {
        inCamera = true;
        OnDamaged((int)health);
    }

    public override void OnDamaged(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            

            if (!group.isSoundPlay)
                group.FirstMemberSoundPlay(SoundManager.Sfx.BreakGlass);

            if (gameObject.activeInHierarchy)
                StartCoroutine(DamageRoutine());
        }
    }

    IEnumerator DamageRoutine()
    {
        yield return new WaitForSeconds(1.5f);

        boxCol.enabled = false;

        effect = ObjectManager.Instance.GetEffect((int)ObjectManager.effect.Glass);
        effect.transform.position = this.transform.position;
        sprite.color = Color.clear;
        effect.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        ObjectManager.Instance.ReturnEffect(effect, (int)ObjectManager.effect.Glass);

        yield return new WaitForSeconds(0.05f);

        group.blockManager.RemovePos(this);
        gameObject.SetActive(false);
    }
}
