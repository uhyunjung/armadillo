using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class BulletBtn : MonoBehaviour
{
    public ToggleGroup bulletBtn;                             // 탄막 버튼
    public int num = 0;                                       // 탄막 선택 번호
    Toggle[] toggles;
    float time = 0f;

    void Start()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber != GameObject.Find("RoomManager").GetComponent<Room>().bossActorNum)
        {
            gameObject.SetActive(false);
        }
        toggles = bulletBtn.GetComponentsInChildren<Toggle>(false);          // 토글 버튼
        toggles[0].isOn = true;
        if(gameObject.activeSelf)
        {
            StartCoroutine(chooseBullet(0));
        }
    }

    void Update()
    {
        if (time == 0)
        {
            for (int i = 0; i < 6; i++)
            {
                if (Input.GetKeyDown((KeyCode)(i + 49)))                          // 키보드 숫자 1은 49 ~ 6은 54
                {
                    StartCoroutine(chooseBullet(i));
                }
            }
        }
    }

    IEnumerator chooseBullet(int n)
    {
        if (num != n)
        {
            toggles[n].isOn = true;                                                   // 탄막 버튼 선택
            toggles[num].isOn = false;
            num = n;                                                                  // 선택한 탄막 번호 저장

            while (time < 3)
            {
                time += Time.deltaTime;
                yield return null;
            }
            time = 0;
        }
    }
}
