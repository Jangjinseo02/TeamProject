using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Restore : Item, IItem
{
    [SerializeField] float restoreOxygen;

    public void Use(GameObject target)
    {
        if (isUse)
            return;

        PlayerMoveMent player = target.GetComponent<PlayerMoveMent>();

        if (player != null)
        {
            OxygenRestore(player);

            UIManager.Instance.SettingAirImage(1); // type == 1 +air

            if (group != null)
                group.blockManager.RemovePos(this);

            gameObject.SetActive(false);
        }
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Block")
        {
            ObjectManager.Instance.ReturnBlock(this.gameObject);
            gameObject.SetActive(false);
        }
    }
}
