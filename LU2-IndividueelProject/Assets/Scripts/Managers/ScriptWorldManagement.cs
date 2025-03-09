using System;
using System.Security;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    private Environment2D environment2D;

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
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
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

    [ContextMenu("Environment2D/Create")]
    public async void CreateEnvironment2D()
    {
        IWebRequestReponse webRequestResponse = await environment2DApiClient.CreateEnvironment(environment2D);

        switch (webRequestResponse)
        {
            case WebRequestData<Environment2D> dataResponse:
                environment2D = dataResponse.Data; // Update het lokale object
                Debug.Log("✅ Environment2D aangemaakt met ID: " + environment2D.id);

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
