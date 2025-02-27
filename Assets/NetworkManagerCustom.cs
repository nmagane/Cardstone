using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class NetworkManagerCustom : NetworkManager
{
    //public Server server;
    //public bool clientConnected = false;
    //public Board board;
#if UNITY_EDITOR
    //public Board testBoard;
#endif

    private static NetworkManagerCustom _instance;

    public static NetworkManagerCustom Instance { get { return _instance; } }


    new private void Awake()
    {
        base.Awake();
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        // Always call the base method
        base.OnServerDisconnect(conn); 

        ServerHandleDisconnect(conn);
    }
    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        // Always call the base method
        base.OnServerConnect(conn); 

        ServerHandleConnect(conn);
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();

        //clientConnected = true;
        ClientHandleConnect();
    }
    public override void OnClientDisconnect()
    {;
        base.OnClientDisconnect();

        //clientConnected = false;
        ClientHandleDisconnect();
    }

    //===============SERVER CODE=============================
    private void ServerHandleConnect(NetworkConnectionToClient conn)
    {
        Debug.Log($"ClientID {conn.connectionId} connected. {conn.address}");

        Server.Instance.HandleConnection();
    }

    private void ServerHandleDisconnect(NetworkConnectionToClient conn)
    {
       Debug.Log($"ClientID {conn.connectionId} disconnected.");
        
       Server.Instance.DisconnectClient(conn.connectionId);
    }

    //====================CLIENT CODE==============
    private void ClientHandleConnect()
    {
        Board.Instance.ConfirmConnection();
#if (UNITY_EDITOR)
        Board.InstanceTest.ConfirmConnection();
#endif
    }
    private void ClientHandleDisconnect()
    {
        Board.Instance.ConfirmDisconnect();
    }

}
