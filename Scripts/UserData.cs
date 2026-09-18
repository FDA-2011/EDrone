using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.Text;

public class UserData : MonoBehaviour
{
    public string responseText;

    private TMP_Text res;

    [SerializeField] private string serverUrl = "http://172.22.41.95:3000/api/gameround/create";

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void SendData(int challenge1, int challenge2, int challenge3, int challenge4, int time)
    {
        res = GameObject.Find("/Drone/Frame/Main Camera/Canvas/res").GetComponent<TMP_Text>();
        StartCoroutine(RequestCoroutine(responseText, challenge1, challenge2, challenge3, challenge4, time));
    }

    private IEnumerator RequestCoroutine(string rText, int challenge1, int challenge2, int challenge3, int challenge4, int time)
    {
        AuthResponse auth = JsonUtility.FromJson<AuthResponse>(rText);
        string id = auth.user.id;
        print(id);

        var gameroundData = new GameroundData { id = id, challenge1 = challenge1, challenge2 = challenge2, challenge3 = challenge3, challenge4 = challenge4, time = time };
        string jsonBody = JsonUtility.ToJson(gameroundData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        using (UnityWebRequest request = new UnityWebRequest(serverUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            string responseText = request.downloadHandler.text;
            Debug.Log("Ответ сервера: " + responseText);

            try
            {
                StatusResponse statusResponse = JsonUtility.FromJson<StatusResponse>(responseText);
                ProcessStatus(statusResponse.status, responseText);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Ошибка разбора JSON: " + e.Message);
            }
        }
    }

    private void ProcessStatus(int statusCode, string responseText)
    {
        switch (statusCode)
        {
            case 200:
                Debug.Log("Успешно");
                res.text = "Успешно";
                break;

            case 400:
                Debug.LogWarning("Ошибка тестирующей системы, попробуйте еще раз");
                res.text = "Ошибка тестирующей системы, попробуйте еще раз";
                break;

            case 500:
                Debug.LogWarning("Внутренняя ошибка сервера");
                res.text = "Внутренняя ошибка сервера";
                break;

            default:
                Debug.LogError("Неизвестный статус: " + statusCode);
                res.text = "Неизвестный статус";
                break;
        }
        OnDestroy();
    }   

    void OnDestroy()
    {
        Destroy(gameObject);
    }

    [System.Serializable]
    private class GameroundData
    {
        public string id;
        public int challenge1;
        public int challenge2;
        public int challenge3;
        public int challenge4;
        public int time;
    }

    [System.Serializable]
    private class AuthResponse
    {
        public bool success;
        public UserDataInfo user;
        public int status;
    }

    [System.Serializable]
    private class UserDataInfo
    {
        public string id;
    }

    [System.Serializable]
    private class StatusResponse
    {
        public int status;
    }
}
