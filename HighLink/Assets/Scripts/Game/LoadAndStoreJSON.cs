using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;

public class LoadAndStoreJSON : MonoBehaviour
{
    [SerializeField] private string jsonURL = "http://localhost:4000/api/config";
    [SerializeField] public static JSONclass datosJSON;

    [System.Serializable]
    public class JSONclass
    {
        public string name;
        public string value;
        public string type;
    }

    void Start()
    {
        StartCoroutine(GetJSONData(jsonURL));
    }

    IEnumerator GetJSONData(string jsonURL)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(jsonURL))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error al descargar JSON: " + request.error);
            }
            else
            {
                string jsonText = request.downloadHandler.text;
                Debug.Log("JSON Descargado: " + jsonText);

                datosJSON = JsonUtility.FromJson<JSONclass>(jsonText);

                if (datosJSON != null)
                {
                    Debug.Log("Name: " + datosJSON.name);
                    Debug.Log("Value: " + datosJSON.value);
                    Debug.Log("Type: " + datosJSON.type);
                }
            }
        }

        // UnityWebRequest www = UnityWebRequest.Get(jsonURL);
        // yield return www.SendWebRequest();

        // if (www.result == UnityWebRequest.Result.ConnectionError ||
        //     www.result == UnityWebRequest.Result.ProtocolError)
        // {
        //     Debug.LogError("Error al descargar JSON: " + www.error);
        // }
        // else
        // {
        //     string jsonText = www.downloadHandler.text;
        //     Debug.Log("JSON Descargado: " + jsonText);

        //     datosJSON = JsonUtility.FromJson<JSONclass>(jsonText);

        //     if (datosJSON != null)
        //     {
        //         Debug.Log("Name: " + datosJSON.name);
        //         Debug.Log("Value: " + datosJSON.value);
        //         Debug.Log("Type: " + datosJSON.type);
        //     }
        // }
    }
}