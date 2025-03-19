using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    private Rigidbody2D body;
    private Animator anim;
    private bool grounded;
    [SerializeField] private float charSize = 0.15f;
    [SerializeField] private float jumpMultiplier = 1.5f;
    private HashSet<Collider2D> groundContacts = new HashSet<Collider2D>();
    [SerializeField] private float maxSpeed = 10f;

    public KeyCode JumpKey = KeyCode.UpArrow; // public KeyCode JumpKey 
    public KeyCode LeftKey = KeyCode.LeftArrow; // public KeyCode LeftKey
    public KeyCode RightKey = KeyCode.RightArrow; // public KeyCode RightKey

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
        
        if (Input.GetKey(LeftKey))
        {
            if(!grounded)
            {
                moveHorizontal = -0.5f;
            }
            else
            {
                moveHorizontal = -1f;
            }
        }
        if (Input.GetKey(RightKey))
        {
            if(!grounded)
            {
                moveHorizontal = 0.7f;
            }
            else
            {
                moveHorizontal = 1f;
            }
        }

        // body.linearVelocity = new Vector2(moveHorizontal * speed, body.linearVelocity.y);
        body.AddForce(new Vector2(moveHorizontal * speed * Time.deltaTime, 0), ForceMode2D.Impulse);

        // Flip the sprite
        if(moveHorizontal > 0.01f)
        {
            transform.localScale = new Vector3(charSize, charSize, charSize);
        }
        else if(moveHorizontal < -0.01f)
        {
            transform.localScale = new Vector3(-charSize, charSize, charSize);
        }

        if (Input.GetKeyDown(JumpKey) && grounded)
        {
            Jump();
        }

        anim.SetBool("Walking", moveHorizontal != 0);
        anim.SetBool("Grounded", grounded);

        if (body.linearVelocity.y < -0.15f)
        {
            // grounded = false;
            anim.SetBool("Falling", true);  // Trigger fall animation
        }

        if (body.linearVelocity.y >= -0.15f) 
        {
            anim.SetBool("Falling", false);  // Stop fall animation
        }

        if(body.linearVelocity.magnitude > maxSpeed)
        {
            body.linearVelocity = Vector3.ClampMagnitude(body.linearVelocity, maxSpeed);
        }
    }

    private void Jump()
    {

        // body.linearVelocity = new Vector2(body.linearVelocity.x, speed * jumpMultiplier);
        body.AddForce(new Vector2(0, speed * jumpMultiplier), ForceMode2D.Impulse);
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
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the trigger zone.");
            grounded = true;
        }
        
        
    }
}
