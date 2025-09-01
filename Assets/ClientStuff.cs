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
    //public static ClientStuff client;

    public Unit selectedUnit;
    public Transform trans2;
    public LineRenderer line;

    public TMP_Text prodText;

    public Tile Core;

    public Color color;

    public Button nextTurnButton;

    public Transform cam;
    public Camera cam2;

    public PhotonView PV;

    public GameObject stuffToDestroy;

    public bool MyTurn;

    public Transform map;

    public string[] stuff;

    //public string[] units;

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
        selectedUnit = null;
        cam2 = cam.GetComponent<Camera>();
        color = Color.HSVToRGB(Random.Range(0f, 1f), 1, 1);
        PV.RPC("RPC_SetUp", RpcTarget.All);
        if (!PV.IsMine)
        {
            Destroy(stuffToDestroy);
        }
        else
        {
            //client = this;
            Debug.Log("g");
            Invoke("SetClient", 0.5f);
            Invoke("SetClient", 1.0f);
            Invoke("SetClient", 2.5f);

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
        if (PV.IsMine)
        {
            if (Core != null)
            {
                prodText.text = "Production: \n" + Core.ResourcesSum[0] + "\n" + Core.ResourcesSum[1];
            }
            else
            {
                prodText.text = "Please place your Core.";
            }
            nextTurnButton.interactable = MyTurn;
            trans2.position = new Vector3(cam2.ScreenToWorldPoint (Input.mousePosition).x, cam2.ScreenToWorldPoint (Input.mousePosition).y, 0);
            if (pos == null)
            {
                line.SetPosition(0, trans2.position);
                line.SetPosition(1, trans2.position);
            }
            else
            {
                line.SetPosition(1, trans2.position);
                line.SetPosition(0, pos.position);
            }
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

    public void NextTurn()
    {
        RoomManager.rm.NextTurn();
        map.GetComponent<MapGenerator>().NextTurn();
    }

    public void Select(int sel){
        selection = sel;
    }
}
