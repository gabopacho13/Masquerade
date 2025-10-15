using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CameraManager : MonoBehaviour
{
    private static List<Camera> cameras = new();
    private LayerMask cameraLayerMask;
    private static readonly CameraManager instance;

    private void Start()
    {
        if (instance != null)
        {
            Debug.LogWarning("Multiple instances of CameraManager detected. Destroying the new instance.");
            Destroy(gameObject);
            return;
        }
        cameraLayerMask = LayerMask.NameToLayer("Cameras");
        cameras = FindObjectsByType<GameObject>(FindObjectsSortMode.None)
            .Where(obj => obj.layer == cameraLayerMask)
            .Select(obj => obj.GetComponent<Camera>())
            .Where(cam => cam != null)
            .ToList();
        Camera mainCamera = null;
        for (int i = 0; i < cameras.Count; i++)
        {
            cameras[i].depth = i;
            if (cameras[i].CompareTag("MainCamera"))
            {
                mainCamera = cameras[i];
            }
            if (i == cameras.Count - 1)
            {
                cameras[i].GetComponent<AudioListener>().enabled = true;
            }
            else
            {
                cameras[i].GetComponent<AudioListener>().enabled = false;
            }
        }
        ChangeToCamera(mainCamera.name);
    }

    private void Update()
    {
        GameObject activeCamera = GetActiveCamera().gameObject;
        if (activeCamera != null && (activeCamera.name == "IntroMaskCamera" || activeCamera.name == "IntroVillagerCamera"))
        {
            activeCamera.GetComponent<IntroCameraBehaviour>().IsActive = true;
        }
    }

    public static void ChangeToCamera(string cameraName)
    {
        Camera currentCamera = null;
        Camera newCamera = cameras.FirstOrDefault(cam => cam.name == cameraName);
        if (newCamera == null || !newCamera.gameObject.activeSelf)
        {
            Debug.LogWarning($"Camera with name {cameraName} not found. Maintaining current camera instead.");
            return;
        }
        currentCamera = GetActiveCamera();
        if (currentCamera != null && currentCamera != newCamera)
        {
            (currentCamera.depth, newCamera.depth) = (newCamera.depth, currentCamera.depth);
            (currentCamera.GetComponent<AudioListener>().enabled, newCamera.GetComponent<AudioListener>().enabled) = 
                (newCamera.GetComponent<AudioListener>().enabled, currentCamera.GetComponent<AudioListener>().enabled);
        }
    }

    public static Camera GetActiveCamera()
    {
        Camera currentCamera = null;
        foreach (Camera cam in cameras)
        {
            if (currentCamera == null || cam.depth > currentCamera.depth)
            {
                currentCamera = cam;
            }
        }
        if (currentCamera == null)
        {
            Debug.LogWarning("No active camera found. Returning null.");
            return null;
        }
        return currentCamera;
    }
}
