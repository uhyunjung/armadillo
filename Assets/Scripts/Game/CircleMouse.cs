using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class CircleMouse : MonoBehaviour
{
    BulletBtn bulletBtn;
    public Vector3 mousePosition;

    int shotNum = 3;                     // 총 탄막 개수(마우스 클릭 횟수) 제한
    public int cnt = 0;
    public bool isFinish = false;
    bool isFire = false;

    public GameObject skillRange;        // 스킬 범위 최상위 오브젝트
    GameObject[] skillObject;            // 스킬 범위 오브젝트

    void Start()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber != GameObject.Find("RoomManager").GetComponent<Room>().bossActorNum)
        {
            gameObject.SetActive(false);
        }
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
                        if (bulletBtn.num == 0)
                        {
                            // 마우스 따라다니기
                            gameObject.GetComponent<SpriteRenderer>().enabled = true;
                            gameObject.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1));

                            if (Input.GetMouseButton(0))
                            {
                                if ((!isFire) && (cnt < shotNum) && (!isFinish))
                                {
                                    isFinish = true;
                                    mousePosition = this.transform.position;
                                    StartCoroutine(makeCircle());
                                    cnt++;
                                }
                            }
                            else
                            {
                                isFire = false;
                            }
                        }
                        else
                        {
                            gameObject.GetComponent<SpriteRenderer>().enabled = false;
                        }
                    }
                }
            }
        }
    }

    IEnumerator makeCircle()
    {
        PhotonNetwork.Instantiate("FollowingMouse", mousePosition, Quaternion.identity);
        isFire = true;
        yield break;
    }
}
