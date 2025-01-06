using System.Collections;
using System.Collections.Generic;
using Riptide;
using Riptide.Utils;
using UnityEngine;

public class Server : MonoBehaviour
{
    public Riptide.Server server = new Riptide.Server();

    void Start()
    {
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
        ushort port = 7777;
        ushort maxPlayers = 65534; //ushort.max-1
        server.Start(port, maxPlayers);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        server.Update();
    }

    [MessageHandler(1)]
    private static void HandleSomeMessageFromServer(ushort id,Message message)
    {
        int someInt = message.GetInt();
        Debug.Log(someInt);
    }
}
