using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashMove : MonoBehaviour
{
    //max 체력과 현재 체력 변수
    public int maxHealth = 100;
    public int currentHealth;

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
    public float delayTime = 0.5f; //대쉬 딜레이 시간

    SpriteRenderer spriteRenderer; //스프라이트 렌더러
    Animator anim; //애니메이터

    //hp바, fill area 조절
    private Slider HpBar;
    private GameObject fillArea;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        dashTime = startDashTime;
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        currentHealth = maxHealth;

        //슬라이더 오브젝트 찾아고 hp값 초기화
        if (GameObject.FindGameObjectWithTag("hpslider"))
            HpBar = (Slider)FindObjectOfType(typeof(Slider));
        HpBar.value = 1f;
        //fill area 찾기
        fillArea = GameObject.FindWithTag("fillarea");
    }


    void Update()
    {
        //체력 체크해서 죽음 판정
        if (currentHealth == 0)
        {
            //fill area 비활성화로 0임을 표시
            fillArea.gameObject.SetActive(false);

            if (!isDie)
                Die();
            return;
        }

        //체력바에 현재 체력 구현
        if (currentHealth != 0)
            HpBar.value = (float)currentHealth / (float)100;

        //애니메이션 - 스프라이트 방향 전환
        if (Input.GetButtonDown("Horizontal"))
        {
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == 1;
        }

        //이동
        if(Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
        {
            anim.SetBool("isWalking", true);
            speed_vec.x = Input.GetAxis("Horizontal") * speed;
            speed_vec.y = Input.GetAxis("Vertical") * speed;

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
                StartCoroutine("DashDelay"); //대시 딜레이
            }
            else  //dashTime이 0보다 크면 dashTime감소 및 속도 변화
            {
                dashTime -= Time.deltaTime;

                speed_vec.x = Input.GetAxis("Horizontal") * speed * 3; //대쉬 중 이동속도 3배
                speed_vec.y = Input.GetAxis("Vertical") * speed * 3;

                rb.velocity = speed_vec;

                StartCoroutine("UnBeatTime"); //무적모드
            }
        }
        //hp_bar 이동 범위 제한 함으로써 아르마딜로 오브젝트를 따라다니게 함
        if (currentHealth != 0)
            HpBar.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(-0.02f, 0.5f, 0));

    }
    void Die()
    {
        isDie = true;
        HpBar.gameObject.SetActive(false);
        Destroy(gameObject);
    }

    //탄환과 충돌 판정
    private void OnTriggerEnter2D(Collider2D collision)
    {
        beShot = true; //탄환과 충돌
        
        if (collision.CompareTag("bullet") && isUnBeatTime == false) //탄환과 만났고, 무적타임이 아닐 시
        {
            Debug.Log("탄환 피격");

            //체력 25감소
            currentHealth = currentHealth - 25;
            Debug.Log("탄환에 피격됐습니다 현재 체력은" + currentHealth);

            //탄환 피격 시 3초 간 무적모드
            StartCoroutine("UnBeatTime");

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
            while (countTime < 30)
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

}
