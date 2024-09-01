using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ItemSpeed :  MonoBehaviourPun, IItem
{
    
    public int score = 250; //점수를 500점 증가
    public void Use(GameObject target)
    {
        OnTriggerEnter(target.GetComponent<Collider>());    //아이템과 충돌한 경우
    }
    [PunRPC]
    void OnTriggerEnter(Collider col)
    {
        GameManager.instance.AddScore(score);   //점수 추가
        MeshRenderer Render = GetComponent<MeshRenderer>();
        SphereCollider Collider = GetComponent<SphereCollider>();
        Render.enabled = false;
        Collider.enabled = false;

        //StartCoroutine(SpeedUp());  //타이머 작동
    }
    IEnumerator SpeedUp()
    {
        yield return new WaitForSeconds(5); //5초 동안 아이템 효과 유효
    }
}
