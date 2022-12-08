using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

// 탄막 3번
public class TimeSpeed : MonoBehaviour
{
    public PhotonView pv;
    public Sprite FastSprite;       //배속 sprite
    public Sprite SlowSprite;       //감속 sprite
    public Sprite normalSprite;     //보통 sprite
    BulletBtn bulletBtn;  // 탄막 버튼 스크립트

    bool check = true;
    int selectMode = 0;

    Controller_Time ct;
    SpriteRenderer spriteRenderer;
    void Start()
    {
        ct = GameObject.Find("TimeController").GetComponent<Controller_Time>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

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
                        if (Input.GetMouseButtonDown(0) && bulletBtn.num == 2 && check)
                        {
                            selectMode = Random.Range(1, 3);    //랜덤으로 1 혹은 2의 정수 생성
                            pv.RPC("startMode", RpcTarget.All, selectMode);
                        }
                    }
                }
            }
        }  
    }

    void modeReset()
    {
        spriteRenderer.sprite = normalSprite;
        Time.timeScale = 1.0f;
        ct.isFastSpeed = false;
        ct.isSlowSpeed = false;
    }

    void fastslow(int selectMode)
    {
        // 2배속
        if (selectMode == 1)
        {
            ct.isFastSpeed = true;
            Time.timeScale = 1.5f;
            spriteRenderer.sprite = FastSprite;
        }
        // 1배속
        else if (selectMode == 2)
        {
            ct.isSlowSpeed = true;
            Time.timeScale = 0.75f;
            spriteRenderer.sprite = SlowSprite;
        }
    }

    [PunRPC]
    public void startMode(int selectMode)
    {
        StartCoroutine(Cooltime(10, selectMode));
    }

    IEnumerator Cooltime(float cool, int selectMode)
    {
        check = false;
        while (cool > 0.0f)
        {
            cool -= Time.deltaTime;
            fastslow(selectMode);
            yield return null;
        }
        modeReset();
        check = true;
    }
}


