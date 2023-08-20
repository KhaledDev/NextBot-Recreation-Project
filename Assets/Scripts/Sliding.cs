using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// -----------------------------------------------------------------------------------------
/// Same thing with the movement script this script was taken from "Dave / GameDevelopment".
/// Channel: 'https://www.youtube.com/@davegamedevelopment'
/// Video: 'https://www.youtube.com/watch?v=SsckrYYxcuM'
/// -----------------------------------------------------------------------------------------
/// 
/// I recommend watching the video's to get a better understandment of the script.
/// 
/// </summary>

public class Sliding : MonoBehaviour
{
    [Header("Ref")]
    public Transform orientation;
    public Transform playerObj;
    private Rigidbody rb;
    private PlayerMovement pm;

    [Header("Sliding")]
    public float maxSlideTime;
    public float slideForce;
    private float slideTime;

    public float slideYScale;
    private float startYScale;

    [Header("Input")]
    public KeyCode slideKey = KeyCode.C;
    private float hor;
    private float ver;

    private bool sliding;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();

        startYScale = playerObj.localScale.y;
    }

    private void Update()
    {
        hor = Input.GetAxisRaw("Horizontal");
        ver = Input.GetAxisRaw("Vertical");

        if(Input.GetKeyDown(slideKey) && (hor != 0 || ver != 0))
        {
            StartSlide();
        }

        if (Input.GetKeyUp(slideKey) && sliding)
        {
            StopSlide();
        }
    }

    private void FixedUpdate()
    {
        if (sliding)
            SlidingMovement();
    }

    void StartSlide()
    {
        sliding = true;

        playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale, playerObj.localScale.z);
        //rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        slideTime = maxSlideTime;
    }

    void SlidingMovement()
    {

        Vector3 inputDirection = orientation.forward * ver + orientation.right * hor;

        rb.AddForce(inputDirection.normalized * slideForce,ForceMode.Force);

        slideTime -= Time.deltaTime;

        if (slideTime <= 0)
            StopSlide();

    }

    void StopSlide()
    {
        sliding = false;

        playerObj.localScale = new Vector3(playerObj.localScale.x, startYScale, playerObj.localScale.z);
    }

}
