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

    public Tile SelectedTile;

    public List<Tile> OwnedTiles;
    public List<Unit> OwnedUnit;

    [PunRPC]
    public void RPC_SetUp(){
            gameObject.name = PV.Owner.ActorNumber + "";
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        selectedUnit = null;
        cam2 = cam.GetComponent<Camera>();
        color = Color.HSVToRGB((1f / PhotonNetwork.CurrentRoom.PlayerCount) * PhotonNetwork.LocalPlayer.ActorNumber, 1, 1);
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
            Invoke("SetClient", 3.5f);
            Invoke("SetClient", 5f);
            Invoke("SetClient", 7.5f);
            Invoke("SetClient", 10f);
            //ffs is this doesnt work then that's your problem

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
                Resources = Core.ResourcesSum;
                prodText.text = "Production: \n" + Core.ResourcesSum[0] + "\n" + Core.ResourcesSum[1] + "\n" + Core.ResourcesSum[2];
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
        foreach(Tile t in OwnedTiles){
            Resources[0] += t.ResourcesProducing[0];
            Resources[1] += t.ResourcesProducing[1];
            Resources[2] += t.ResourcesProducing[2];
        }
        foreach (Unit unit in OwnedUnit)
        {
            unit.UpdateMoves();
            unit.movementPoints = 2;
            
         }
       // map.GetComponent<MapGenerator>().NextTurn();
    }

    public void Select(int sel){
        selection = sel;
        if(sel == 2){
            requirments[0] = 1;
            requirments[1] = 0;
        } else if(sel == 3){
            requirments[0] = 1;
            requirments[1] = 0;
        } else if(sel == 4){
            requirments[0] = 3;
            requirments[1] = 0;
        } else {
            requirments[0] = 0;
            requirments[1] = 0;
        }
    }
}
