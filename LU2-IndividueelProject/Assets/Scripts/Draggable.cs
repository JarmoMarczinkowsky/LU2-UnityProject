using System;
using UnityEngine;

/*
* The GameObject also needs a collider otherwise OnMouseUpAsButton() can not be detected.
*/
public class Draggable: MonoBehaviour
{
    public ObjectManager objectManager;

    private string _objectId;
    public string objectId
    {
        get => _objectId;
        set
        {
            _objectId = value;
            this.name = value;
        }
    }

    private string _prefabId;
    public string prefabId
    {
        get => _prefabId;
        set
        {
            _prefabId = value;
            this.name = value;
        }
    }

    public bool isDragging = false;

    public void Update()
    {
        if (isDragging)
            this.transform.position = GetMousePosition();
    }

    private void OnMouseUpAsButton()
    {
        isDragging = !isDragging;

        Vector3 mousePos = GetMousePosition();
        Debug.Log($"Mouse position: {mousePos.x}, {mousePos.y}");

        if (!isDragging)
        {
            if (objectManager != null)
            {
                objectManager.ShowMenu();

                //if (mousePos.x < 0 || mousePos.x > 200)
                //{
                //    Debug.Log($"Mouse position (x: {mousePos.x}) out of bounds");
                //    return;
                //}

                //if (mousePos.y < 0 || mousePos.y > 100)
                //{
                //    Debug.Log($"Mouse position (y: {mousePos.y}) out of bounds");
                //    return;
                //}

                SaveObjectToDatabase();
            }
            else
            {
                Debug.Log("Updating item");

                Object2D henk = new Object2D()
                {
                    id = _objectId,
                    environmentId = ScriptGameState.chosenEnvironment.id,
                    prefabId = _prefabId,
                    positionX = (float)Math.Round(this.transform.localPosition.x, 4),
                    positionY = (float)Math.Round(this.transform.localPosition.y, 4),
                    rotationZ = (float)Math.Round(this.transform.localRotation.eulerAngles.z, 4),
                    sortingLayer = this.gameObject.GetComponent<SortingLayer>().value,
                    scaleX = (float)Math.Round(this.transform.localScale.x, 4),
                    scaleY = (float)Math.Round(this.transform.localScale.y, 4)
                };

                
            }
        }
    }



    private async void UpdateDatabase(Object2D saveThisObject)
    {
        //await objectManager.object2DApiClient.UpdateObject2D(saveThisObject);
        //Debug.Log($"Updated object {saveThisObject.id}");
    }

    private void SaveObjectToDatabase()
    {
        //Transform objectTransform = this.transform;
        //if (objectTransform.localPosition.x < 0 || objectTransform.localPosition.x > ScriptGameState.chosenEnvironment.maxLength)
        //{
        //    Debug.Log($"Object position out of bounds (x: {objectTransform.localPosition.x})");
        //    return;
        //}

        //if (objectTransform.localPosition.y < 0 || objectTransform.localPosition.y > ScriptGameState.chosenEnvironment.maxHeight)
        //{
        //    Debug.Log($"Object position out of bounds (y: {objectTransform.localPosition.y})");
        //    return;
        //}

        objectManager.PrepareObjectForDatabase(this.transform);
    }

    private Vector3 GetMousePosition()
    {
        Vector3 positionInWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        positionInWorld.z = 0;
        return positionInWorld;
    }
}
