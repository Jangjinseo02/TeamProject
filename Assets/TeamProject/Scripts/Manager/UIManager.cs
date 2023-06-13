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
    [SerializeField] Image FadeScreen;

    [Header("---------------------AirSpeechBubble")]
    [SerializeField] Image airSpeechBubbleImage;
    [SerializeField] Sprite[] speechBubbleSprites;

    [Header("---------------------CountSpeechBubble")]
    [SerializeField] Image countSpeechBubbleImage;
    [SerializeField] Image countImage;
    [SerializeField] Sprite[] countSprites;

    [Header("---------------------ItemUI")]
    [SerializeField] Image itemUIImage;
    [SerializeField] Sprite[] itemUISprites;

    [Header("---------------------SpeechBubblePos")]
    [SerializeField] Transform speechBubblePos;
    [SerializeField] Transform itemUIPos;

    [Header("---------------------OtherText")]
    [SerializeField] Text hpText;
    [SerializeField] Text oxygenText;
    [SerializeField] Text depthText;
    [SerializeField] Text scoreText;
    [SerializeField] Text catchText;

    [Header("---------------------OtherImage")]
    [SerializeField] Image hpImage;
    [SerializeField] Image oxygenImage;

    float fadeSpeed = 2f;

    private void Awake()
    {
        player = GameManager.Instance.player.GetComponent<PlayerMoveMent>();
    }

    private void LateUpdate()
    {
        if (airSpeechBubbleImage.gameObject.activeInHierarchy || countSpeechBubbleImage.gameObject.activeInHierarchy)
        {
            Vector2 pos;
            pos.x = speechBubblePos.position.x;
            pos.y = speechBubblePos.position.y;

            if(airSpeechBubbleImage.gameObject.activeInHierarchy)
                airSpeechBubbleImage.transform.position = pos;
            if(countSpeechBubbleImage.gameObject.activeInHierarchy)
                countSpeechBubbleImage.transform.position = pos;
        }
        if (itemUIImage.gameObject.activeInHierarchy)
        {
            Vector2 pos;
            pos.x = itemUIPos.position.x;
            pos.y = itemUIPos.position.y;

            itemUIImage.transform.position = pos;
        }

        UpdateHpText(player.hp);
        UpdateOxygenText(player.oxygen);

        if (player.isDead || GameManager.Instance.clear)
            return;
        UpdateDepthText(player.transform.position.y);
        UpdateScoreText(GameManager.Instance.score);
        UpdateCatchText(GameManager.Instance.curCatch);

    }

    public void ResultScreenPopup()
    {
        Text depth = resultScreen.transform.GetChild(0).GetComponent<Text>();
        depth.text = depthText.text;
        Text score = resultScreen.transform.GetChild(1).GetComponent<Text>();
        score.text = scoreText.text;
        Text curCatch = resultScreen.transform.GetChild(2).GetComponent<Text>();
        curCatch.text = GameManager.Instance.curCatch.ToString();
        Text clearCatch = resultScreen.transform.GetChild(3).GetComponent<Text>();
        clearCatch.text = GameManager.Instance.clearCatch.ToString();

        resultScreen.gameObject.SetActive(true);
    }

    public void UpdateHpText(float hp)
    {
        hpText.text = ((int)hp).ToString() + "/100";
        hpImage.fillAmount = hp / 100;
    }
    public void UpdateOxygenText(float oxygen)
    {
        oxygenText.text = ((int)oxygen).ToString() + "/100";
        oxygenImage.fillAmount = oxygen / 100;
    }

    public void UpdateDepthText(float yPos)
    {
        if (yPos > 0)
            yPos = 0;
        else if (yPos <= 0)
            yPos *= -1;

        depthText.text = ((int)yPos).ToString() + " M";
    }
    public void UpdateScoreText(int score)
    {
        scoreText.text = score.ToString();
    }
    public void UpdateCatchText(float curCatch)
    {
        catchText.text = ((int)curCatch).ToString() + "/" + GameManager.Instance.clearCatch.ToString();
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
        if (value >= 10 || value < 0)
            return;
        countImage.sprite = countSprites[value];
        countImage.SetNativeSize();

        countImage.gameObject.SetActive(true);
        countSpeechBubbleImage.gameObject.SetActive(true);
    }

    public void ClearCountSpeechBubble()
    {
        countSpeechBubbleImage.gameObject.SetActive(false);
    }

    public void SettingItemUIImage(int type)
    {
        itemUIImage.sprite = itemUISprites[type - 14]; // type == 0 +air, type == 1 -air
        itemUIImage.SetNativeSize();

        Vector2 pos;
        pos.x = itemUIPos.position.x;
        pos.y = itemUIPos.position.y;

        itemUIImage.transform.position = pos;

        StartCoroutine(UIImageClear());
    }

    IEnumerator UIImageClear()
    {
        itemUIImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        itemUIImage.gameObject.SetActive(false);
    }

    public void Fade()
    {
        StartCoroutine(FadeRoutine());
    }

    IEnumerator FadeRoutine()
    {
        Color fadeColor = FadeScreen.color;

        FadeScreen.gameObject.SetActive(true);

        while (fadeColor.a < 1)
        {
            fadeColor.a += fadeSpeed * Time.deltaTime;
            FadeScreen.color = fadeColor;
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        while (fadeColor.a > 0)
        {
            fadeColor.a -= fadeSpeed * Time.deltaTime;
            FadeScreen.color = fadeColor;
            yield return null;
        }

        FadeScreen.gameObject.SetActive(false);
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutRoutine());
    }

    IEnumerator FadeOutRoutine()
    {
        Color fadeColor = FadeScreen.color;
        fadeColor.a = 1f;
        FadeScreen.color = fadeColor;

        FadeScreen.gameObject.SetActive(true);

        while (fadeColor.a < 1)
        {
            fadeColor.a += fadeSpeed * Time.deltaTime;
            FadeScreen.color = fadeColor;
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        while (fadeColor.a > 0)
        {
            fadeColor.a -= fadeSpeed * Time.deltaTime;
            FadeScreen.color = fadeColor;
            yield return null;
        }

        FadeScreen.gameObject.SetActive(false);
    }
}
