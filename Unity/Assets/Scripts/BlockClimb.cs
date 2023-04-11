using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BlockClimb : MonoBehaviour
{
    [SerializeField] float speed = 5f, jumpForce = 600f;
    float moveX, flipRatio = 1f;
    Rigidbody2D rb;
    bool isJump, isBlockJump;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        PlayerFlip();
    }
void Movement()
    {
        moveX = Input.GetAxis("Horizontal") * speed;
        rb.velocity = new Vector2 (moveX, rb.velocity.y);

        if (rb.velocity.y == 0 || isBlockJump)
            isJump = true;
        else
            isJump = false;

        if(Input.GetButtonDown("Jump")&&isJump)
            rb.AddForce(Vector2.up*jumpForce);
    }
    void PlayerFlip()
    {
        Vector3 FlipScale = transform.localScale;

        if (Input.GetAxis("Horizonal") < 0)
            FlipScale.x = -flipRatio;
        if (Input.GetAxis("Horizonal") > 0)
            FlipScale.x = flipRatio;

        transform.localScale = FlipScale;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag.Equals("Block"))
            isBlockJump= true;
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if(col.gameObject.tag.Equals("Block"))
            isBlockJump = false;
    }
}
