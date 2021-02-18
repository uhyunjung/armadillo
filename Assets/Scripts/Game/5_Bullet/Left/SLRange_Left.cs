using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class SLRange_Left : MonoBehaviour
{
    BulletBtn bulletBtn;  // 탄막 버튼 스크립트
    public PhotonView pv;

    public bool check = true;
    SpriteRenderer rangesr;

    Vector2 MousePosition;

    void Update()
    {
        if (SceneManager.GetActiveScene().name.Equals("Game Scene"))
        {
            if (GameObject.Find("RoomManager") != null)
            {
                if (PhotonNetwork.LocalPlayer.ActorNumber == GameObject.Find("RoomManager").GetComponent<Room>().bossActorNum)
                {
                    bulletBtn = GameObject.Find("BulletBtn").GetComponent<BulletBtn>();
                    int selectNum = bulletBtn.num;
                    if (selectNum == 4)
                    {
                        MousePosition = Input.mousePosition;
                        MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                        //왼쪽에 가까울때만 스킬 적용
                        if (Input.GetMouseButtonDown(0) && bulletBtn.num == 4 && check && MousePosition.x < 0)
                        {
                            gameObject.GetComponent<LaserLotation_Left>().isFinish = false;
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
        check = false;
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

        this.transform.rotation = new Quaternion(0, 0, 0, 0);
        check = true;
    }
}
