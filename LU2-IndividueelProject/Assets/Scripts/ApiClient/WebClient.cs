using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class WebClient : MonoBehaviour
{
    public string baseUrl;
    private string token;

    public void SetToken(string token)
    {
        this.token = token;
        if (!string.IsNullOrEmpty(token))
        {
            PlayerPrefs.SetString("authToken", token);
            PlayerPrefs.Save();
            Debug.Log("✅ Token opgeslagen in PlayerPrefs: " + token);
        }
        else
        {
            PlayerPrefs.DeleteKey("authToken");
            Debug.LogWarning("⚠️ Token is verwijderd.");
        }
    }

    public string GetToken()
    {
        if (string.IsNullOrEmpty(token))
        {
            token = PlayerPrefs.GetString("authToken", "");
        }

        Debug.Log("🔍 Token opgehaald: " + token);
        return token;
    }

    private void AddToken(UnityWebRequest webRequest)
    {
        string token = GetToken();

        if (!string.IsNullOrEmpty(token))
        {
            string authHeader = "Bearer " + token;
            webRequest.SetRequestHeader("Authorization", authHeader);
            Debug.Log("🛡️ Authorization header toegevoegd: " + authHeader);
        }
        else
        {
            Debug.LogWarning("⚠️ Geen token beschikbaar voor authenticatie!");
        }
    }

    public async Awaitable<IWebRequestReponse> SendGetRequest(string route)
    {
        UnityWebRequest webRequest = CreateWebRequest("GET", route, "");
        return await SendWebRequest(webRequest);
    }

    public async Awaitable<IWebRequestReponse> SendPostRequest(string route, string data)
    {
        UnityWebRequest webRequest = CreateWebRequest("POST", route, data);
        return await SendWebRequest(webRequest);
    }

    public async Awaitable<IWebRequestReponse> SendPutRequest(string route, string data)
    {
        UnityWebRequest webRequest = CreateWebRequest("PUT", route, data);
        return await SendWebRequest(webRequest);
    }

    public async Awaitable<IWebRequestReponse> SendDeleteRequest(string route)
    {
        UnityWebRequest webRequest = CreateWebRequest("DELETE", route, "");
        return await SendWebRequest(webRequest);
    }

    private UnityWebRequest CreateWebRequest(string type, string route, string data)
    {
        string url = baseUrl + route;
        Debug.Log("Creating " + type + " request to " + url + " with data: " + data);

        data = RemoveIdFromJson(data); // Backend throws error if it receiving empty strings as a GUID value.
        var webRequest = new UnityWebRequest(url, type);
        byte[] dataInBytes = new UTF8Encoding().GetBytes(data);
        webRequest.uploadHandler = new UploadHandlerRaw(dataInBytes);
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");
        AddToken(webRequest);
        return webRequest;
    }

    private async Awaitable<IWebRequestReponse> SendWebRequest(UnityWebRequest webRequest)
    {
        await webRequest.SendWebRequest();

        //Debug.Log("hit webclient -> sendwebrequest");
        
        switch (webRequest.result)
        {
            case UnityWebRequest.Result.Success:
                string responseData = webRequest.downloadHandler.text;
                return new WebRequestData<string>(responseData);
            default:
                return new WebRequestError(webRequest.error);
        }
    }
 
    //private void AddToken(UnityWebRequest webRequest)
    //{
    //    webRequest.SetRequestHeader("Authorization", "Bearer " + token);
    //}

    private string RemoveIdFromJson(string json)
    {
        return json.Replace("\"id\":\"\",", "");
    }

}

[Serializable]
public class Token
{
    public string tokenType;
    public string accessToken;
}
