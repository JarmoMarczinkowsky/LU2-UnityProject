﻿using System;
using System.Collections.Generic;
using System.Linq;
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
                txbLoadLevelError.text = "Er is een error opgetreden tijdens het laden van levels";
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
        if (!ValidateNameField())
        {
            return;
        }

        if(!ValidateLengthAndHeight())
        {
            return;
        }

        int length = int.Parse(FieldLength.text.Trim());
        int height = int.Parse(FieldHeight.text.Trim());

        string environmentName = FieldName.text.Trim();

        // Maak een nieuwe Environment2D instantie
        environment2D = new Environment2D
        {
            name = environmentName,
            maxLength = length,
            maxHeight = height
        };

        // Roep de CreateEnvironment2D methode aan
        CreateEnvironment2D();
    }

    private bool ValidateNameField()
    {
        if (string.IsNullOrWhiteSpace(FieldName.text))
        {
            txbErrorMessage.text = "Voer een naam in";
            return false;
        }

        if (FieldName.text.Trim().Length < 1 || FieldName.text.Trim().Length > 25)
        {
            txbErrorMessage.text = "Naam moet tussen de 1 en 25 karakters zijn";
            return false;
        }

        return true;
    }

    private bool ValidateLengthAndHeight()
    {
        if (string.IsNullOrWhiteSpace(FieldLength.text) || string.IsNullOrWhiteSpace(FieldHeight.text))
        {
            txbErrorMessage.text = "Voer een lengte en hoogte in";
            return false;
        }

        if (!int.TryParse(FieldLength.text, out int length) || !int.TryParse(FieldHeight.text, out int height))
        {
            txbErrorMessage.text = "Voer een geldige lengte en hoogte in";
            return false;
        }

        if (length < 20 || length > 200)
        {
            txbErrorMessage.text = "Lengte moet tussen de 20 en 200 zijn";
            return false;

        }

        if (height < 10 || height > 100)
        {
            txbErrorMessage.text = "Hoogte moet tussen de 10 en 100 zijn";
            return false;

        }

        return true;
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
                ScriptGameState.chosenEnvironment = environment2D;
                SceneManager.LoadScene("SceneBuilder");
                break;

            case WebRequestError errorResponse:
                string errorMessage = errorResponse.ErrorMessage;
                Debug.LogError($"Er is een fout opgetreden bij het aanmaken van een wereld: {errorMessage}");
                txbErrorMessage.text = "Er is een fout opgetreden bij \nhet aanmaken van een wereld";
                break;

            default:
                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }
    }

    public void SetWorldToLoad(int buttonValue)
    {
        if(buttonValue > userEnvironments.Count - 1)
        {
            txbLoadLevelError.text = "I dont know how you did this, but it should not be possible";
            return;
        }

        //Debug.Log($"Value of button = {buttonValue}");
        string textInButton = lstLoadGameButtons[buttonValue].GetComponentInChildren<TMP_Text>().text;

        var chosenEnvironment = userEnvironments.FirstOrDefault(env=> env.name.ToLower() == textInButton.ToLower());
        Debug.Log($"Clicked on button {buttonValue} that has name {chosenEnvironment.name}");

        for (int i = 0; i < lstLoadGameButtons.Count; i++)
        {
            if (i ==  buttonValue)
            {
                lstLoadGameButtons[i].GetComponentInChildren<TMP_Text>().color = Color.green;
            }
            else
            {
                lstLoadGameButtons[i].GetComponentInChildren<TMP_Text>().color = Color.black;
            }
        }

        ScriptGameState.chosenEnvironment = chosenEnvironment;
        //SceneManager.LoadScene("SceneBuilder");
    }

    public void LoadLevelOnClick()
    {
        if (ScriptGameState.chosenEnvironment != null)
        {
            Debug.Log($"Going to load level {ScriptGameState.chosenEnvironment.name}");
            SceneManager.LoadScene("SceneBuilder");
            Debug.Log($"Loaded level {ScriptGameState.chosenEnvironment.name}");
        }
    }

    public void DeleteLevelOnClick()
    {
        DeleteEnvironment2D();
    }

    private async void DeleteEnvironment2D()
    {
        IWebRequestReponse webRequestResponse = await environment2DApiClient.DeleteEnvironment(ScriptGameState.chosenEnvironment.id);

        switch (webRequestResponse)
        {
            case WebRequestData<string> dataResponse:
                string responseData = dataResponse.Data;
                // TODO: Handle succes scenario.
                
                foreach (var buttons in lstLoadGameButtons)
                {
                    if (buttons.GetComponentInChildren<TMP_Text>().text == ScriptGameState.chosenEnvironment.name)
                    {
                        buttons.gameObject.SetActive(false);
                        break;
                    }
                }

                userEnvironments.RemoveAll(env => env.id == ScriptGameState.chosenEnvironment.id);
                ScriptGameState.chosenEnvironment = null;
                Debug.Log("De wereld is succesvol verwijderd");


                break;
            case WebRequestError errorResponse:
                string errorMessage = errorResponse.ErrorMessage;
                Debug.Log("Delete environment error: " + errorMessage);
                // TODO: Handle error scenario. Show the errormessage to the user.
                txbLoadLevelError.text = "Kon wereld niet verwijderen";
                break;
            default:
                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }
    }

    public void BackToLogin()
    {
        SceneManager.LoadScene("SceneMenuLogin");
    }

}
