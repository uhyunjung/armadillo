using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

// 탄막 2번 스킬 범위
public class SLRange : MonoBehaviour
{
    BulletBtn bulletBtn;  // 탄막 버튼 스크립트
    public PhotonView pv;
    public PhotonView pvLaser;

    SpriteRenderer rangesr;

    void Update()
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
                        if (Input.GetMouseButtonDown(0) && bulletBtn.num == 1 && gameObject.GetComponent<LaserRotation>().check)
                        {
                            pv.RPC("FadeIn", RpcTarget.All, 1f);
                        }
                    }
                }
            }
        }
    }

    [PunRPC]
    public void FadeIn(float fadeOutTime)
    {
        StartCoroutine(CoFadeIn(fadeOutTime));
    }

    IEnumerator CoFadeIn(float fadeOutTime)  // 2초 스킬 범위
    {
        gameObject.GetComponent<LaserRotation>().check = false;
        gameObject.GetComponent<LaserRotation>().isFinish = false;
        rangesr = this.gameObject.GetComponent<SpriteRenderer>();

        // 투명도 0
        Color tempColor = rangesr.color;
        tempColor.a = 0f;
        rangesr.color = tempColor;
        while (tempColor.a < 0.5f)
        {
            tempColor.a += Time.deltaTime / fadeOutTime;
            rangesr.color = tempColor;

            if (tempColor.a >= 0.5f)
            {
                tempColor.a = 0f;
                rangesr.color = tempColor;
                break;
            }

            yield return null;
        }

        pvLaser.RPC("Laser", RpcTarget.All, transform.rotation);
        this.transform.rotation = new Quaternion(0, 0, 0, 0);
    }
}
