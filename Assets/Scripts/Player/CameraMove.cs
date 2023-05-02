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
}
