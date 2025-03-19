using UnityEngine;
using System.Collections.Generic;

public class RedPlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    private Rigidbody2D body;
    private Animator anim;
    private bool grounded;
    [SerializeField] private float charSize = 0.15f;
    [SerializeField] private float jumpMultiplier = 1.5f;
    private HashSet<Collider2D> groundContacts = new HashSet<Collider2D>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        body.linearVelocity = new Vector2(0, 0);
        body.gravityScale = 1;
        body.freezeRotation = true; 

        anim = GetComponent<Animator>();
        grounded = true;

        
    }

    // Update is called once per frame
    void Update()
    {
        float moveHorizontal = 0f;
        
        if (Input.GetKey(KeyCode.A))
        {
            moveHorizontal = -1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveHorizontal = 1f;
        }

        body.linearVelocity = new Vector2(moveHorizontal * speed, body.linearVelocity.y);

        // Flip the sprite
        if(moveHorizontal > 0.01f)
        {
            transform.localScale = new Vector3(charSize, charSize, charSize);
        }
        else if(moveHorizontal < -0.01f)
        {
            transform.localScale = new Vector3(-charSize, charSize, charSize);
        }

        if (Input.GetKeyDown(KeyCode.W) && grounded)
        {
            Jump();
        }

        anim.SetBool("Walking", moveHorizontal != 0);
        anim.SetBool("Grounded", grounded);

        if (body.linearVelocity.y < -0.25f)
        {
            // grounded = false;
            anim.SetBool("Falling", true);  // Trigger fall animation
        }

        if (grounded && body.linearVelocity.y  >= -0.25f) 
        {
            anim.SetBool("Falling", false);  // Stop fall animation
        }
    }

    private void Jump()
    {

        // body.AddForce(new Vector2(0, 5), ForceMode2D.Impulse);
        body.linearVelocity = new Vector2(body.linearVelocity.x, speed * jumpMultiplier);
        anim.SetTrigger("Jump");
        grounded = false;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsGroundTag(collision.gameObject.tag))
        {
            groundContacts.Add(collision.collider);
            grounded = true;

        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (IsGroundTag(collision.gameObject.tag))
        {
            groundContacts.Remove(collision.collider);
        }
    }

    public bool IsGrounded()
    {
        return groundContacts.Count > 0;
    }

    private bool IsGroundTag(string tag)
    {
        return tag == "Grounded" || tag == "Grounded2" || tag == "Grounded3" || tag == "ground";
    }

     void OnTriggerEnter2D (Collider2D other)
    {
        
        grounded = true;
        
    }
}
