using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Environment2DApiClient : MonoBehaviour
{
    public WebClient webClient;

    public async Awaitable<IWebRequestReponse> ReadEnvironment2Ds() 
    {
        string route = "/environment2d";

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequest(route);
        return ParseEnvironment2DListResponse(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> CreateEnvironment(Environment2D environment)
    {
        string route = "/environment2d";
        string data = JsonUtility.ToJson(environment);

        Debug.Log($"create environment; Creating POST request to {webClient.baseUrl}{route} with data {data}");

        IWebRequestReponse webRequestResponse = await webClient.SendPostRequest(route, data);

        if(webRequestResponse is WebRequestError requestError)
        {
            Debug.Log($"Create environment error: code={requestError.ErrorCode}. Message={requestError.ErrorMessage}");
        }

        return ParseEnvironment2DResponse(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> DeleteEnvironment(string environmentId)
    {
        string route = "/environment2d/" + environmentId;
        return await webClient.SendDeleteRequest(route);
    }

    private IWebRequestReponse ParseEnvironment2DResponse(IWebRequestReponse webRequestResponse)
    {
        Debug.Log("Here");
        switch (webRequestResponse)
        {
            case WebRequestData<string> data:
                Debug.Log("Response data raw: " + data.Data);
                Environment2D environment = JsonUtility.FromJson<Environment2D>(data.Data);
                WebRequestData<Environment2D> parsedWebRequestData = new WebRequestData<Environment2D>(environment);
                return parsedWebRequestData;
            default:
                return webRequestResponse;
        }
    }

    private IWebRequestReponse ParseEnvironment2DListResponse(IWebRequestReponse webRequestResponse)
    {
        switch (webRequestResponse)
        {
            case WebRequestData<string> data:
                Debug.Log("Response data raw: " + data.Data);
                List<Environment2D> environment2Ds = JsonHelper.ParseJsonArray<Environment2D>(data.Data);
                WebRequestData<List<Environment2D>> parsedWebRequestData = new WebRequestData<List<Environment2D>>(environment2Ds);
                return parsedWebRequestData;
            default:
                return webRequestResponse;
        }
    }

}

