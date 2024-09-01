using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Photon.Pun;

public class LivingEntity : MonoBehaviourPun, IDamageable
{
    public static LivingEntity instance  //싱글톤 접근용 프로퍼티
    {
        get
        {
            if (m_instance == null) //만약 싱글톤 변수에 아직 오브젝트가 할당되지 않은 경우
            {
                m_instance = FindObjectOfType<LivingEntity>();   //씬에서 GameManager 오브젝트 찾아 할당
            }
            return m_instance;  //싱글톤 오브젝트 변환
        }
    }

    private static LivingEntity m_instance;  //싱글톤이 할당될 static 변수
    public float startingHealth = 100f; //시작 체력
    public float health { get; protected set; } //현재 체력
    public bool dead { get; protected set; }    //사망 상태
    public event Action onDeath;    //사망시 발동할 이벤트

    //호스트 (모든 클라이언트 방향으로 체력과 사망 상태 동기화)
    [PunRPC]
    public void ApplyUpdatedHealth(float newHealth, bool newDead)
    {
        health = newHealth;
        dead = newDead;
    }
    protected virtual void OnEnable()
    {
        //생명체가 활성화 될때 상태를 리셋
        dead = false;   //사망하지 않은 상태로 시작
        health = startingHealth;    //체력을 시작 체력으로 초기화
    }
    [PunRPC]    //데미지 입는 기능, 호스트에서 먼저 단독 실행 후 호스트를 통해 다른 클라이언트에서 일괄 실행됨
    public virtual void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            health -= damage;   //데미지를 입는 기능
            photonView.RPC("ApplyUpdatedHealth", RpcTarget.Others, health, dead);   //호스트에서 클라이언트로 동기화
            photonView.RPC("OnDamage", RpcTarget.Others, damage, hitPoint, hitNormal);  //다른 클라이언트도 onDamage를 통해 실행
        }
        if (health <= 0 && !dead) { Die(); }    //체력이 0이하고 죽지 않으면 사망처리
    }
    [PunRPC]
    public virtual void RestoreHealth(float newHealth)
    {
        //체력을 회복하는 기능
        if (dead) { return; }   //이미 사망한경우 체력 회복 불가

        if (PhotonNetwork.IsMasterClient)   //호스트만 체력 직접 갱신 가능
        {
            if (health < 100) { health += newHealth; }      //플레이어가 데미지를 입은 경우만 체력 추가
            if (health > 100) { health = 100; }             //체력이 100을 넘어가서 회복된 경우 100으로 설정
            photonView.RPC("ApplyUpdatedHealth", RpcTarget.Others, health, dead);   //서버에서 클라이언트로 동기화
            photonView.RPC("RestoreHealth", RpcTarget.Others, newHealth);   //다른 클라이언트도 RestoreHealth실행
        }

    }
    public virtual void Die()
    {
        //사망 처리
        if (onDeath != null) { onDeath(); } //onDeath 이벤트에 등록된 메서드가 있으면 실행
        dead = true;    //사망상태 참
    }
}
