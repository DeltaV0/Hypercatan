using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Tech : MonoBehaviour
{
    public TechTree techTree;

    public string Name;

    public int ID;

    public int Price;
    
    public string Description;

    public bool bought;

    public Tech[] unlocks;

    public Button button;

    public void Select(){
        techTree.SelectedTech = this;
    }


}
