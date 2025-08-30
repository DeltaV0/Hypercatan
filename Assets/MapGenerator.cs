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
    public float[] randMast = new float[2];
    public float[] randCL = new float[2];

    public static MapGenerator map;

    public int ID;

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
            PhotonNetwork.Instantiate(Path.Combine("Tile"), transform.position, transform.rotation);

            for (int i = 0; i < 200; i++)
            {
                int rand = Random.Range(0, 6);
                if (rand == 0)
                {
                    transform.Translate(1, 0, 0);
                }
                else if (rand == 1)
                {
                    transform.Translate(-1, 0, 0);
                }
                else if (rand == 2)
                {
                    transform.Translate(0.5f, 0.8660254f, 0);
                }
                else if (rand == 3)
                {
                    transform.Translate(0.5f, -0.8660254f, 0);
                }
                else if (rand == 4)
                {
                    transform.Translate(-0.5f, 0.8660254f, 0);
                }
                else if (rand == 5)
                {
                    transform.Translate(-0.5f, -0.8660254f, 0);
                }
                Collider2D q = Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y), 0.1f);
                if (q == null)
                {
                    ID++;
                    PhotonNetwork.Instantiate(Path.Combine("Tile"), transform.position, transform.rotation);
                }
                else
                {
                    i--;
                }
            }
            transform.position = Vector3.zero;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
