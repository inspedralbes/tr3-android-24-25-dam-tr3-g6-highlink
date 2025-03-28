using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    private string ServerUri = "http://localhost:4000";
    private string GameAPIUri = "/api/games";

    private bool ServerOnline = false;
    private bool StatsOnline = false;

    private int GameId;
    [SerializeField] private TMP_Text gameIdText;

    private void Awake()
    {
        Debug.Log("GameController Awake");
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Debug.Log("Going to create a game");
        StartCoroutine("CreateGame");
    }

    IEnumerator CreateGame() {
        Debug.Log("Creating game...");
        using (UnityWebRequest req = UnityWebRequest.PostWwwForm(ServerUri + GameAPIUri, "")) {
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.ConnectionError || req.result == UnityWebRequest.Result.ProtocolError) {
                Debug.LogError(req.error);
            } else {
                var response = req.downloadHandler.text;
                var game = JsonUtility.FromJson<Game>(response);
                ShowGameId(game.id);
            }
        }
    }

    public void UpdatePosition(Vector3 newPosition)
    {
        // Debug.Log($"Camera position updated to: {newPosition}");
        // Add your Camera position update logic here
        var heightToSave = newPosition.y;

        var heightToShow = (heightToSave + 1.012f)*10;

        HeightController.Instance.UpdateHeight(heightToShow);
    }

    private void ShowGameId(int id)
    {
        GameId = id;
        gameIdText.text = "Game ID: " + GameId.ToString();
    }
}

public class Game
{
    public int id = -1;
}