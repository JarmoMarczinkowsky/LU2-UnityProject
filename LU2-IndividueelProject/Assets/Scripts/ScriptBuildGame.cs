using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScriptBuildGame : MonoBehaviour
{
    public GameObject uiBuilderBar;
    public GameObject prefabSaloon;
    public Canvas theCanvas;

    [Header("Hideable menu items")]
    public GameObject menuSaloon;
    public GameObject pauseMenu;    

    private GameObject newSaloon;

    private bool isDragging = false;
    private Transform moveItem;

    public void ToggleBuilderBar()
    {
        uiBuilderBar.SetActive(!uiBuilderBar.activeSelf);
    }

    public void ClickTest(string name)
    {
        Debug.Log($"Clicked {name}");
        

        if(name == "Saloon")
        {
            newSaloon = Instantiate(prefabSaloon);
            newSaloon.transform.SetParent(theCanvas.transform);
            newSaloon.transform.localScale = new Vector3(20, 20, 0);
            moveItem = newSaloon.transform;
        }
    }

    public void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            pauseMenu.SetActive(!pauseMenu.activeSelf);
        }

        if (Input.GetMouseButtonDown(0))
        {
            isDragging = !isDragging;

            if(isDragging)
            {
                Debug.Log("Is dragging");
            }
            else
            {
                Debug.Log("Is not dragging");
            }
        }

        if (isDragging)
        {
            if(moveItem != null) moveItem.position = GetMousePosition();

        }
        else if (!isDragging)
        {
            moveItem = null;
        }
    }

    public void ChangeSaloonColor()
    {
        menuSaloon.GetComponent<SpriteRenderer>().color = Color.gray;
    }

    public void ResetSaloonColor()
    {
        menuSaloon.GetComponent<SpriteRenderer>().color = Color.white;
    }

    private Vector3 GetMousePosition()
    {
        Vector3 positionInWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        positionInWorld.z = 0;
        return positionInWorld;
    }
}
