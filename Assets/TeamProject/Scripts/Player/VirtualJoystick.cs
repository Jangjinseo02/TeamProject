using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
	private Image BackGroundImage; //첫 터치 배경 이미지
	private Image imageBackground;  // 우리가 실제 터치하는 컨트롤러의 배경 이미지
	private Image imageController;  // 터치 정보에 따라 위치가 바뀌는 컨트롤러 이미지
	private Vector2 touchPosition;      // 터치 위치를 외부로 보내기 위해 지역변수가 아닌 멤버변수로 선언

	private Vector2 inputPos;

	// x, y 방향 값을 외부에서 열람할 수 있도록 Get 전용 프로퍼티 정의
	public float Horizontal => touchPosition.x;
	public float Vertical => touchPosition.y;

	private void Awake()
	{
		BackGroundImage = GetComponent<Image>();
		imageBackground = transform.GetChild(0).GetComponent<Image>();
		imageController = imageBackground.transform.GetChild(0).GetComponent<Image>();
	}

	/// <summary>
	/// 터치하는 순간 1회 호출
	/// </summary>
	public void OnPointerDown(PointerEventData eventData)
	{
		Debug.Log("Touch Began : " + eventData);
		inputPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		imageBackground.gameObject.transform.position = inputPos;
		imageBackground.gameObject.SetActive(true);
	}

	/// <summary>
	/// 터치 상태로 드래그할 때 매 프레임 호출
	/// </summary>
	public void OnDrag(PointerEventData eventData)
	{
		//Vector2 touchPosition = Vector2.zero;
		touchPosition = Vector2.zero;

		// 조이스틱의 위치가 어디에 있든 동일한 값을 연산하기 위해
		// touchPosition의 위치 값은 이미지의 현재 위치를 기준으로
		// 얼마나 떨어져 있는지에 따라 다르게 나온다
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
			imageBackground.rectTransform, eventData.position, eventData.pressEventCamera, out touchPosition))
		{
			// touchPosition 값의 정규화 [0 ~ 1]
			// touchPosition을 이미지 크기로 나눔
			touchPosition.x = (touchPosition.x / imageBackground.rectTransform.sizeDelta.x);
			touchPosition.y = (touchPosition.y / imageBackground.rectTransform.sizeDelta.y);

			// touchPosition 값의 정규화 [-n ~ n]
			// 왼쪽(-1), 중심(0), 오른쪽(1)로 변경하기 위해 touchPosition.x*2-1
			// 아래(-1), 중심(0), 위(1)로 변경하기 위해 touchPosition.y*2-1
			// 이 수식은 Pivot에 따라 달라진다. (좌하단 Pivot 기준)
			touchPosition = new Vector2(touchPosition.x * 2 - 0, touchPosition.y * 2 - 0);

			// touchPosition 값의 정규화 [-1 ~ 1]
			// 가상 조이스틱 배경 이미지 밖으로 터치가 나가게 되면 -1 ~ 1보다 큰 값이 나올 수 있다
			// 이 때 normailzed를 이용해 -1 ~ 1사이의 값으로 정규화
			touchPosition = (touchPosition.magnitude > 1) ? touchPosition.normalized : touchPosition;

			// 가상 조이스틱 컨트롤러 이미지 이동
			imageController.rectTransform.anchoredPosition = new Vector2(
				touchPosition.x * imageBackground.rectTransform.sizeDelta.x / 2,
				touchPosition.y * imageBackground.rectTransform.sizeDelta.y / 2);

			//좌우 이동속도 같게
			if (touchPosition.x > 0)
				touchPosition.x = Vector2.right.x;
			else if (touchPosition.x < 0)
				touchPosition.x = Vector2.left.x;

			//일정 y값 이상일 경우 좌우 이동 멈추기
			if (touchPosition.y > 0.45f || touchPosition.y < -0.45f)
				touchPosition.x = 0;
			if (touchPosition.x > 0.45f || touchPosition.x < -0.45f)
				touchPosition.y = 0;
		}
	}

	/// <summary>
	/// 터치를 종료하는 순간 1회 호출
	/// </summary>
	public void OnPointerUp(PointerEventData eventData)
	{
		// 터치 종료시 이미지의 위치를 중앙으로 이동
		imageController.rectTransform.anchoredPosition = Vector2.zero;
		// 터치 종료시 touchPosition 값도 (0, 0)으로 초기화
		InputManager.Instance.ExitEnter();
		touchPosition = Vector2.zero;
		imageBackground.gameObject.SetActive(false);
	}
}


