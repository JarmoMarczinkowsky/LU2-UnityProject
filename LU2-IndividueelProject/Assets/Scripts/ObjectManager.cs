using System;
using System.Collections.Generic;
using System.Security.Claims;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.Progress;

public class ObjectManager : MonoBehaviour
{
    // Menu om objecten vanuit te plaatsen
    public GameObject UISideMenu;
    // Lijst met objecten die geplaatst kunnen worden die overeenkomen met de prefabs in de prefabs map
    public List<GameObject> prefabObjects;

    // Lijst met objecten die geplaatst zijn in de wereld
    private List<GameObject> placedObjects;
    private GameObject obj;
    private int lastCreatedObject;

    [Header("Dependencies")]
    public UserApiClient userApiClient;
    public Environment2DApiClient environment2DApiClient;
    public Object2DApiClient object2DApiClient;

    private void Start()
    {
        ReadObjectsByLevel();
    }

    private async void ReadObjectsByLevel()
    {
        IWebRequestReponse webRequestResponse = await object2DApiClient.ReadObject2Ds(ScriptGameState.chosenEnvironment.id);

        switch (webRequestResponse)
        {
            case WebRequestData<List<Object2D>> dataResponse:
                List<Object2D> object2Ds = dataResponse.Data;
                Debug.Log("List of object2Ds: " + object2Ds);
                object2Ds.ForEach(object2D => Debug.Log(object2D.id));
                
                // TODO: Succes scenario. Show the enviroments in the environment
                PlaceObjectsInLevel(object2Ds);

                break;
            case WebRequestError errorResponse:
                string errorMessage = errorResponse.ErrorMessage;
                Debug.Log("Read object2Ds error: " + errorMessage);
                // TODO: Error scenario. Show the errormessage to the user.
                break;
            default:
                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }
    }

    private void PlaceObjectsInLevel(List<Object2D> WorldObjects)
    {
        foreach (var item in WorldObjects)
        {
            Debug.Log($"Item {item.prefabId} wordt geplaatst");

            int prefabId = Convert.ToInt32(item.prefabId);


            if (prefabId >= 0 && prefabId <= 4)
            {
                obj = Instantiate(prefabObjects[prefabId]);
            }
            else
            {
                obj = null;
            }

            if (obj != null)
            {
                placeSingleObject(obj, item);
            }
        }
    }

    private void placeSingleObject(GameObject newlyPlacedObject, Object2D item)
    {
        newlyPlacedObject.transform.localPosition = new Vector2(item.positionX, item.positionY);
        newlyPlacedObject.transform.Rotate(0, 0, item.rotationZ);
    }

    // Methode om een nieuw 2D object te plaatsen
    public void PlaceNewObject2D(int index)
    {
        // Verberg het zijmenu
        UISideMenu.SetActive(false);
        
        // Instantieer het prefab object op de positie (0,0,0) zonder rotatie
        Debug.Log($"Going to instantiate prefab {index}");
        GameObject instanceOfPrefab = Instantiate(prefabObjects[index], Vector3.zero, Quaternion.identity);
        lastCreatedObject = index;
        Debug.Log($"Instantiated prefab {index}");

        // Haal het Object2D component op van het nieuw geplaatste object
        Draggable draggableObject = instanceOfPrefab.GetComponent<Draggable>();
        
        // Stel de objectManager van het object in op deze instantie van ObjectManager
        draggableObject.objectManager = this;
        
        // Zet de isDragging eigenschap van het object op true zodat het gesleept kan worden
        draggableObject.isDragging = true;
    }

    public void PrepareObjectForDatabase(Transform objectLocation)
    {
        Object2D objectInformation = new Object2D()
        {
            id = Convert.ToString(Guid.NewGuid()),
            prefabId = Convert.ToString(lastCreatedObject),
            environmentId = ScriptGameState.chosenEnvironment.id,
            positionX = (float)Math.Round(objectLocation.localPosition.x, 4),
            positionY = (float)Math.Round(objectLocation.localPosition.y, 4),
            rotationZ = (float)Math.Round(objectLocation.localPosition.z, 4),
            scaleX = (float)Math.Round(objectLocation.localScale.x, 4),
            scaleY = (float)Math.Round(objectLocation.localScale.y, 4),
            sortingLayer = 0
        };

        SaveObjectToDatabase(objectInformation);
    }

    private async void SaveObjectToDatabase(Object2D saveThisObject)
    {
        IWebRequestReponse webRequestResponse = await object2DApiClient.CreateObject2D(saveThisObject);

        switch (webRequestResponse)
        {
            case WebRequestData<Object2D> dataResponse:
                saveThisObject.id = dataResponse.Data.id;
                // TODO: Handle succes scenario.
                Debug.Log("Succesfully saved object");
                break;
            case WebRequestError errorResponse:
                string errorMessage = errorResponse.ErrorMessage;
                Debug.Log("Create Object2D error: " + errorMessage);
                // TODO: Handle error scenario. Show the errormessage to the user.
                break;
            default:
                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }
    }

    // Methode om het menu te tonen
    public void ShowMenu()
    {
        UISideMenu.SetActive(true);
    }

    // Methode om de huidige scène te resetten
    //public void Reset()
    //{
    //    // Laad de huidige scène opnieuw
    //    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    //}
}
