using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
/*
 탄막 6번 스피커 왼쪽 등장 관련 스크립트입니다.
 */

public class left_move : MonoBehaviour
{
    public PhotonView pv;
    int cnt = 8;

    Vector2 MousePosition;
    Vector3 originPos;
    Camera Camera;

    SpriteRenderer RangespriteRenderer;
    SpriteRenderer ArmspriteRenderer;
    GameObject rightAmp; //비활성화 하기 위해
    BulletBtn bulletBtn; //탄막 버튼 스크립트
    int selectNum = 0; //탄막 선택 번호

    //클론으로 만든 오브젝트들을 각각 다루기 위함
    // public GameObject prefab;
    public GameObject[] prefab = new GameObject[6];
    List<GameObject> bulletList = new List<GameObject>();

    //bool isleftHandAppear; //좌/우 동시 등장 방지
    bool leftskillStart;
    void Start()
    {
        originPos = transform.localPosition;
        ArmspriteRenderer = GetComponent<SpriteRenderer>();
        RangespriteRenderer = GameObject.FindGameObjectWithTag("skillRangeL").GetComponent<SpriteRenderer>();

        rightAmp = GameObject.FindGameObjectWithTag("ampRight");
        Camera = GameObject.Find("Main Camera").GetComponent<Camera>();

        ArmspriteRenderer.color = new Color32(255, 255, 255, 0);
        RangespriteRenderer.color = new Color32(255, 255, 255, 0);

    }

    // Update is called once per frame
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
                        selectNum = bulletBtn.num;

                        if (Input.GetMouseButtonUp(0))
                        {
                            if (selectNum == 5)
                            {
                                MousePosition = Input.mousePosition;
                                MousePosition = Camera.ScreenToWorldPoint(MousePosition);
                                if (MousePosition.x < 0)
                                {
                                    Debug.Log("<<왼쪽 스킬 발동>>");
                                    //스킬 중복 발동 방지
                                    if (!leftskillStart && GameObject.FindGameObjectWithTag("ampLeft").activeSelf)
                                    {
                                        rightAmp.SetActive(false);
                                        pv.RPC("startSkill", RpcTarget.All);
                                        // StartCoroutine("skill_start");
                                        // pv.RPC("random_bullet", RpcTarget.All);
                                        pv.RPC("endingSkill", RpcTarget.All);
                                        //Invoke("skillEnd", 8);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    [PunRPC]
    public void startSkill()
    {
        StartCoroutine("skill_start");
    }
    //알파값 변화 함수
    IEnumerator skill_start()
    {
        Debug.Log("1. 알파값 변화 시작");
        leftskillStart = true;
        for (float i = 0f; i <= 1f; i += 0.1f)
        {
            //스프라이트 렌더러를 통해 문어발은 1까지 스킬 범위는 0.5까지만
            ArmspriteRenderer.color = new Color(ArmspriteRenderer.color.r, ArmspriteRenderer.color.g, ArmspriteRenderer.color.b, i);
            RangespriteRenderer.color = new Color(ArmspriteRenderer.color.r, ArmspriteRenderer.color.g, ArmspriteRenderer.color.b, i / 2f);
            yield return new WaitForSeconds(.1f);
        }
        ArmspriteRenderer.color = new Color(ArmspriteRenderer.color.r, ArmspriteRenderer.color.g, ArmspriteRenderer.color.b, 1f);
        Debug.Log("2. 알파값 변화 종료");
        // Camera.main.GetComponent<cameraShake>().ShakeCamera(1.0f);코드 질문
     //   yield return new WaitForSeconds(.1f);
        random_bullet();
        Debug.Log("왼쪽 문어발 등장");

    }
    [PunRPC]
    public void endingSkill()
    {
        Invoke("skillEnd", 8);
    }
    [PunRPC]
    public void skillEnd()
    {
        for (int i = 0; i < cnt; i++)
            Destroy(bulletList[i]);
        StartCoroutine("skill_end");
        leftskillStart = false;
        rightAmp.SetActive(true);
    }

    IEnumerator skill_end()
    {
        Debug.Log("1. 알파값 변화 시작");

        for (float i = 1f; i >= 0f; i -= 0.1f)
        {
            //스프라이트 렌더러를 통해 문어발은 1까지 스킬 범위는 0.5까지만
            ArmspriteRenderer.color = new Color(ArmspriteRenderer.color.r, ArmspriteRenderer.color.g, ArmspriteRenderer.color.b, i);
            RangespriteRenderer.color = new Color(ArmspriteRenderer.color.r, ArmspriteRenderer.color.g, ArmspriteRenderer.color.b, i / 2f);
            yield return new WaitForSeconds(.1f);
        }
        ArmspriteRenderer.color = new Color(ArmspriteRenderer.color.r, ArmspriteRenderer.color.g, ArmspriteRenderer.color.b, 0f);
        RangespriteRenderer.color = new Color(ArmspriteRenderer.color.r, ArmspriteRenderer.color.g, ArmspriteRenderer.color.b, 0f);

        Debug.Log("2. 알파값 변화 종료");

    }
    //탄막 랜덤 생성 함수

    [PunRPC]
    public void random_bullet()
    {   //반복문을 통해 랜덤한 크기, 위치의 탄막 cnt개 생성
        Debug.Log("4. 탄막 생성");
        for (int i = 0; i < cnt; i++)
        {
            //크기 좌표(x,y) 오브젝트 랜덤으로 설정
            float randomS = UnityEngine.Random.Range(0.4f, 0.8f);
            float randomX = UnityEngine.Random.Range(-12f, -11f);
            float randomY = UnityEngine.Random.Range(2f, 3f);
            int randomO = UnityEngine.Random.Range(0, prefab.Length);


            Vector2 randomPos = new Vector2(randomX, randomY);
            //클론 생성

            GameObject obj = (GameObject)Instantiate(prefab[randomO], randomPos, Quaternion.identity);

            obj.transform.SetParent(ArmspriteRenderer.transform, true);
            obj.transform.localScale = new Vector3(randomS, randomS, 0);
            obj.GetComponent<Rigidbody2D>().gravityScale = UnityEngine.Random.Range(0.8f, 1.2f); //3
                                                                                                 //  obj.transform.SetParent(ArmspriteRenderer.transform, false); //프리팹 크기 일정하게

            bulletList.Add(obj);
            //test
            float randomSpeed = UnityEngine.Random.Range(8f, 10f);
            obj.GetComponent<Rigidbody2D>().velocity = transform.right * randomSpeed; ///10
        }

    }
}
