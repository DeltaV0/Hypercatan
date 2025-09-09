using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.IO;

public class Unit : MonoBehaviour
{
    public SpriteRenderer spr;
    public int Type;

   // public int MoveSpeed;

    public int[] MaxmovementPoints;
    public int[] movementPoints;

    public bool OmniMove;

    //public int moveRange;
    public int attackRange;

    public ClientStuff Owner;

    public int OwnerNum;

    public ClientStuff Client;

    public Tile current;

    public List<Tile> moveable;

    public List<Tile> attackable;

    public PhotonView PV;

    public int Health;
    public int MaxHealth;

    public int damage;

    public int CounterAttackDamage;

    public int[] MovableTerrainTypes;
    public int[] AttackableTerrainTypes;

    public bool colonize;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        
        
        MapGenerator.map.units.Add(this);
        moveable.Clear();
        attackable.Clear();
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Collider2D q = Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y), 0.1f, LayerMask.GetMask("Tile"));
        current = q.gameObject.GetComponent<Tile>();
        Owner = current.ownerClient;
        Owner.OwnedUnit.Add(this);
        OwnerNum = current.Owner;
        Client = current.client;
        spr.color = current.spr[2].color;
        current.unit = this;
        transform.position = new Vector3(transform.position.x, transform.position.y, current.transform.position.z - 0.1f);

        damage =(int)(damage * (1 + current.ownerClient.techTree.Bonuses[Type,0]));
        CounterAttackDamage = (int)(CounterAttackDamage * (1 + current.ownerClient.techTree.Bonuses[Type,0]));
        MaxHealth = (int)(MaxHealth * (current.ownerClient.techTree.Bonuses[Type,1] + 1));
        Health = MaxHealth;
        Collider2D[] l = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), 1f, LayerMask.GetMask("Tile"));
        for (int i = 0; i < l.Length; i++)
        {   
            Tile t = l[i].gameObject.GetComponent<Tile>();
            if (t != current && t.Building == 0 && t.unit == null && MovableTerrainTypes.Contains(t.terrainType))
            {
                moveable.Add(t);
             }
         }

         Collider2D[] b = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), 0.95f * attackRange, LayerMask.GetMask("Tile"));
        for (int i = 0; i < b.Length; i++)
        {   
            Tile t = b[i].gameObject.GetComponent<Tile>();
            if (t != current && (t.Building != 0 || t.unit != null) && !moveable.Contains(t) && !(t.Building != 0 && t.ownerClient == Owner && (t.unit == null || (t.unit != null && t.unit.OwnerNum == OwnerNum))) && AttackableTerrainTypes.Contains(t.terrainType))
            {
                attackable.Add(t);
             }
         }
    }

    public void Clicked() {
        if (Owner.MyTurn)
        {
            if(Owner.selection == -3){
            if (Owner.selectedUnit != this)
            {
                if(movementPoints[0] > 0){
                    foreach (Tile tile in moveable)
                {
                    tile.MoveCircle[0].SetActive(true);
                }
                Owner.selectedUnit = this;
                }
                if(movementPoints[1] > 0){
                    foreach (Tile tile in attackable)
                {
                    tile.MoveCircle[1].SetActive(true);
                }
                Owner.selectedUnit = this;
                }
                
                //Owner.selection = -3;
            }
            else
            {
                foreach (Tile tile in moveable)
                {
                    tile.MoveCircle[0].SetActive(false);
                }
                foreach (Tile tile in attackable)
                {
                    tile.MoveCircle[1].SetActive(false);
                }
                Owner.selectedUnit = null;
                //Owner.selection = -3;
             }
            }
        }/* else if(Client.selectedUnit.attackable.Contains(current)){
            Health--;
            Client.selectedUnit.movementPoints--;
        }*/
    }

    public void ToggleMoveZone(bool tf){
        PV.RPC("RPC_ToggleMoveZone", RpcTarget.All, tf);
    }

    [PunRPC]
    public void RPC_ToggleMoveZone(bool tf)
    {
        foreach (Tile tile in moveable)
            {
                tile.MoveCircle[0].SetActive(tf);
             }
             foreach (Tile tile in attackable)
            {
                tile.MoveCircle[1].SetActive(tf);
             }
    }

    public void Move(Tile tile)
    {
        Debug.Log("assdsa");
        PV.RPC("RPC_Move", RpcTarget.All, tile.transform.position.x, tile.transform.position.y);
     }

    public void UpdateMoves(){
        PV.RPC("RPC_Moves", RpcTarget.All);
    }

    public void Damage(int a){
        PV.RPC("RPC_DamageUnit", RpcTarget.All, a);
    }

    [PunRPC]
    public void RPC_DamageUnit(int a){
        Health -= a;
    }

    [PunRPC]
    public void RPC_Moves()
    {
        attackable.Clear();
        moveable.Clear();
        Collider2D[] l = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), 1f, LayerMask.GetMask("Tile"));
        for (int i = 0; i < l.Length; i++)
        {   
            Tile t = l[i].gameObject.GetComponent<Tile>();
            if (t != current && t.Building == 0 && t.unit == null && MovableTerrainTypes.Contains(t.terrainType))
            {
                moveable.Add(t);
             }
         }

         Collider2D[] b = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), 0.95f * attackRange, LayerMask.GetMask("Tile"));
        for (int i = 0; i < b.Length; i++)
        {   
            Tile t = b[i].gameObject.GetComponent<Tile>();
            if (t != current && (t.Building != 0 || t.unit != null) && !moveable.Contains(t) && !(t.Building != 0 && t.ownerClient == Owner && (t.unit == null || (t.unit != null && t.unit.OwnerNum == OwnerNum))) && AttackableTerrainTypes.Contains(t.terrainType))
            {
                attackable.Add(t);
             }
         }
    }

    [PunRPC]
    public void RPC_Move(float x, float y)
    {
        foreach (Tile tile in moveable)
            {
                tile.MoveCircle[0].SetActive(false);
             }
             foreach (Tile tile in attackable)
            {
                tile.MoveCircle[1].SetActive(false);
             }
             current.unit = null;
        attackable.Clear();
        moveable.Clear();
        transform.position = new Vector3(x, y, -1);
        Collider2D q = Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y), 0.1f, LayerMask.GetMask("Tile"));
        current = q.gameObject.GetComponent<Tile>();
        current.unit = this;
        transform.position = new Vector3(transform.position.x, transform.position.y, current.transform.position.z - 0.1f);

        Collider2D[] l = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), 1f, LayerMask.GetMask("Tile"));
        for (int i = 0; i < l.Length; i++)
        {   
            Tile t = l[i].gameObject.GetComponent<Tile>();
            if (t != current && t.Building == 0 && t.unit == null && MovableTerrainTypes.Contains(t.terrainType))
            {
                moveable.Add(t);
             }
         }

         Collider2D[] b = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), 0.95f  * attackRange, LayerMask.GetMask("Tile"));
        for (int i = 0; i < b.Length; i++)
        {   
            Tile t = b[i].gameObject.GetComponent<Tile>();
            if (t != current && (t.Building != 0 || t.unit != null) && !moveable.Contains(t) && !(t.Building != 0 && t.ownerClient == Owner && (t.unit == null || (t.unit != null && t.unit.OwnerNum == OwnerNum))) && AttackableTerrainTypes.Contains(t.terrainType))
            {
                attackable.Add(t);
             }
         }
     }
    // Update is called once per frame
    void FixedUpdate()
    {
        if(movementPoints[0] + movementPoints[1] <= 0){
        spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, 0.5f);
        } else {
            spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, 1f);
        }
        if(Health <= 0){
            current.map.units.Remove(this);
                                Owner.OwnedUnit.Remove(this);
                                PV.TransferOwnership(PV.Owner.Get(OwnerNum));
                                PhotonNetwork.Destroy(gameObject);
        }
        if(colonize){
            if(current.terrainType != -1 && (current.Owner == OwnerNum || current.Owner == 0) && current.Building == 0){
                current.Click(200, false);
                current.map.units.Remove(this);
                Owner.OwnedUnit.Remove(this);
                PV.TransferOwnership(PV.Owner.Get(OwnerNum));
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}
