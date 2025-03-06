using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScriptLoginManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_InputField UsernameField;
    public TMP_InputField PasswordField;

    [Header("Dependencies")]
    public UserApiClient userApiClient;
    public Environment2DApiClient enviroment2DApiClient;
    public Object2DApiClient object2DApiClient;

    
    public async void StartGame()
    {
        User user = new User()
        {
            email = UsernameField.text.ToLower().Trim(),
            password = PasswordField.text
        };

        IWebRequestReponse webRequestResponse = await userApiClient.Login(user);

        switch (webRequestResponse)
        {
            case WebRequestData<string> dataResponse:
                Debug.Log("Login succes!");
                // TODO: Todo handle succes scenario.
                break;
            case WebRequestError errorResponse:
                string errorMessage = errorResponse.ErrorMessage;
                Debug.Log("Login error: " + errorMessage);
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
