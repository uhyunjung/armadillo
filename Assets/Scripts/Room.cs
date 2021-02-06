using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

// GetPlayerNumber를 위한 namespace
public class Room : MonoBehaviourPunCallbacks, IPunObservable
{
    public PhotonView pv;       // 룸매니저 포톤뷰       
    int num = 5;                // 사용자 인원
    bool[] playerList;          // 해당 위치에 아르마딜로 존재하는지 확인

    public Text msg;            // 게임 대기, 시작 안내 메시지
    public Text countDown;      // 카운트다운 메시지
    public Text countNum;       // 접속한 인원 메세지

    Hashtable temp;             // 임시 해시테이블(프로퍼티 설정)
    Vector3[] playerPos;        // 플레이어 위치 저장
    Vector3[] readyPos;         // Ready 텍스트 위치 저장

    int cnt;                    // Ready한 사용자 수
    float time;                 // 카운트다운 5초

    public void Awake()
    {
        // 로비와 합치면 바꾸기 및 삭제
        Screen.SetResolution(960, 540, false);                     // 1960*1080 바꾸기
        PhotonNetwork.SendRate = 100;
        PhotonNetwork.SerializationRate = 100;
        PhotonNetwork.ConnectUsingSettings();                      // OnConnectedToMaster 콜백함
        PhotonNetwork.AutomaticallySyncScene = true;

        playerList = new bool[num];
        playerPos = new Vector3[num];
        readyPos = new Vector3[num];

        for (int i = 0; i < num; i++)
        {
            playerPos[i] = new Vector3(-6 + 3f * i, -2, 0);        // 아르마딜로 간격 3
            readyPos[i] = new Vector3(-630 + 315 * i, -350, 0);    // Ready 버튼 간격 315(1960 기준)
        }
    }

