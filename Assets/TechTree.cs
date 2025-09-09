using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class TechTree : MonoBehaviour
{
    public ClientStuff client;

    public List<Tech> BoughtTechs;

    public TMP_Text[] texts;

    public Tech SelectedTech;

    public int[,] UnitPrices = new int[5, 5];

    public float[,] Bonuses = new float[5, 2];

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //              tank            infantry            artillery       battleship          colony
        UnitPrices = new int[,] {{0, 0, 3, 0, 2}, {0, 0, 1, 0, 2}, {0, 0, 2, 0, 2}, {2, 0, 3, 0, 1}, {0, 0, 3, 4, 4}};
    }

    public void Buy(){
        SelectedTech.button.interactable = false;
        foreach(Tech tech in SelectedTech.unlocks){
            tech.gameObject.SetActive(true);
        }
        client.TechPoints -= SelectedTech.Price;
        if(SelectedTech.ID == 0){ 
            client.inv.units[0].SetActive(true);
            //Bonuses[0, 0] += 0.25f;
            //Bonuses[0, 1] += 0.25f;
        }
        if(SelectedTech.ID == 1){
            client.inv.units[1].SetActive(true); 
           // Bonuses[0, 0] += 0.25f;
           // Bonuses[0, 1] += 0.25f;
        }
        if(SelectedTech.ID == 2){
            client.inv.units[2].SetActive(true); 
           // Bonuses[0, 0] += 0.25f;
           // Bonuses[0, 1] += 0.25f;
        }
        if(SelectedTech.ID == 3){
            client.inv.units[3].SetActive(true); 
           // Bonuses[0, 0] += 0.25f;
           // Bonuses[0, 1] += 0.25f;
        }
        if(SelectedTech.ID == 4){
            client.inv.units[4].SetActive(true); 
           // Bonuses[0, 0] += 0.25f;
           // Bonuses[0, 1] += 0.25f;
        }
        SelectedTech = null;
    }

    // Update is called once per frame
    void Update()
    {
        if(SelectedTech != null){
        texts[0].text = SelectedTech.Name;
        texts[1].text = SelectedTech.Description;

        texts[2].text = "Buy: " + SelectedTech.Price;
        } else {
            texts[0].text = "";
            texts[1].text = "";
            texts[2].text = "";
        }
    }
}
