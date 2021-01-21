using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletBtn : MonoBehaviour
{
    public ToggleGroup bulletBtn;                             // 탄막 버튼
    public GameObject followingMouse;                         // 마우스 따라다니는 오브젝트
    public int num = 0;                                       // 탄막 선택 번호
    Toggle[] toggles;

    void Start()
    {
        toggles = bulletBtn.GetComponentsInChildren<Toggle>(false);          // 토글 버튼
        followingMouse.transform.GetChild(0).gameObject.SetActive(true);     // 시작 시 1번 On
        for(int i=1; i<6; i++)
        {
            followingMouse.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    void Update()
    {
        for(int i=0; i<6; i++)
        {
            if (Input.GetKeyDown((KeyCode)(i+49)))                          // 키보드 숫자 1은 49 ~ 6은 54
            {
                chooseBullet(i);
            }
        }

        // 마우스 따라다니기
        Vector3 newPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        followingMouse.transform.position = new Vector3(newPosition.x, newPosition.y, 0);
    }

    void chooseBullet(int n)
    {
        toggles[n].isOn = true;                                                   // 탄막 버튼 선택
        followingMouse.transform.GetChild(num).gameObject.SetActive(false);       // 이전 선택된 오브젝트 Off
        followingMouse.transform.GetChild(n).gameObject.SetActive(true);          // 해당 오브젝트 On
        num = n;                                                                  // 선택한 탄막 번호 저장
    }
}
