using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Gun : MonoBehaviourPun, IPunObservable
{
    private static Gun m_instance;  //싱글톤이 할당될 static 변수
    public static Gun instance  //싱글톤 접근용 프로퍼티
    {
        get
        {
            if (m_instance == null) //만약 싱글톤 변수에 아직 오브젝트가 할당되지 않은 경우
            {
                m_instance = FindObjectOfType<Gun>();   //씬에서 PlayerMovement 오브젝트 찾아 할당
            }
            return m_instance;  //싱글톤 오브젝트 변환
        }
    }
    public enum State
    {
        Ready,  //발사 준비
        Empty  //쏠 점수가 없음
    }
    public State state { get; private set; }    //현재 총 상태
    public Transform fireTransform; //총알 발사될 위치
    private LineRenderer bulletLineRenderer;    //총알 궤적 렌더러
    private AudioSource gunAudioPlayer; //총 소리 재생
    public AudioClip shotClip;          //발사 소리
    public float damage = 25;           //공격력
    private float fireDistance = 50f;   //사정거리
    private int Score = 0;    //현재 점수
    public int magAmmo;             //현재 쏠 수 있는 총알 수
    public float timeBetFire = 0.12f;   //총알 발사 간격
    private float lastFireTime;         //총을 마지막으로 발사한 시점

    //주기적으로 자동 실행되는 동기화 메서드
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        //로컬 오브젝트라면 쓰기 부분 실행됨
        if (stream.IsWriting)
        {
            stream.SendNext(Score);
            stream.SendNext(magAmmo);
            stream.SendNext(state);
        }
        else {
            Score = (int)stream.ReceiveNext();
            magAmmo = (int)stream.ReceiveNext();
            state = (State)stream.ReceiveNext();
        }
    }
    [PunRPC]
    private void Update()
    {
        Score = GameManager.instance.score;
        magAmmo = Score;  //현재 점수로 동기화
        if (magAmmo > 0) { state = State.Ready; }
    }
    private void Awake()
    {
        gunAudioPlayer = GetComponent<AudioSource>();
        bulletLineRenderer = GetComponent<LineRenderer>();
        bulletLineRenderer.positionCount = 2;   //사용할 점 2개
        bulletLineRenderer.enabled = false; //라인 렌더링 비활성화
    }
    private void OnEnable()
    { //총 상태 초기화
        magAmmo = Score;  //초기 점수로 초기화
        state = State.Ready;    //준비 상태 설정
        lastFireTime = 0;       //총쏜 시점 초기화
    }
    public void Fire()
    {
        if (state == State.Ready && Time.time >= lastFireTime + timeBetFire && Score>0)
        {
            //발사 가능한 상태와 마지막 총 발사 시점에서 총알 발사 간격 지남
            lastFireTime = Time.time;   //마지막 총 발사 시점 갱신
            Shot(); //실제 발사
        }
    }
    [PunRPC]
    private void Shot()
    {
        //실제 발사 처리는 호스트에게 대리
        photonView.RPC("ShotProcessOnServer", RpcTarget.MasterClient);
        if (magAmmo <= 0) { state = State.Empty; }  //만약 남은 탄알이 없으면 Empty상태

    }
    [PunRPC]
    private void ShotProcessOnServer() {
        RaycastHit hit; //레이캐스트 충돌 정보 저장 컨테이너
        Vector3 hitPosition = Vector3.zero; //탄알 맞은 곳 저장 변수

        if (Score != 0) GameManager.instance.DeleteScore(10);   //점수 10만큼 소모하여 총알 쏨

        //레이가 어떤 물체와 충돌한 경우 충돌한 상대방으로부터 오브젝트 가져오기 시도
        if (Physics.Raycast(fireTransform.position, fireTransform.forward, out hit, fireDistance))
        {
            //레이가 물체와 충돌한 경우 충돌한 상대방의 IDamageable 가져오기
            IDamageable target = hit.collider.GetComponent<IDamageable>();
            if (target != null && hit.collider.tag!="Player")   //팀킬 불가능
            {
                target.OnDamage(damage, hit.point, hit.normal); //가져오는데 성공하면 상대방에게 대미지 주기
            }
            hitPosition = hit.point;    //레이가 충돌한 위치 저장
        }
        else
        {
            //충돌하지 않고 사정거리까지 날아갔을 때 위치를 충돌 위치
            hitPosition = fireTransform.position + fireTransform.forward * fireDistance;
        }
        photonView.RPC("ShotEffectProcessOnClients", RpcTarget.All, hitPosition);
    }
    [PunRPC]
    private void ShotEffectProcessOnClients(Vector3 hitPosition) {
        StartCoroutine(ShotEffect(hitPosition));
    }
    private IEnumerator ShotEffect(Vector3 hitPosition)
    {
        gunAudioPlayer.PlayOneShot(shotClip);   //총격 소리 발생
        bulletLineRenderer.SetPosition(0, fireTransform.position);  //선의 시작점 : 총구 위치
        bulletLineRenderer.SetPosition(1, hitPosition); //선의 끝점 : 입력으로 들어온 충돌 위치
        bulletLineRenderer.enabled = true;  //라인 렌더러를 활성화해 총알 궤적 그림
        yield return new WaitForSeconds(0.03f); //0.03초 동안 처리 대기
        bulletLineRenderer.enabled = false; //라인 렌더러 비활성화 총알 궤적 삭제
    }
    //호스트 (총알 데미지 동기화)
    [PunRPC]
    public void ApplyUpdatedgun(float g) {damage = g;}
    [PunRPC]
    public virtual void setdamage(float g)
    {
        if (PhotonNetwork.IsMasterClient)   //호스트만 갱신 가능
        {
            damage = g;
            photonView.RPC("ApplyUpdatedgun", RpcTarget.Others, damage);   //서버에서 클라이언트로 동기화
            photonView.RPC("setdamage", RpcTarget.Others, g);   //다른 클라이언트도 실행
        }
    }
}
