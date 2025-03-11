using System;
using UnityEngine;

/*
* The GameObject also needs a collider otherwise OnMouseUpAsButton() can not be detected.
*/
public class Draggable: MonoBehaviour
{
    public ObjectManager objectManager;

    public bool isDragging = false;

    public void Update()
    {
        if (isDragging)
            this.transform.position = GetMousePosition();
    }

    private void OnMouseUpAsButton()
    {
        isDragging = !isDragging;

        if (!isDragging)
        {
            objectManager.ShowMenu();

            SaveObjectToDatabase();
        }
    }

    private void SaveObjectToDatabase()
    {
        objectManager.PrepareObjectForDatabase(this.transform);
    }

    private Vector3 GetMousePosition()
    {
        Vector3 positionInWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        positionInWorld.z = 0;
        return positionInWorld;
    }
}
