using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassBlock : Block
{
    Camera mainCamera;
    CameraMove checkCamera;

    private void Awake()
    {
        mainCamera = FindObjectOfType<Camera>();
        checkCamera = mainCamera.GetComponent<CameraMove>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(mainCamera);
        Debug.Log(checkCamera);
    }

    public override void OnDamaged(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            StartCoroutine(DamageRoutine());
        }
    }

    IEnumerator DamageRoutine()
    {
        yield return new WaitForSeconds(1.5f);
        group.blockManager.RemovePos(this);
        gameObject.SetActive(false);
    }
}
