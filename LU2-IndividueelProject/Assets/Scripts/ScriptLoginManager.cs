using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScriptLoginManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_InputField FieldUsername;
    public TMP_InputField FieldPassword;
    public TMP_Text txbErrorMessage;

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
        if (Input.GetKeyUp(KeyCode.KeypadEnter))
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
        if(FieldUsername.text == "" || FieldPassword.text == "")
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
                SceneManager.LoadScene("SceneMenuWorld");
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

    public void QuitGame()
    {
        Application.Quit();
    }
}
