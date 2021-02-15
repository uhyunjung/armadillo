using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class StageLight : MonoBehaviour
{
    /* 
     * [스킬 4번]과 관련된 코드입니다.
     * - 숫자키 4번으로 스테이지 조명 스킬을 선택했을 때, 스킬 범위를 출력시키는 코드
     * - 스테이지 조명 스프라이트의 출력과 점멸을 제어하는 코드
     */
    public PhotonView pv;
    BulletBtn bulletBtn;
    public GameObject TotalStageLight;        // 스킬 4번 전체 스프라이트 (조명 + 조명대)
    public GameObject FlashingLight;          // 조명 부분
    public GameObject SLRange;                // 숫자키 4번 입력 시 화면에 출력되는 스킬범위 스프라이트
    private SpriteRenderer renderer;
    private bool check = true;                // 중복 입력 방지를 위한 플래그

    [PunRPC]
    void setAct(bool val)
    {
        SLRange.SetActive(val);
    }

    [PunRPC]
    // 조명 점멸제어 코루틴 호출 함수 
    public void Flash()
    {
        StartCoroutine(CoFlash());
    }

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == GameObject.Find("RoomManager").GetComponent<Room>().bossActorNum)
        {
            bulletBtn = GameObject.Find("BulletBtn").GetComponent<BulletBtn>();
        }
    }

    // 조명 점멸제어 코루틴
    IEnumerator CoFlash()
    {
        check = false;                          // 플래그를 false로 전환
        int SLcount = 0;

        yield return new WaitForSeconds(2);
        TotalStageLight.SetActive(true);        // 전체 스프라이트 등장

        yield return new WaitForSeconds(4);
        FlashingLight.SetActive(false);         // 조명 OFF

        while (SLcount < 2)                     // 조명 점멸을 2회 반복
        {
            yield return new WaitForSeconds(2);
            FlashingLight.SetActive(true);      //  조명 ON
            yield return new WaitForSeconds(4);
            FlashingLight.SetActive(false);     //  조명 OFF
            SLcount++;
        }
        TotalStageLight.SetActive(false);       // 패턴 종료 (전체 스프라이트 비활성화)
        FlashingLight.SetActive(true);          // 조명은 다시 Active 상태로 복구
        yield return new WaitForSeconds(1);     // 1초 delay
        check = true;                           // 플래그를 true로 전환
        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == GameObject.Find("RoomManager").GetComponent<Room>().bossActorNum)
        {
            if (bulletBtn.num == 3 && check)        // 스킬범위 표시
            {
                pv.RPC("setAct", RpcTarget.All, true);
            }
            else
            {
                pv.RPC("setAct", RpcTarget.All, false);           // 마우스 입력 OR 다른 스킬 선택 시, 스킬범위 표시를 중지함
            }

            if (Input.GetMouseButtonDown(0) && bulletBtn.num == 3 && check) // 실제로 스킬을 발동시키는 경우
            {
                pv.RPC("Flash", RpcTarget.All);                            // 스킬 발동
            }
        }
    }
}
