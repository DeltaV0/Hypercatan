using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour
{
    public ParticleSystem[] parts;
    public int Health;
    public int MaxHealth;
    public bool TakesInput;

    public SpriteRenderer[] spr;

    public SpriteRenderer spr2;

    public GameObject[] MoveCircle;

    public GameObject[] TerrainIndicator;

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

    public int cooldown;

    public GameObject SelectedObject;

    public Tile NearestCity;

    public int[] CityVars;

    public bool port;

    public int[] returnResources;
    //private int sel;

    public void PlayParticle(int a)
    {
        if(ownerClient == client){
        PV.RPC("RPC_Parts", RpcTarget.All, a);
        }
     }

    [PunRPC]
    public void RPC_Parts(int a)
    {
        //if(PV.IsMine){
        parts[a].Play();
        //}
     }

    public void GetNeighbors()
    {
        Collider2D[] l = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), 1f, LayerMask.GetMask("Tile"));
        for (int i = 0; i < l.Length; i++)
        {
            Tile t = l[i].gameObject.GetComponent<Tile>();
            if (t != this)
            {
                neighbors.Add(t);
                if (t.terrainType == -1)
                {
                    port = true;
                }
            }
        }
    }

    [PunRPC]
    public void RPC_SetUp(int ID, float selx, float sely)
    {
        port = false;
        map = MapGenerator.map;
        //connection.RemoveAt(0);
        map.tiles.Add(this);
        me = this;
        tileID = ID;
        
        float samp = Mathf.PerlinNoise((transform.localPosition.x * 0.4f) + selx, (transform.localPosition.y * 0.4f) + sely);
        float samp2 = Mathf.PerlinNoise((transform.localPosition.x * 0.2f) - selx * 3f, (transform.localPosition.y * 0.1f) + sely * 3f);
        float samp4 = Mathf.PerlinNoise((transform.localPosition.x * 0.2f) - selx * 2f, (transform.localPosition.y * 0.4f) + sely * 2f);

        float samp3 = Mathf.PerlinNoise((transform.localPosition.x * 0.1f) - selx, (transform.localPosition.y * 0.1f) + sely);
        float noise = Mathf.PerlinNoise((transform.localPosition.x * 0.4f) - selx * 2f, (transform.localPosition.y * 0.4f) + sely);
        float rivers = Mathf.PerlinNoise((transform.localPosition.x * 0.4f) - selx, (transform.localPosition.y * 0.4f) + sely);
        float height = Mathf.Pow(samp3, 7) - (Vector2.Distance(new Vector2(map.size / 3, -1.5f * (map.size / 3)), new Vector2(transform.localPosition.x, transform.localPosition.y)) * 0.02f) + (noise * 0.5f) + 0.75f - (Mathf.Pow(Mathf.Sin(Mathf.Abs(rivers - 0.5f) * 0.5f - 1), 6) * 2);
        gameObject.name = "tile " + tileID;
        if (Mathf.Pow(samp + (height * 0.1f), 1.5f) > 0.55f)
        {
            resource = 1;

        } else if(samp < 0.25f){

            resource = 2;
        }
        else if (samp2 > 0.6f && height < 0.65f)
        {
            resource = -1;

        } else if (samp4 > 0.55f && height < 0.65f)
        {
            resource = -2;
        }
        transform.position = new Vector3(transform.position.x, transform.position.y, height * -3);

        if (height > 0.65f)
        {
            terrainType = 1;
            if(Random.Range(0, 2) == 0){
            TerrainIndicator[0].SetActive(true);
            TerrainIndicator[1].SetActive(false);
            } else {
                TerrainIndicator[1].SetActive(true);
            TerrainIndicator[0].SetActive(false);
            }
            TerrainIndicator[2].SetActive(false);
            //spr[1].color = new Color(0.5f, 0.5f, 0.5f);
        } else if(height <= 0.65f && height > 0.25f){
            terrainType = 0;
            TerrainIndicator[0].SetActive(false);
            TerrainIndicator[2].SetActive(false);
            TerrainIndicator[1].SetActive(false);
            //spr[1].color = new Color(1f, 1f, 1f);
        } else {
            transform.position = new Vector3(transform.position.x, transform.position.y, -0.5f);
            resource = 0;
            terrainType = -1;
            TerrainIndicator[2].SetActive(true);
            TerrainIndicator[0].SetActive(false);
            TerrainIndicator[1].SetActive(false);
            spr[0].color = new Color(0.5f, 0.5f, 0.75f);
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
            spr[0].color = new Color(1f, 1f, 0.25f);
        }
        if(resource == -2){
            spr[0].color = new Color(0.25f, 0.75f, 0.25f);
        }
    }

    public void SetColor(int sel)
    {
        PV.RPC("RPC_ColourChange", RpcTarget.All, sel);
     }

     [PunRPC]
     public void RPC_ColourChange(int sel){
        if (sel != 0)
        {

            spr2.color = Color.HSVToRGB((1f / PhotonNetwork.CurrentRoom.PlayerCount) * sel, 1, 1);
            spr[2].color = new Color(spr2.color.r, spr2.color.g, spr2.color.b, 0.1f);
            spr2.color = new Color(spr2.color.r, spr2.color.g, spr2.color.b, 0.1f);
            Color col = new Color(spr2.color.r, spr2.color.g, spr2.color.b, 0.75f);
            var start = parts[0].main.startColor; 
            start = col;
            start = parts[1].main.startColor; 
            start = col;
            start = parts[2].main.startColor; 
            start = col;
            start = parts[3].main.startColor; 
            start = col;
            start = parts[4].main.startColor; 
            start = col;
        }
        else
        {
            // spr2.color = Color.HSVToRGB((1f / PhotonNetwork.CurrentRoom.PlayerCount) * sel, 1, 1);
            spr[2].color = new Color(1, 1, 1, 0.1f);
            spr2.color = new Color(1, 1, 1, 0.1f);
        }
     }




    [PunRPC]
    public void RPC_Clicked(int sel, int buildingsel, bool first){
        //ClientStuff user;
        //if(PV.IsMine){
          //  user = client;
       // }
        if(ownerClient == null){
            ownerClient = GameObject.Find(sel + "").GetComponent<ClientStuff>();
        }
      // ownerClient = GameObject.Find(sel + "").GetComponent<ClientStuff>();
        if(first){
            if ((Owner == sel || Owner == 0) && terrainType != -1)
            {
                //  PhotonNetwork.Instantiate(client.stuff[0], transform.position, transform.rotation);
                ResourcesSum[0] = 3;
                ResourcesSum[1] = 0;
                ResourcesSum[2] = 00;
                ResourcesSum[3] = 2;
                ResourcesSum[4] = 00;

                
                
                CityVars[0] = 2;
                Building = 1;
                spr2.sprite = sprites[Building];
                Owner = sel;
                ownerClient = GameObject.Find(Owner + "").GetComponent<ClientStuff>();
                ownerClient.OwnedTiles.Add(this);
                ownerClient.cities.Add(this);
                ownerClient.Resources[0] = 3;
                ownerClient.Resources[1] = 0;
                ownerClient.Resources[2] = 00;
                ownerClient.Resources[3] = 2;
                ownerClient.Resources[4] = 00;
                Collider2D[] l = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), 2.9f, LayerMask.GetMask("Tile"));
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
                //if(PV.IsMine){
                PlayParticle(1);
                //}
                MaxHealth = 200;
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
            if (buildingsel != -1 && buildingsel != 0 && ownerClient.Resources[0] >= ownerClient.requirments[0] && ownerClient.Resources[1] >= ownerClient.requirments[1] && ownerClient.Resources[2] >= ownerClient.requirments[2] && ownerClient.Resources[3] >= ownerClient.requirments[3] && ownerClient.Resources[4] >= ownerClient.requirments[4] && unit == null && terrainType != -1)
            {
                if (Owner == sel)
                {
                    if (Building == 0)
                    {
                       // if (buildingsel > 0)
                       // {
                            if (buildingsel == 3 && (resource == 1 || resource == 2))
                            {
                                PV.RPC("RPC_GetNearestCityForClient", RpcTarget.All);
                                if(NearestCity.CityVars[1] < (int)(3f * NearestCity.CityVars[0])){
                                Owner = sel;
                                ownerClient = GameObject.Find(Owner + "").GetComponent<ClientStuff>();
                                ownerClient.Build();
                                returnResources[0] = ownerClient.requirments[0];
                                returnResources[1] = ownerClient.requirments[1];
                                returnResources[2] = ownerClient.requirments[2];
                                returnResources[3] = ownerClient.requirments[3];
                                returnResources[4] = ownerClient.requirments[4];
                                Building = buildingsel;
                                spr2.sprite = sprites[3];
                                SetColor(sel);
                                PlayParticle(3);
                                
                                if(Building == 3){
                                    MaxHealth = 50;
                                    Health = MaxHealth;
                                }
                                NearestCity.CityVars[1]++;
                                }
                                
                            }
                            else if (buildingsel == 4)
                            {
                                PV.RPC("RPC_GetNearestCityForClient", RpcTarget.All);
                                if(NearestCity.CityVars[1] < (int)(3f * NearestCity.CityVars[0])){
                                Owner = sel;
                                ownerClient = GameObject.Find(Owner + "").GetComponent<ClientStuff>();
                                ownerClient.Build();
                                returnResources[0] = ownerClient.requirments[0];
                                returnResources[1] = ownerClient.requirments[1];
                                returnResources[2] = ownerClient.requirments[2];
                                returnResources[3] = ownerClient.requirments[3];
                                returnResources[4] = ownerClient.requirments[4];
                                //Inputs[0] = 1;
                                Building = buildingsel;
                                spr2.sprite = sprites[4];
                                SetColor(sel);
                                PlayParticle(3);
                                Owner = sel;
                                ownerClient = GameObject.Find(Owner + "").GetComponent<ClientStuff>();
                                TakesInput = true;
                                //ResourcesProducing[1] = 1;
                                if(Building == 4){
                                    MaxHealth = 125;
                                    Health = MaxHealth;
                                }
                                NearestCity.CityVars[1]++;
                                }
                            } else if (buildingsel == 5 && resource == -1)
                            {
                                PV.RPC("RPC_GetNearestCityForClient", RpcTarget.All);
                                Owner = sel;
                                ownerClient = GameObject.Find(Owner + "").GetComponent<ClientStuff>();
                                ownerClient.Build();
                                returnResources[0] = ownerClient.requirments[0];
                                returnResources[1] = ownerClient.requirments[1];
                                returnResources[2] = ownerClient.requirments[2];
                                returnResources[3] = ownerClient.requirments[3];
                                returnResources[4] = ownerClient.requirments[4];
                                //Inputs[0] = 1;
                                Building = buildingsel;
                                spr2.sprite = sprites[6];
                                SetColor(sel);
                                PlayParticle(3);
                                TakesInput = true;
                                //ResourcesProducing[1] = 1;
                                if(Building == 5){
                                    MaxHealth = 75;
                                    Health = MaxHealth;
                                }
                                NearestCity.CityVars[1]--;
                            } else if (buildingsel == 6 && resource == -2)
                            {
                                PV.RPC("RPC_GetNearestCityForClient", RpcTarget.All);
                                if(NearestCity.CityVars[1] < (int)(3f * NearestCity.CityVars[0])){
                                    Owner = sel;
                                ownerClient = GameObject.Find(Owner + "").GetComponent<ClientStuff>();
                                ownerClient.Build();
                                returnResources[0] = ownerClient.requirments[0];
                                returnResources[1] = ownerClient.requirments[1];
                                returnResources[2] = ownerClient.requirments[2];
                                returnResources[3] = ownerClient.requirments[3];
                                returnResources[4] = ownerClient.requirments[4];
                                //Inputs[0] = 1;
                                Building = buildingsel;
                                spr2.sprite = sprites[7];
                                SetColor(sel);
                                PlayParticle(3);
                                TakesInput = true;
                                //ResourcesProducing[1] = 1;
                                if(Building == 6){
                                    MaxHealth = 125;
                                    Health = MaxHealth;
                                }
                                NearestCity.CityVars[1]++;
                                }
                            } else if (buildingsel == 7)
                            {
                                PV.RPC("RPC_GetNearestCityForClient", RpcTarget.All);
                            //if(NearestCity.CityVars[1] < (int)(2.5f * NearestCity.CityVars[0])){
                                Owner = sel;
                                ownerClient = GameObject.Find(Owner + "").GetComponent<ClientStuff>();
                                ownerClient.Build();
                                returnResources[0] = ownerClient.requirments[0];
                                returnResources[1] = ownerClient.requirments[1];
                                returnResources[2] = ownerClient.requirments[2];
                                returnResources[3] = ownerClient.requirments[3];
                                returnResources[4] = ownerClient.requirments[4];
                                //Inputs[0] = 1;
                                Building = buildingsel;
                                spr2.sprite = sprites[8];
                                SetColor(sel);
                                PlayParticle(3);
                                TakesInput = true;
                                //ResourcesProducing[1] = 1;
                                if(Building == 7){
                                    MaxHealth = 150;
                                    Health = MaxHealth;
                                }
                                //NearestCity.CityVars[1]++;
                                //}
                            }  else if (buildingsel == 8)
                            {
                                PV.RPC("RPC_GetNearestCityForClient", RpcTarget.All);
                                if(NearestCity.CityVars[1] < (int)(3f * NearestCity.CityVars[0])){
                                Owner = sel;
                                ownerClient = GameObject.Find(Owner + "").GetComponent<ClientStuff>();
                                ownerClient.Build();
                                returnResources[0] = ownerClient.requirments[0];
                                returnResources[1] = ownerClient.requirments[1];
                                returnResources[2] = ownerClient.requirments[2];
                                returnResources[3] = ownerClient.requirments[3];
                                returnResources[4] = ownerClient.requirments[4];
                                //Inputs[0] = 1;
                                Building = buildingsel;
                                spr2.sprite = sprites[9];
                                SetColor(sel);
                                PlayParticle(3);
                                
                                //TakesInput = true;
                                //ResourcesProducing[1] = 1;
                                if(Building == 8){
                                    MaxHealth = 100;
                                    Health = MaxHealth;
                                }
                                NearestCity.CityVars[1]++;
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
                            if (resource == -1 && Building == 5)
                            {
                                ResourcesProducing[4] = 1;
                            }
                            if (resource == -2 && Building == 6)
                            {
                                ResourcesProducing[3] = 1;
                            }

                        //}
                       // else 
                    }
                    
                    if (buildingsel == -2 && Building != 1 && Building != 100)
                    {
                        
                        Deconstruct(sel, buildingsel, true);
                       // NearestCity.CityVars[1]--;
                    }
                    if(buildingsel == 100){
                        if(Building == 0 && ownerClient.Resources[2] >= 2 && ownerClient.Resources[3] >= 4 && terrainType != -1){
                        Owner = sel;
                        ownerClient = GameObject.Find(Owner + "").GetComponent<ClientStuff>();
                        MaxHealth = 150;
                        Health = MaxHealth;
                        CityVars[0] = 1;
                        Building = buildingsel;
                                spr2.sprite = sprites[5];
                                SetColor(sel);
                                PlayParticle(2);
                                ownerClient.ChangeResource(2, -2);
                                ownerClient.ChangeResource(3, -4);
                                ownerClient.cities.Add(this);
                            Collider2D[] l = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), 1.95f, LayerMask.GetMask("Tile"));
                            for (int i = 0; i < l.Length; i++)
                        {   
                            Tile t = l[i].gameObject.GetComponent<Tile>();
                            if (t != this && (t.Owner == 0))
                        {       
                                t.Owner = sel;
                                t.ownerClient = GameObject.Find(Owner + "").GetComponent<ClientStuff>();
                                t.SetColor(sel);
                                SphereOfInfluence.Add(t);
                                ownerClient.OwnedTiles.Add(t);
                            }
                        }      
                        } 
                    } else if(buildingsel == 200){
                        Owner = sel;
                        ownerClient = GameObject.Find(Owner + "").GetComponent<ClientStuff>();
                        MaxHealth = 150;
                        Health = MaxHealth;
                        CityVars[0] = 1;
                        Building = 100;
                                spr2.sprite = sprites[5];
                                SetColor(sel);
                                PlayParticle(2);
                               // ownerClient.Resources[2] -= 2;
                               // ownerClient.Resources[3] -= 4;
                                ownerClient.cities.Add(this);
                            Collider2D[] l = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), 1.95f, LayerMask.GetMask("Tile"));
                            for (int i = 0; i < l.Length; i++)
                        {   
                            Tile t = l[i].gameObject.GetComponent<Tile>();
                            if (t != this && (t.Owner == 0))
                        {       
                                t.Owner = sel;
                                t.ownerClient = GameObject.Find(Owner + "").GetComponent<ClientStuff>();
                                t.SetColor(sel);
                                SphereOfInfluence.Add(t);
                                ownerClient.OwnedTiles.Add(t);
                            }
                        } 
                    }
                }
                
                }
                else if(buildingsel == 0){
                    client.SelectedTile = this;
                    if(unit != null){
                        client.selectedUnit = unit;
                    }
                }
                if (buildingsel == -3)
                {
                    Debug.Log("sdas");
                    if(unit != null){
                        Debug.Log("aaa");
                        unit.Clicked();
                    }
                    if (client.selectedUnit != null && client.selectedUnit.moveable.Contains(this) && client.selectedUnit.movementPoints[0] > 0)
                    {
                        //client.selectedUnit
                        client.selectedUnit.Move(this);
                        client.selectedUnit.movementPoints[0]--;
                        client.selectedUnit = null;
                        
                    }
                    else if(client.selectedUnit != null && client.selectedUnit.attackable.Contains(this) && client.selectedUnit.movementPoints[1] > 0 && !(client.selectedUnit.OmniMove && client.selectedUnit.movementPoints[0] <= 0))
                    {
                        client.selectedUnit.ToggleMoveZone(false);
                        if(unit == null){
                            Damage(client.selectedUnit.damage);
                        } else {
                            unit.Damage(client.selectedUnit.damage);
                            if(unit.Health > 0){
                                client.selectedUnit.Damage(unit.CounterAttackDamage);
                            }
                        }
                       client.selectedUnit.movementPoints[1]--;
                       if(client.selectedUnit.OmniMove){
                        client.selectedUnit.movementPoints[0]--;
                       }
                       client.selectedUnit = null;
                     }
                 }
                if(buildingsel == -4){
                    if(Building == 4 && cooldown == 0 && sel == Owner){
                        if(ownerClient.Resources[0] >= 1 && ownerClient.Resources[1] >= 1){
                            PlayParticle(4);
                            spr2.color = new Color(spr2.color.r, spr2.color.g, spr2.color.b, 0.5f);
                            cooldown = 1;
                            if(PV.IsMine){
                            ownerClient.ChangeResource(0, -1);
                            ownerClient.ChangeResource(1, -1);
                            ownerClient.ChangeResource(2, 1);
                            }
                            //NearestCity.CityVars[2]--;
                        }
                    }
                }
                if(buildingsel == -5){
                    if(Building == 7 && cooldown == 0 && sel == Owner){
                        if(ownerClient.Resources[0] >= 1 && ownerClient.Resources[1] >= 1){
                            spr2.color = new Color(spr2.color.r, spr2.color.g, spr2.color.b, 0.5f);
                            cooldown = 1;
                           // ownerClient.Resources[0] -= 1;
                            ownerClient.Resources[1] -= 1;
                            NearestCity.CityVars[2] += 2;
                           // ownerClient.Resources[2]++;
                        }
                    }
                }
            
            }
            if(Health <= 0 && Building != 0){
                
            Deconstruct(PhotonNetwork.LocalPlayer.ActorNumber, -2, false);
            
            }
            map.UpdateMoves();
    }

    public void Damage(int a){
        PV.RPC("RPC_Damage", RpcTarget.All, a);
    }

    [PunRPC]
    public void RPC_Damage(int a){
        Health -= a;
    }

    [PunRPC]
    public void RPC_GetNearestCityForClient(){
        Tile nearby = null;
        float dist = 100f;
        foreach(Tile t in ownerClient.cities){
            if(Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(t.transform.position.x, t.transform.position.y)) < dist){
                nearby = t;
                dist = Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(t.transform.position.x, t.transform.position.y));
            }
        }
        NearestCity = nearby;
    }

    public void Deconstruct(int sel, int buildingsel, bool refund){
        if(refund){
        ownerClient.Resources[0] += returnResources[0];
        ownerClient.Resources[1] += returnResources[1];
        ownerClient.Resources[2] += returnResources[2];
        ownerClient.Resources[3] += returnResources[3];
        ownerClient.Resources[4] += returnResources[4];
        }
        PV.RPC("RPC_Deconstruct", RpcTarget.All, sel ,buildingsel, refund);      
    }

    [PunRPC]
    public void RPC_Deconstruct(int sel, int buildingsel, bool re)
    {
        if(NearestCity != null && (Building == 3 || Building == 4 || Building == 6 || Building == 7 || Building == 8)){
        NearestCity.CityVars[1]--;
        }
        Building = 0;
        spr2.sprite = sprites[Building];
        ResourcesProducing[1] = 0;
        ResourcesProducing[0] = 0;
        ResourcesProducing[2] = 0;
        ResourcesProducing[3] = 0;
        ResourcesProducing[4] = 0;
        
        foreach(Tile t in SphereOfInfluence){
            t.Owner = 0;
            t.ownerClient = null;
            t.SetColor(0);
            ownerClient.OwnedTiles.Remove(t);
        }
        SphereOfInfluence.Clear();
                        
                        
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


    // Update is called once per frame
    void Update()
    {
       if(Health > 0){
        if(cooldown > 0){
        spr2.color = new Color(spr2.color.r, spr2.color.g, spr2.color.b, (((Health / MaxHealth) * 0.75f) + 0.25f) / 2);
        } else {
            spr2.color = new Color(spr2.color.r, spr2.color.g, spr2.color.b, (((Health / MaxHealth) * 0.75f) + 0.25f));
        }
       }
       if(client != null){
       SelectedObject.SetActive(client.SelectedTile == this);
       }
    }

    public void Click(int s, bool a){
        if(client.MyTurn){
            if(ownerClient != null){
            if (s == -100 && ownerClient.MyTurn)
                        {
                            Debug.Log("as");
                        if ((Building == 1 || Building == 100) && unit == null && ownerClient.Resources[2] >= ownerClient.techTree.UnitPrices[0, 2] && ownerClient.Resources[4] >= ownerClient.techTree.UnitPrices[0, 4])
                        {
                            ownerClient.Resources[0] -= ownerClient.techTree.UnitPrices[0,0];
                            ownerClient.Resources[1] -= ownerClient.techTree.UnitPrices[0,1];
                            ownerClient.Resources[2] -= ownerClient.techTree.UnitPrices[0,2];
                            ownerClient.Resources[3] -= ownerClient.techTree.UnitPrices[0,3];
                            ownerClient.Resources[4] -= ownerClient.techTree.UnitPrices[0, 4];
                            PhotonNetwork.Instantiate(Path.Combine("UnitTank"), transform.position, transform.rotation);
                            
                            }
                         }
            if (s == -101 && ownerClient.MyTurn)
                        {
                            Debug.Log("as");
                        if ((Building == 1 || Building == 100) && unit == null && ownerClient.Resources[2] >= ownerClient.techTree.UnitPrices[1, 2] && ownerClient.Resources[4] >= ownerClient.techTree.UnitPrices[1, 4])
                        {
                            ownerClient.Resources[0] -= ownerClient.techTree.UnitPrices[1, 0];
                            ownerClient.Resources[1] -= ownerClient.techTree.UnitPrices[1, 1];
                            ownerClient.Resources[2] -= ownerClient.techTree.UnitPrices[1, 2];
                            ownerClient.Resources[3] -= ownerClient.techTree.UnitPrices[1, 3];
                            ownerClient.Resources[4] -= ownerClient.techTree.UnitPrices[1, 4];
                            PhotonNetwork.Instantiate(Path.Combine("UnitInfantry"), transform.position, transform.rotation);
                            
                            }
                         }
            if (s == -102 && ownerClient.MyTurn)
                        {
                            Debug.Log("as");
                        if ((Building == 1 || Building == 100) && unit == null && ownerClient.Resources[2] >= ownerClient.techTree.UnitPrices[2, 2] && ownerClient.Resources[4] >= ownerClient.techTree.UnitPrices[2, 4])
                        {
                            ownerClient.Resources[0] -= ownerClient.techTree.UnitPrices[2, 0];
                            ownerClient.Resources[1] -= ownerClient.techTree.UnitPrices[2, 1];
                            ownerClient.Resources[2] -= ownerClient.techTree.UnitPrices[2, 2];
                            ownerClient.Resources[3] -= ownerClient.techTree.UnitPrices[2, 3];
                            ownerClient.Resources[4] -= ownerClient.techTree.UnitPrices[2, 4];
                            PhotonNetwork.Instantiate(Path.Combine("UnitArtillery"), transform.position, transform.rotation);
                            
                            }
                         }
            if (s == -103 && ownerClient.MyTurn && port)
                        {
                            Debug.Log("as");
                        if ((Building == 1 || Building == 100) && unit == null && ownerClient.Resources[2] >= ownerClient.techTree.UnitPrices[3, 2] && ownerClient.Resources[4] >= ownerClient.techTree.UnitPrices[3, 4] && ownerClient.Resources[0] >= ownerClient.techTree.UnitPrices[3, 0])
                        {
                            ownerClient.Resources[0] -= ownerClient.techTree.UnitPrices[3, 0];
                            ownerClient.Resources[1] -= ownerClient.techTree.UnitPrices[3, 1];
                            ownerClient.Resources[2] -= ownerClient.techTree.UnitPrices[3, 2];
                            ownerClient.Resources[3] -= ownerClient.techTree.UnitPrices[3, 3];
                            ownerClient.Resources[4] -= ownerClient.techTree.UnitPrices[3, 4];
                            PhotonNetwork.Instantiate(Path.Combine("UnitBattleShip"), transform.position, transform.rotation);
                            
                            }
                         }
            if (s == -104 && ownerClient.MyTurn && port)
                        {
                            Debug.Log("as");
                        if ((Building == 1 || Building == 100) && unit == null && ownerClient.Resources[2] >= ownerClient.techTree.UnitPrices[4, 2] && ownerClient.Resources[3] >= ownerClient.techTree.UnitPrices[4, 3] && ownerClient.Resources[4] >= ownerClient.techTree.UnitPrices[4, 4])
                        {
                            ownerClient.Resources[0] -= ownerClient.techTree.UnitPrices[4, 0];
                            ownerClient.Resources[1] -= ownerClient.techTree.UnitPrices[4, 1];
                            ownerClient.Resources[2] -= ownerClient.techTree.UnitPrices[4, 2];
                            ownerClient.Resources[3] -= ownerClient.techTree.UnitPrices[4, 3];
                            ownerClient.Resources[4] -= ownerClient.techTree.UnitPrices[4, 4];
                            PhotonNetwork.Instantiate(Path.Combine("UnitColony"), transform.position, transform.rotation);
                            
                            }
                         }
            }
            PV.RPC("RPC_Clicked", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, s, client.FirstTurn);
        }
    }

    void OnMouseDown() {
        if(!EventSystem.current.IsPointerOverGameObject()){
        Click(client.selection, client.FirstTurn);
        }
    }
}
