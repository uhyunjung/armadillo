using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserRotation : MonoBehaviour
{
    BulletBtn bulletBtn;  // 탄막 버튼 스크립트

    float angle;
    float stopAngle;
    bool check = true;
    Vector2 target, mouse;
    Color color;

    public float cool;

    private void Start()
    {
        bulletBtn = GameObject.Find("BulletBtn").GetComponent<BulletBtn>();

        target = transform.position;
    }
    private void Update()
    {
        disappear();

        mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        angle = Mathf.Atan2(mouse.y - target.y, mouse.x - target.x) * Mathf.Rad2Deg;
        //this.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);

        if (Input.GetMouseButtonDown(0) && bulletBtn.num == 1 && check)
        {
            //When mouse button clicked, the angle of laser is fixed.
            stopAngle = angle;
            this.transform.rotation = Quaternion.AngleAxis(stopAngle - 90, Vector3.forward);
            check = false;

            //During waitingTime, occur click delay and check the collision. 
            //Not Yet Collision Code.
            FixAngle();
        }
    }

    public void disappear()
    {
        SpriteRenderer spr = GetComponent<SpriteRenderer>();
        Color color = spr.color;
        color.a = 0f;
        spr.color = color;

    }
    public void FixAngle()
    {
        StartCoroutine(Cooltime(cool));
    }

    IEnumerator Cooltime(float cool)
    {
        check = false;
        while (cool > 0.0f)
        {
            cool -= Time.deltaTime;
            GameObject.Find("Main Camera").GetComponent<CameraShake>().Shake();
            yield return null;
        }
        check = true;
    }
}
