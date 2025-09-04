using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.IO;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public ClientStuff client;

    public GameObject[] gobj;

    public GameObject g;

    public TMP_Text[] text;

    public GameObject[] gobj2;

    public void Activate(int a){
        foreach(GameObject g in gobj){
            g.SetActive(false);
        }
        gobj[a].SetActive(true);
    }
    
    public void Activate2(int a){
        foreach(GameObject g in gobj2){
            g.SetActive(false);
        }
        gobj2[a].SetActive(true);
    }

    public void Do(int a)
    {
        client.SelectedTile.Click(a);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (g != null)
        {
            if (client.selection == 0)
            {
                g.SetActive(true);
                if (client.SelectedTile != null)
                {
                    text[0].text = "Tile: " + client.SelectedTile.Building;
                    text[1].text = "Building: " + client.SelectedTile.Building + "\nHealth: " + client.SelectedTile.Health + "/" + client.SelectedTile.MaxHealth + "\nTerrain: " + client.SelectedTile.terrainType + "\nOwner: " + client.SelectedTile.Owner + "\nTile Resource: " + client.SelectedTile.resource + "\nResources: " + client.SelectedTile.ResourcesSum[0] + ", " + client.SelectedTile.ResourcesSum[1] + "\n" + client.SelectedTile.ResourcesProducing[0] + ", " + client.SelectedTile.ResourcesProducing[1];
                    if (client.selectedUnit != null)
                    {
                        text[2].text = "Unit";
                        text[3].text = "Health: " + client.selectedUnit.Health + "\nMovement Points: " + client.selectedUnit.movementPoints;
                    }
                    else
                    {
                        text[2].text = "No Selected Unit";
                        text[3].text = "";
                    }
                    if (client.SelectedTile.Owner == PhotonNetwork.LocalPlayer.ActorNumber)
                    {
                        if (client.SelectedTile.Building == 1)
                        {
                            Activate2(0);
                        }
                        else if (client.SelectedTile.Building == 4)
                        {
                            Activate2(1);
                        }
                        else if (client.SelectedTile.Building == 0)
                        {
                            Activate2(2);
                            //establish city
                        }
                    }
                }
            }
            else
            {
                g.SetActive(false);
            }
        }
    }
}
