using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

// 탄막 2번
public class SLRange : MonoBehaviour
{
    BulletBtn bulletBtn;  // 탄막 버튼 스크립트
    public PhotonView pv;

    private bool check = true;
    public float timer;
    public int waitingTime;

    SpriteRenderer rangesr;
    //public GameObject SLRng;
    // private bool state;          추후에 다른 스킬들이 추가될 경우, 스킬의 발동 상태를 표시하기 위한 플래그 변수(중복 체크용)

    [PunRPC]
    public void FadeIn(float fadeOutTime)
    {
        StartCoroutine(CoFadeIn(fadeOutTime));
    }

    IEnumerator CoFadeIn(float fadeOutTime)
    {
        check = false;
        rangesr = this.gameObject.GetComponent<SpriteRenderer>();
        Color tempColor = rangesr.color;
        while (tempColor.a < 0.5f)
        {
            tempColor.a += Time.deltaTime / fadeOutTime;
            rangesr.color = tempColor;

            if (tempColor.a >= 0.5f) tempColor.a = 0.5f;

            yield return null;
        }

        rangesr.color = tempColor;
        this.transform.rotation = new Quaternion(0, 0, 0, 0);
        check = true;
    }

    void Start()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == GameObject.Find("RoomManager").GetComponent<Room>().bossActorNum)
        {
            bulletBtn = GameObject.Find("BulletBtn").GetComponent<BulletBtn>();
        }

        timer = 0.0F;
        waitingTime = 2;
        //SLRng = GameObject.Find("StageLightRange");
    }

    void Update()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == GameObject.Find("RoomManager").GetComponent<Room>().bossActorNum)
        {
            if (Input.GetMouseButtonDown(0) && bulletBtn.num == 1 && check)
            {
                pv.RPC("FadeIn", RpcTarget.All, 2f);
                timer += Time.deltaTime;
                if (timer > waitingTime)
                {
                    //Action
                    //SLRng.SetActive(false);
                }
            }
        }

        //state = false;   // 추후에 추가될 플래그 변수
    }
}
