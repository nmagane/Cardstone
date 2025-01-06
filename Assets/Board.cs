using System.Collections;
using System.Collections.Generic;
using Riptide;
using Riptide.Utils;
using UnityEngine;

public class Board : MonoBehaviour
{
    public Riptide.Client client = new Riptide.Client();

    void Start()
    {
        client.Connect("127.0.0.1:7777",5,0,null,false);
        client.MessageReceived += msgr;
    }
    public void msgr(object sender, MessageReceivedEventArgs e)
    {
        Debug.Log(e.Message.GetInt());
        //SendInt(e.Message.GetInt(), e.FromConnection.Id);
    }
    // Update is called once per frame

    ushort SendInt(int x, ushort messageID=1)
    {
        Message message = Message.Create(MessageSendMode.Reliable, messageID);
        message.AddInt(x);
        return client.Send(message);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            SendInt(88);
            SendInt(388);
            SendInt(884);
        }
    }
    private void FixedUpdate()
    {
        client.Update();
    }

    [MessageHandler(1)]
    private static void ReceiveMessage(ushort id, Message message)
    {
        int someInt = message.GetInt();
        Debug.Log(id);
        Debug.Log(someInt);
    }
}
