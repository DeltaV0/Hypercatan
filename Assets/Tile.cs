using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.IO;

public class Tile : MonoBehaviour
{
    public SpriteRenderer spr;

    public SpriteRenderer spr2;

    public Sprite[] sprites;

    public int tileID;

    public int resource;

    public PhotonView PV;

    public int Owner;

    public ClientStuff client;

    public int Building;

    public bool selectedForCon;

    public Transform connection;

    public MapGenerator map;
    //private int sel;

    [PunRPC]
    public void RPC_SetUp(float selx, float sely){
            tileID = MapGenerator.map.ID;
            float samp = Mathf.PerlinNoise((transform.position.x * 0.4f) + selx, (transform.position.y * 0.4f) + sely);
            gameObject.name = "tile " + tileID;
            if(samp > 0.65f){
                resource = 1;
            }
            if(resource == 1){
            spr.color = new Color(1f, 0.5f, 0.5f);
        }
    }

    [PunRPC]
    public void RPC_Clicked(int sel, int buildingsel, bool first){
        //ClientStuff user;
        //if(PV.IsMine){
          //  user = client;
       // }
        if(first){
          //  PhotonNetwork.Instantiate(client.stuff[0], transform.position, transform.rotation);
            Building = 1;
            spr2.sprite = sprites[Building];
            Owner = sel;
            if(client != null){
            client.NextTurn();
            }
            //spr.color = new Color(0f, 0.5f, 0.5f);
        } else {
            if(buildingsel != -1){
            Building = buildingsel;
            spr2.sprite = sprites[Building];
            Owner = sel;
            } else {
                if(client.sec == false){
                    selectedForCon = true;
                    client.sec = true;
                    client.pos = transform;
                } else {
                    if(selectedForCon){
                        selectedForCon = false;
                        client.sec = false;
                        client.pos = null;
                    } else {
                        client.pos.gameObject.GetComponent<Tile>().connection = transform;
                        connection = client.pos;
                        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
                        lineRenderer.material = RoomManager.rm.linemat;
                        lineRenderer.startWidth = 0.2f;
                        lineRenderer.endWidth = 0.2f;
                        lineRenderer.positionCount = 2;
                        lineRenderer.SetPosition(0, transform.position);
                        lineRenderer.SetPosition(1, connection.position);
                        selectedForCon = false;
                        client.sec = false;
                        client.pos = null;
                    }
                }
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        //Debug.Log(PhotonNetwork.LocalPlayer.ActorNumber);
        //Debug.Log(GameObject.Find(PhotonNetwork.LocalPlayer.ActorNumber.ToString()).name);
        //client = GameObject.Find(PhotonNetwork.LocalPlayer.ActorNumber + "").GetComponent<ClientStuff>(); //FIX THIS (Extremely Important!!!!!!!!!)
        //client = ClientStuff.client;
        Vector3 pos = transform.position;
        transform.position = Vector3.zero;
        transform.SetParent(GameObject.Find("Map").transform);
        transform.localPosition = pos;
        
        if(PhotonNetwork.IsMasterClient){
            //sel = Random.Range(-1000f, 1000f);
            map = MapGenerator.map;
            PV.RPC("RPC_SetUp", RpcTarget.All, map.randCL[0], map.randCL[1]);
        }
       // Debug.Log(PhotonNetwork.LocalPlayer.ActorNumber);
       // client = GameObject.Find(PhotonNetwork.LocalPlayer.ActorNumber + "").GetComponent<ClientStuff>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown() {
        if(client.MyTurn){
            PV.RPC("RPC_Clicked", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, client.selection, client.FirstTurn);
        }
    }
}
