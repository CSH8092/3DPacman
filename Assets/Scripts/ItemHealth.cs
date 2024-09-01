using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ItemHealth : MonoBehaviourPun, IItem
{
    public float health = 50;   //증가 체력(100)
    public int score = 25;      //증가 점수(200)
    public void Use(GameObject target)
    {
        OnTriggerEnter(target.GetComponent<Collider>());
    }
    [PunRPC]
    void OnTriggerEnter(Collider col)
    {
        LivingEntity life = col.GetComponent<LivingEntity>();
        if (life != null) { life.RestoreHealth(health); }
        MeshRenderer Render = GetComponent<MeshRenderer>();
        SphereCollider Collider = GetComponent<SphereCollider>();
        Render.enabled = false;
        Collider.enabled = false;
        GameManager.instance.AddScore(score);   //점수 추가
    }
}
