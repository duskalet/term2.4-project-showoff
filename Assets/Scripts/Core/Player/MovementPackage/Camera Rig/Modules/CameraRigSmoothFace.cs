using UnityEngine;

public class CameraRigSmoothFace : CameraRigModule
{
    [SerializeField] Transform forwardHelper;

    private void LateUpdate()
    {
       transform.LookAt(forwardHelper);
    }
}
