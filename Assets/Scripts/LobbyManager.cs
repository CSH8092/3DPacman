using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;   //포톤 컴포넌트
using Photon.Realtime;  //포톤 서비스 관련 라이브러리
using UnityEngine.SceneManagement;  //씬 관리자

public class LobbyManager : MonoBehaviourPunCallbacks
{
    private string gameVersion = "1.0";   //게임 버전
    public Text CText;
    public Text UText;
    public Button joinButton;
    private string username;
    public AudioSource back;    //오디오소스
    public AudioClip button;    //오디오클립

    private void Start()
    {
        //로비 화면 실행과 동시에 마스터 서버 접속 시도
        PhotonNetwork.GameVersion = gameVersion;    //게임 버전 설정
        PhotonNetwork.ConnectUsingSettings();       //설정한 정보로 마스터 서버 접속 시도
        joinButton.interactable = false;    //접속 중일땐 버튼 비활성화
        CText.text = "마스터 서버에 접송중...";
        username = login.userNameData;
        UText.text = "환영합니다! " + username + "님!";
        back.clip = button; //소리 초기화 설정
    }
    public override void OnConnectedToMaster()
    {
        //마스터 서버 접속 성공 시
        joinButton.interactable = true;     //접속 버튼 활성화
        CText.text = "온라인 : 마스터 서버와 연결됨";
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        //마스터 서버 접속 실패 시
        joinButton.interactable = false;    //접속 버튼 비활성화
        CText.text = "오프라인 : 마스터 서버와 연결되지 않음\n접속 재시도 중...";

        PhotonNetwork.ConnectUsingSettings();
    }
    public void Connect()
    {
        back.Play();    //버튼 클릭 소리 재생
        //룸 접속 시도
        joinButton.interactable = false;    //중복 접속 시도 방지 (버튼 비활성화)
        if (PhotonNetwork.IsConnected)
        {
            //만약 마스터 서버 접속 중이라면
            CText.text = "룸에 접속...";
            PhotonNetwork.JoinRandomRoom(); //룸 접속 실행
        }
        else
        {
            CText.text = "오프라인 : 마스터 서버와 연결되지 않음\n접속 재시도 중...";
            PhotonNetwork.ConnectUsingSettings();   //마스터 서버로 재접속 시도
        }
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        CText.text = "빈 방이 없음, 새로운 방 생성...";
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 2 }); //빈 방이 없는 경우 자동 실행
    }
    public override void OnJoinedRoom()
    {
        //룸에 참가 완료 된 경우
        CText.text = "방 참가 성공";
        PhotonNetwork.LoadLevel("Stage");    //모든 참가자가 Stage씬을 로드하게 함
    }
    public void Rank()   //랭킹 버튼
    {
        back.Play();    //버튼 클릭 소리 재생
        SceneManager.LoadScene("Rank");    //랭킹으로 이동
    }
    public void Logout()
    {
        back.Play();    //버튼 클릭 소리 재생
        SceneManager.LoadScene("Login");    //로그아웃함
    }
}
