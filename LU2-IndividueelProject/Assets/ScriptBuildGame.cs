using UnityEngine;

public class ScriptBuildGame : MonoBehaviour
{
    public GameObject uiBuilderBar;

    public void ToggleBuilderBar()
    {
        uiBuilderBar.SetActive(!uiBuilderBar.activeSelf);
    }

}
