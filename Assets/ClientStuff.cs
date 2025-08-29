using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.IO;
using UnityEngine.UI;

public class ClientStuff : MonoBehaviour
{
    public static ClientStuff client;

    public Button nextTurnButton;

    public Transform cam;

    public PhotonView PV;

    public GameObject stuffToDestroy;

    public bool MyTurn;

    public Transform map;

    public string[] stuff;

    public string[] units;

    public int selection;

    public int[] requirments;

    public int[] Resources;

    public bool FirstTurn;

    public bool sec;

    public Transform pos;

    [PunRPC]
    public void RPC_SetUp(){
            gameObject.name = PV.Owner.ActorNumber + "";
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        
        PV.RPC("RPC_SetUp", RpcTarget.All);
        if(!PV.IsMine){
            Destroy(stuffToDestroy);
        } else {
            client = this;
            //Invoke("SetClient", 0.5f);

        }
    }

    void SetClient(){
                    map = GameObject.Find("Map").transform;
            foreach(Transform ti in map){
                Debug.Log(ti.name);
                ti.GetComponent<Tile>().client = this;
            }
    }

    // Update is called once per frame
    void Update()
    {
        FirstTurn = RoomManager.rm.Turn == PV.Owner.ActorNumber;
        MyTurn = (((RoomManager.rm.Turn - 1) % PhotonNetwork.CurrentRoom.PlayerCount) + 1 == PV.Owner.ActorNumber);
        if(PV.IsMine){
            nextTurnButton.interactable = MyTurn;
        }
    }

    void FixedUpdate(){
        if(cam != null){
            if(Input.GetKey("w")){
                cam.Translate(0, 0.2f, 0);
            }
            if(Input.GetKey("a")){
                cam.Translate(-0.2f, 0, 0);
            }
            if(Input.GetKey("s")){
                cam.Translate(0, -0.2f, 0);
            }
            if(Input.GetKey("d")){
                cam.Translate(0.2f, 0, 0);
            }
        }
    }

    public void NextTurn(){
        RoomManager.rm.NextTurn();
    }

    public void Select(int sel){
        selection = sel;
    }
}
