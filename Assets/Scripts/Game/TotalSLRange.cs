using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class TotalSLRange : MonoBehaviour
{
    /* 
    * [스킬 4번]과 관련된 코드입니다.
    * 스테이지 조명의 전체 스킬 범위 범위를 페이드인 시키는 코드
    */

    public PhotonView pv;
    BulletBtn bulletBtn;
    private bool check = true;                // 중복 입력 방지를 위한 플래그
    SpriteRenderer rangesr;

    // 전체 스킬범위 페이드인 코루틴 호출 함수
    [PunRPC]
    public void FadeIn(float fadeTime)
    {
        StartCoroutine(CoFadeIn(fadeTime));
    }

    //  전체 스킬범위 페이드인 코루틴
    IEnumerator CoFadeIn(float fadeTime)
    {
        check = false;                                     // 플래그를 false로 전환
        rangesr = this.gameObject.GetComponent<SpriteRenderer>();
        Color tempColor = rangesr.color;
        while (tempColor.a < 0.5f)
        {
            tempColor.a += Time.deltaTime / fadeTime;       // 알파값을 0.5까지 증가 
            rangesr.color = tempColor;                      // 증가시킨 알파값 적용

            if (tempColor.a >= 0.5f) tempColor.a = 0.5f;
            yield return null;
        }
        rangesr.color = tempColor;
        tempColor.a = 0f;                                   //스킬범위 투명화
        rangesr.color = tempColor;
        yield return new WaitForSeconds(17);                // 스킬 발동 시간 + 1초 delay
        check = true;                                       // 플래그를 true로 전환
    }

    void Start()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == GameObject.Find("RoomManager").GetComponent<Room>().bossActorNum)
        {
            bulletBtn = GameObject.Find("BulletBtn").GetComponent<BulletBtn>();
        }
    }

    void Update()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == GameObject.Find("RoomManager").GetComponent<Room>().bossActorNum)
        {
            if (Input.GetMouseButtonDown(0) && bulletBtn.num == 3 && check)
            {
                pv.RPC("FadeIn", RpcTarget.All, 4f);                                     // 페이드인
            }
        }
    }
}
