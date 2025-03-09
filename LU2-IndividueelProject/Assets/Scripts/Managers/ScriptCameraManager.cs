using UnityEngine;

public class ScriptCameraManager : MonoBehaviour
{
    public float cameraSpeed = 1;
    private void FixedUpdate()
    {
        //Camera movement based on wasd keys
        if(Input.GetAxisRaw("Horizontal") > 0)
        {
            transform.position += new Vector3(0.1f * cameraSpeed, 0, 0);
        }
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            transform.position += new Vector3(-0.1f * cameraSpeed, 0, 0);
        }

        if(Input.GetAxisRaw("Vertical") > 0)
        {
            transform.position += new Vector3(0, 0.1f * cameraSpeed, 0);
        }
        else if (Input.GetAxisRaw("Vertical") < 0)
        {
            transform.position += new Vector3(0, -0.1f * cameraSpeed, 0);
        }
    }
}
