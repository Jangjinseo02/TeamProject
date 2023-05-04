using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : SingleTon<UIManager>
{
    [SerializeField] Image selectBackGround;
    [SerializeField] Image airImage;
    [SerializeField] Sprite[] speechBubbleSprites;
    [SerializeField] Transform speechBubblePos;

    [SerializeField] Text hpText;
    [SerializeField] Text oxygenText;

    private void LateUpdate()
    {
        
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

    public void UpdateHpText(float hp)
    {
        
        hpText.text = ((int)hp).ToString() + "/100";
    }
    public void UpdateOxygenText(float oxygen)
    {
        oxygenText.text = ((int)oxygen).ToString() + "/100";
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
