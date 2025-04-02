using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using UnityEngine.Networking;
using System.Collections;

public class GamesForIdPlayer : MonoBehaviour
{
    [Header("Input Fields")]
    [SerializeField] private Button firstGameIdInputField;
    [SerializeField] private Button secondGameIdInputField;
    [SerializeField] private Button thirdGameIdInputField;

    [Header("Settings")]
    private string gamesForIdEndpoint = "http://localhost:4000/api/games/user/";
    private string userId;

    private void Start()
    {
        userId = UserDataHolder.Instance.CurrentUser.id;
        StartCoroutine(FetchGamesData());
    }

    private IEnumerator FetchGamesData()
    {
        Debug.Log($"Fetching games data for user ID: {userId}");
        Debug.Log($"Endpoint: {gamesForIdEndpoint + userId}");
        using (UnityWebRequest webRequest = UnityWebRequest.Get(gamesForIdEndpoint + userId))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error: {webRequest.error}");
                yield break;
            }

            string jsonText = webRequest.downloadHandler.text;
            Debug.Log($"JSON received: {jsonText}");
        }
    }
}