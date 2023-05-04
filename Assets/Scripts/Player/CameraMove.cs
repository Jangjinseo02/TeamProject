using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private GameObject player;

    Vector3 pos;

    private void Start()
    {
        pos.x = player.transform.position.x;
        pos.y = player.transform.position.y;

        this.transform.position = pos;
    }

    void Update()
    {
        this.transform.position = new Vector3(transform.position.x, player.transform.position.y, -10);
        
    }

    public void CheckGlassBlock(GameObject target)
    {
        Vector2 screenPoint = Camera.main.WorldToViewportPoint(target.transform.position);

        bool onScreen = screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;

        if (onScreen)
            target.GetComponent<GlassBlock>().OnDamaged(100);
    }
}
