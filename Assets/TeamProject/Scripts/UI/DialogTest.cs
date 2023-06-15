using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DialogTest : MonoBehaviour
{
    [SerializeField]
    private DialogSystem dialogSystem01;
    [SerializeField]
    private GameObject blackImage;
    [SerializeField]
    private DialogSystem dialogSystem02;
    [SerializeField]
    private Image backGround01;
    [SerializeField]
    private Image backGround02;

    private IEnumerator Start()
    {
        backGround01.gameObject.SetActive(true);
        blackImage.SetActive(false);

        // 첫 번째 대사 분기 시작
        yield return new WaitUntil(() => dialogSystem01.UpdateDialog());

        // 대사 분기 사이에 원하는 행동을 추가할 수 있다.
        // 캐릭터를 움직이거나 아이템을 획득하는 등의.. 현재는 5-4-3-2-1 카운트 다운 실행
        blackImage.SetActive(true);
        int count = 1;
        while (count > 0)
        {
            count--;

            yield return new WaitForSeconds(1);
        }
        blackImage.SetActive(false);
        backGround01.gameObject.SetActive(false);
        backGround02.gameObject.SetActive(true);

        // 두 번째 대사 분기 시작
        yield return new WaitUntil(() => dialogSystem02.UpdateDialog());

        blackImage.gameObject.SetActive(true);

        yield return new WaitForSeconds(2);

        //UnityEditor.EditorApplication.ExitPlaymode();

        SceneManager.LoadScene("SampleScene");
    }
}

