using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.IO;
using System.Linq;

public class Tile : MonoBehaviour
{
    public int Health;
    public int MaxHealth;
    public bool TakesInput;

    public SpriteRenderer[] spr;

    public SpriteRenderer spr2;

    public GameObject[] MoveCircle;

    public GameObject Mountains;

    public Unit unit;

    public Sprite[] sprites;

    public int tileID;

    public int resource;

    public PhotonView PV;

    public int Owner;

    public ClientStuff client;

    public ClientStuff ownerClient;

    public int Building;

    public bool selectedForCon;

    public List<Transform> connection;
    public List<Transform> connectionFrom;

    public List<LineRenderer> Lines;

    public GameObject PlaceHolderPf;

    public MapGenerator map;

    public int[] ResourcesSum;

    public int[] ResourcesProducing;

    public List<Transform> alreadychecked;

    public int[] Inputs;

    public bool Active;

    public Tile me;

    public int terrainType;

    public List<Tile> SphereOfInfluence;

    public List<Tile> neighbors;
    //private int sel;

    

    [PunRPC]
    public void RPC_SetUp(int ID, float selx, float sely)
    {
        map = MapGenerator.map;
        //connection.RemoveAt(0);
        map.tiles.Add(this);
        me = this;
        tileID = ID;
        Collider2D[] l = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), 1f, LayerMask.GetMask("Tile"));
        for (int i = 0; i < l.Length; i++)
        {   
            Tile t = l[i].gameObject.GetComponent<Tile>();
            if (t != this)
            {
                neighbors.Add(t);
             }
         }
        float samp = Mathf.PerlinNoise((transform.localPosition.x * 0.4f) + selx, (transform.localPosition.y * 0.4f) + sely);
        float samp2 = Mathf.PerlinNoise((transform.localPosition.x * 0.2f) - selx, (transform.localPosition.y * 0.2f) + sely);
        float samp3 = Mathf.PerlinNoise((transform.localPosition.x * 0.1f) - selx, (transform.localPosition.y * 0.1f) + sely);
        gameObject.name = "tile " + tileID;
        if (samp + (samp3 * 0.1f) > 0.65f)
        {
            resource = 1;
        } else if(samp < 0.25f){
            resource = 2;
        }
        else if (samp2 > 0.65f)
        {
            resource = -1;
        }
        transform.position = new Vector3(transform.position.x, transform.position.y, samp3 * -10);
        if (samp3 > 0.65f)
        {
            terrainType = 1;
            Mountains.SetActive(true);
            //spr[1].color = new Color(0.5f, 0.5f, 0.5f);
        } else {
            terrainType = 0;
            Mountains.SetActive(false);
            //spr[1].color = new Color(1f, 1f, 1f);
        }
        if (resource == 1)
        {
            spr[0].color = new Color(0.75f, 0.75f, 0.5f);
        }
        if (resource == 2)
        {
            spr[0].color = new Color(0.25f, 0.25f, 0.25f);
        }
        
        if(resource == -1){
            spr[0].color = new Color(0.5f, 1f, 0.5f);
        }
    }

    public void SetColor(int sel)
    {
        spr2.color = Color.HSVToRGB((1f / PhotonNetwork.CurrentRoom.PlayerCount) * sel, 1, 1);
        spr[2].color = new Color(spr2.color.r, spr2.color.g, spr2.color.b, 0.1f);
        spr2.color = new Color(spr2.color.r, spr2.color.g, spr2.color.b, 0.1f);
     }

    public void FindResourceSum(Tile origin)
    {
        if (origin.Active)
        {
            ResourcesSum[0] += origin.ResourcesProducing[0];
            ResourcesSum[1] += origin.ResourcesProducing[1];
            ResourcesSum[0] -= origin.Inputs[0];
            ResourcesSum[1] -= origin.Inputs[1];
         }
        alreadychecked.Add(origin.transform);
        //if (origin.connection.Count() == 0) {
        foreach (Transform t in origin.connection)
        {
            if (!alreadychecked.Contains(t))
            {
                FindResourceSum(t.GetComponent<Tile>());
            }
        }
        return;
        //}
    }



    [PunRPC]
    public void RPC_Clicked(int sel, int buildingsel, bool first){
        //ClientStuff user;
        //if(PV.IsMine){
          //  user = client;
       // }
       ownerClient = GameObject.Find(sel + "").GetComponent<ClientStuff>();
        if(first){
            if (Owner == sel || Owner == 0)
            {
                //  PhotonNetwork.Instantiate(client.stuff[0], transform.position, transform.rotation);
                ResourcesSum[0] = 2;
                ResourcesSum[1] = 00;
                Building = 1;
                spr2.sprite = sprites[Building];
                Owner = sel;
                ownerClient = GameObject.Find(Owner + "").GetComponent<ClientStuff>();
                ownerClient.OwnedTiles.Add(this);
                Collider2D[] l = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), 3f, LayerMask.GetMask("Tile"));
                for (int i = 0; i < l.Length; i++)
                {   
                    Tile t = l[i].gameObject.GetComponent<Tile>();
                    if (t != this && (t.Owner == 0 || t.Owner == sel))
                    {       
                        t.Owner = sel;
                        t.ownerClient = GameObject.Find(Owner + "").GetComponent<ClientStuff>();
                        t.SetColor(sel);
                        SphereOfInfluence.Add(t);
                        ownerClient.OwnedTiles.Add(t);
                    }
                }
                SetColor(sel);
                MaxHealth = 10;
                Health = MaxHealth;
                //ownerClient = GameObject.Find(sel + "").GetComponent<ClientStuff>();
                if (client != null)
                {
                    if (client.MyTurn)
                    {
                        client.Core = this;
                    }
                    client.NextTurn();
                }
            }
            //spr.color = new Color(0f, 0.5f, 0.5f);
            }
            else
            {
            if (buildingsel != -1 && buildingsel != 0 && ownerClient.Resources[0] >= ownerClient.requirments[0] && ownerClient.Resources[1] >= ownerClient.requirments[1])
            {
                if (Owner == sel)
                {
                    if (Building == 0)
                    {
                        if (buildingsel > 0)
                        {
                            if (buildingsel != 4)
                            {
                                ownerClient.Resources[0] -= ownerClient.requirments[0];
                                ownerClient.Resources[1] -= ownerClient.requirments[1];
                                Building = buildingsel;
                                spr2.sprite = sprites[Building];
                                SetColor(sel);
                                Owner = sel;
                                ownerClient = GameObject.Find(Owner + "").GetComponent<ClientStuff>();
                                if(Building == 2){
                                    MaxHealth = 2;
                                    Health = MaxHealth;
                                } else if(Building == 3){
                                    MaxHealth = 3;
                                    Health = MaxHealth;
                                }
                            }
                            else if (resource == -1)
                            {
                                ownerClient.Resources[0] -= ownerClient.requirments[0];
                                ownerClient.Resources[1] -= ownerClient.requirments[1];
                                ownerClient.Resources[2] -= ownerClient.requirments[2];
                                //Inputs[0] = 1;
                                Building = buildingsel;
                                spr2.sprite = sprites[Building];
                                SetColor(sel);
                                Owner = sel;
                                ownerClient = GameObject.Find(Owner + "").GetComponent<ClientStuff>();
                                TakesInput = true;
                                //ResourcesProducing[1] = 1;
                                if(Building == 4){
                                    MaxHealth = 4;
                                    Health = MaxHealth;
                                }
                            }

                            if (resource == 1 && Building == 3)
                            {
                                ResourcesProducing[0] = 1;
                            }
                            if (resource == 2 && Building == 3)
                            {
                                ResourcesProducing[1] = 1;
                            }

                        }
                       // else 
                    }
                    
                    if (buildingsel == -2 && Building != 1)
                    {

                        Deconstruct(sel, buildingsel, true);
                    }
                }
                if (buildingsel == -3)
                {
                    if(unit != null){
                        unit.Clicked();
                    }
                    if (client.selectedUnit != null && client.selectedUnit.moveable.Contains(this) && client.selectedUnit.movementPoints > 0)
                    {
                        //client.selectedUnit
                        client.selectedUnit.Move(this);
                        client.selectedUnit.movementPoints--;
                        client.selectedUnit = null;
                        
                    }
                    else if(client.selectedUnit != null && client.selectedUnit.attackable.Contains(this) && client.selectedUnit.movementPoints > 0)
                    {
                        client.selectedUnit.ToggleMoveZone(false);
                        if(unit == null){
                       this.Health--;
                        } else {
                            unit.Health--;
                        }
                       client.selectedUnit.movementPoints--;
                       client.selectedUnit = null;
                     }
                 }
                }
                else if(buildingsel == 0){
                    client.SelectedTile = this;
                    if(unit != null){
                        client.selectedUnit = unit;
                    }
                }
                if(buildingsel == -4){
                    if(Building == 4){
                        if(ownerClient.Resources[0] >= 1 && ownerClient.Resources[1] >= 1){
                            ownerClient.Resources[0] -= 1;
                            ownerClient.Resources[1] -= 1;
                            ownerClient.Resources[2]++;
                        }
                    }
                }
            
            }
            if(Health <= 0 && Building != 0){
                
            Deconstruct(PhotonNetwork.LocalPlayer.ActorNumber, -2, false);
            
            }
            map.UpdateMoves();
    }

    public void Deconstruct(int sel, int buildingsel, bool refund){
        PV.RPC("RPC_Deconstruct", RpcTarget.All, sel ,buildingsel);      
    }

    public void Connections(int sel, int buildingsel){
         PV.RPC("RPC_Connections", RpcTarget.All, sel ,buildingsel);
    }

    [PunRPC]
    public void RPC_Connections(int sel, int buildingsel)
    {
        
    }
    [PunRPC]
    public void RPC_Deconstruct(int sel, int buildingsel)
    {
                        Building = 0;
                        spr2.sprite = sprites[Building];
                        
                        for (int i = 0; i < connection.Count; i++)
                        {
                            //int index = connection.IndexOf(client.pos);
                            //Destroy(Lines[i].gameObject);
                            //Destroy(Lines[i].gameObject);
                            //Lines.RemoveAt(i);
                            //int ind = connection[i].gameObject.GetComponent<Tile>().connectionFrom.IndexOf(transform);
                            connection[i].gameObject.GetComponent<Tile>().connectionFrom.Remove(transform);
                            
                            //Destroy(connection[i].gameObject.GetComponent<Tile>().Lines[ind]);
                            connection.RemoveAt(i);
                            //Lines.RemoveAt(i);
                        }
                        for (int i = 0; i < connectionFrom.Count; i++)
                        {
                            //int index = connection.IndexOf(client.pos);
                           // Destroy(Lines[i].gameObject);
                            //connection[i].gameObject.GetComponent<Tile>().connection.Remove(transform);
                            Destroy(connectionFrom[i].gameObject.GetComponent<Tile>().Lines[i].gameObject);
                            connectionFrom[i].gameObject.GetComponent<Tile>().Lines.RemoveAt(i);
                            connectionFrom.RemoveAt(i);
                            //Lines.RemoveAt(i);
                        }
                        for (int i = 0; i < Lines.Count; i++)
                        {
                            //int index = connection.IndexOf(client.pos);
                            Destroy(Lines[i].gameObject);
                            //connection[i].gameObject.GetComponent<Tile>().connection.Remove(transform);
                            //connectionFrom.RemoveAt(i);
                            Lines.RemoveAt(i);
                        }
                        selectedForCon = false;
                        client.sec = false;
                        client.pos = null;
                        //ResourcesSum[0] = 0;
                        //map.ReloadAllResources();
                        //alreadychecked.Clear();
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
        //tileID = MapGenerator.map.ID;
        
        if (PhotonNetwork.IsMasterClient)
        {
            //sel = Random.Range(-1000f, 1000f);
            map = MapGenerator.map;
            PV.RPC("RPC_SetUp", RpcTarget.All, MapGenerator.map.ID ,map.randCL[0], map.randCL[1]);
        }
       // Debug.Log(PhotonNetwork.LocalPlayer.ActorNumber);
       // client = GameObject.Find(PhotonNetwork.LocalPlayer.ActorNumber + "").GetComponent<ClientStuff>();
    }

    public void BackProp(Tile origin)
    {
        if((((RoomManager.rm.Turn - 1) % PhotonNetwork.CurrentRoom.PlayerCount) + 1 == origin.Owner)){
        alreadychecked.Add(origin.transform);
        Transfer2(origin);
        foreach (Transform tile in origin.connectionFrom)
        {
            if (!alreadychecked.Contains(tile))
            {
                BackProp(tile.GetComponent<Tile>());
            }
        }
        }
        return;
     }

    public void Transfer2(Tile orig)
    { 
         if (orig.ResourcesSum[0] >= orig.Inputs[0] && orig.ResourcesSum[1] >= orig.Inputs[1])
        {
            Active = true;
            orig.ResourcesSum[0] -= orig.Inputs[0];
            orig.ResourcesSum[1] -= orig.Inputs[1];
        }
        else
        {
            Active = false;
        }
        if (Active)
        {
            orig.ResourcesSum[0] += orig.ResourcesProducing[0];
            orig.ResourcesSum[1] += orig.ResourcesProducing[1];
        }
        Tile t = null;
        //Tile t2 = null;
        //iuh
        //while()
        foreach (Transform trans in orig.connection)
        {
            Tile a = trans.gameObject.GetComponent<Tile>();
            if (a.TakesInput)
            {
                //int test = 0;
                while (a.Inputs[0] >= 1 && orig.ResourcesSum[0] >= a.Inputs[0])
                {
                    a.ResourcesSum[0]++;
                    orig.ResourcesSum[0]--;
                    //test++;
                }
            }
            if (a.Building == 1 || a.Building == 2)
            {
                t = a;
                
            }


        }
        if (t != null)
        {
            while (orig.ResourcesSum[0] > 0)
            {
                t.ResourcesSum[0]++;
                orig.ResourcesSum[0]--;
            }
                while (orig.ResourcesSum[1] > 0)
                {
                    t.ResourcesSum[1]++;
                    orig.ResourcesSum[1]--;
                }
         }
    }

    [PunRPC]
    public void RPC_Transfer()
    {
        //alreadychecked.Clear();
        //Transfer2(this);
        //BackProp(this);
     }
    public void TransferResources()
    {
        PV.RPC("RPC_Transfer", RpcTarget.All);
    }

    // Update is called once per frame
    void Update()
    {
       if(Health > 0){
        spr2.color = new Color(spr2.color.r, spr2.color.g, spr2.color.b, ((Health / MaxHealth) * 0.75f) + 0.25f);
       }
       if(Building == 1){
        //Resources[0]
        //Resources[0]
       }
    }

    void OnMouseDown() {
        if(client.MyTurn){
            if(ownerClient != null){
            if (ownerClient.selection == -3 && ownerClient.MyTurn)
                        {
                            Debug.Log("as");
                        if (Building == 1 && unit == null && ownerClient.Resources[2] >= 3)
                        {
                            ownerClient.Resources[0] -= 0;
                            ownerClient.Resources[1] -= 0;
                            ownerClient.Resources[2] -= 3;
                            PhotonNetwork.Instantiate(Path.Combine("Unit"), transform.position, transform.rotation);
                            
                            }
                         }
            }
            PV.RPC("RPC_Clicked", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, client.selection, client.FirstTurn);
        }
    }
}
