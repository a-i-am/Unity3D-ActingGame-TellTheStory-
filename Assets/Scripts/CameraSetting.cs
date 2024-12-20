using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSetting : MonoBehaviour
{
    public Transform player; // 플레이어의 Transform
    public CinemachineFreeLook freeLookCam; // FreeLookCam
    public CinemachineFreeLook dialogueCamRight; // 우측 카메라
    public CinemachineFreeLook dialogueCamLeft; // 좌측 카메라

    void Update()
    {
        // 카메라의 상대적 위치를 판단하는 함수 호출
        if(Input.GetKeyDown(KeyCode.Z)) CheckCameraSide();
    }

    // 카메라가 플레이어의 우측에 있는지 좌측에 있는지 판단하는 메서드
    void CheckCameraSide()
    {
        // 플레이어와 FreeLookCam 사이의 방향 벡터 계산
        Vector3 directionToPlayer = player.position - freeLookCam.transform.position;

        // 플레이어의 우측 방향 벡터 (플레이어의 오른쪽)
        Vector3 playerRight = player.right;

        // 도트곱을 계산하여 우측, 좌측 판단
        float dotProduct = Vector3.Dot(directionToPlayer, playerRight);

        // 도트곱 값에 따라 카메라 활성화
        if (dotProduct > 0)
        {
            // 플레이어의 우측에 위치
            dialogueCamRight.gameObject.SetActive(true);
            dialogueCamLeft.gameObject.SetActive(false);
        }
        else
        {
            // 플레이어의 좌측에 위치
            dialogueCamRight.gameObject.SetActive(false);
            dialogueCamLeft.gameObject.SetActive(true);
        }
    }

}
