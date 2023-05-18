using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : SingleTon<UIManager>
{
    PlayerMoveMent player;

    [Header("---------------------Screen")]
    [SerializeField] Image selectBackGround;
    [SerializeField] Image resultScreen;

    [Header("---------------------AirSpeechBubble")]
    [SerializeField] Image airSpeechBubbleImage;
    [SerializeField] Sprite[] speechBubbleSprites;

    [Header("---------------------CountSpeechBubble")]
    [SerializeField] Image countSpeechBubbleImage;
    [SerializeField] Image countImage;
    [SerializeField] Sprite[] countSprites;

    [Header("---------------------SpeechBubblePos")]
    [SerializeField] Transform speechBubblePos;

    [Header("---------------------OtherText")]
    [SerializeField] Text hpText;
    [SerializeField] Text oxygenText;
    [SerializeField] Text depthText;
    [SerializeField] Text scoreText;

    private void Awake()
    {
        player = GameManager.Instance.player.GetComponent<PlayerMoveMent>();
    }

    private void LateUpdate()
    {
        if (airSpeechBubbleImage.gameObject.activeInHierarchy)
        {
            Vector2 pos;
            pos.x = speechBubblePos.position.x;
            pos.y = speechBubblePos.position.y;

            airSpeechBubbleImage.transform.position = pos;
        }
        if (countSpeechBubbleImage.gameObject.activeInHierarchy)
        {
            Vector2 pos;
            pos.x = speechBubblePos.position.x;
            pos.y = speechBubblePos.position.y;

            countSpeechBubbleImage.transform.position = pos;
        }

        UpdateHpText(player.hp);
        UpdateOxygenText(player.oxygen);
        UpdateDepthText(player.transform.position.y * (-1));
        UpdateScoreText(GameManager.Instance.score);
    }


    public void SelectBackGroundPopup()
    {
        Time.timeScale = 0;
        selectBackGround.gameObject.SetActive(true);
    }

    public void SelectBackGroundPopdown()
    {
        Time.timeScale = 1;
        selectBackGround.gameObject.SetActive(false);
    }

    public void ResultScreenPopup()
    {
        Text depth = resultScreen.GetComponentInChildren<Text>();
        depth.text = depthText.text;
        resultScreen.gameObject.SetActive(true);
    }

    public void UpdateHpText(float hp)
    {
        
        hpText.text = ((int)hp).ToString() + "/100";
    }
    public void UpdateOxygenText(float oxygen)
    {
        oxygenText.text = ((int)oxygen).ToString() + "/100";
    }

    public void UpdateDepthText(float yPos)
    {
        depthText.text = ((int)yPos).ToString() + "/" + GameManager.Instance.clearDepth.ToString();
    }
    public void UpdateScoreText(int score)
    {

        scoreText.text = score.ToString();
    }

    public void SettingAirImage(int type)
    {
        airSpeechBubbleImage.sprite = speechBubbleSprites[type]; // type == 0 +air, type == 1 -air

        if (type == 0)
            airSpeechBubbleImage.rectTransform.pivot = new Vector2(1, 0);
        else if (type == 1)
            airSpeechBubbleImage.rectTransform.pivot = new Vector2(0, 0);
       
        

        Vector2 pos;
        pos.x = speechBubblePos.position.x;
        pos.y = speechBubblePos.position.y;

        airSpeechBubbleImage.transform.position = pos;

        StartCoroutine(ImageClear());
    }

    IEnumerator ImageClear()
    {
        airSpeechBubbleImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        airSpeechBubbleImage.gameObject.SetActive(false);
    }

    public void SettingCountSpeechBubble(float oxygen)
    {
        int value = Mathf.FloorToInt(oxygen) - 1;
        countImage.sprite = countSprites[value];
        countImage.SetNativeSize();

        countImage.gameObject.SetActive(true);
        countSpeechBubbleImage.gameObject.SetActive(true);
    }

    public void ClearCountSpeechBubble()
    {
        countSpeechBubbleImage.gameObject.SetActive(false);
    }
}
