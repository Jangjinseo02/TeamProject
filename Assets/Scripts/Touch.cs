using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Touch : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D rayhit = Physics2D.Raycast(mousPos, Vector3.down);

            if (rayhit.collider != null)
            {
                foreach (Block block in rayhit.collider.GetComponent<Block>().group)
                {
                    //block.group.GroupUnbalance();
                    block.OnDamaged(10);
                }
            }
        }

    }
}
