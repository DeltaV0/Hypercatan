using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.IO;

public class Unit : MonoBehaviour
{
    public int Type;

    public int MoveSpeed;

    public ClientStuff Owner;

    public Tile current;

    public List<Tile> moveable;

    public PhotonView PV;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        moveable.Clear();
        transform.position = new Vector3(transform.position.x, transform.position.y, -1);
        Collider2D q = Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y), 0.1f, LayerMask.GetMask("Tile"));
        current = q.gameObject.GetComponent<Tile>();
        Owner = GameObject.Find(current.Owner +"").GetComponent<ClientStuff>();

        current.unit = this;

        Collider2D[] l = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), 1f, LayerMask.GetMask("Tile"));
        for (int i = 0; i < l.Length; i++)
        {
            if (l[i].gameObject.GetComponent<Tile>() != current)
            {
                moveable.Add(l[i].gameObject.GetComponent<Tile>());
             }
         }
    }

    void OnMouseDown() {
        if (Owner.MyTurn)
        {
            if (Owner.selectedUnit != this)
            {
                foreach (Tile tile in moveable)
                {
                    tile.MoveCircle.SetActive(true);
                }
                Owner.selectedUnit = this;
                Owner.selection = -3;
            }
            else
            {
                foreach (Tile tile in moveable)
                {
                    tile.MoveCircle.SetActive(false);
                }
                Owner.selectedUnit = null;
                //Owner.selection = -3;
             }
        }
    }

    public void Move(Tile tile)
    {
        Debug.Log("assdsa");
        PV.RPC("RPC_Move", RpcTarget.All, tile.transform.position.x, tile.transform.position.y);
     }

    [PunRPC]
    public void RPC_Move(float x, float y)
    {
        foreach (Tile tile in moveable)
            {
                tile.MoveCircle.SetActive(false);
             }
             current.unit = null;
        moveable.Clear();
        transform.position = new Vector3(x, y, -1);
        Collider2D q = Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y), 0.1f, LayerMask.GetMask("Tile"));
        current = q.gameObject.GetComponent<Tile>();
        current.unit = this;

        Collider2D[] l = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), 1f, LayerMask.GetMask("Tile"));
        for (int i = 0; i < l.Length; i++)
        {
            if (l[i].gameObject.GetComponent<Tile>() != current)
            {
                moveable.Add(l[i].gameObject.GetComponent<Tile>());
             }
         }
     }
    // Update is called once per frame
    void Update()
    {
        
    }
}
