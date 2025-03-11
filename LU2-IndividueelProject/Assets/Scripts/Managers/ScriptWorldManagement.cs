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
    public TMP_Text txbLoadLevelError;
    public List<Button> lstLoadGameButtons;

    public GameObject pnlNewWorld;
    public GameObject pnlLoadWorld;

    [Header("Dependencies")]
    public UserApiClient userApiClient;
    public Environment2DApiClient environment2DApiClient;
    public Object2DApiClient object2DApiClient;

    public static Environment2D environment2D;

    private List<Environment2D> userEnvironments;

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
                userEnvironments = dataResponse.Data;
                Debug.Log("List of environment2Ds: ");
                //environment2Ds.ForEach(environment2D => Debug.Log(environment2D.id));
                userEnvironments.ForEach(environment2D => Debug.Log(environment2D.name));

                PlaceLevelsInButtons();
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

    private void PlaceLevelsInButtons()
    {
        int maxLoopValue;

        Debug.Log($"Amount of worlds found: {userEnvironments.Count}");

        if(userEnvironments.Count > 5)
        {
            maxLoopValue = 5;
        }
        else
        {
            maxLoopValue = userEnvironments.Count;
        }

        for (int i = 0; i < maxLoopValue; i++)
        {
            Debug.Log($"Loop {i}= {userEnvironments[i].name}");

            if (lstLoadGameButtons[i].GetComponentInChildren<TMP_Text>() == null || userEnvironments[i].name == null)
            {
                Debug.Log($"Found no text to change at {i}");
                continue;
            }

            lstLoadGameButtons[i].gameObject.SetActive(true);
            lstLoadGameButtons[i].GetComponentInChildren<TMP_Text>().text = userEnvironments[i].name;
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
                Debug.LogError($"Er is een fout opgetreden bij het aanmaken van een wereld: {errorMessage}");
                break;

            default:
                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }
    }

    public void LoadLevelOnClick(int buttonValue)
    {
        if(buttonValue > userEnvironments.Count - 1)
        {
            txbLoadLevelError.text = "I dont know how you did this, but it should not be possible";
            return;
        }

        Debug.Log($"Clicked on button {buttonValue} that has name {userEnvironments[buttonValue].name}");

    }


}
