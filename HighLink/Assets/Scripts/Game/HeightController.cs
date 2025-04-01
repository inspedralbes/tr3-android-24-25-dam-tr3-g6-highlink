using UnityEngine;
using TMPro;

public class HeightController : MonoBehaviour
{
    public static HeightController Instance { get; private set; }

    public int Height { get; private set; }
    [SerializeField]private TMP_Text heightText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void UpdateHeight(float newHeight)
    {
        Height = (int)newHeight;
        // Debug.Log($"Height updated to: {Height}");
        heightText.text = "Height: " + Height.ToString();
        // Add your Height update logic here
    }
}