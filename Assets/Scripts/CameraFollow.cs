using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    static public Transform player;
    public float distance;
    public float height;
    public float cameraSpeed;
    public float rotateSpeed;

    public Vector3 lookOffSet = new Vector3(0, 1, 0);

    private void FixedUpdate()
    {
        if(player)
        {
            Vector3 lookPosition = player.position + lookOffSet;
            Vector3 relativePos = lookPosition - transform.position;
            Quaternion rotate = Quaternion.LookRotation(relativePos);

            transform.rotation = Quaternion.Slerp(this.transform.rotation, rotate, Time.deltaTime * rotateSpeed * 0.1f);

            Vector3 targetPos = player.transform.position + player.transform.up * height - player.transform.forward * distance;

            this.transform.position = Vector3.Lerp(this.transform.position, targetPos, Time.deltaTime * cameraSpeed * 0.1f);
        }
    }
}