    // ConnectUsingSettings 콜백 함수
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRoom("1번방");                           // OnJoinedRoom or OnJoinRoomFailed 콜백함
    }

    // 방이 없으면 방 새로 만들기(로비와 연결 수정 필요) + 사용자 없으면 룸 삭제
    // 로비 https://devsquare.tistory.com/2 변경 필요 PhotonNetwork.LocalPlayer.UserId + "_" + System.DateTime.UtcNow.ToFileTime()
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Hashtable ht = new Hashtable() { { "RoomName", "방이름 저장 필요" } };
        string[] str = new string[1];
        str[0] = "RoomName";

        PhotonNetwork.CreateRoom("1번방", new RoomOptions
        {
            MaxPlayers = 5,
            IsVisible = true,
            IsOpen = true,
            CustomRoomProperties = ht,
            CustomRoomPropertiesForLobby = str
        });
    }

    // 방에 접속하면 플레이어 생성
    public override void OnJoinedRoom()
    {
        StartCoroutine("CreatePlayer");
    }

    // 아르마딜로 생성
    IEnumerator CreatePlayer()
    {
        // SetPlayerCustomProperties 사용X, 사용자의 커스텀프로퍼티 삭제
        PhotonNetwork.RemovePlayerCustomProperties(null);

        // 사용자 위치 판별 배열 초기화
        for (int i=0; i<num; i++)
        {
            playerList[i] = false;
        }

        // 플레이어가 생성되면 index 프로퍼티가 적용되므로 사용자 존재 여부 확인
        for(int i=0; i<PhotonNetwork.PlayerList.Length; i++)
        {
            temp = PhotonNetwork.PlayerList[i].CustomProperties;

            if(temp.ContainsKey("index"))
            {
                playerList[(int)temp["index"]] = true;
            }
        }

        // 사용자가 없는 위치에 플레이어 생성 및 커스텀 프로퍼티 적용, Ready 텍스트 생성, index 자리 저장
        for (int i = 0; i < num; i++)
        {
            if (!playerList[i])
            {
                PhotonNetwork.SetPlayerCustomProperties(new Hashtable() { { "Ready", "No" }, { "index", i }, { "ActorNum", PhotonNetwork.LocalPlayer.ActorNumber } });  // 위치 할당 위해 인덱스 필요(actornum 사용X)
                PhotonNetwork.Instantiate("PlayerPrefab", playerPos[i], Quaternion.identity, 0);
                PhotonNetwork.Instantiate("ReadyText", readyPos[i], Quaternion.identity, 0);
                break;
            }
        }

        yield return null;
    }

    // SetPlayerCustomProperties 콜백 함수(모든 사용자 실행) => 여기서는 플레이어가 룸을 접속할 때 실행
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        // Ready한 사용자 변수 초기화
        if(PhotonNetwork.IsMasterClient)
        {
            cnt = 0;

            // Ready한 사용자 인원 확인
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                temp = PhotonNetwork.PlayerList[i].CustomProperties;

                if (temp.ContainsKey("Ready"))
                {
                    if (((string)temp["Ready"]).Equals("Yes"))
                    {
                        cnt++;
                    }
                }
            }

            // 3명 이상이면 카운트다운시작
            if (cnt >= 3)
            {
                time = 5f;
                pv.RPC("StartCD", RpcTarget.All);
            }
        }

        // 3명 미만 Ready
        if(cnt < 3)
        {
            msg.text = "다른 유저를 대기중입니다";
            countNum.text = PhotonNetwork.PlayerList.Length.ToString() + "/5";     // CountofPlayers는 즉시 갱신 안 됨
        }
    }

    // 사용자가 방을 나갈 때 실행(Ready 판별)
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // Ready한 사용자 변수 초기화
        if (PhotonNetwork.IsMasterClient)
        {
            cnt = 0;

            // Ready한 사용자 인원 확인
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                temp = PhotonNetwork.PlayerList[i].CustomProperties;

                if (temp.ContainsKey("Ready"))
                {
                    if (((string)temp["Ready"]).Equals("Yes"))
                    {
                        cnt++;
                    }
                }
            }

            // 3명 이상이면 카운트다운시작
            if (cnt >= 3)
            {
                time = 5f;
                pv.RPC("StartCD", RpcTarget.All);
            }
        }

        // 3명 미만 Ready
        if (cnt < 3)
        {
            msg.text = "다른 유저를 대기중입니다";
            countNum.text = PhotonNetwork.PlayerList.Length.ToString() + "/5";     // CountofPlayers는 즉시 갱신 안 됨
        }
    }

    // 모든 사용자들 카운트다운 실행
    [PunRPC]
    void StartCD()
    {
        StartCoroutine("CountDown");
    }

    // 카운트다운
    IEnumerator CountDown()
    {
        msg.text = "게임이 곧 시작됩니다";
        countNum.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        countDown.gameObject.SetActive(true);
        countDown.text = ((int)time).ToString();

        yield return new WaitForSeconds(1);

        while (true)
        {
            countDown.text = ((int)time).ToString();

            if(PhotonNetwork.IsMasterClient)
            {
                // 1초
                time -= Time.deltaTime;

                cnt = 0;
                for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
                {
                    temp = PhotonNetwork.PlayerList[i].CustomProperties;

                    if (temp.ContainsKey("Ready"))
                    {
                        if (((string)temp["Ready"]).Equals("Yes"))
                        {
                            cnt++;
                        }
                    }
                }

                // 카운트다운 후 게임 씬 이동
                if (time < 0)
                {
                    PhotonNetwork.CurrentRoom.IsOpen = false;
                    PhotonNetwork.LoadLevel("Game Scene");
                    SceneManager.LoadScene("Game Scene");
                }
            }

            // 카운트다운 시작 후 Ready 해제하여 Ready한 플레이어가 3명 미만일 때 실행
            if (cnt < 3)
            {
                msg.text = "다른 유저를 대기중입니다";
                countNum.text = PhotonNetwork.PlayerList.Length.ToString() + "/5";

                countNum.gameObject.SetActive(true);
                countDown.gameObject.SetActive(false);
                yield break;
            }

            yield return null;
        }
    }

    // 뒤로가기(로비 이동) 바꿔야 됨
    public void Back()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("Start Scene");
    }

    // 카운트다운 시간, Ready한 인원 동기화
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(time);
            stream.SendNext(cnt);
        }
        else
        {
            this.time = (float)stream.ReceiveNext();
            this.cnt = (int)stream.ReceiveNext();
        }
    }
}
