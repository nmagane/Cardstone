using System;
using System.Collections;
using System.Collections.Generic;
using Riptide;
using Riptide.Utils;
using Unity.Networking.Transport;
using UnityEditor.PackageManager;
using UnityEngine;

public class Server : MonoBehaviour
{
    public Riptide.Server server = new Riptide.Server();

    void Start()
    {
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
        ushort port = 7777;
        ushort maxPlayers = 65534; //ushort.max-1
        server.Start(port, maxPlayers,0,false);
        server.MessageReceived += msgr;
    }

    void colorfixer()
    {
        //ReceiveMessage(0,Message.Create());
    }
    private void SendInt(int x, ushort clientID, ushort messageID = 1)
    {
        Message message = Message.Create(MessageSendMode.Reliable, messageID);
        message.AddInt(x);
        server.Send(message,clientID);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        server.Update();
    }
    /*
    [MessageHandler(1)]
    private static void ReceiveMessage(ushort id,Message message)
    {
        int someInt = message.GetInt();
        Debug.Log(id);
        Debug.Log(someInt);
        SendInt(someInt, id);
        //change this to event
       
    }*/

    //public event EventHandler<MessageReceivedEventArgs> MessageReceived;
    public void msgr(object sender, MessageReceivedEventArgs e)
    {
        int f = e.Message.GetInt();
        Debug.Log(f);
        SendInt(f+1,e.FromConnection.Id);
    }


    

}
