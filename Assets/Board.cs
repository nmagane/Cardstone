using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Riptide;
using Riptide.Utils;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static bool RNG(float percent) //Two point precision. RNG(XX.XXf)
    {
        percent *= 100;
        return (Random.Range(0, 10000) <= percent);
    }
    public static T RandElem<T>(List<T> L)
    {
        return L[Random.Range(0, L.Count)];
    }
    public static KeyValuePair<T, Q> RandElem<T, Q>(Dictionary<T, Q> L)
    {
        List<T> L2 = new List<T>(L.Keys);

        T K2 = RandElem(L2);
        Q V2 = L[K2];

        KeyValuePair<T, Q> kvp = new KeyValuePair<T, Q>(K2, V2);

        return kvp;
    }
    public static T RandElem<T>(T[] L)
    {
        return L[Random.Range(0, L.Length)];
    }
    public static List<T> Shuffle<T>(List<T> l)
    {
        return l.OrderBy(x => Random.value).ToList();
    }
    public static T[] Shuffle<T>(T[] l)
    {
        return l.OrderBy(x => Random.value).ToArray();
    }

    public Riptide.Client client = new Riptide.Client();
    public ulong currentMatchID;
    public ulong playerID=100;
    void Start()
    {
        client.Connect("127.0.0.1:7777",5,0,null,false);
        client.MessageReceived += OnMessageReceived;
    }
    public void OnMessageReceived(object sender, MessageReceivedEventArgs eventArgs)
    {

        int messageID = eventArgs.MessageId;
        switch ((Server.MessageType)messageID)
        {
            case Server.MessageType.ConfirmMatch:
                ulong matchID = eventArgs.Message.GetULong();
                Debug.Log("Player "+playerID+" entered game "+matchID);
                currentMatchID = matchID;
                break;
        }
    }
    // Update is called once per frame

    ushort SendInt(int x, ushort messageID=1)
    {
        Message message = Message.Create(MessageSendMode.Reliable, messageID);
        message.AddInt(x);
        
        return client.Send(message);
    }

    void StartMatchmaking()
    {
        Message message = Message.Create(MessageSendMode.Reliable, (ushort)Server.MessageType.Matchmaking);
        message.AddULong(playerID);
        client.Send(message);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            SendInt(88);
        }
        if (Input.GetKeyDown(KeyCode.Q) && playerID==100)
        {
            StartMatchmaking();
        }
        if (Input.GetKeyDown(KeyCode.W) && playerID==101)
        {
            StartMatchmaking();
        }
        
    }
    private void FixedUpdate()
    {
        client.Update();
    }

}
