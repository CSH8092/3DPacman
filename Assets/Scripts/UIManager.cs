using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  //씬 관리자
using UnityEngine.UI;   //UI 관련
using Photon.Pun;

//필요한 UI에 즉시 접근 및 변경할 수 있도록 허용
public class UIManager : MonoBehaviourPun
{
    private bool onetime = false;   //랭크에 등록될때 무한호출되는걸 막음
    private string username1;
    private string username2;
    public AudioSource back;    //오디오소스
    public AudioClip button;    //오디오클립
    public Text scoreText;  //점수 표시 텍스트
    public Text resultscoreText;    //최종 점수 텍스트
    public GameObject gameoverUI;   //게임 오버 활성 UI
    private bool isGameEnd = false;
    private int SCORE = 0;

    private static UIManager m_instance;    //싱글톤 할당 변수
    //싱글톤 접근용 프로퍼티
    public static UIManager instance
    {
        get
        {
            if (m_instance == null) { m_instance = FindObjectOfType<UIManager>(); }
            return m_instance;
        }
    }

    [PunRPC]
    public void UpdateScoreText(int newScore)   //점수 갱신
    {
        scoreText.text = "Score : " + newScore;
        if(isGameEnd==false) SCORE = newScore;  //아직 게임이 끝나지 않았다면
    }
    public void SetActiveGameoverUI(bool active)    //타이머가 완료되면 게임오버 화면
    {
        isGameEnd = true;   //게임이 끝났으므로 점수 갱신 X
        gameoverUI.SetActive(active);           //게임오버 UI 활성화
        scoreText.gameObject.SetActive(false);  //기존 점수 UI 비활성화
        resultscoreText.text = "Your Score : " + SCORE; //최종 점수 플레이어에게 보여줌

        setRank(SCORE);
    }
    public void GameEnd()   //게임 종료 버튼
    {
        back.clip = button; back.Play();
        PhotonNetwork.LeaveRoom();          //룸에서 나감
    }

    public void setRank(int score) {
        //호스트만 랭크에 등록 가능 (리모트는 호스트와 함께 자동으로 올라감)
        if (PhotonNetwork.IsMasterClient && onetime==false) {
            onetime = true;
            //호스트인 경우 모든 플레이어(2명)의 닉네임 정보를 가지고 옴
            username1 = login.userNameData;
            if (PhotonNetwork.PlayerList.Length==1) { username2 = ""; }   //솔로 플레이인 경우 username2를 ""로 설정
            else {
                //듀오인 경우 마지막플레이어(리모트플레이어) 닉네임 정보 가지고 옴
                username2 = PhotonNetwork.PlayerList[1].NickName;   
            }

            string address = "http://222.98.197.73:8080/insert.php";
            WWWForm Form = new WWWForm();

            Form.AddField("Score", score);
            Form.AddField("UserName1", username1);
            Form.AddField("UserName2", username2);

            WWW wwwURL = new WWW(address, Form);
        }
    }
}
