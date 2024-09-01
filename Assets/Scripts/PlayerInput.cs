using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerInput : MonoBehaviourPun
{
    public string moveAxisName = "Vertical";    //앞뒤 움직임 입력축 이름
    public string rotateAxisName = "Horizontal";    //좌우 회전 입력축 이름
    public string fireButtonName = "Fire1";     //발사 입력버튼 이름
    public string changeCam = "Fire2";          //캠 변경 이름
    //값 할당은 내부에서만 가능
    public float move { get; private set; } //감지된 움직임 입력값
    public float rotate { get; private set; }   //감지된 회전 입력값
    public bool fire { get; private set; }          //감지된 발사 입력값
    public bool cam { get; private set; }           //감지된 캠 입력값

    //사용자 입력 감지
    private void Update()
    {
        //로컬 플레이어가 아닌 경우 입력 X
        if (!photonView.IsMine) { return; }

        //게임오버 상태에서는 사용자 입력 감지 불가
        if (GameManager.instance != null && GameManager.instance.isGameover)
        {
            move = 0;
            rotate = 0;
            fire = false;
            cam = false;
            return;
        }
        move = Input.GetAxis(moveAxisName);
        rotate = Input.GetAxis(rotateAxisName);
        fire = Input.GetButton(fireButtonName);
        cam = Input.GetButton(changeCam);
    }
}
