using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScriptBuildGame : MonoBehaviour
{
    public GameObject uiBuilderBar;
    public GameObject prefabSaloon;
    public Canvas theCanvas;

    public TMP_Text txtStatus;

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
        if (isDragging) return;

        Debug.Log($"Clicked {name}");
 
        if(name == "Saloon")
        {
            newSaloon = Instantiate(prefabSaloon);
            newSaloon.transform.SetParent(theCanvas.transform);
            newSaloon.transform.localScale = new Vector3(20, 20, 0);
            moveItem = newSaloon.transform;

            isDragging = true;
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
            Debug.Log("Mouse clicked");

            //if (moveItem == null) return;
            //isDragging = !isDragging;
            if(isDragging)
            {
                Debug.Log("Stopped dragging");
                isDragging = false;
                moveItem = null;
            }


            txtStatus.text = isDragging ? "Is dragging" : "Is not dragging";
        }

        if (isDragging && moveItem != null)
        {
            //Debug.Log($"MoveItem niet null");
            moveItem.position = GetMousePosition();
            Debug.Log("Dragging item");
        }
        //else if (!isDragging)
        //{
        //    //Debug.Log($"MoveItem is null");
        //    moveItem = null;
        //}
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
