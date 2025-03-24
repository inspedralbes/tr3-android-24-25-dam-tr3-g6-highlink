using UnityEngine;

public class GameManagerController : MonoBehaviour
{

    public static GameManagerController Instance { get; private set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
