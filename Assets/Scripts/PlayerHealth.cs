using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerHealth : LivingEntity
{
    public float LimitTime = 5f;
    private float currentTime = 5;  //(리스폰타임 : 5초)
    private bool isRespawnTime = false;
    public Text RespawnTime;    //리스폰 타임 표시할 텍스트
    public Slider healthSlider; //체력을 표시할 UI슬라이더
    public AudioClip deathClip; //사망소리
    public AudioClip hitClip;   //피격 소리
    public AudioClip itemPickupClip;    //아이템 습득 소리
    private AudioSource playerAudioPlayer;  //플레이어 소리 재생기
    private PlayerMovement playerMovement;  //플레이어 움직임 컴포넌트
    private PlayerShooter playerShooter;    //플레이어 슈터 컴포넌트
    public Animation hitani;    //플레이어 다칠때 나타나는 애니메이션
    public GameObject hitimage;      //플레이어 다칠때 나타나는 이미지

    private void Update()
    {
        if (!photonView.IsMine) { return; } //로컬 플레이어만 해당
        if (isRespawnTime == true)
        {
            //만약 게임오버 상태가 아니고 플레이어가 리스폰중이라면
            RespawnTime.gameObject.SetActive(true);
            RespawnTime.text = ((int)currentTime + 1).ToString();  //리스폰 타임 UI 업데이트
            currentTime -= Time.deltaTime;              //1초씩 줄어듦
            if (currentTime <= 0)
            {
                //만약 리스폰타임이 5초가 지난 경우 다 초기화
                isRespawnTime = false;
                RespawnTime.gameObject.SetActive(false);
                currentTime = 5;
            }
        }
        if (GameManager.instance.isGameover == true)
        {
            RespawnTime.gameObject.SetActive(false);    //만약 플레이어가 게임오버 상태라면 리스폰타임 비활성화
            hitimage.SetActive(false);
        }
    }

    private void Awake()
    {
        playerAudioPlayer = GetComponent<AudioSource>();
        playerMovement = GetComponent<PlayerMovement>();
        playerShooter = GetComponent<PlayerShooter>();
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        healthSlider.gameObject.SetActive(true);    //체력 슬라이더 활성화
        healthSlider.maxValue = startingHealth; //체력 슬라이더 최댓값 기본 체력값
        healthSlider.value = health;    //체력 슬라이더 값 현재 체력 값
        playerMovement.enabled = true;
        playerShooter.enabled = true;
    }
    [PunRPC]
    public override void RestoreHealth(float newHealth)
    {
        base.RestoreHealth(newHealth);  //체력 회복
        healthSlider.value = health;    //갱신된 체력으로 슬라이더 갱신
    }
    [PunRPC]
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (!dead)
        {
            if (photonView.IsMine) { hitani.Play(); }
            playerAudioPlayer.PlayOneShot(hitClip); //사망하지 않은 경우 효과음 재생

        }
        base.OnDamage(damage, hitPoint, hitNormal);
        healthSlider.value = health;
    }
    public override void Die()
    {
        base.Die();
        healthSlider.gameObject.SetActive(false);   //체력슬라이더 비활성화
        playerAudioPlayer.PlayOneShot(deathClip);    //사망음 재생
        playerMovement.enabled = false;
        playerShooter.enabled = false;
        isRespawnTime = true;   //리스폰타임을 알려주기위해 UI 활성화
        Invoke("Respawn", 5f);  //리스폰
    }

    [PunRPC]
    private void OnTriggerEnter(Collider other)
    {
        //아이템과 충돌한 경우, 사망하지 않은 경우 아이템 사용 가능
        if (!dead)
        {
            IItem item = other.GetComponent<IItem>();   //컴포넌트 가져오기 시도
            if (item != null)
            {
                //호스트만 아이템 직접 사용 가능, 호스트는 아이템 사용 후 사용된 아이템 효과를 모든 클라이언트에 동기화
                if (PhotonNetwork.IsMasterClient)
                {
                    item.Use(gameObject);
                }
                if (other.tag == "SpeedItem")
                {
                    //만약 아이템이 속도 증가 아이템인 경우
                    StartCoroutine(SpeedUp());  //타이머 작동
                }
                playerAudioPlayer.PlayOneShot(itemPickupClip);  //아이템 습득 소리 재생
            }
        }
    }
    public void Respawn()
    {
        Transform PlayerSpawn = GameManager.instance.PlayerSpawn;
        if (photonView.IsMine)  //로컬 플에이어만 직접 위치 변경 가능
        {
            transform.position = PlayerSpawn.position;    //지정된 위치로 리스폰
        }
        gameObject.SetActive(false);    //컴포넌트를 리셋하기 위해 비활성화
        gameObject.SetActive(true);     //다시 활성화
    }
    [PunRPC]
    IEnumerator SpeedUp()
    {
        Behaviour halo = (Behaviour)gameObject.GetComponent("Halo");
        PlayerMovement.instance.speedup(8);
        Gun.instance.setdamage(75);
        halo.enabled = true;
        yield return new WaitForSeconds(LimitTime); //LimitTime(5초) 동안 아이템 효과 유효
        halo.enabled = false;
        PlayerMovement.instance.speedup(5);
        Gun.instance.setdamage(25);
    }
}
