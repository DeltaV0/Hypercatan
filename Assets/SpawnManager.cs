using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public Transform[] RespawnPoint;
    public static SpawnManager spm;
    // Start is called before the first frame update
    void Awake()
    {
        spm = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
