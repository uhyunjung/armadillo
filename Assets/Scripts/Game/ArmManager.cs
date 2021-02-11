using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ArmManager : MonoBehaviour
{
    /*
     탄막 2번 스피커 등장 해제 함수입니다.
     */

    BulletBtn bulletBtn; // 탄막 버튼 스크립트
    public PhotonView pv;
    SpriteRenderer spr;
    Color color;

    private void Start()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == GameObject.Find("RoomManager").GetComponent<Room>().bossActorNum)
        {
            bulletBtn = GameObject.Find("BulletBtn").GetComponent<BulletBtn>();
        }
        spr = GetComponent<SpriteRenderer>();        //스프라이트 렌더러 선언
        color = spr.color;
        pv.RPC("setColor", RpcTarget.All, 0);
    }
    private void Update()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == GameObject.Find("RoomManager").GetComponent<Room>().bossActorNum)
        {
            if (bulletBtn.num == 1)             //활성화
            {
                //Debug.Log("2번 탄환 실행");
                pv.RPC("setColor", RpcTarget.All, 1);
            }
            else if (bulletBtn.num != 1)   //bulletBtn.num != 1일때 비활성화
            {
                //Debug.Log("2번 탄환 종료");
                pv.RPC("setColor", RpcTarget.All, 0);
            }
        }
    }

    [PunRPC]
    void setColor(int value)
    {
        color.a = value;
        spr.color = color;
    }
}