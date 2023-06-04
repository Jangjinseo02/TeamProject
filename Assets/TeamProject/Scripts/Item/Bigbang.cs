using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bigbang : Item, IItem
{
    public override void Awake()
    {
        base.Awake();
    }

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public void Use(GameObject target)
    {
        if (isUse)
            return;

        PlayerMoveMent player = target.GetComponent<PlayerMoveMent>();
        
        if (player!= null && !isUse)
        {
            UIManager.Instance.SettingItemUIImage(type);
            HashSet<Block> destroyBlock = new HashSet<Block>(player.followCamera.ReturnBlock());

            StartCoroutine(DestroyRoutine(destroyBlock));
        }
    }

    IEnumerator DestroyRoutine(HashSet<Block> destroyBlock)
    {
        effect = ObjectManager.Instance.GetEffect((int)ObjectManager.effect.BigBang);
        effect.transform.position = GameManager.Instance.player.transform.position;
        effect.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        ObjectManager.Instance.ReturnEffect(effect, (int)ObjectManager.effect.BigBang);
        yield return new WaitForSeconds(0.05f);
        //sprite.color = Color.white;
        foreach (Block block in destroyBlock)
        {
            if (block.tag == "Item")
                continue;
            block.OnDamaged(Mathf.CeilToInt(block.health));
        }
        yield return new WaitForSeconds(0.05f);
        ObjectManager.Instance.ReturnItem(this.gameObject, (int)ObjectManager.item.Bigbang);
    }
}
