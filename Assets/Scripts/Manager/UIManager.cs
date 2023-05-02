using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : SingleTon<UIManager>
{
    [SerializeField] Image selectBackGround;
    [SerializeField] Image airImage;
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

    public void SettingAirImage()
    {
        Vector2 pos;
        pos.x = GameManager.Instance.player.transform.position.x + 0.5f;
        pos.y = GameManager.Instance.player.transform.position.y + 0.5f;

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
