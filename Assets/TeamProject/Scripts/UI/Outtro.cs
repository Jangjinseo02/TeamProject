using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


[System.Serializable]
public class Dialoge
{
    [TextArea]
    public string dialogue;
    public Sprite cg;
}
public class Outtro : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite_CharacterCG;
    [SerializeField] private SpriteRenderer sprite_DialogueBar;
    [SerializeField] private TMP_Text txt_Dialogue;

    private bool isDialogue = true; //대화가 진행 중 임을 알려주는 변수

    private int count = -1; // 대화가 얼마나 진행됬는지 알려줌

    [SerializeField] private List<Dialoge> dialogue;

    public void ShowDialogue()
    {
        //sprite_DialogueBar.gameObject.SetActive(true);
        //sprite_CharacterCG.gameObject.SetActive(true);
        //txt_Dialogue.gameObject.SetActive(true);

        OnOff(true);
        count = 0;
        //isDialogue = true;
        NextDialogue();
    }

    private void OnOff(bool _flag)
    {
        //sprite_DialogueBar.gameObject.SetActive(_flag);
        sprite_CharacterCG.gameObject.SetActive(_flag);
        txt_Dialogue.gameObject.SetActive(_flag);
        isDialogue = _flag;
    }
    /*private void HideDialogue()
    {
        sprite_DialogueBar.gameObject.SetActive(false);
        sprite_CharacterCG.gameObject.SetActive(false);
        txt_Dialogue.gameObject.SetActive(false);
        isDialogue = false;

    }*/

    private void NextDialogue()
    {
        count++;
        if (count < dialogue.Count)
        {
            txt_Dialogue.text = dialogue[count].dialogue;
            //sprite_CharacterCG.sprite = dialogue[count].cg;
        }
        else
        {
            OnOff(false);
            SceneManager.LoadScene("TitleScene");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        OnOff(true);
        NextDialogue();
        //예외처리 활성화 할 그림을 정함. 잡은 외계인수에 따라서 달라짐. 총 3가지
    }

    // Update is called once per frame
    void Update()
    {
        if (isDialogue)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                NextDialogue();
            }
        }
    }
}
