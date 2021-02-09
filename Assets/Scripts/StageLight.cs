using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class StageLight : MonoBehaviour
{
    /* 
     * [스킬 4번]과 관련된 코드입니다.
     * - 숫자키 4번으로 스테이지 조명 스킬을 선택했을 때, 스킬 범위를 출력시키는 코드
     * - 스테이지 조명 스프라이트의 출력과 점멸을 제어하는 코드
     */

    BulletBtn bulletBtn;
    public GameObject TotalStageLight;        // 스킬 4번 전체 스프라이트 (조명 + 조명대)
    public GameObject FlashingLight;          // 조명 부분
    public GameObject SLRange;                // 숫자키 4번 입력 시 화면에 출력되는 스킬범위 스프라이트
    private SpriteRenderer renderer;
    private bool check = true;                // 중복 입력 방지를 위한 플래그

    // 조명 점멸제어 코루틴 호출 함수 
    public void Flash()
    {
        StartCoroutine(CoFlash());
    }

    // Start is called before the first frame update
    void Start()
    {
        bulletBtn = GameObject.Find("BulletBtn").GetComponent<BulletBtn>();
    }

    // 조명 점멸제어 코루틴
    IEnumerator CoFlash()
    {
        check = false;                          // 플래그를 false로 전환
        int SLcount = 0;
        float cool = 4.0f;

        yield return new WaitForSeconds(2);
        TotalStageLight.SetActive(true);        // 전체 스프라이트 등장

        while (cool > 0.0f)                 //카메라 흔들기
        {
            cool -= Time.deltaTime;
            GameObject.Find("Main Camera").GetComponent<CameraShake>().Shake();
            yield return null;
        }
        cool = 4.0f;
        GameObject.Find("Main Camera").GetComponent<CameraShake>().cameraReset();   //카메라 흔들림 초기화

        //yield return new WaitForSeconds(4);
        FlashingLight.SetActive(false);         // 조명 OFF
        

        while (SLcount < 2)                     // 조명 점멸을 2회 반복
        {
            yield return new WaitForSeconds(2);
            FlashingLight.SetActive(true);      //  조명 ON

            while (cool > 0.0f)                 //카메라 흔들기
            {
                cool -= Time.deltaTime;
                GameObject.Find("Main Camera").GetComponent<CameraShake>().Shake();
                yield return null;
            }
            cool = 4.0f;
            //yield return new WaitForSeconds(4);

            FlashingLight.SetActive(false);     //  조명 OFF
            GameObject.Find("Main Camera").GetComponent<CameraShake>().cameraReset();   //카메라 흔들림 초기화
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
        if (bulletBtn.num == 3 && check)        // 스킬범위 표시
        {
            SLRange.SetActive(true);
        }
        else
        {
            SLRange.SetActive(false);           // 마우스 입력 OR 다른 스킬 선택 시, 스킬범위 표시를 중지함
        }


        if (Input.GetMouseButtonDown(0) && bulletBtn.num == 3 && check) // 실제로 스킬을 발동시키는 경우
        {
            Flash();                            // 스킬 발동
        }
    }
}
