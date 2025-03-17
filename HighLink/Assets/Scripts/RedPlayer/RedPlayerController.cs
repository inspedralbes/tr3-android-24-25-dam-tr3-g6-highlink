using UnityEngine;

public class RedPlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    private Rigidbody2D body;
    private Animator anim;
    private bool grounded;
    [SerializeField] private float charSize = 0.15f;
    private float jumpTime = 10f;

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

        if (body.linearVelocity.y < -0.1f)
        {
            // grounded = false;
            anim.SetBool("Falling", true);  // Trigger fall animation
        }

        if (grounded && body.linearVelocity.y == 0) 
        {
            anim.SetBool("Falling", false);  // Stop fall animation
        }
    }

    private void Jump()
    {

        // body.AddForce(new Vector2(0, 5), ForceMode2D.Impulse);
        body.linearVelocity = new Vector2(body.linearVelocity.x, speed);
        anim.SetTrigger("Jump");
        jumpTime = anim.GetCurrentAnimatorStateInfo(0).length;
        grounded = false;

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "ground")
        {
            
            grounded = true;

        }
    }

     void OnTriggerEnter2D (Collider2D other)
    {
        
        grounded = true;
        
    }
}
