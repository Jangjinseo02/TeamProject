using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Outtro : MonoBehaviour
{
    [SerializeField]
    private TMP_Text TMP_txt;

    public TMP_Text txt;
    public GameObject Easy;
    public GameObject Normal;
    public GameObject Hard;

    private float typingSpeed = 0.1f;           // 텍스트 타이핑 효과의 재생 속도
    private bool isTypingEffect = false;        // 텍스트 타이핑 효과를 재생중인지
    // Start is called before the first frame update
    void Start()
    {
        if (DataManager.Instance != null && DataManager.Instance.level == DataManager.Level.Easy)
        {
            Easy.SetActive(true);
            Time.timeScale = 1f;
            TMP_txt.text = "4번 접어 작고 편하다! 스켈라Z 플립플립플립플립 출시!";
        }
        else if (DataManager.Instance != null && DataManager.Instance.level == DataManager.Level.Nomal)
        {
            Normal.SetActive(true);
            Time.timeScale = 1f;
            TMP_txt.text = "이것이 진정한 비행기 모드! 스텔라 플레인 출시!";
        }
        else if (DataManager.Instance != null && DataManager.Instance.level == DataManager.Level.Hard)
        {
            Hard.SetActive(true);
            Time.timeScale = 1f;
            TMP_txt.text = "당신의 집과 사무실! 이제부터 스텔라HOME이 책임진다.";
        }
        StartCoroutine(OnTypingText());
    }
    private IEnumerator OnTypingText()
    {
        int index = 0;
        
        isTypingEffect = true;
       
        // 텍스트를 한글자씩 타이핑치듯 재생
        while (index < TMP_txt.text.Length)
        { 
            txt.text += TMP_txt.text[index++];
            yield return new WaitForSeconds(typingSpeed);
        }
        isTypingEffect = false;
    }
    // Update is called once per frame
    void Update()
    {   
        
        if (Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene("TitleScene");
            Time.timeScale = 1f;
        }
    }
}
