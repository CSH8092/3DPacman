using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviourPun
{
    public float moveSpeed = 5f;    //플레이어 기본 속도
    public float rotateSpeed = 180f;//좌우 회전 속도
    private PlayerInput playerInput;    //플레이어 입력을 알려줌
    private Rigidbody playerRigidbody;  //플레이어 리지드바디
    private Animator playerAnimator;    //플레이어 캐릭터 애니메이터
    public bool isGameover { get; private set; }    //게임 오버 상태
    private static PlayerMovement m_instance;  //싱글톤이 할당될 static 변수
    public static PlayerMovement instance  //싱글톤 접근용 프로퍼티
    {
        get
        {
            if (m_instance == null) //만약 싱글톤 변수에 아직 오브젝트가 할당되지 않은 경우
            {
                m_instance = FindObjectOfType<PlayerMovement>();   //씬에서 PlayerMovement 오브젝트 찾아 할당
            }
            return m_instance;  //싱글톤 오브젝트 변환
        }
    }
    void Start()
    {
        //참조 컴포넌트 가져옴
        playerInput = GetComponent<PlayerInput>();
        playerRigidbody = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();

        if (!photonView.IsMine) { return; }
    }
    //물리 갱신 주기에 맞춰 실행
    private void FixedUpdate()
    {
        if (!photonView.IsMine) { return; } //로컬 플레이어만 직접 위치와 회전 변경 가능

        Rotate();   //회전 실행
        Move();     //움직임 실행
    }
    private void Move()
    {
        //앞뒤 움직임, 이동거리 계산
        Vector3 moveDistance = playerInput.move * transform.forward * moveSpeed * Time.deltaTime;
        //리지드바디를 이용해 게임 오브젝트 위치 변경
        playerRigidbody.MovePosition(playerRigidbody.position + moveDistance);
    }
    //입력값에 따라 캐릭터 좌우 회전
    private void Rotate()
    {
        //상대 회전할 수치 계산
        float turn = playerInput.rotate * rotateSpeed * Time.deltaTime;
        //게임 오브젝트 회전 변경
        playerRigidbody.rotation = playerRigidbody.rotation * Quaternion.Euler(0, turn, 0f);
    }
    //호스트 (모든 클라이언트 방향으로 체력과 사망 상태 동기화)
    [PunRPC]
    public void ApplyUpdated(float speed)
    {
        moveSpeed = speed;
    }
    [PunRPC]
    public virtual void speedup(float s)
    {
        if (PhotonNetwork.IsMasterClient)   //호스트만 갱신 가능 
        {
            moveSpeed = s;
            photonView.RPC("ApplyUpdated", RpcTarget.Others, moveSpeed);   //서버에서 클라이언트로 동기화
            photonView.RPC("speedup", RpcTarget.Others, s);   //다른 클라이언트도 실행
        }
    }
}
