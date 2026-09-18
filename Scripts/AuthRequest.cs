using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using TMPro;

public class AuthRequest : MonoBehaviour
{
    [SerializeField] private string serverUrl = "http://172.22.41.3:3000/api/auth/signin";
    public TMP_InputField EmailInput;
    public TMP_InputField PasswordInput;
    public TMP_Text res;
    public UserData UD;
    private LevelLoader ll;

    private void Awake()
    {
        ll = GetComponent<LevelLoader>();
    }


    public void SendLoginRequest()
    {
        string email = EmailInput.text;
        if (email == "") email = null;
        string password = PasswordInput.text;
        if (password == "") password = null;
        StartCoroutine(LoginCoroutine(email, password));
    }

    private IEnumerator LoginCoroutine(string email, string password)
    {
        var loginData = new LoginData { email = email, password = password };
        string jsonBody = JsonUtility.ToJson(loginData);
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
                Debug.Log("Успешный вход");
                res.text = "ОК";
                UD.responseText = responseText;
                ll.LoadLevel(3);
                break;

            case 400:
                Debug.LogWarning("Не все поля заполнены");
                res.text = "Не все поля заполнены";
                break;

            case 401:
                Debug.LogWarning("Неверный email или пароль");
                res.text = "Неверный email или пароль";
                break;

            default:
                Debug.LogError("Неизвестный статус: " + statusCode);
                res.text = "Неизвестный статус";
                break;
        }
    }

    [System.Serializable]
    private class LoginData
    {
        public string email;
        public string password;
    }

    [System.Serializable]
    private class StatusResponse
    {
        public int status;
    }
}