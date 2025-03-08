using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScriptLoginManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_InputField FieldUsername;
    public TMP_InputField FieldPassword;
    public TMP_Text txbErrorMessage;
    //public TMP_Text txbMenuLoadWorld;
    //public TMP_Text txbMenuNewWorld;
    //public TMP_Text txbMenuLeaveGame;
    public List<TMP_Text> lstMenuText;

    public GameObject pnlLogin;
    public GameObject pnlStartGame;

    [Header("Dependencies")]
    public UserApiClient userApiClient;
    public Environment2DApiClient enviroment2DApiClient;
    public Object2DApiClient object2DApiClient;

    private void Start()
    {
        txbErrorMessage.text = "";        
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            Login();
        }

        if(Input.GetKeyUp(KeyCode.Tab))
        {
            if(FieldUsername.isFocused)
            {
                FieldPassword.Select();
                return;
            }
            else if (FieldPassword.isFocused)
            {
                FieldUsername.Select();
                return;
            }
        }
    }

    public async void Login()
    {
        if (FieldUsername.text == "" || FieldPassword.text == "")
        {
            txbErrorMessage.text = "Vul alle velden in";
            return;
        }

        User user = new User()
        {
            email = FieldUsername.text.ToLower().Trim(),
            password = FieldPassword.text
        };

        IWebRequestReponse webRequestResponse = await userApiClient.Login(user);

        switch (webRequestResponse)
        {
            case WebRequestData<string> dataResponse:
                Debug.Log("Login succes!");
                // TODO: Todo handle succes scenario.

                string token = dataResponse.Data;
                userApiClient.webClient.SetToken(token);
                pnlLogin.SetActive(false);
                pnlStartGame.SetActive(true);

                
                break;
            case WebRequestError errorResponse:
                string errorMessage = errorResponse.ErrorMessage;
                Debug.Log("Login error: " + errorMessage);
                txbErrorMessage.text = "Wachtwoord of gebruikersnaam is onjuist";
                // TODO: Handle error scenario. Show the errormessage to the user.
                break;
            default:
                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }

        //SceneManager.LoadScene("SceneMenuWorld");
    }

    public async void Register()
    {
        if (FieldUsername.text == "" || FieldPassword.text == "")
        {
            txbErrorMessage.text = "Vul alle velden in";
            return;
        }

        if (FieldPassword.text.Count() < 10 
            || !FieldPassword.text.Any(char.IsUpper) 
            || !FieldPassword.text.Any(char.IsLower) 
            || !FieldPassword.text.Any(char.IsDigit) 
            || !FieldPassword.text.Any(char.IsLetterOrDigit))
        {
            Debug.Log($"Langer dan 10 characters: {!(FieldPassword.text.Count() < 10)}");
            Debug.Log($"Een character heeft hoofdletter: {FieldPassword.text.Any(char.IsUpper)}");
            Debug.Log($"Een character heeft kleine letter: {FieldPassword.text.Any(char.IsLower)}");
            Debug.Log($"Een character is cijfer: {FieldPassword.text.Any(char.IsDigit)}");
            Debug.Log($"Een character is symbool: {FieldPassword.text.Any(char.IsLetterOrDigit)}");
            Debug.Log("--------");

            txbErrorMessage.text = $"Zorg ervoor dat je wachtwoord aan de komende eisen voldoet: " +
                $"{Environment.NewLine}Minimaal 10 characters" +
                $"{Environment.NewLine}Mininmaal één hoofd- en kleine letter" +
                $"{Environment.NewLine}Minimaal één cijfer" +
                $"{Environment.NewLine}Minimaal 1 niet alphanumeric character";
            return;
        }

        User user = new User()
        {
            email = FieldUsername.text.ToLower().Trim(),
            password = FieldPassword.text
        };

        IWebRequestReponse webRequestResponse = await userApiClient.Register(user);

        switch (webRequestResponse)
        {
            case WebRequestData<string> dataResponse:
                Debug.Log("Register succes!");
                txbErrorMessage.text = "Registratie voltooid";
                FieldUsername.text = "";
                FieldPassword.text = "";
                // TODO: Handle succes scenario;
                break;
            case WebRequestError errorResponse:
                string errorMessage = errorResponse.ErrorMessage;
                Debug.Log("Register error: " + errorMessage);
                txbErrorMessage.text = "Registratie gefaald, check of je gebruikersnaam al bestaat en probeer het opnieuw";
                // TODO: Handle error scenario. Show the errormessage to the user.
                break;
            default:
                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }
    }

    public void MenuTextPointerEnter(int index)
    {
        Debug.Log($"Cursor entered menu item: {index}");
        HoverText(lstMenuText[index]);
    }

    public void MenuTextPointerExit(int index)
    {
        Debug.Log($"Cursor exited menu item: {index}");
        ResetText(lstMenuText[index]);
    }

    public void HoverText(TMP_Text text)
    {
        text.GetComponent<TMP_Text>().color = Color.red;
        text.GetComponent<TMP_Text>().fontStyle = FontStyles.Underline;
    }

    public void ResetText(TMP_Text text)
    {
        text.GetComponent<TMP_Text>().color = Color.white;
        text.GetComponent<TMP_Text>().fontStyle = FontStyles.Normal;
    }

    public void CreateNewWorld()
    {
        SceneManager.LoadScene("SceneMenuWorld");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
