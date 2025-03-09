using System;
using System.Collections.Generic;
using System.Security;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScriptWorldManagement : MonoBehaviour
{
    [Header("Scripts")]
    public Environment2D environment;

    [Header("UI items")]
    public TMP_InputField FieldName;
    public TMP_InputField FieldLength;
    public TMP_InputField FieldHeight;
    public TMP_Text txbErrorMessage;

    public GameObject pnlNewWorld;
    public GameObject pnlLoadWorld;

    [Header("Dependencies")]
    public UserApiClient userApiClient;
    public Environment2DApiClient environment2DApiClient;
    public Object2DApiClient object2DApiClient;

    public static Environment2D environment2D;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        txbErrorMessage.text = "";
        pnlLoadWorld.SetActive(false);
        pnlNewWorld.SetActive(false);

        string getEnvironmentState = ScriptGameState.EnvironmentState;

        switch (getEnvironmentState)
        {
            case "New":
                pnlNewWorld.SetActive(true);
                break;
            case "Load":
                pnlLoadWorld.SetActive(true);
                LoadWorldsAssociatedByUser();
                break;
        }
    }

    private async void LoadWorldsAssociatedByUser()
    {
        IWebRequestReponse webRequestResponse = await environment2DApiClient.ReadEnvironment2Ds();

        switch (webRequestResponse)
        {
            case WebRequestData<List<Environment2D>> dataResponse:
                List<Environment2D> environment2Ds = dataResponse.Data;
                Debug.Log("List of environment2Ds: ");
                //environment2Ds.ForEach(environment2D => Debug.Log(environment2D.id));
                environment2Ds.ForEach(environment2D => Debug.Log(environment2D.name));

                // TODO: Handle succes scenario.
                break;
            case WebRequestError errorResponse:
                string errorMessage = errorResponse.ErrorMessage;
                Debug.Log("Read environment2Ds error: " + errorMessage);
                // TODO: Handle error scenario. Show the errormessage to the user.
                break;
            default:
                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }
    }

    }

    public void CreateWorld()
    {
        if (string.IsNullOrWhiteSpace(FieldName.text))
        {
            txbErrorMessage.text = "Voer een naam in";
            return;
        }

        string environmentName = FieldName.text.Trim();

        // Maak een nieuwe Environment2D instantie
        environment2D = new Environment2D
        {
            name = environmentName,
            maxLength = 123,
            maxHeight = 456
        };

        // Roep de CreateEnvironment2D methode aan
        CreateEnvironment2D();
    }

    public async void CreateEnvironment2D()
    {
        IWebRequestReponse webRequestResponse = await environment2DApiClient.CreateEnvironment(environment2D);

        switch (webRequestResponse)
        {
            case WebRequestData<Environment2D> dataResponse:
                environment2D = dataResponse.Data; // Update het lokale object
                Debug.Log($"Wereld '{environment2D.name}' aangemaakt met ID: {environment2D.id}");

                // Ga naar EnvironmentScene
                //SceneManager.LoadScene("EnvironmentScene");
                break;

            case WebRequestError errorResponse:
                string errorMessage = errorResponse.ErrorMessage;
                Debug.LogError("❌ Fout bij het maken van environment2D: " + errorMessage);
                break;

            default:
                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }
    }
}
