using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;   //UI 관련
using UnityEngine.SceneManagement;  //씬 관리자
using Photon.Pun;

public class Ranking : MonoBehaviourPun
{
    public Text Rank1;  //1등
    public Text Rank2;  //2등
    public Text Rank3;  //3등
    public Text Rank4;  //4등
    public Text Rank5;  //5등 (까지)
    private string rankinfo;
    private string rankinfo2;
    public AudioSource back;    //오디오소스
    public AudioClip button;    //오디오클립

    void Start()
    {
        PhotonNetwork.Disconnect(); //재접속을 위해 접속 해제
        StartCoroutine(rankingUI());    //랭킹 코루틴 시작
        back.clip = button;
    }
    IEnumerator rankingUI()
    {
        string url = "http://222.98.197.73:8080/compare.php";
        WWW www = new WWW(url);
        yield return www;

        if (www.isDone)
        {
            if (www.error == null)
            {
                Debug.Log(www.text);
                rankinfo = www.text;
            }
            else
            {
                Debug.Log("error : " + www.error);
            }
        }
        string[] sp = rankinfo.Split('!');  //!를 기준으로 문자열 분리 (1차 분리)
        for (int i = 0; i < sp.Length - 1; i++)
        {
            rankinfo2 = sp[i];
            string[] sp2 = rankinfo2.Split(',');  //,를 기준으로 문자열 분리 (2차 분리)
            if (sp2[1] != "") { sp2[1] = "& " + sp2[1] + "님 "; }

            if (i == 0) Rank1.text = "1등! " + sp2[0] + "님 " + sp2[1] + ": " + sp2[2] + "점";
            if (i == 1) Rank2.text = "2등! " + sp2[0] + "님 " + sp2[1] + ": " + sp2[2] + "점";
            if (i == 2) Rank3.text = "3등! " + sp2[0] + "님 " + sp2[1] + ": " + sp2[2] + "점";
            if (i == 3) Rank4.text = "4등! " + sp2[0] + "님 " + sp2[1] + ": " + sp2[2] + "점";
            if (i == 4) Rank5.text = "5등! " + sp2[0] + "님 " + sp2[1] + ": " + sp2[2] + "점";
        }
    }
    public void Return()   //게임 돌아가기 버튼
    {
        SceneManager.LoadScene("Lobby");    //로비로 이동
        back.Play();
    }
}
