using UnityEngine;

public class CameraManger : MonoBehaviour
{

    private void OnEnable()
    {
        GameManager.SetMainCamera(this);
    }

    private void OnDisable()
    {
        GameManager.SetMainCamera(null);
    }

    private void OnDestroy()
    {
        GameManager.SetMainCamera(null);
    }
}
