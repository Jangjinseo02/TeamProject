using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Restore : Item, IItem
{
    [SerializeField] float restoreOxygen;

    public override void Awake()
    {
        base.Awake();
    }

    public override void OnEnable()
    {
        base.OnEnable();

        sprite.color = Color.white;
    }

    public void Use(GameObject target)
    {
        if (isUse)
            return;

        PlayerMoveMent player = target.GetComponent<PlayerMoveMent>();

        if (player != null)
        {
            OxygenRestore(player);

            if (type == 11)
            {
                UIManager.Instance.SettingAirImage(1); // type == 1 +air
                SoundManager.Instance.SfxPlay(SoundManager.Sfx.Restore, false);
            }
            else if (type == 16)
            {
                UIManager.Instance.SettingItemUIImage(type);
                SoundManager.Instance.SfxPlay(SoundManager.Sfx.Recovery, false);
            }

            if (gameObject.activeInHierarchy)
                StartCoroutine(DestroyRoutine());
        }
    }

    IEnumerator DestroyRoutine()
    {
        effect = ObjectManager.Instance.GetEffect((int)ObjectManager.effect.AirCore);
        effect.transform.position = GameManager.Instance.player.transform.position;
        effect.SetActive(true);
        sprite.color = Color.clear;
        yield return new WaitForSeconds(0.1f);
        ObjectManager.Instance.ReturnEffect(effect, (int)ObjectManager.effect.AirCore);
        yield return new WaitForSeconds(0.05f);

        if (group != null)
            group.blockManager.RemovePos(this);
        yield return new WaitForSeconds(0.05f);
        if (type == 11)
            ObjectManager.Instance.ReturnItem(this.gameObject, (int)ObjectManager.item.Restore);
        else
            ObjectManager.Instance.ReturnItem(this.gameObject, (int)ObjectManager.item.Recovery);
    }

    void OxygenRestore(PlayerMoveMent player)
    {
        if (isUse)
            return;

        UsingItem();

        GameManager.Instance.GetScore(GameManager.BreakType.AirCore);
        player.oxygen += restoreOxygen;
        if (player.oxygen >= 100)
            player.oxygen = 100;
    }
}
