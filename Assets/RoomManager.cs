using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager rm;

    public int Turn;

    public PhotonView PV;

    public Material linemat;
    // Start is called before the first frame update
    void Awake()
    {
        if(rm){
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        rm = this;
        Turn = 1;
    }

    public override void OnEnable(){
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable(){
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode){
        if(scene.buildIndex == 1){
            PhotonNetwork.Instantiate(Path.Combine("PlayerManager"), Vector3.zero, Quaternion.identity);
        }
    }

    public void NextTurn(){
         PV.RPC("RPC_Next", RpcTarget.All, Turn + 1);
    }

    [PunRPC]
    public void RPC_Next(int sel){
            Turn = sel;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
