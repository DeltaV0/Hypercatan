using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.IO;

public class MapGenerator : MonoBehaviour
{
    public float randx;
    public float randy;

    public static MapGenerator map;

    public int ID;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        ID = 1;
        map = this;
        if(PhotonNetwork.IsMasterClient){
            randx = Random.Range(-1000f, 1000f);
            randy = Random.Range(-1000f, 1000f);
            PhotonNetwork.Instantiate(Path.Combine("Tile"), transform.position, transform.rotation);

            for(int i = 0; i < 200; i++){
                int rand = Random.Range(0, 6);
                if(rand == 0){
                    transform.Translate(1, 0, 0);
                } else if(rand == 1){
                    transform.Translate(-1, 0, 0);
                } else if(rand == 2){
                    transform.Translate(0.5f, 0.8660254f, 0);
                } else if(rand == 3){
                    transform.Translate(0.5f, -0.8660254f, 0);
                } else if(rand == 4){
                    transform.Translate(-0.5f, 0.8660254f, 0);
                } else if(rand == 5){
                    transform.Translate(-0.5f, -0.8660254f, 0);
                }
                Collider2D q = Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y), 0.1f);
                if(q == null){
                    ID++;
                    PhotonNetwork.Instantiate(Path.Combine("Tile"), transform.position, transform.rotation);
                } else {
                    i--;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
