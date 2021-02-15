using System.Collections;
using System.Collections.Generic;
using Photon.Pun; // 유니티용 포톤 컴포넌트들
using Photon.Realtime; // 포톤 서비스 관련 라이브러리
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

// 마스터(매치 메이킹) 서버와 룸 접속을 담당
public class LobbyManager : MonoBehaviourPunCallbacks
{
    public InputField roomInput; // 룸 이름 입력받는 영역
    public Text connectionInfoText; // 네트워크 정보를 표시할 텍스트

    public Button back; // 뒤로 버튼
    public Button joinButton; // 룸 접속 버튼(이미 만들어진 방)
    public Button joinNewRoomButton; //룸 접속 버튼(새로 만든 방)

    public GameObject room; 
    public Transform gridTr;

    // 게임 실행과 동시에 마스터 서버 접속 시도
    private void Start()
    {

        if (!PhotonNetwork.IsConnected)
        {
            // 마스터 서버 접속 시도
            PhotonNetwork.ConnectUsingSettings();

            // 룸 접속 버튼을 잠시 비활성화
            joinButton.interactable = false;
            joinNewRoomButton.interactable = false;

            // 접속을 시도 중임을 텍스트로 표시
            connectionInfoText.text = "마스터 서버에 접속중...";
        }
    }

    // 마스터 서버 접속 성공시 자동 실행
    public override void OnConnectedToMaster()
    {
        // 룸 접속 버튼을 활성화
        joinButton.interactable = true;
        joinNewRoomButton.interactable = true;

        // 접속 정보 표시
        connectionInfoText.text = "온라인 : 마스터 서버와 연결됨";

        //로비 참가
        PhotonNetwork.JoinLobby();
    }

    // 마스터 서버 접속 실패시 자동 실행
    public override void OnDisconnected(DisconnectCause cause)
    {
        // 룸 접속 버튼을 비활성화
        joinButton.interactable = false;
        joinNewRoomButton.interactable = false;

        // 접속 정보 표시
        connectionInfoText.text = "오프라인 : 마스터 서버와 연결되지 않음\n접속 재시도 중...";

        // 마스터 서버로의 재접속 시도
        PhotonNetwork.ConnectUsingSettings();
    }
    
    //로비 참가 성공시 자동 실행
    public override void OnJoinedLobby()
    {
        connectionInfoText.text = "로비에 참가됨";
    }
    
    //룸 리스트 보여주기
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo roomInfo in roomList)
        {
            bool check = false;

            GameObject[] obj = GameObject.FindGameObjectsWithTag("Room");

            if (roomInfo.PlayerCount==0)
            {
                for (int i = 0; i < obj.Length; i++)
                {
                    Destroy(obj[i]);
                }
                PhotonNetwork.JoinLobby();
                return;
            }
            
            for(int i=0; i<obj.Length; i++)
            {
                if(((string)roomInfo.CustomProperties["RoomID"]).Equals(obj[i].GetComponent<RoomData>().roomID))
                {
                    obj[i].GetComponent<RoomData>().playerCount = roomInfo.PlayerCount;
                    obj[i].GetComponent<RoomData>().UpdateInfo();
                    check = true;
                    break;
                }
            }

            if(!check)
            {
                GameObject _room = Instantiate(room, gridTr);
                RoomData roomData = _room.GetComponent<RoomData>();
                roomData.roomName = (string)roomInfo.CustomProperties["RoomName"];
                roomData.roomID = (string)roomInfo.CustomProperties["RoomID"];
                roomData.maxPlayer = roomInfo.MaxPlayers;
                roomData.playerCount = roomInfo.PlayerCount;
                roomData.UpdateInfo();
                roomData.GetComponentInChildren<Button>().onClick.AddListener
                (
                    delegate
                    {
                        OnClickRoom(roomData.roomID); //선택한 방에 참가
                    }
                );
            }
        }
    }

    //선택한 방에 참가
    void OnClickRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName, null);
    }

    //새로운 방을 만든 후 참가
    public void OnCreateRoomClick()
    {
        if (roomInput.text == string.Empty) //방 이름이 입력되지 않은 경우
        {
            connectionInfoText.text = "방 이름을 입력하세요!";
        }
        else //방 이름이 입력된 경우
        {
            // 중복 접속 시도를 막기 위해, 접속 버튼 잠시 비활성화
            joinNewRoomButton.interactable = false;

            // 마스터 서버에 접속중이라면
            if (PhotonNetwork.IsConnected)
            {
                // 룸 접속 실행
                connectionInfoText.text = "룸에 접속중...";
                Hashtable ht = new Hashtable() { };
                string temp = roomInput.text + "_" + System.DateTime.UtcNow.ToFileTime();
                ht.Add("RoomName", roomInput.text);
                ht.Add("RoomID", temp);
                string[] str = new string[2];
                str[0] = "RoomName";
                str[1] = "RoomID";
                PhotonNetwork.JoinOrCreateRoom(temp, new RoomOptions { MaxPlayers = 5, IsVisible = true, IsOpen = true, CustomRoomProperties = ht, CustomRoomPropertiesForLobby = str }, null);
            }
            else
            {
                // 마스터 서버에 접속중이 아니라면, 마스터 서버에 접속 시도
                connectionInfoText.text = "오프라인 : 마스터 서버와 연결되지 않음\n접속 재시도 중...";
                // 마스터 서버로의 재접속 시도
                PhotonNetwork.ConnectUsingSettings();
            }
        }

    }

    // 룸에 참가 완료된 경우 자동 실행
    public override void OnJoinedRoom()
    {
        // 접속 상태 표시
        connectionInfoText.text = "방 참가 성공";
        // 모든 룸 참가자들이 Main 씬을 로드하게 함
        PhotonNetwork.LoadLevel("Room Scene");
    }

    // 타이틀 화면으로 이동
    public void OnBackClick()
    {
        PhotonNetwork.Disconnect();
        connectionInfoText.text = "서버 연결 끊김";
        SceneManager.LoadScene("Start Scene");
    }

}

