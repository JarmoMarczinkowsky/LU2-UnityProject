using UnityEngine;

public class ScriptBuildGame : MonoBehaviour
{
    public GameObject uiBuilderBar;

    public void ToggleBuilderBar()
    {
        uiBuilderBar.SetActive(!uiBuilderBar.activeSelf);
    }

    public void ConfirmImageClick()
    {
        Debug.Log("Clicked image");
    }

}
