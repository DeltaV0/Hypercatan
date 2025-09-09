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

    public TMP_Text[] prodText;

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

    public List<Tile> cities;

    public AudioSource aud;

    public GameObject cursor;
    public SpriteRenderer cursorsprite;

    public int TechPoints;

    public TechTree techTree;

    public Inventory inv;

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
            //map.canvas.EventCamera = cam2;
            foreach(Transform ti in map){
                Debug.Log(ti.name);
                ti.GetComponent<Tile>().GetNeighbors();
                ti.GetComponent<Tile>().client = this;
            }
    }

    // Update is called once per frame
    void Update()
    {
        //cursor.transform.localPosition = new Vector3(cam2.ScreenToWorldPoint (Input.mousePosition).x, cam2.ScreenToWorldPoint (Input.mousePosition).y, 0);
        //cursorsprite.color = Color.HSVToRGB((1f / PhotonNetwork.CurrentRoom.PlayerCount) * PhotonNetwork.LocalPlayer.ActorNumber, 1, 1);
        FirstTurn = RoomManager.rm.Turn == PV.Owner.ActorNumber;
        MyTurn = (((RoomManager.rm.Turn - 1) % PhotonNetwork.CurrentRoom.PlayerCount) + 1 == PV.Owner.ActorNumber);
        if (PV.IsMine)
        {
            if (Core != null)
            {
                Resources = Core.ResourcesSum;
                prodText[0].text = "Production: \n Iron: " + Core.ResourcesSum[0] + "\n Coal: " + Core.ResourcesSum[1] + "\n Steel: " + Core.ResourcesSum[2] + "\n Wood: " + Core.ResourcesSum[3] + "\n Wheat: " + Core.ResourcesSum[4];
            }
            else
            {
                prodText[0].text = "Please place your Capital.";
            }
            prodText[1].text = TechPoints + "";
            nextTurnButton.interactable = MyTurn;
            if(MyTurn){
                aud.volume = 1;
            } else{
                aud.volume = 0;
            }
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
        PV.RPC("RPC_TurnSwitch", RpcTarget.All);
        
       // map.GetComponent<MapGenerator>().NextTurn();
    }

    [PunRPC]
    public void RPC_TurnSwitch(){
        StartCoroutine(CountResources());
        foreach (Unit unit in OwnedUnit)
        {
            unit.UpdateMoves();
            unit.movementPoints[0] = unit.MaxmovementPoints[0];
            unit.movementPoints[1] = unit.MaxmovementPoints[1];
            
         }
    }

    public void ChangeResource(int a, int b){
        PV.RPC("RPC_ChangeResource", RpcTarget.All, a, b);
    }

    [PunRPC]
    public void RPC_ChangeResource(int a, int b){
        Resources[a] += b;
    }

    public void Build(){
            PV.RPC("RPC_ChangeResource", RpcTarget.All, 0, -1 * requirments[0]);
            PV.RPC("RPC_ChangeResource", RpcTarget.All, 1,-1 * requirments[1]);
            PV.RPC("RPC_ChangeResource", RpcTarget.All, 2,-1 * requirments[2]);
            PV.RPC("RPC_ChangeResource", RpcTarget.All, 3,-1 * requirments[3]);
            PV.RPC("RPC_ChangeResource", RpcTarget.All, 4,-1 * requirments[4]);
    }

    IEnumerator CountResources(){
        foreach (Tile t in OwnedTiles) {
            if (t.Building != 0 && t.Building != 1 && t.Building != 100) {
                if (PV.IsMine) {
                    PV.RPC("RPC_ChangeResource", RpcTarget.All, 0, t.ResourcesProducing[0]);
                    PV.RPC("RPC_ChangeResource", RpcTarget.All, 1, t.ResourcesProducing[1]);
                    PV.RPC("RPC_ChangeResource", RpcTarget.All, 2, t.ResourcesProducing[2]);
                    PV.RPC("RPC_ChangeResource", RpcTarget.All, 3, t.ResourcesProducing[3]);
                    PV.RPC("RPC_ChangeResource", RpcTarget.All, 4, t.ResourcesProducing[4]);
                }
                // Resources[0] += t.ResourcesProducing[0];
                //Resources[1] += t.ResourcesProducing[1];
                // Resources[2] += t.ResourcesProducing[2];
                // Resources[3] += t.ResourcesProducing[3];
                //  Resources[4] += t.ResourcesProducing[4];
                if (t.Building == 8) {
                    TechPoints++;
                }
                t.spr2.color = new Color(t.spr2.color.r, t.spr2.color.g, t.spr2.color.b, 1f);
                if (t.cooldown > 0) {
                    t.cooldown--;
                }
                if ((t.Building == 3 || t.Building == 5 || t.Building == 6) && PV.IsMine)
                {
                    t.PlayParticle(0);
                    yield return new WaitForSeconds(0.1f);
                }
            }
                else
                {
                    yield return new WaitForSeconds(0f);
                }
        }
        yield return new WaitForSeconds(0f);
    }

    public void Select(int sel){
        selection = sel;
        if(sel == 2){
            requirments[0] = 1;
            requirments[1] = 0;
            requirments[2] = 0;
            requirments[3] = 1;
            requirments[4] = 0;
        } else if(sel == 3){
            requirments[0] = 1;
            requirments[1] = 0;
            requirments[2] = 0;
            requirments[3] = 1;
            requirments[4] = 0;
        } else if(sel == 4){
            requirments[0] = 3;
            requirments[1] = 0;
            requirments[2] = 0;
            requirments[3] = 2;
            requirments[4] = 0;
        } else if(sel == 5){
            requirments[0] = 1;
            requirments[1] = 0;
            requirments[2] = 0;
            requirments[3] = 3;
            requirments[4] = 0;
        }else if(sel == 6){
            requirments[0] = 2;
            requirments[1] = 0;
            requirments[2] = 0;
            requirments[3] = 1;
            requirments[4] = 0;
        } else if(sel == 7){
            requirments[0] = 3;
            requirments[1] = 0;
            requirments[2] = 0;
            requirments[3] = 3;
            requirments[4] = 0;
        } else if(sel == 8){
            requirments[0] = 2;
            requirments[1] = 0;
            requirments[2] = 0;
            requirments[3] = 4;
            requirments[4] = 0;
        } else {
            requirments[0] = 0;
            requirments[1] = 0;
        }
    }
}
