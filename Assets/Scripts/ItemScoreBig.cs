using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ItemScoreBig : MonoBehaviourPun, IItem
{
    public int score = 250; //점수를 500점 증가
    public void Use(GameObject target)
    {
        OnTriggerEnter(target.GetComponent<Collider>());
    }
    [PunRPC]
    void OnTriggerEnter(Collider col)
    {
        GameManager.instance.AddScore(score);   //점수 추가
        MeshRenderer Render = GetComponent<MeshRenderer>();
        SphereCollider Collider = GetComponent<SphereCollider>();
        Render.enabled = false;
        Collider.enabled = false;
    }
}
