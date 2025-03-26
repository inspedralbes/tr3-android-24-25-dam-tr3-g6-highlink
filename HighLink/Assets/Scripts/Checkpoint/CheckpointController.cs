using UnityEngine;
using System.Collections; // Required for coroutines


public class CheckpointController : MonoBehaviour
{
    [SerializeField] private GameObject checkpointPrefab;
    [SerializeField] private Transform player1;
    [SerializeField] private Transform player2;

    private GameObject[] checkpoints = new GameObject[2];
    private Vector3[] checkpointPosition = new Vector3[2];

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnCheckpoint();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SpawnCheckpoint();
        }
    }

    private void SpawnCheckpoint()
    {
        for (int i = 0; i < checkpoints.Length; i++)
        {
            if (checkpoints[i] == null)
            {
                checkpoints[i] = Instantiate(checkpointPrefab);
            }
            checkpointPosition[i] = (i == 0) ? player1.position : player2.position;
            checkpoints[i].transform.position = checkpointPosition[i];
        }
        foreach (var checkpoint in checkpoints)
        {
            Animator animator = checkpoint.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger("Spawn");

                StartCoroutine(ResetToIdle(animator, 1.5f)); // Replace 1.5f with the actual animation duration

                // animator.GetComponent<AnimationEventHandler>().OnAnimationEnd(() =>
                // {
                //     animator.SetTrigger("Idle");
                // });
            }
        }
    }

    private IEnumerator ResetToIdle(Animator animator, float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the animation duration
        animator.SetTrigger("Idle");
    }

}
