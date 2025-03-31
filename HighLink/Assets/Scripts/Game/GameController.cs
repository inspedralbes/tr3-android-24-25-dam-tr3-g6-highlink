using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    private string ServerUri = "http://localhost:4000";
    private string StatsAPIUri = "http://localhost:4001";
    private string GameAPIUri = "/api/games";
    private string CheckStatsUri = "/state-stats";

    private bool ServerOnline = false;
    private bool StatsOnline = false;

    private float heightToShow = 0f;
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
        GameStart();
    }

    private void GameStart() {
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
                ServerOnline = true;
                CheckStatsService();
            }
        }
    }

    private void CheckStatsService() {
        Debug.Log("Checking stats service...");
        StartCoroutine("CheckStats");
    }

    IEnumerator CheckStats() {
        using (UnityWebRequest req = UnityWebRequest.Get(ServerUri + CheckStatsUri)) {
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.ConnectionError || req.result == UnityWebRequest.Result.ProtocolError) {
                Debug.LogError(req.error);
            } else {
                var response = req.downloadHandler.text;
                var Stats = JsonUtility.FromJson<StatsServiceState>(response);
                if (Stats.state == "running") {
                    Debug.Log("Stats service is online");
                    StatsOnline = true;
                    InvokeRepeating("StartSendingStats", 0, 5);
                } else {
                    Debug.Log("Stats service is offline");
                }
                
            }
        }
    }

    private void StartSendingStats()
    {
        StartCoroutine("SendStats");
    }

    IEnumerator SendStats() {
        if (!(ServerOnline && StatsOnline)) {
            Debug.Log("Server or Stats service is offline");
            yield break;
        };

        Debug.Log("Sending stats...");

        StatsData jsonData = new StatsData(GameId, Mathf.Round(heightToShow * 100f) / 100f);

        // Convert the JSON object to a string
        string jsonString = JsonUtility.ToJson(jsonData);

        Debug.Log(jsonString);

        using (UnityWebRequest req = new UnityWebRequest(StatsAPIUri, "POST"))
        {
            byte[] jsonToSend = System.Text.Encoding.UTF8.GetBytes(jsonString);
            req.uploadHandler = new UploadHandlerRaw(jsonToSend);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.ConnectionError || req.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(req.error);
                StatsOnline = false;
                CancelInvoke("SendStats");
            }
            else
            {
                Debug.Log("Stats sent successfully");
            }
        }
    }

    public void UpdatePosition(Vector3 newPosition)
    {
        // Debug.Log($"Camera position updated to: {newPosition}");
        // Add your Camera position update logic here
        var heightToSave = newPosition.y;

        heightToShow = (heightToSave + 1.012f)*10;

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

public class StatsServiceState
{
    public string state = "";
}

[System.Serializable]
public class StatsData
{
    public int game_id;
    public float height;

    public StatsData(int gameId, float height)
    {
        this.game_id = gameId;
        this.height = height;
    }
}