using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;

public class EnemySpawner : MonoBehaviourPun
{
    public Enemy enemyPrefab;
    public Transform[] spawnPoints;
    public float g_damage = 20f;
    public float g_health = 75f;
    public float g_speed_min = 1.5f;
    public float g_speed_max = 2f;
    public Color strongEnemyColor = Color.red;
    private List<Enemy> enemies = new List<Enemy>();
    private int wave;

    void Awake()
    {
        PhotonPeer.RegisterType(typeof(Color), 128, ColorSerialization.SerializeColor, ColorSerialization.DeserializeColor);
    }
    private void Update()
    {
        //호스트만 적 생성, 다른 클라이언트들은 호스트가 생성한 적 동기화로 받아옴
        if (PhotonNetwork.IsMasterClient)
        {
            if (GameManager.instance != null && GameManager.instance.isGameover) { return; }
            if (enemies.Count <= 0) { SpawnWave(); }    //적을 모두 물리쳤으면 다음 스폰 실행
        }
    }
    private void SpawnWave()
    {
        if (wave < 8) { wave++; } int spawnCount = wave;
        for (int i = 0; i < spawnCount; i++)    //wave만큼 적 생성 (총 8마리까지 생성됨)
        {
            float enemyIntensity = Random.Range((wave-1), 1f);    //웨이브가 높아질수록 센 적이 나올 확률 올라감
            CreateEnemy(enemyIntensity);    //적 처리 실행
        }
    }
    //적을 생성하고 적에게 추적할 대상 할당
    private void CreateEnemy(float intensity)
    {
        //능력치 결정
        float speed = Mathf.Lerp(g_speed_min, g_speed_max, intensity);  //스피드값 랜덤 설정 (1.5~2);
        Color skinColor = Color.Lerp(Color.white, strongEnemyColor, intensity); //피부색 결정
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];    //생성 위치 결정

        //적 프리팹으로부터 적 생성, 네트워크 상 모든 클라이언트들에게 생성됨
        GameObject createdEnemy = PhotonNetwork.Instantiate(enemyPrefab.gameObject.name, spawnPoint.position, spawnPoint.rotation);
        Enemy enemy = createdEnemy.GetComponent<Enemy>();   //생성한 적 셋업
        enemy.photonView.RPC("Setup", RpcTarget.All, g_health, g_damage, speed, skinColor); //생성한 적 능력치와 추격 대상 결정
        enemies.Add(enemy); //생성된 적을 리스트에 추가

        //적의 onDeath를 이벤트에 등록
        enemy.onDeath += () => enemies.Remove(enemy);   //사망한 적을 리스트에서 제거
        enemy.onDeath += () => Destroy(enemy.gameObject, 10f);  //사망한 적을 10초뒤에 파괴
        enemy.onDeath += () => GameManager.instance.AddScore(100);  //적 사망 시 점수 상승
    }

    IEnumerator DestoryAfter(GameObject target, float delay)
    {
        yield return new WaitForSeconds(delay); //delay만큼 쉰 후 target 모든 네트워크 상에서 파괴
        if (target != null)
        {
            PhotonNetwork.Destroy(target);
        }
    }
}
