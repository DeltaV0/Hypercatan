using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher launch;
    public TMP_InputField input;

    

    public TMP_Text err;

    public TMP_Text rntxt;

    public Transform roomListCont;
    public GameObject RlPre;

    public Transform playerListCont;
    public GameObject PLPre;

    public GameObject startButton;

    void Awake(){
        launch = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster(){
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby(){
        MenuManager.menuManager.OpenMenu("title");
        Debug.Log("joined");
    }
    
    public void CreateRoom(){
        if(string.IsNullOrEmpty(input.text)){
            return;
        }
        PhotonNetwork.CreateRoom(input.text);
        MenuManager.menuManager.OpenMenu("loading");
    }

    public override void OnJoinedRoom(){
        MenuManager.menuManager.OpenMenu("room");
        rntxt.text = PhotonNetwork.CurrentRoom.Name;

        Player[] players = PhotonNetwork.PlayerList;
        foreach(Transform child in playerListCont){
            Destroy(child.gameObject);
        }
        for(int i = 0; i < players.Length; i++){
            Instantiate(PLPre, playerListCont).GetComponent<PlayerListItem>().SetUp(players[i]);
        }

        startButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnMasterClientSwitched(Player newMasterClient){
        startButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message){
        MenuManager.menuManager.OpenMenu("error");
        err.text = "Someone fucked up: " + message;
    }

    public void LeaveRoom(){
      PhotonNetwork.LeaveRoom();
      MenuManager.menuManager.OpenMenu("loading");  
    }

    public override void OnLeftRoom(){
        MenuManager.menuManager.OpenMenu("title"); 
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList){
        foreach(Transform teans in roomListCont){
            Destroy(teans.gameObject);
        }
        for(int i = 0; i < roomList.Count; i++){
            if(roomList[i].RemovedFromList){
                continue;
            }
            Instantiate(RlPre, roomListCont).GetComponent<RoomListItem>().SetUp(roomList[i]);
        }

    }

    public void JoinRoom(RoomInfo info){
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.menuManager.OpenMenu("loading");  

        
    }

    public override void OnPlayerEnteredRoom(Player newPlayer){
        Instantiate(PLPre, playerListCont).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }

    public void StartGame(){
        PhotonNetwork.LoadLevel(1);
    }
}
