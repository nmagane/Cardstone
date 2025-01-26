
using UnityEngine;
using WebSocketSharp;
using System;
using System.IO;
using Riptide;

public partial class Board
{
    private WebSocketTransport transport;

    private void ConnectTransport()
    {
        transport = new WebSocketTransport("ws://161.35.25.172:8080", HandleReceivedMessage);
        transport.Connect();
    }


    // Send a message to the server
    public void SendMessageWebsocket(Message msg,Server.MessageType type)
    {
        // Convert data into bytes manually. This is just an example.
        Server.CustomMessage m = Server.CopyMessage(msg,type);

        string jsonText = JsonUtility.ToJson(m);
        byte[] messageBytes = SerializeMessage(jsonText);
        // Send the message via WebSocket transport
        transport.SendMessage(messageBytes);
    }

    // Handle received message bytes manually
    private void HandleReceivedMessage(byte[] messageBytes)
    {
        // Manually parse the message bytes
        string receivedData = DeserializeMessage(messageBytes);

        // Process the received data (this is where you can trigger Riptide-specific logic)
        Debug.Log("Received message: " + receivedData);
    }

    // Serialize message (this is an example of serializing a string into a byte array)
    private byte[] SerializeMessage(string data)
    {
        using (MemoryStream memoryStream = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(memoryStream))
            {
                // Add data to the writer
                writer.Write(data);

                // Return the byte array
                return memoryStream.ToArray();
            }
        }
    }

    // Deserialize message (this is an example of deserializing the byte array back into a string)
    private string DeserializeMessage(byte[] messageBytes)
    {
        using (MemoryStream memoryStream = new MemoryStream(messageBytes))
        {
            using (BinaryReader reader = new BinaryReader(memoryStream))
            {
                // Read the data (assumes it's a string)
                return reader.ReadString();
            }
        }
    }

    private void DisconnectTransport()
    {
        transport.Disconnect();
    }
}


