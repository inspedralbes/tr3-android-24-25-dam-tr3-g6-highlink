using UnityEngine;

public class RopeCreator : MonoBehaviour
{
    public Transform player1; // Reference to the first player
    public Transform player2; // Reference to the second player
    public GameObject ropeFragmentPrefab; // Prefab for each rope fragment
    public int fragmentCount = 10; // Number of rope fragments
    public float ropeLength = 10f; // Total length of the rope
    public float fragmentSpacing = 0.5f; // Spacing between fragments

    private GameObject[] ropeFragments; // Array to store rope fragments

    void Start()
    {
        // Check if player1 and player2 are assigned
        if (player1 == null || player2 == null)
        {
            Debug.LogError("Player1 or Player2 is not assigned in the Inspector!");
            return;
        }

        // Check if the rope fragment prefab is assigned
        if (ropeFragmentPrefab == null)
        {
            Debug.LogError("RopeFragmentPrefab is not assigned in the Inspector!");
            return;
        }

        // Initialize the rope fragments
        ropeFragments = new GameObject[fragmentCount];
        Vector2 ropeDirection = (player2.position - player1.position).normalized;
        Vector2 spawnPosition = player1.position;

        for (int i = 0; i < fragmentCount; i++)
        {
            // Instantiate a rope fragment
            ropeFragments[i] = Instantiate(ropeFragmentPrefab, spawnPosition, Quaternion.identity);
            spawnPosition += (Vector2)(ropeDirection * fragmentSpacing);

            // Add Rigidbody2D and Collider2D to the fragment (if not already on the prefab)
            Rigidbody2D rb = ropeFragments[i].GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = ropeFragments[i].AddComponent<Rigidbody2D>();
            }
            // rb.gravityScale = 0.05f; // Disable gravity for the rope fragments

            Collider2D collider = ropeFragments[i].GetComponent<Collider2D>();
            if (collider == null)
            {
                collider = ropeFragments[i].AddComponent<CircleCollider2D>();
            }

            // Connect the fragment to the previous fragment (or player1 for the first fragment)
            if (i == 0)
            {
                ConnectFragments(player1.gameObject, ropeFragments[i]);
            }
            else
            {
                ConnectFragments(ropeFragments[i - 1], ropeFragments[i]);
            }
        }

        // Connect the last fragment to player2
        ConnectFragments(ropeFragments[fragmentCount - 1], player2.gameObject);

        // Ignore collisions between the first two and last two fragments and players
        IgnorePlayerCollisions();
    }

    void ConnectFragments(GameObject startObject, GameObject endObject)
    {
        // Add a DistanceJoint2D to connect the fragments
        DistanceJoint2D joint = startObject.AddComponent<DistanceJoint2D>();
        joint.connectedBody = endObject.GetComponent<Rigidbody2D>();
        joint.distance = fragmentSpacing; // Set the distance between fragments
        joint.autoConfigureDistance = false;
        joint.maxDistanceOnly = true; // Allow stretching but not compressing
    }

     void IgnorePlayerCollisions()
    {
        // Get all GameObjects with the "Player" tag
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        // Ignore collisions for the first two and last two fragments
        for (int i = 0; i < 2; i++)
        {
            foreach (GameObject player in players)
            {
                Collider2D playerCollider = player.GetComponent<Collider2D>();
                if (playerCollider != null)
                {
                    Physics2D.IgnoreCollision(ropeFragments[i].GetComponent<Collider2D>(), playerCollider);
                    Physics2D.IgnoreCollision(ropeFragments[ropeFragments.Length - 1 - i].GetComponent<Collider2D>(), playerCollider);
                }
            }
        }
    }

    
}