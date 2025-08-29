using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public string menuName;

    public bool Open;

    public void open(){
        gameObject.SetActive(true);
        Open = true;
    }

    public void close(){
        Open = false;
      gameObject.SetActive(false); 
    }
}
