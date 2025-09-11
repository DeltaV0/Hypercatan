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

    public GameObject[] units;

    public GameObject[] Upgrade;

    public void Activate(int a)
    {
        foreach (GameObject g in gobj)
        {
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
    
    public void Activateadd(int a){
        gobj2[a].SetActive(true);
    }

    public void DisableAll()
    {
        foreach (GameObject g in gobj2)
        {
            g.SetActive(false);
        }
        foreach (GameObject o in Upgrade)
        {
            o.SetActive(false);
        }
        //gobj2[a].SetActive(true);
    }

    public void Do(int a)
    {
        client.SelectedTile.Click(a, false);
        if (a == -3)
        {
            client.Select(-3);
         }
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
                    //text[0].text = "Tile: " + client.SelectedTile.Building;

                    if(client.SelectedTile.Building == 0){
                        text[0].text = "Empty";
                    } else if(client.SelectedTile.Building == 1){
                        text[0].text = "City";
                    } else if(client.SelectedTile.Building == 3){
                        text[0].text = "Mine";
                    } else if(client.SelectedTile.Building == 4){
                        text[0].text = "Factory";
                    } else if(client.SelectedTile.Building == 100){
                        text[0].text = "Outpost";
                    } else if(client.SelectedTile.Building == 5){
                        text[0].text = "Farm";
                    } else if(client.SelectedTile.Building == 6){
                        text[0].text = "Forestry";
                    } else if(client.SelectedTile.Building == 7){
                        text[0].text = "Power Plant";
                    } else if(client.SelectedTile.Building == 8){
                        text[0].text = "University";
                    }
                    //text[0].text = text[1].text;
                    if(client.SelectedTile.Building != 1 && client.SelectedTile.Building != 100){
                    if(client.SelectedTile.terrainType == -1){
                        text[1].text = "Water, ";
                    } else if(client.SelectedTile.terrainType == 0){
                        text[1].text = "Plains, ";
                    } else if(client.SelectedTile.terrainType == 1){
                        text[1].text = "Mountain, ";
                    }

                    if(client.SelectedTile.resource == 0){
                        text[1].text = text[1].text + "None";
                    } else if(client.SelectedTile.resource == 1){
                        text[1].text = text[1].text + "Iron";
                    } else if(client.SelectedTile.resource == 2){
                        text[1].text = text[1].text + "Coal";
                    } else if(client.SelectedTile.resource == -1){
                        text[1].text = text[1].text + "Fertile";
                    } else if(client.SelectedTile.resource == -2){
                        text[1].text = text[1].text + "Forest";
                    }
                    } else {
                        text[1].text = "Buildings: " + client.SelectedTile.CityVars[1] + "/" + (int)(3f * client.SelectedTile.CityVars[0]);
                    }
                    text[1].text = text[1].text + "\nHealth: " + client.SelectedTile.Health + "/" + client.SelectedTile.MaxHealth;
                   // text[1].text = "Building: " + client.SelectedTile.Building + "\nHealth: " + client.SelectedTile.Health + "/" + client.SelectedTile.MaxHealth + "\nTerrain: " + client.SelectedTile.terrainType + "\nOwner: " + client.SelectedTile.Owner + "\nTile Resource: " + client.SelectedTile.resource + "\nResources: " + client.SelectedTile.ResourcesSum[0] + ", " + client.SelectedTile.ResourcesSum[1] + "\n" + client.SelectedTile.ResourcesProducing[0] + ", " + client.SelectedTile.ResourcesProducing[1];
                    if (client.selectedUnit != null)
                    {
                        text[2].text = "Unit";
                        text[3].text = "Health: " + client.selectedUnit.Health + "/" + client.selectedUnit.MaxHealth + "\nMovement Points: " + client.selectedUnit.movementPoints[0] + ", " + client.selectedUnit.movementPoints[1] + "\nDamage: " + client.selectedUnit.damage + ", " + client.selectedUnit.CounterAttackDamage;
                    }
                    else
                    {
                        text[2].text = "No Selected Unit";
                        text[3].text = "";
                    }
                    if (client.SelectedTile.ownerClient == client)
                    {
                        if ((client.SelectedTile.Building == 1 || client.SelectedTile.Building == 100) && client.SelectedTile.Owner == client.PV.Owner.ActorNumber && client.MyTurn)
                        {
                            Activate2(0);
                            if (client.SelectedTile.Building == 100)
                            {
                                Upgrade[0].SetActive(true);
                            }
                            else
                            {
                                Upgrade[0].SetActive(false);
                            }
                        }
                        else if (client.SelectedTile.Building == 4 && client.SelectedTile.Owner == client.PV.Owner.ActorNumber && client.MyTurn)
                        {
                            Activate2(1);
                            Upgrade[1].SetActive(client.SelectedTile.Upgradable);
                        }
                        else if (client.SelectedTile.Building == 0 && client.SelectedTile.Owner == client.PV.Owner.ActorNumber && client.MyTurn)
                        {
                            //Activate2(2);
                            //establish city
                        }
                        else if (client.SelectedTile.Building == 7 && client.SelectedTile.Owner == client.PV.Owner.ActorNumber && client.MyTurn)
                        {
                            Activate2(3);
                        }
                        else if (client.SelectedTile.Building != 0 && client.SelectedTile.Owner == client.PV.Owner.ActorNumber && client.MyTurn)
                        {
                            Activate2(4);
                            Upgrade[2].SetActive(client.SelectedTile.Upgradable);
                        }
                        else
                        {
                            DisableAll();
                        }
                        if (client.selectedUnit != null)
                        {
                            Activateadd(5);
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
