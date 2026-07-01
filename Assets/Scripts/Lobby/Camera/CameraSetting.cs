using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSetting : MonoBehaviour
{
    public Transform player;
    public CinemachineFreeLook freeLookCam;
    public CinemachineFreeLook dialogueCamRight;
    public CinemachineFreeLook dialogueCamLeft;

    void Update()
    {

        if(Input.GetKeyDown(KeyCode.Z)) CheckCameraSide();
    }


    void CheckCameraSide()
    {

        Vector3 directionToPlayer = player.position - freeLookCam.transform.position;


        Vector3 playerRight = player.right;


        float dotProduct = Vector3.Dot(directionToPlayer, playerRight);


        if (dotProduct > 0)
        {

            dialogueCamRight.gameObject.SetActive(true);
            dialogueCamLeft.gameObject.SetActive(false);
        }
        else
        {

            dialogueCamRight.gameObject.SetActive(false);
            dialogueCamLeft.gameObject.SetActive(true);
        }
    }

}
