using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public static MenuManager menuManager;
    public Menu[] menus;

    public TMP_InputField user;

    public CamMover cam;

    //public Slider col;

    //public float colour;
    
    void Awake(){
        menuManager = this;
    }
    void Update()
    {
        if(string.IsNullOrEmpty(PhotonNetwork.NickName)){
            PhotonNetwork.NickName = Random.Range(0, 1000) + "";
        } else {
            PhotonNetwork.NickName = user.text;
        }

       // colour = col.value;
    }
    public void OpenMenu(string name){

        for(int i = 0; i < menus.Length; i++){
            if(menus[i].menuName == name){
                cam.trans2.position = menus[i].gameObject.transform.position;
            } else if(menus[i].Open){
                //menus[i].close();
            }
        }

    }
    public void OpenMenu(Menu menu){
        for(int i = 0; i < menus.Length; i++){
             if(menus[i].Open){
               // menus[i].close();
            }
        }
         cam.trans2.position = menu.gameObject.transform.position;
    }
}
