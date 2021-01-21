using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    BulletBtn bulletBtn;  // 탄막 버튼 스크립트
    int selectNum;  // 탄막 선택 번호
    public float ShakeAmount;
    public float ShakeTime;
    Vector3 initialPosition;

    public void VibrateForTime(float time)
    {
        ShakeTime = time;
    }

    private void Start()
    {
        bulletBtn = GameObject.Find("BulletBtn").GetComponent<BulletBtn>();
        
        initialPosition = new Vector3(0f, 0f, -10f);
    }

    private void Update()
    {
        selectNum = bulletBtn.num;  // 탄막 선택 번호

        if (Input.GetMouseButtonDown(0)&&selectNum==1)
        {
            while (ShakeTime >0)
            {
                if (ShakeTime > 0)
                {
                    transform.position = Random.insideUnitSphere * ShakeAmount + initialPosition;
                    ShakeTime -= Time.deltaTime;
                }
                else
                {
                    ShakeTime = 0.0f;
                    transform.position = initialPosition;
                }
            }
            
        }
    }
}