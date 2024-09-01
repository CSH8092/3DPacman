using System.Collections;
using System.Collections.Generic;
using Cinemachine;  //시네머신
using Photon.Pun;   //PUN
using UnityEngine;

public class CameraSetup : MonoBehaviourPun
{
    public CinemachineVirtualCamera arrCam;   //카메라 요소 추가
    CinemachineVirtualCamera followCam;         //선택될 카메라
    public AudioSource back;    //오디오소스
    public AudioClip button;    //오디오클립
    int i = 1;      //플레이어 입력 확인
    private void Update()
    {
        if (Input.GetButtonUp("Fire2"))
        {
            back.PlayOneShot(button);
            if (i == 0) { i = 1; arrCam.enabled = true; }
            else { i = 0; arrCam.enabled = false; }
        }
        if (photonView.IsMine)
        {
            followCam = FindObjectOfType<CinemachineVirtualCamera>();
            if (i == 0) followCam = arrCam;
            followCam.Follow = transform;   //가상 카메라 추적 대상을 자신의 트랜스폼으로 변경
            followCam.LookAt = transform;
        }
    }
}
