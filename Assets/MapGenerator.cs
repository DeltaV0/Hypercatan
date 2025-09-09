using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.IO;

public class MapGenerator : MonoBehaviour
{
    public PhotonView PV;
    public float[] randMast;
    public float[] randCL;

    public static MapGenerator map;

    public int ID;

    public List<Tile> tiles;
    public List<Unit> units;

    public int size;

    //public Canvas canvas;

    [PunRPC]
    public void RPC_SetUp(float selx, float sely)
    {
        randCL[0] = selx;
        randCL[1] = sely;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        ID = 1;
        map = this;
        if (PhotonNetwork.IsMasterClient)
        {
            randMast[0] = Random.Range(-1000f, 1000f);
            randMast[1] = Random.Range(-1000f, 1000f);
            PV.RPC("RPC_SetUp", RpcTarget.All, randMast[0], randMast[1]);
            //randy = Random.Range(-1000f, 1000f);
            //PhotonNetwork.Instantiate(Path.Combine("Tile"), transform.position, transform.rotation);
            //transform.Translate(1, 0, 0);
            for (int i = 0; i < size + 1; i++)
            {
                //int c = 9 + (-1 * Mathf.Abs(i - 4)) % 2;
                Vector3 p = transform.position;
                for(int a = 0; a <= size + (-1 * Mathf.Abs(i - (size/2))) + Random.Range(-1, 2); a++){
                    
                   // if (c == 0)
                //{
                    transform.Translate(1, 0, 0);
                //}
                /*else if (c == 1)
                {
                    transform.Translate(-1, 0, 0);
                }
                else if (c == 2)
                {
                    transform.Translate(0.5f, 0.8660254f, 0);
                }
                else if (c == 3)
                {
                    transform.Translate(0.5f, -0.8660254f, 0);
                }
                else if (c == 4)
                {
                    transform.Translate(-0.5f, 0.8660254f, 0);
                }
                else if (c == 5)
                {
                    transform.Translate(-0.5f, -0.8660254f, 0);
                }*/
                Collider2D q = Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y), 0.1f);
                if (q == null)
                {
                    ID++;
                    PhotonNetwork.Instantiate(Path.Combine("Tile"), transform.position, transform.rotation);
                }
                else
                {
                    a--;
                }
                }
                transform.position = p;
                transform.Translate(Random.Range(-1, 2), 0, 0);
                if (i-(size/2) >= 0)
                {
                    transform.Translate(0.5f, -0.8660254f, 0);
                }
                else
                {
                    transform.Translate(-0.5f, -0.8660254f, 0);
                }
                
                
            }
            transform.position = Vector3.zero;
        }
    }

    public void NextTurn()
    {
         foreach (Unit unit in units)
            {
            unit.UpdateMoves();
            unit.movementPoints[0] = unit.MaxmovementPoints[0];
            unit.movementPoints[1] = unit.MaxmovementPoints[1];
            }

         
     }

    public void UpdateMoves(){
        foreach (Unit unit in units)
        {
            unit.UpdateMoves();
            
         }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
