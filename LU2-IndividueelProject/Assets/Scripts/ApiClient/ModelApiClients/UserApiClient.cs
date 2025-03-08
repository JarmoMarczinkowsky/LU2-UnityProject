using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class UserApiClient : MonoBehaviour
{
    public WebClient webClient;

    public async Awaitable<IWebRequestReponse> Register(User user)
    {
        string route = "/account/register";
        string data = JsonUtility.ToJson(user);

        return await webClient.SendPostRequest(route, data);
    }

    public async Awaitable<IWebRequestReponse> Login(User user)
    {
        string route = "/account/login";
        string data = JsonUtility.ToJson(user);

        IWebRequestReponse response = await webClient.SendPostRequest(route, data);
        return ProcessLoginResponse(response);
    }

    //private IWebRequestReponse ProcessLoginResponse(IWebRequestReponse webRequestResponse)
    //{
    //    switch (webRequestResponse)
    //    {
    //        case WebRequestData<string> data:
    //            Debug.Log("Response data raw: " + data.Data);
    //            string token = JsonHelper.ExtractToken(data.Data);
    //            webClient.SetToken(token);
    //            return new WebRequestData<string>("Succes");
    //        default:
    //            return webRequestResponse;
    //    }
    //}

    private IWebRequestReponse ProcessLoginResponse(IWebRequestReponse webRequestResponse)
    {
        if (webRequestResponse is WebRequestData<string> dataResponse)
        {
            Debug.Log("Response data raw: " + dataResponse.Data);

            try
            {
                // Parse de JSON-response en haal alleen de accessToken eruit
                Token tokenObject = JsonUtility.FromJson<Token>(dataResponse.Data);

                if (!string.IsNullOrEmpty(tokenObject.accessToken))
                {
                    Debug.Log("✅ Token ontvangen en opgeslagen: " + tokenObject.accessToken);

                    // Token correct opslaan
                    webClient.SetToken(tokenObject.accessToken);

                    return new WebRequestData<string>(tokenObject.accessToken); // Hier moet je de token terugsturen
                }
                else
                {
                    Debug.LogError("⚠️ Geen geldig token ontvangen!");
                    return new WebRequestError("Geen geldig token ontvangen!", 500);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("❌ Fout bij het verwerken van de login response: " + e.Message);
                return new WebRequestError("Ongeldig antwoord van server", 500);
            }
        }

        return webRequestResponse;
    }

}

