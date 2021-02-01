using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 1번 원형 탄막 생성
public class CircleFire : MonoBehaviour
{
    BulletBtn bulletBtn;                 // 탄막 버튼 스크립트
    int selectNum = 0;                   // 탄막 선택 번호

    GameObject[] shot;                   // 총 탄막(유닛 단위) 오브젝트
    int shotNum = 10;                    // 총 탄막 개수(마우스 클릭 횟수) 제한

    public GameObject skillRange;        // 스킬 범위 최상위 오브젝트
    GameObject[] skillObject;            // 스킬 범위 오브젝트
    Vector3[] mouseLocation;             // 마우스 클릭 위치
    SpriteRenderer[] skillSprite;        // 스킬 범위 스프라이트
    Color[] skillColor;                  // 스킬 범위 투명도
    
    // 1번 탄막
    public GameObject CircleBulletUnit;  // 원형 탄막 유닛 모음
    SpriteRenderer[] circleSprite;       // 원형 탄막 스프라이트
    Vector3[] scale;                     // 원형 탄막 스프라이트 크기
    Color[] circleColor;                 // 원형 탄막 명도
    GameObject[] circleBullet;           // 작은 소품 오브젝트
    public int circleBulletNum;          // 작은 소품 개수(8개)

    bool[] isMake;                       // 작은 소품 생성 확인
    public bool[] isFire;                // 작은 소품 발사 확인
    public Queue<int> number;            // 탄막 객체 번호
    
    void Start()
    {
        bulletBtn = GameObject.Find("BulletBtn").GetComponent<BulletBtn>();
        shot = new GameObject[shotNum];

        skillObject = new GameObject[shotNum];
        skillSprite = new SpriteRenderer[shotNum];
        skillColor = new Color[shotNum];
        mouseLocation = new Vector3[shotNum];

        circleBullet = new GameObject[circleBulletNum];
        circleSprite = new SpriteRenderer[shotNum];
        circleColor = new Color[shotNum];
        scale = new Vector3[shotNum];

        isMake = new bool[shotNum];
        isFire = new bool[shotNum];
        number = new Queue<int>();
    }

    void Update()
    {
        selectNum = bulletBtn.num;           // 탄막 선택 번호

        if ((Input.GetMouseButtonUp(0)))     // 마우스 클릭
        {
            if(selectNum == 0)               // 1번 선택
            {
                for (int i = 0; i < shotNum; i++)
                {
                    if (shot[i] == null)
                    {
                        // 스킬 위치 지정
                        mouseLocation[i] = bulletBtn.followingMouse.transform.position;

                        // 스킬 범위 발생
                        StartCoroutine(skillStamp(i));

                        // 원형 탄막 발생
                        StartCoroutine(makeBullet(i));
                        break;
                    }
                }
            }
        }
    }

    // 스킬 범위 발생
    IEnumerator skillStamp(int n)
    {
        // 스킬 범위 초기화
        skillObject[n] = (GameObject)Instantiate(bulletBtn.followingMouse.transform.GetChild(selectNum).gameObject, mouseLocation[n], Quaternion.identity);
        skillObject[n].transform.SetParent(skillRange.transform);
        skillObject[n].SetActive(true);
        skillObject[n].transform.localScale = new Vector3(1f, 1f, 1f);

        // 스킬 범위 투명도 초기화
        skillSprite[n] = skillObject[n].GetComponent<SpriteRenderer>();
        skillColor[n] = skillSprite[n].color;
        skillColor[n].a = 0f;
        skillSprite[n].color = skillColor[n];

        // 스킬 범위 투명도 조절
        while (skillColor[n].a < 0.5f)
        {
            skillColor[n].a += Time.deltaTime * 0.5f;
            skillSprite[n].color = skillColor[n];
            yield return null;
        }

        // 스킬 범위 삭제
        Destroy(skillObject[n]);
    }

    // 원형 탄막 발생
    IEnumerator makeBullet(int n)
    {
        // 원형 탄막 초기화
        shot[n] = (GameObject)Instantiate(CircleBulletUnit.transform.GetChild(selectNum).gameObject, mouseLocation[n], Quaternion.identity);
        shot[n].transform.SetParent(CircleBulletUnit.transform);
        shot[n].SetActive(false);

        // 원형 탄막 스프라이트
        circleSprite[n] = shot[n].GetComponent<SpriteRenderer>();
        circleColor[n] = circleSprite[n].color;
        scale[n] = new Vector3(1f, 1f, 1f);            // 0.1cm 바꾸기
        shot[n].transform.localScale = scale[n];
        
        yield return new WaitForSeconds(1);

        shot[n].SetActive(true);

        // 원형 탄막 유닛 스프라이트 크기 조절
        while (scale[n].x < 3f)                       // 1.5cm 바꾸기
        {
            scale[n].x += Time.deltaTime * 1.5f;
            scale[n].y += Time.deltaTime * 1.5f;
            shot[n].transform.localScale = scale[n];

            // 1초에 3번 깜빡이기                    // 시간 확인
            if((int)(scale[n].x /0.25f % 2) == 0)
            {
                circleSprite[n].color = Color.white;
            }
            else
            {
                circleSprite[n].color = circleColor[n];
            }

            if ((isMake[n] == false) && (scale[n].x > 1))
            {
                // 작은 소품 랜덤 생성
                for (int i = 0; i < circleBulletNum; i++)
                {
                    circleBullet[i] = (GameObject)Instantiate(shot[n].transform.GetChild((int)Random.Range(0, 3)).gameObject, mouseLocation[n], Quaternion.identity);
                    circleBullet[i].transform.SetParent(shot[n].transform);
                    circleBullet[i].SetActive(false);
                    circleBullet[i].transform.localScale = new Vector3(0.1f, 0.1f, 0.1f); // 소품 크기
                }
                isMake[n] = true;
                number.Enqueue(n);
            }
            yield return null;
        }

        Destroy(circleSprite[n]);
        
        isMake[n] = false;
        isFire[n] = true;
    }
}
