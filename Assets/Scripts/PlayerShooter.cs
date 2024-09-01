using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerShooter : MonoBehaviourPun
{
    public Gun gun; //사용할 총
    private PlayerInput playerInput;    //플레이어

    private void Start()
    {   //사용할 컴포넌트 가져옴
        playerInput = GetComponent<PlayerInput>();
    }
    private void OnEnable()
    {   //슈터가 활성화 될 때 총도 활성화
        gun.gameObject.SetActive(true);
    }
    private void OnDisable()
    {   //슈터가 비활성화 될 때 총도 비활성화
        gun.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (!photonView.IsMine) { return; } //로컬 플레이어만 직접 사격 및 탄알 UI 갱신 가능
        if (playerInput.fire) { gun.Fire(); }   //입력을 감지하고 총 발사
    }
}
