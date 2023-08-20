using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// This is a simple Camera controller script. No further explination needed.
/// </summary>

public class CameraController : MonoBehaviourPunCallbacks
{
    public float sensX;
    public float sensY;

    public Transform oriantation;

    float xRotation;
    float yRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (!photonView.IsMine) return;

        //Get mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        //Rotate cam and oriantation
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        oriantation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
