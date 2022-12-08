using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// 1번 원형 탄막 생성
public class CircleFire : MonoBehaviour
{
    public PhotonView pv;

    SpriteRenderer skillSprite;        // 스킬 범위 스프라이트
    Color skillColor;                  // 스킬 범위 투명도

    public Queue<int> number;            // 탄막 객체 번호

    void Start()
    {
        pv.RPC("startStamp", RpcTarget.All);
    }

    [PunRPC]
    void startStamp()
    {
        StartCoroutine(skillStamp());
    }

    // 스킬 범위 발생
    IEnumerator skillStamp()
    {
        // 스킬 범위 투명도 초기화
        skillSprite = gameObject.GetComponent<SpriteRenderer>();
        skillColor = skillSprite.color;
        skillColor.a = 0f;
        skillSprite.color = skillColor;
        
        // 스킬 범위 투명도 조절
        while (skillColor.a < 0.5f)
        {
            skillColor.a += Time.deltaTime * 0.4f;
            skillSprite.color = skillColor;
            yield return null;
        }

        if(pv.AmOwner)
        {
            PhotonNetwork.Instantiate("CircleBulletUnit", this.gameObject.transform.position, Quaternion.identity);
            PhotonNetwork.Destroy(gameObject);
            yield break;
        }
    }
}