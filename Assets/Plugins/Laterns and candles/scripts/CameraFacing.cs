using UnityEngine;

/// <summary>
/// Allows specified orientation axis. <br/>
/// By: Neil Carter (NCarter), Hayden Scott-Baron (Dock) - http://starfruitgames.com
/// </summary>
public class CameraFacing : MonoBehaviour
{
    public Camera cameraToLookAt;

    private void Awake()
    {
        cameraToLookAt = FindFirstObjectByType<Camera>();
    }
    
    private void Update()
    {
        Vector3 v = cameraToLookAt.transform.position - transform.position;
        v.x = v.z = 0.0f;
        transform.LookAt(cameraToLookAt.transform.position - v);
    }
}