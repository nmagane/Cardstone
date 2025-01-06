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
        client.Connect("127.0.0.1:7777");
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
            SendInt(388,1);
            SendInt(884);
        }
    }
    private void FixedUpdate()
    {
        client.Update();
    }
}
