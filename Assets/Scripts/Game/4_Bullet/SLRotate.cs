using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SLRotate : MonoBehaviour
{
    /* 
    * [스킬 4번]과 관련된 코드입니다.
    * 조명 스프라이트 회전 코드
    */

    float rotateSpeed = -22.5f;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, Time.deltaTime * rotateSpeed, Space.World);      // 회전
    }
}