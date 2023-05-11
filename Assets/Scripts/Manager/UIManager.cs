using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : SingleTon<UIManager>
{
    PlayerMoveMent player;

    [SerializeField] Image selectBackGround;
    [SerializeField] Image airImage;
    [SerializeField] Sprite[] speechBubbleSprites;
    [SerializeField] Transform speechBubblePos;

    [SerializeField] Image resultScreen;

    [SerializeField] Text hpText;
    [SerializeField] Text oxygenText;
    [SerializeField] Text depthText;

    private void Awake()
    {
        player = GameManager.Instance.player.GetComponent<PlayerMoveMent>();
    }

    private void LateUpdate()
    {
        UpdateHpText(player.hp);
        UpdateOxygenText(player.oxygen);
        UpdateDepthText(player.transform.position.y * (-1));
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

    public void SettingAirImage(int type)
    {
        airImage.sprite = speechBubbleSprites[type]; // type == 0 +air, type == 1 -air

        if (type == 0)
            airImage.rectTransform.pivot = new Vector2(1, 0);
        else if (type == 1)
            airImage.rectTransform.pivot = new Vector2(0, 0);
       
        

        Vector2 pos;
        pos.x = speechBubblePos.position.x;
        pos.y = speechBubblePos.position.y;

        airImage.transform.position = pos;

        StartCoroutine(ImageClear());
    }

    IEnumerator ImageClear()
    {
        airImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        airImage.gameObject.SetActive(false);
    }
}
