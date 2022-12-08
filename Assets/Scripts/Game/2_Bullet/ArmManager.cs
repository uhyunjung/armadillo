using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        spr = GetComponent<SpriteRenderer>();        //스프라이트 렌더러 선언
        color = spr.color;
        pv.RPC("setColor", RpcTarget.All, 0);
    }
    private void Update()
    {
        if (SceneManager.GetActiveScene().name.Equals("Game Scene"))
        {
            if (GameObject.Find("RoomManager") != null)
            {
                if (PhotonNetwork.LocalPlayer.ActorNumber == GameObject.Find("RoomManager").GetComponent<Room>().bossActorNum)
                {
                    bulletBtn = GameObject.Find("BulletBtn").GetComponent<BulletBtn>();

                    if (bulletBtn)
                    {
                        if (Input.GetMouseButtonDown(0) && (bulletBtn.num == 1))             //활성화
                        {
                            //Debug.Log("2번 탄환 실행");
                            pv.RPC("setColor", RpcTarget.All, 1);
                        }
                    }
                }
            }
        }
    }

    [PunRPC]
    void setColor(int value)
    {
        spr = GetComponent<SpriteRenderer>();
        color.a = value;
        spr.color = color;
    }

    public void setColorZero()
    {
        pv.RPC("setColor", RpcTarget.All, 0);
    }
}