using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;   //슬라이더 UI

//점수 및 게임오버 관리
public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public Slider Timer;
    public float MaxTime = 180f;    //게임 플레이 시간
    float CurrentTime=0;
    public GameObject playerPrefab;    //생성할 플레이어 캐릭터 프리팹
    public Transform PlayerSpawn;       //플레이어 스폰포인트
    public int score = 0;  //현재 게임 점수
    public bool isGameover { get; private set; }    //게임 오버 상태
    public bool one = false;
    public AudioSource end;    //오디오소스
    public AudioClip endsound;    //오디오클립
    public AudioSource back;    //오디오소스
    public AudioClip button;    //오디오클립
    private string username1;
    private string username2;
    public Text currentuser;        //현재 유저 표시할 텍스트
    //채팅에 필요한 오브젝트
    public GameObject Chat;
    public Text msglist;
    public InputField send;
    public ScrollRect sc;
    int width=230, height=100;

    private static GameManager m_instance;  //싱글톤이 할당될 static 변수
    public static GameManager instance  //싱글톤 접근용 프로퍼티
    {
        get
        {
            if (m_instance == null) //만약 싱글톤 변수에 아직 오브젝트가 할당되지 않은 경우
            {
                m_instance = FindObjectOfType<GameManager>();   //씬에서 GameManager 오브젝트 찾아 할당
            }
            return m_instance;  //싱글톤 오브젝트 변환
        }
    }

    public void OnSendChatMsg()
    {
        string msg = string.Format("[{0}] {1}", login.userNameData, send.text);
        //Debug.Log(login.userNameData+ " send " + send.text);
        photonView.RPC("ReceiveMsg", RpcTarget.OthersBuffered, msg);
        ReceiveMsg(msg); send.text = "";
    }
    [PunRPC]
    void ReceiveMsg(string msg) {
        msglist.text += "\n"+msg;
        height += 8;
        sc.content.sizeDelta = new Vector2(width, height);
        sc.verticalNormalizedPosition = 0f;
    }
    public void MsgButton() { OnSendChatMsg(); }

    private void Update()
    {
        Chat.transform.SetAsLastSibling();  //채팅 기능 사용할 수 있게 설정 (Hierarchy 맨 뒤로 오게하여 우선순위 높임) + 플레이어 캔버스의 Blocking Mask설정
        if (PhotonNetwork.IsMasterClient) {
            //호스트인 경우 타이머 값 변경 (만약 타이머의 시간이 0이 아니라면 시간 줄어듦)
            if (Timer.value > 0f) { Timer.value -= Time.deltaTime; CurrentTime = Timer.value; }
            username1 = login.userNameData + "(방장)";
            if (PhotonNetwork.PlayerList.Length == 1) { username2 = ""; }   //솔로 플레이인 경우 username2를 ""로 설정
            else {
                username2 = " & " + PhotonNetwork.PlayerList[1].NickName;
            } //듀오인 경우 마지막플레이어(리모트플레이어) 닉네임 정보 가지고 옴
            currentuser.text = "현재 접속자 : " + username1 + username2;
        }
        if (Input.GetKeyDown(KeyCode.Escape)) { PhotonNetwork.LeaveRoom(); } // ESC누르면 꺼지도록 함
        if (Timer.value < 60) { GameObject.Find("FillTime").GetComponent<Image>().color = Color.red; }  //시간이 촉박한경우(60초 이하)
        if (Timer.value <= 0) { EndGame(); }    //시간이 초과된 경우 게임 오버
    }
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Lobby");
    }
    //주기적으로 자동 실행, 동기화 메서드
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //로컬 오브젝트인 경우
            stream.SendNext(score);
            stream.SendNext(CurrentTime);   //현재 시간 리모트에게 전송
            stream.SendNext(username1); //닉네임 전송
            stream.SendNext(username2); //닉네임 전송
        }
        else
        {
            //리모트 오브젝트인 경우
            score = (int)stream.ReceiveNext();  //네트워크에서 score값 받고
            UIManager.instance.UpdateScoreText(score);  //UI로 표시
            CurrentTime = (float)stream.ReceiveNext();      //네트워크에서 CurrentTime값 받고
            Timer.value = CurrentTime;  //타이머 업데이트
            username1 = (string)stream.ReceiveNext();  //네트워크에서 값 받기
            username2 = (string)stream.ReceiveNext();  //네트워크에서 값 받기
            currentuser.text = "현재 접속자 : " + username1 + username2; //표시
        }    
    }

    private void Awake()
    {
        //현재 싱글톤 오브젝트가 된 다른 GameManager 오브젝트가 있는 경우
        if (instance != this) { Destroy(gameObject); }  //자신을 파괴 (오브젝트 완전히 없앰)
    }

    private void Start()
    {
        PhotonNetwork.IsMessageQueueRunning = true; //데이터 통신 연결

        //네트워크 상 모든 클라이언트에서 실행
        PhotonNetwork.Instantiate(playerPrefab.name, PlayerSpawn.position, Quaternion.identity);

        if (PhotonNetwork.IsMasterClient) {
            Timer.maxValue = MaxTime;
            Timer.value = MaxTime;
        } //타이머값 설정

        PhotonNetwork.NickName = login.userNameData;    //닉네임 데이터 설정
    }

    [PunRPC]
    public void AddScore(int newSocre)
    {
        if (!isGameover)
        {
            score += newSocre;
            UIManager.instance.UpdateScoreText(score);
        }
    }
    public void DeleteScore(int d)
    {
        if (!isGameover)
        {
            score -= d;
            UIManager.instance.UpdateScoreText(score);
        }
    }
    public void EndGame()
    {
        if (one == false) { end.clip = endsound; end.Play(); one = true; }
        isGameover = true;
        UIManager.instance.SetActiveGameoverUI(true);
    }
    public void Rank()   //랭킹 버튼
    {
        back.clip = button; back.Play();
        SceneManager.LoadScene("Rank");    //랭킹으로 이동
    }
}
