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

    public int MoveSpeed;

    public int movementPoints;

    public int attackRange;

    public ClientStuff Owner;

    public int OwnerNum;

    public ClientStuff Client;

    public Tile current;

    public List<Tile> moveable;

    public List<Tile> attackable;

    public PhotonView PV;

    public int Health;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        
        Health = 3;
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

        Collider2D[] l = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), 1f, LayerMask.GetMask("Tile"));
        for (int i = 0; i < l.Length; i++)
        {   
            Tile t = l[i].gameObject.GetComponent<Tile>();
            if (t != current && t.Building == 0 && t.unit == null)
            {
                moveable.Add(t);
             }
         }

         Collider2D[] b = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), attackRange, LayerMask.GetMask("Tile"));
        for (int i = 0; i < b.Length; i++)
        {   
            Tile t = b[i].gameObject.GetComponent<Tile>();
            if (t != current && (t.Building != 0 || t.unit != null) && !moveable.Contains(t) && !(t.Building != 0 && t.ownerClient == Owner && (t.unit == null || (t.unit != null && t.unit.OwnerNum == OwnerNum))))
            {
                attackable.Add(t);
             }
         }
    }

    public void Clicked() {
        if (Owner.MyTurn)
        {
            if(Owner.selection == -3 && movementPoints > 0){
            if (Owner.selectedUnit != this)
            {
                foreach (Tile tile in moveable)
                {
                    tile.MoveCircle[0].SetActive(true);
                }
                foreach (Tile tile in attackable)
                {
                    tile.MoveCircle[1].SetActive(true);
                }
                Owner.selectedUnit = this;
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

    [PunRPC]
    public void RPC_Moves()
    {
        attackable.Clear();
        moveable.Clear();
        Collider2D[] l = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), 1f, LayerMask.GetMask("Tile"));
        for (int i = 0; i < l.Length; i++)
        {   
            Tile t = l[i].gameObject.GetComponent<Tile>();
            if (t != current && t.Building == 0 && t.unit == null)
            {
                moveable.Add(t);
             }
         }

         Collider2D[] b = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), 1f, LayerMask.GetMask("Tile"));
        for (int i = 0; i < b.Length; i++)
        {   
            Tile t = b[i].gameObject.GetComponent<Tile>();
            if (t != current && (t.Building != 0 || t.unit != null) && !moveable.Contains(t) && !(t.Building != 0 && t.Owner == OwnerNum && (t.unit == null || (t.unit != null && t.unit.OwnerNum == OwnerNum))))
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
            if (t != current && t.Building == 0 && t.unit == null)
            {
                moveable.Add(t);
             }
         }

         Collider2D[] b = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), 1f, LayerMask.GetMask("Tile"));
        for (int i = 0; i < b.Length; i++)
        {   
            Tile t = b[i].gameObject.GetComponent<Tile>();
            if (t != current && (t.Building != 0 || t.unit != null) && !moveable.Contains(t) && !(t.Building != 0 && t.ownerClient == Owner && (t.unit == null || (t.unit != null && t.unit.OwnerNum == OwnerNum))))
            {
                attackable.Add(t);
             }
         }
     }
    // Update is called once per frame
    void FixedUpdate()
    {
        if(movementPoints <= 0){
        spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, 0.5f);
        } else {
            spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, 1f);
        }
        if(Health <= 0){
            PV.TransferOwnership(PhotonNetwork.LocalPlayer);
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
}
