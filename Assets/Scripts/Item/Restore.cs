using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Restore : Item, IItem
{
    public void Use(GameObject target)
    {
        PlayerMoveMent player = target.GetComponent<PlayerMoveMent>();

        if (player != null)
        {
            player.oxygen += 20f;
            if (player.oxygen >= 100)
                player.oxygen = 100;

            //ObjectManager.Instance.ReturnBlock(this.gameObject);
            UIManager.Instance.SettingAirImage(1); // type == 1 +air
            if (group != null)
                group.blockManager.RemovePos(this);
            gameObject.SetActive(false);
            //gameObject (false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Block")
        {
            ObjectManager.Instance.ReturnBlock(this.gameObject);
            gameObject.SetActive(false);
        }
    }
}
