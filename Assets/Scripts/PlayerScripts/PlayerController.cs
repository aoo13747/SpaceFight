using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class PlayerController : NetworkBehaviour
{
    public float ForwardSpeed, StrafeSpeed, HoverSpeed,rollSpeed;
    private float activeForwardspeed, activeStrafeSpeed, activeHoverSpeed;
    public float forwardAcclation,strafeAcclation,hoverAcclation,rollAcclation;

    public float lookRoatateSpeed;
    private Vector2 lookInput,screenCenter,mouseDistance;

    private float rollInput;

    GameObject camera;
    public Transform defaultCamPos;
    public Transform frontCamPos;
    bool frontCam;
    private void Start()
    {
        screenCenter.x = Screen.width * 0.5f;
        screenCenter.y = Screen.height * 0.5f;

        Cursor.lockState = CursorLockMode.Confined;       
        
        if(IsLocalPlayer)
        {
            //CameraFollow.player = this.gameObject.transform;
            camera = GameObject.FindGameObjectWithTag("MainCamera");
            camera.transform.SetParent(this.gameObject.transform);
            camera.transform.position = defaultCamPos.transform.position;           
        }
        else
        {
            return;
        }
    }
    private void Update()
    {
        if (!IsLocalPlayer)
            return;
        else
        {
            Movement();
            if (Input.GetKeyDown(KeyCode.V))
            {
                if (!frontCam)
                {
                    camera.transform.position = frontCamPos.transform.position;
                    frontCam = true;
                }
                else if (frontCam)
                {
                    camera.transform.position = defaultCamPos.transform.position;
                    frontCam = false;
                }
            }
        }
        
    }
    private void Movement()
    {
        lookInput.x = Input.mousePosition.x;
        lookInput.y = Input.mousePosition.y;

        mouseDistance = Vector2.ClampMagnitude(mouseDistance, 1f);
        transform.Rotate(-mouseDistance.y * lookRoatateSpeed * Time.deltaTime, mouseDistance.x * lookRoatateSpeed * Time.deltaTime, rollInput * rollSpeed * Time.deltaTime, Space.Self);

        mouseDistance.x = (lookInput.x - screenCenter.x) / screenCenter.x;
        mouseDistance.y = (lookInput.y - screenCenter.y) / screenCenter.y;

        activeForwardspeed = Mathf.Lerp(activeForwardspeed, Input.GetAxisRaw("Vertical") * ForwardSpeed, forwardAcclation * Time.deltaTime);
        activeStrafeSpeed = Mathf.Lerp(activeStrafeSpeed, Input.GetAxisRaw("Horizontal") * StrafeSpeed, strafeAcclation * Time.deltaTime);
        activeHoverSpeed = Mathf.Lerp(activeHoverSpeed, Input.GetAxisRaw("Hover") * HoverSpeed, hoverAcclation * Time.deltaTime);
        rollInput = Mathf.Lerp(rollInput, Input.GetAxisRaw("Roll"), rollAcclation * Time.deltaTime);

        transform.position += transform.forward * activeForwardspeed * Time.deltaTime;
        transform.position += (transform.right * activeStrafeSpeed * Time.deltaTime) + (transform.up * activeHoverSpeed * Time.deltaTime);
    }
    
}
