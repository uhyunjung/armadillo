using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Room : MonoBehaviourPunCallbacks, IPunObservable
{
    public PhotonView pv;       // 룸매니저 포톤뷰
    int num = 5;                // 사용자 인원
    int bossnum = -1;            // 보스 유저 번호
    public int bossActorNum = -1;  // 보스 ActorNum
    bool[] playerList;          // 해당 위치에 아르마딜로 존재하는지 확인

    public Text msg;            // 게임 대기, 시작 안내 메시지
    public Text countDown;      // 카운트다운 메시지
    public Text countNum;       // 접속한 인원 메세지

    Hashtable temp;             // 임시 해시테이블(프로퍼티 설정)
    Vector3[] playerPos;        // 플레이어 위치 저장
    Vector3[] readyPos;         // Ready 텍스트 위치 저장
    public Vector3[] bossPos;

    int cnt;                    // Ready한 사용자 수
    int readyCnt=3;             // 게임 시작 인원 조건 3명
    float time;                 // 카운트다운 5초

    public List<int> checkBoss;        // 보스되었는지 확인
    public bool isPrepare = false;

    public void Awake()
    {
        // 로비와 합치면 바꾸기 및 삭제                    // 1920*1080 바꾸기
        PhotonNetwork.SendRate = 100;
        PhotonNetwork.SerializationRate = 100;
        PhotonNetwork.AutomaticallySyncScene = true;

        playerList = new bool[num];
        playerPos = new Vector3[num];
        readyPos = new Vector3[num];
        bossPos = new Vector3[num];

        for (int i = 0; i < num; i++)
        {
            playerPos[i] = new Vector3(-6 + 3f * i, -2, 0);        // 아르마딜로 간격 3
            readyPos[i] = new Vector3(-630 + 315 * i, -350, 0);    // Ready 버튼 간격 315(1960 기준)
            bossPos[i] = new Vector3(2.67f + 0.7f * i, 0.5f, 1);
        }

        bossPos[0] = new Vector3(0f, 4f, 1);
        bossPos[1] = new Vector3(-3f, 4f, 1);
        bossPos[2] = new Vector3(3f, 4f, 1);
        bossPos[3] = new Vector3(-4f, 4f, 1);
        bossPos[4] = new Vector3(4f, 4f, 1);
    }

    // 방에 접속하면 플레이어 생성
    public void Start()
    {
        checkBoss.Clear();
        StartCoroutine("CreatePlayer");
    }

    public void Update()
    {
        if (SceneManager.GetActiveScene().name.Equals("Game Scene"))
        {
            if (PhotonNetwork.IsMasterClient)
            {
                // 사람 수 적음
                if ((PhotonNetwork.PlayerList.Length < readyCnt) || (checkBoss.Count == 0))
                {
                    PhotonNetwork.DestroyAll();
                    if (PhotonNetwork.InRoom)
                    {
                        PhotonNetwork.LeaveRoom();
                    }
                }
                else if (checkBoss.Count == PhotonNetwork.PlayerList.Length + 1)             // 게임 종료
                {
                    // 모두 보스됨
                    PhotonNetwork.DestroyAll();
                    if (PhotonNetwork.InRoom)
                    {
                        PhotonNetwork.LeaveRoom();
                    }
                }
            } 
        }  
    }

    // 아르마딜로 생성
    IEnumerator CreatePlayer()
    {
        // SetPlayerCustomProperties 사용X, 사용자의 커스텀프로퍼티 삭제
        PhotonNetwork.RemovePlayerCustomProperties(null);

        findPlayer();

        // 사용자가 없는 위치에 플레이어 생성 및 커스텀 프로퍼티 적용, Ready 텍스트 생성, index 자리 저장
        for (int i = 0; i < num; i++)
        {
            if (!playerList[i])
            {
                PhotonNetwork.SetPlayerCustomProperties(new Hashtable() { { "Ready", "No" }, { "index", i }, { "ActorNum", PhotonNetwork.LocalPlayer.ActorNumber } });  // 위치 할당 위해 인덱스 필요(actornum 사용X)
                // 사용자 접속 순서에 따른 아르마딜로 색상 할당
                switch (i)
                {
                    case 0:
                        PhotonNetwork.Instantiate("PlayerPrefab", playerPos[i], Quaternion.identity, 0);
                        break;
                    case 1:
                        PhotonNetwork.Instantiate("RedPlayerPrefab", playerPos[i], Quaternion.identity, 0);
                        break;
                    case 2:
                        PhotonNetwork.Instantiate("GreenPlayerPrefab", playerPos[i], Quaternion.identity, 0);
                        break;
                    case 3:
                        PhotonNetwork.Instantiate("BluePlayerPrefab", playerPos[i], Quaternion.identity, 0);
                        break;
                    case 4:
                        PhotonNetwork.Instantiate("PurplePlayerPrefab", playerPos[i], Quaternion.identity, 0);
                        break;
                    // default : 기본 아르마딜로 할당
                    default:
                        PhotonNetwork.Instantiate("PlayerPrefab", playerPos[i], Quaternion.identity, 0);
                        break;
                }
                // 각 사용자마다 Ready 텍스트 할당
                PhotonNetwork.Instantiate("ReadyText", readyPos[i], Quaternion.identity, 0);
                break;
            }
        }
        yield return null;
    }

    // SetPlayerCustomProperties 콜백 함수(모든 사용자 실행) => 여기서는 플레이어가 룸을 접속할 때 실행
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if ((SceneManager.GetActiveScene().name.Equals("Room Scene")) && (!isPrepare))
            readyCount();
    }

    // 사용자가 방을 나갈 때 실행(Ready 판별)
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if ((SceneManager.GetActiveScene().name.Equals("Room Scene")) && (!isPrepare))
            readyCount();
        if (SceneManager.GetActiveScene().name.Equals("Game Scene"))
        {
            for(int i=0; i<checkBoss.Count; i++)
            {
                if(checkBoss[i].Equals(otherPlayer.ActorNumber))
                {
                    checkBoss.RemoveAt(i);
                }
            }
        }
    }

    public void readyCount()
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
            if (cnt >= readyCnt)
            {
                time = 5f;
                pv.RPC("StartCD", RpcTarget.All);
            }
        }

        // 3명 미만 Ready
        if (cnt < readyCnt)
        {
            msg.text = "다른 유저를 대기중입니다";
            countNum.text = PhotonNetwork.PlayerList.Length.ToString() + "/5";     // CountofPlayers는 즉시 갱신 안 됨
        }
    }

    // 모든 사용자들 카운트다운 실행
    [PunRPC]
    void StartCD()
    {
        bossnum = -1;
        bossActorNum = -1;
        StartCoroutine("CountDown");
    }

    // 카운트다운
    IEnumerator CountDown()
    {
        isPrepare = true;
        msg.text = "게임이 곧 시작됩니다";
        countNum.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        countDown.gameObject.SetActive(true);
        countDown.text = ((int)time).ToString();

        yield return new WaitForSeconds(1);

        while ((true) && (SceneManager.GetActiveScene().name.Equals("Room Scene")))
        {
            countDown.text = ((int)time).ToString();

            if (PhotonNetwork.IsMasterClient)
            {
                // 1초
                time -= Time.deltaTime;

                while (bossActorNum == -1)
                {
                    findPlayer();

                    bossnum = Random.Range(0, PhotonNetwork.PlayerList.Length);             // 번호 랜덤 선택

                    temp = PhotonNetwork.PlayerList[bossnum].CustomProperties;

                    // bossnum 접근 편하게 index에서 ActorNum으로 변경
                    playerPos[(int)temp["index"]].z -= 1;  // 깜박임 방지
                    PhotonNetwork.Instantiate("BossPrefab", playerPos[(int)temp["index"]], Quaternion.identity, 0);

                    bossActorNum = (int)temp["ActorNum"];
                    pv.RPC("setBoss", RpcTarget.All, bossActorNum);
                }

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
                    PhotonNetwork.CurrentRoom.IsVisible = false;
                    pv.RPC("loadRoomManager", RpcTarget.All);
                    cnt = 0;

                    if (PhotonNetwork.IsMasterClient)
                    {
                        PhotonNetwork.Destroy(GameObject.Find("BossPrefab(Clone)"));
                        playerPos[(int)temp["index"]].z += 1;
                    }
                    PhotonNetwork.LoadLevel("Game Scene");
                    SceneManager.LoadScene("Game Scene");
                    yield break;
                }
            }

            // 카운트다운 시작 후 Ready 해제하여 Ready한 플레이어가 3명 미만일 때 실행
            if (SceneManager.GetActiveScene().name.Equals("Room Scene") && (cnt < readyCnt))
            {
                isPrepare = false;
                msg.text = "다른 유저를 대기중입니다";
                countNum.text = PhotonNetwork.PlayerList.Length.ToString() + "/5";
                bossActorNum = -1;

                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.Destroy(GameObject.Find("BossPrefab(Clone)"));
                    pv.RPC("setBoss", RpcTarget.All, -1);
                }
                pv.RPC("clearCheckBoss", RpcTarget.All);
                countNum.gameObject.SetActive(true);
                countDown.gameObject.SetActive(false);
                yield break;
            }

            yield return null;
        }
    }

    [PunRPC]
    void loadRoomManager()
    {
        DontDestroyOnLoad(this);
    }

    public void findPlayer()
    {
        // 사용자 위치 판별 배열 초기화
        for (int i = 0; i < num; i++)
        {
            playerList[i] = false;
        }

        // 플레이어가 생성되면 index 프로퍼티가 적용되므로 사용자 존재 여부 확인
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            temp = PhotonNetwork.PlayerList[i].CustomProperties;

            if (temp.ContainsKey("index"))
            {
                playerList[(int)temp["index"]] = true;
            }
        }
    }

    // 뒤로가기(로비 이동)
    public void Back()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.JoinLobby();
        SceneManager.LoadScene("Lobby Scene");
        Destroy(this.gameObject);
    }

    // 카운트다운 시간, Ready한 인원 동기화
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(time);
            stream.SendNext(cnt);
            stream.SendNext(bossnum);
        }
        else
        {
            this.time = (float)stream.ReceiveNext();
            this.cnt = (int)stream.ReceiveNext();
            this.bossnum = (int)stream.ReceiveNext();
        }
    }

    [PunRPC]
    void setBoss(int n)
    {
        bossActorNum = n;
        checkBoss.Add(n);
    }

    [PunRPC]
    void clearCheckBoss()
    {
        checkBoss.Clear();
    }
}
