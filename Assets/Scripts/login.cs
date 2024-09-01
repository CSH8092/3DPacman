using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class login : MonoBehaviourPun
{
    public InputField inputUserName;
    public InputField inputPassword;
    public GameObject loginButton;
    public GameObject CreateAccountButtonPanel;
    public GameObject howplay;
    public Text Login;
    public AudioSource back;    //오디오소스
    public AudioClip button;    //오디오클립
    public static string userNameData;
    public static string userPWData;

    //string LoginURL = "127.0.0.1/login.php";
    string LoginURL = "http://222.98.197.73:8080/login.php";
    void Start()
    {
        back.clip = button;
        PhotonNetwork.Disconnect(); //재접속을 위해 접속 해제
    }
    void Update()
    {
        userNameData = inputUserName.text;
        userPWData = inputPassword.text;
    }
    public void OnLoginButtonClickEvent()
    {
        back.Play();    //버튼 클릭 소리 재생
        StartCoroutine(LoginToDB(inputUserName.text, inputPassword.text));
    }
    public void OnCreateAccountButtonEvent1()
    {
        CreateAccountButtonPanel.SetActive(true); back.Play();    //버튼 클릭 소리 재생
    }
    public void OnCreateAccountButtonEvent2()
    {
        CreateAccountButtonPanel.SetActive(false); back.Play();    //버튼 클릭 소리 재생
    }
    public void OntButtonEvent()
    {
        back.Play();    //버튼 클릭 소리 재생
        SceneManager.LoadScene("Main");
    }
    IEnumerator LoginToDB(string username, string password)
    {
        Debug.Log("asf");
        WWWForm form = new WWWForm();
        form.AddField("namePost", username);
        form.AddField("passPost", password);
        WWW www = new WWW(LoginURL, form);
        yield return www;
        Login.text = www.text;
        if (www.text == "로그인 성공!")
        {
            SceneManager.LoadScene("Lobby");
        } //로그인 성공인 경우 Lobby씬으로 이동
    }
    public void OnhowplayButtonEvent1()
    {
        howplay.SetActive(true); back.Play();    //버튼 클릭 소리 재생
    }
    public void OnhowplayButtonEvent2()
    {
        howplay.SetActive(false); back.Play();    //버튼 클릭 소리 재생
    }
}
