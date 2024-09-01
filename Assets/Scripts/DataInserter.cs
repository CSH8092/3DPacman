using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataInserter : MonoBehaviour
{
    public InputField inputUserName;
    public InputField inputPassword;
    public GameObject CreateUserInformation;
    public GameObject CreateAccountPanel;
    public Text Login;
    public AudioSource back;    //오디오소스
    public AudioClip button;    //오디오클립
    string CreateUserURL = "http://222.98.197.73:8080/Indexing.php";
    public void OnCreateButtonClickEvent()
    {
        if (inputUserName.text == "")
        {
            Login.text = "가입 아이디 입력 필요. 다시 시도해주세요."; CreateAccountPanel.SetActive(false);
        }
        else if (inputPassword.text == "")
        {
            Login.text = "가입 비밀번호 입력 필요. 다시 시도해주세요."; CreateAccountPanel.SetActive(false);
        }
        else { StartCoroutine(CreateUser(inputUserName.text, inputPassword.text)); }    //아이디와 비밀번호를 입력해야 함
        back.clip = button;
        back.Play();
    }
    IEnumerator CreateUser(string username, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("usernamePost", username);
        form.AddField("passwordPost", password);

        WWW www = new WWW(CreateUserURL, form);

        yield return www;
        Debug.Log(www.text);

        Login.text = www.text;

        CreateAccountPanel.SetActive(false);
    }
}
