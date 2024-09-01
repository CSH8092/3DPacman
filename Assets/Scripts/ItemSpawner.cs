using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.AI;   //내비매쉬 관련
using Photon.Pun;

public class ItemSpawner : MonoBehaviourPun
{
    public GameObject[] items;  //생성할 아이템
    public Transform[] spawnPoints;
    private float timeBetSpawn = 8f; //생성 간격
    private float lastSpawnTime;    //마지막 생성 시점
    int spawnstart; //아이템 생성 개수 설정
    int[] sp = new int[30];       //아이템 스폰 위치 설정

    private void Start()
    {
        lastSpawnTime = 0;
    }
    //주기적으로 아이템 생성 처리 실행
    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient) { return; }  //호스트에서만 아이템 직접생성 가능
        //현재 시점이 마지막 생성 시점에서 생성 주기 이상 지나고 플레이어 캐릭터가 존재하는 경우
        if (Time.time >= lastSpawnTime + timeBetSpawn)
        {
            lastSpawnTime = Time.time;  //마지막 생성시간 갱신
            Spawn();    //아이템 생성
        }
    }
    private void Spawn()
    {
        if (!PhotonNetwork.IsMasterClient) { return; }  //호스트에서만 아이템 직접생성 가능
        spawnstart = Random.Range(5, spawnPoints.Length);
        for (int i = 0; i < spawnstart; i++)    //아이템 스폰위치 중복없이 미리 설정
        {
            sp[i] = Random.Range(0, spawnPoints.Length); //스폰 위치 설정
            for (int j = 0; j < i; j++)
            {
                if (sp[i] == sp[j]) i--; //만약에 중복값 있는 경우 다시 뽑기로 결정
            }
        }
        //한번 스폰이 활성화 되었을때 최소 5개에서 부터 시작해서 한꺼번에 아이템 생성
        for (int i = 0; i < spawnstart ; i++) {
            Transform spawnPoint = spawnPoints[sp[i]];    //생성 위치 결정
            //아이템 중 하나를 무작위로골라 랜덤 스폰포인트 위치 생성
            GameObject selectedItem = items[Random.Range(0, items.Length)];
            GameObject item = PhotonNetwork.Instantiate(selectedItem.name, spawnPoint.position, spawnPoint.rotation);
            StartCoroutine(DestroyAfter(item, 8f));  //8초뒤 아이템 파괴
        }
    }
    //포톤 지연 실행 코루틴
    IEnumerator DestroyAfter(GameObject target, float delay)
    {
        yield return new WaitForSeconds(delay); //딜레이만큼 대기
        if (target != null) { PhotonNetwork.Destroy(target); }  //target이 파괴되지 않았으면 파괴 실행
    }
}
