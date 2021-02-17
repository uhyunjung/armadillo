using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class DashMove : MonoBehaviour, IPunObservable
{
    // 객체 식별하는 포톤뷰
    public PhotonView pv;
    Room room;

    //max 체력과 현재 체력 변수
    public float maxHealth = 120f;
    public float currentHealth;
    float shotHealth = 10f;  //피격 시 감소 체력, 10번 피격 시 죽음, 25에서 10으로

    public Vector2 speed_vec; //플레이어 속도 벡터
    public float speed; //이동 속도
    private Rigidbody2D rb; //리지드바디

    private float dashTime; //대쉬 지속시간
    public float startDashTime; //대쉬 지속시간(초기화 값)
    public bool isDash; //대쉬 여부

    public bool isUnBeatTime; //무적 여부
    public bool isDie = false; //죽음 여부
    public bool beShot;
    public bool isDelay; //딜레이 여부
    public float delayTime = 0.2f; //대쉬 딜레이 시간 ,0.5 -> 0.2로 변경

    SpriteRenderer spriteRenderer; //스프라이트 렌더러
    Animator anim; //애니메이터

    //hp바, fill area 조절
    public Slider HpBar;
    public GameObject fillArea;
    bool isBoss = false;
    Vector3 worldpos;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        dashTime = startDashTime;
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        currentHealth = maxHealth;

        // hp값 초기화
        HpBar.gameObject.SetActive(false);
        HpBar.value = 1f;

        pv.RPC("NotDestroy", RpcTarget.All);
    }

    void Update()
    {
        if (pv.IsMine)
        {
            if (SceneManager.GetActiveScene().name.Equals("Game Scene"))
            {
                // 보스일 경우, 플레이어 삭제
                if ((!isBoss) && (pv.OwnerActorNr == GameObject.Find("RoomManager").GetComponent<Room>().bossActorNum))
                {
                    pv.RPC("BossOnOff", RpcTarget.All, pv.OwnerActorNr);
                }
                else
                {
                    pv.RPC("setHpBar", RpcTarget.All);
                }

                // 맵 밖으로 못나가게
                pv.RPC("setPosition", RpcTarget.All);

                //체력 체크해서 죽음 판정
                if (currentHealth == 0)
                {
                    //fill area 비활성화로 0임을 표시
                    fillArea.gameObject.SetActive(false);

                    if (!isDie)
                        pv.RPC("Die", RpcTarget.All);
                    return;
                }

                //체력바에 현재 체력 구현
                if (currentHealth != 0)
                    pv.RPC("setHpValue", RpcTarget.All);

                //애니메이션 - 스프라이트 방향 전환
                if (Input.GetButtonDown("Horizontal"))
                {
                    pv.RPC("FlipImg", RpcTarget.All, (int)Input.GetAxisRaw("Horizontal"));
                }

                //이동
                if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
                {
                    anim.SetBool("isWalking", true);
                    speed_vec.x = Input.GetAxis("Horizontal") * speed * 2; //속도 2배로 증가
                    speed_vec.y = Input.GetAxis("Vertical") * speed * 2;

                    rb.velocity = speed_vec;
                }
                else
                {
                    anim.SetBool("isWalking", false);
                    rb.velocity = Vector2.zero;
                }

                //대쉬
                if (isDash == false) //대쉬하고 있지 않음
                {
                    if (Input.GetKeyDown(KeyCode.Space)) //스페이스바가 눌렸을 때
                    {
                        if (isDelay == false) //딜레이 시간이 아닐 때
                        {
                            anim.SetBool("isDashing", true); //이동에서 대시로 애니메이션 전환
                            isDash = true; //대시 플래그 변경
                        }
                    }
                }

                else //대쉬중임
                {
                    if (dashTime <= 0) //dashTime이 0보다 작아지면 대쉬 끝
                    {
                        isDash = false; //플래그 초기화
                        dashTime = startDashTime; //dashTime 초기화
                        rb.velocity = Vector2.zero; //속도 0으로 초기화
                        anim.SetBool("isDashing", false); //대시에서 이동으로 애니메이션 변환

                        isDelay = true; //딜레이 플래그 변경
                        pv.RPC("StartDash", RpcTarget.All);
                    }
                    else  //dashTime이 0보다 크면 dashTime감소 및 속도 변화
                    {
                        dashTime -= Time.deltaTime;

                        speed_vec.x = Input.GetAxis("Horizontal") * speed * 4; //대쉬 중 이동속도 3배
                        speed_vec.y = Input.GetAxis("Vertical") * speed * 4;   //ㄴ기본 속도가 2배 돼서 4배로 수정함

                        rb.velocity = speed_vec;
                        pv.RPC("StartUnBeat", RpcTarget.All);
                    }
                }

                pv.RPC("MoveBar", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    void setPosition()
    {
        worldpos = Camera.main.WorldToViewportPoint(this.transform.position);
        if (worldpos.x < 0.125f) worldpos.x = 0.125f;
        if (worldpos.y < 0f) worldpos.y = 0f;
        if (worldpos.x > 0.875f) worldpos.x = 0.875f;
        if (worldpos.y > 1f) worldpos.y = 1f;
        this.transform.position = Camera.main.ViewportToWorldPoint(worldpos);
    }

    [PunRPC]
    void BossOnOff(int actorNum)
    {
        isBoss = true;
        HpBar.gameObject.SetActive(false);
        room = GameObject.Find("RoomManager").GetComponent<Room>();
        if (room.checkBoss.Count > 0)
        {
            this.gameObject.transform.position = room.bossPos[room.checkBoss.Count - 1];
            this.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector3(0, 0, 0); // 사망 후 속도 초기화
            room.checkBoss.Add(actorNum);
        }

        this.enabled = false;
    }

    [PunRPC]
    void setHpBar()
    {
        HpBar.gameObject.SetActive(true); //활성화
    }
    [PunRPC]
    void setHpValue()
    {
        HpBar.value = (float)currentHealth / (float)100; //현재 체력 업로드
    }

    [PunRPC]
    void StartDash()
    {
        StartCoroutine("DashDelay"); //대시 딜레이
    }

    [PunRPC]
    void StartUnBeat()
    {
        StartCoroutine("UnBeatTime"); //무적모드
    }

    [PunRPC]
    void Die()
    {
        isDie = true;

        if (pv.IsMine)
        {
            GameObject.Find("BulletBtnObj").transform.GetChild(0).gameObject.SetActive(true);
            GameObject.Find("CircleMouseObj").transform.GetChild(0).gameObject.SetActive(true);
            GameObject.Find("RoomManager").GetComponent<Room>().bossActorNum = PhotonNetwork.LocalPlayer.ActorNumber;
        }
    }

    [PunRPC]
    void IsTrigger()
    {
        beShot = true; //탄환과 충돌

        if (isUnBeatTime == false) //탄환과 만났고, 무적타임이 아닐 시
        {
            //Debug.Log("탄환 피격");

            //체력 25감소
            currentHealth = currentHealth - shotHealth;
            Debug.Log("탄환에 피격됐습니다 현재 체력은" + currentHealth);

            //탄환 피격 시 3초 간 무적모드
            StartCoroutine("UnBeatTime");

        }
    }

    //탄환과 충돌 판정
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (pv.OwnerActorNr != GameObject.Find("RoomManager").GetComponent<Room>().bossActorNum)
        {
            if (collision.CompareTag("bullet"))
            {
                if (pv.IsMine)
                {
                    pv.RPC("IsTrigger", RpcTarget.All);
                }
            }
        }
    }

    //대쉬 딜레이
    IEnumerator DashDelay()
    {
        yield return new WaitForSeconds(delayTime);
        isDelay = false;
    }

    //대쉬 중 무적
    IEnumerator UnBeatTime()
    {
        Debug.Log("무적모드 시작");
        int countTime = 0;

        //무적모드 플래그 t 변경
        isUnBeatTime = true;

        //대시 시 무적모드 
        if (isDash)
        {
            while (countTime < 5)
            {
                //0.5초간 딜레이
                yield return new WaitForSeconds(0.1f);
                countTime++;
            }
        }
        //피격 시 무적모드
        else if (beShot)
        {
            while (countTime < 20)
            {
                //알파값 수정해서 깜빡이도록 
                if (countTime % 2 == 0)
                    spriteRenderer.color = new Color32(255, 255, 255, 90);
                else
                    spriteRenderer.color = new Color32(255, 255, 255, 180);
                //3초간 딜레이
                yield return new WaitForSeconds(0.1f);
                countTime++;
            }
        }

        //알파값 원상 복귀
        spriteRenderer.color = new Color32(255, 255, 255, 255);

        //무적모드 아님
        isUnBeatTime = false;
        Debug.Log("무적모드 종료");

        yield return null;
    }

    // 내 컴퓨터+다른 사용자 컴퓨터에 함수 실행 요청
    [PunRPC]
    void FlipImg(int n)
    {
        spriteRenderer.flipX = n == 1;
    }

    // 속도 향상을 위해 OnPhotonSerializeView로 바꿔야함
    [PunRPC]
    void MoveBar()
    {
        //hp_bar 이동 범위 제한 함으로써 아르마딜로 오브젝트를 따라다니게 함
        if (currentHealth != 0)
        {
            HpBar.transform.position = Camera.main.WorldToScreenPoint(pv.transform.position + new Vector3(-0.02f, 0.5f, 0));
        }
    }

    [PunRPC]
    void NotDestroy()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(worldpos);
        }
        else
        {
            this.worldpos = (Vector3)stream.ReceiveNext();
        }
    }
}