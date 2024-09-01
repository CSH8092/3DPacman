using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Rotator : MonoBehaviourPun
{
    //아이템을 돌게하는 스크립트
    public float rotationSpeed = 30;
    void Update()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }
}
