using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class LaserOn : MonoBehaviour
{
    /*
        실제 조명 스프라이트 (피격 판정되는 스프라이트)의 깜빡임을 제어하는 코드
        Range가 2초동안 FadeIn 될 때까지 대기하다가, 1초간 투명도를 높여 표시시킨 뒤
        다시 원래 위치(벽 쪽)로 이동시킨 뒤 투명하게 전환시키는 코드
    */

    public PhotonView pv;
    public GameObject armManager;
    public GameObject LaserRotationObj;
    SpriteRenderer rangesr;
    Color tempColor;

    public void Start()
    {
        rangesr = this.gameObject.GetComponent<SpriteRenderer>();
        tempColor = rangesr.color;
        tempColor.a = 0f;
        rangesr.color = tempColor;
    }

    [PunRPC]
    public void Laser(Quaternion rotation)
    {
        StartCoroutine(CoLaser(rotation));
    }

    IEnumerator CoLaser(Quaternion rotation)
    {
        tempColor = rangesr.color;
        transform.rotation = rotation;

        // 레이저 표시
        tempColor.a = 1f;
        rangesr.color = tempColor;
        yield return new WaitForSeconds(2); //2초간 레이저 출력

        //이후 레이저 위치 원상복귀 및 투명하게 전환
        armManager.GetComponent<ArmManager>().setColorZero();
        tempColor.a = 0f;
        rangesr.color = tempColor;
        this.transform.rotation = new Quaternion(0, 0, 0, 0);
        LaserRotationObj.GetComponent<SLRange>().check = true;
        LaserRotationObj.GetComponent<LaserRotation>().isFinish = true;
        yield return null;
    }
}
