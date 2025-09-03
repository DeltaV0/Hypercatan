using UnityEngine;
using TMPro;

public class Inventory : MonoBehaviour
{
    public ClientStuff client;

    public GameObject[] gobj;

    public GameObject g;

    public TMP_Text[] text;

    public void Activate(int a){
        foreach(GameObject g in gobj){
            g.SetActive(false);
        }
        gobj[a].SetActive(true);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(client.selection == 0){
            g.SetActive(true);
            text[0].text = "Tile: " + client.SelectedTile.Building;
            text[1].text = "Building: " + client.SelectedTile.Building + "\nHealth: " + client.SelectedTile.Health + "/" + client.SelectedTile.MaxHealth + "\nTerrain: " + client.SelectedTile.terrainType +"\nOwner: " + client.SelectedTile.Owner + "\nTile Resource: " + client.SelectedTile.resource + "\nResources: " + client.SelectedTile.ResourcesSum[0] + ", " + client.SelectedTile.ResourcesSum[1] + "\n" + client.SelectedTile.ResourcesProducing[0] + ", " + client.SelectedTile.ResourcesProducing[1];
            if(client.selectedUnit != null){
                text[2].text = "Unit";
                text[3].text = "Health: " + client.selectedUnit.Health + "\nMovement Points: " + client.selectedUnit.movementPoints;
            } else {
                text[2].text = "No Selected Unit";
                text[3].text = "";
            }
        } else {
            g.SetActive(false);
        }
    }
}
