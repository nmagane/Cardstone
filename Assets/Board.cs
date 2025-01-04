using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Board : MonoBehaviour
{
    public NetworkManager networkManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            //var customMessagingManager = networkManager.CustomMessagingManager;
            Debug.Log(networkManager.IsConnectedClient);
        }
    }
}
