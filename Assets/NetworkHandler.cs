using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkHandler : NetworkBehaviour
{
    public Server server;
    public Board board;
    public Board board_test;
    public bool CLIENT = false;
    public bool SERVER = false;
    string serverAddress = "127.0.0.1"; 
    public void StartServer()
    {
        //NetworkServer.Listen(100);
        NetworkManager.singleton.StartServer(); // Starts the server
        NetworkManager.singleton.exceptionsDisconnect = false;
        Debug.Log("Server Started"); 
        RegisterMessages();
    }
    public void StopConnection()
    {
        NetworkManager.singleton.StopClient();
        Debug.Log("Connection stopped.");
        
    }
    public bool StartClient()
    {
        NetworkManager.singleton.networkAddress = serverAddress;
        bool b = NetworkManager.singleton.StartClient();
        NetworkManager.singleton.exceptionsDisconnect = false;
        //NetworkClient.Connect(serverAddress); // Connects the client to the server
        Debug.Log("Client Started and Connecting");
        RegisterMessages();
        return b;
    }
    // Call this function during the initialization of your server/client
    public void RegisterMessages()
    {
        if (CLIENT)
        {
            NetworkClient.ReplaceHandler<Server.JsonMessage>(ClientReceive);
        }

        if (SERVER)
        {
            NetworkServer.ReplaceHandler<Server.JsonMessage>(ServerReceive);
        }

    }
    private void ClientReceive(Server.JsonMessage msg)
    {
        //Debug.Log(transform.parent.name+"Received JSON: " + msg.jsonString);
        Server.CustomMessageIntermediate data = JsonUtility.FromJson<Server.CustomMessageIntermediate>(msg.jsonString);
        //data.clientID = clientID;
        Server.CustomMessage message = new Server.CustomMessage(data);
#if UNITY_EDITOR
        if (message.clientID == 101)
            board_test.OnMessageReceived(message);
        else
            board.OnMessageReceived(message);
#else
        board.OnMessageReceived(message);
#endif
        // Deserialize and process the JSON
        //Server.CustomMessage data = JsonUtility.FromJson<Server.CustomMessage>(msg.jsonString);
        // Do something with the data
    }
    // Handle incoming messages
    Dictionary<int, NetworkConnectionToClient> connections = new();
    
    private void ServerReceive(NetworkConnectionToClient conn, Server.JsonMessage msg)
    {
        // Deserialize and process the JSON
        Server.CustomMessageIntermediate data = JsonUtility.FromJson<Server.CustomMessageIntermediate>(msg.jsonString);
        Server.CustomMessage message = new Server.CustomMessage(data);


#if (UNITY_EDITOR == false)
        int clientID = conn.connectionId;
        message.clientID = clientID;
#else
        int clientID = data.clientID;
#endif

        if (connections.ContainsKey(clientID) == false)
        {
            connections.Add(clientID, conn);
        }
        else
        {
            connections[clientID] = conn;
        }
        if (connections.Count == 2)
        {
            //Debug.Log(connections[100].connectionId + " " + connections[101].connectionId);
        }
        server.OnMessageReceived(message);
        // Do something with the data
    }

    // To send a JSON message to all clients or specific client
    public void SendJsonMessage(NetworkConnectionToClient networkHandler,string json)
    {
        Server.JsonMessage msg = new Server.JsonMessage
        {
            jsonString = json
        };
        networkHandler.Send(msg);
        // Send to all connected clients

    }
    public void SendClient(Server.CustomMessage m)
    {
        Server.CustomMessageIntermediate mInter = new Server.CustomMessageIntermediate(m);
        string s = JsonUtility.ToJson(mInter);

        Server.JsonMessage msg = new Server.JsonMessage
        {
            jsonString = s
        };
        NetworkClient.Send(msg);
    }
    public void SendServer(Server.CustomMessage m,Player player)
    {
        if (connections.ContainsKey(player.connection.clientID)==false)
        {
            Debug.LogError("PLAYER NOT FOUND");
            return;
        }
        //Debug.Log("SENDING MSG TO PLAYER " + player.connection.clientID);
        NetworkConnectionToClient connection = connections[player.connection.clientID];
        Server.CustomMessageIntermediate mInter = new Server.CustomMessageIntermediate(m);
        mInter.clientID = player.connection.clientID;
        string s = JsonUtility.ToJson(mInter);

        Server.JsonMessage msg = new Server.JsonMessage
        {
            jsonString = s
        };
        connection.Send(msg);
    }
}
