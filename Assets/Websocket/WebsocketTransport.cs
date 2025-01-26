using Riptide;
using UnityEngine;
using WebSocketSharp;
using System;

public class WebSocketTransport : IDisposable
{
    private WebSocket socket;
    private Action<byte[]> onMessageReceived;

    // Constructor that accepts a WebSocket server URL and a callback for message handling
    public WebSocketTransport(string url, Action<byte[]> onMessageReceivedCallback)
    {
        this.onMessageReceived = onMessageReceivedCallback;

        socket = new WebSocket(url);
        socket.OnOpen += OnOpen;
        socket.OnMessage += OnMessage;
        socket.OnError += OnError;
        socket.OnClose += OnClose;
    }

    // Connect to the WebSocket server asynchronously
    public void Connect()
    {
        socket.ConnectAsync();
    }

    // Send a message over the WebSocket
    public void SendMessage(byte[] message)
    {
        if (socket.ReadyState == WebSocketState.Open)  // Check if the WebSocket is open
        {
            socket.Send(message);
        }
        else
        {
            Debug.LogError("WebSocket is not open.");
        }
    }

    // Disconnect from the WebSocket server
    public void Disconnect()
    {
        socket.Close();
    }

    // Triggered when the WebSocket is successfully opened
    private void OnOpen(object sender, EventArgs e)
    {
        Debug.Log("WebSocket connection established.");
    }

    // Triggered when a message is received from the WebSocket server
    private void OnMessage(object sender, MessageEventArgs e)
    {
        onMessageReceived?.Invoke(e.RawData); // Call the callback with received data
    }

    // Triggered when there is an error with the WebSocket
    private void OnError(object sender, ErrorEventArgs e)
    {
        Debug.LogError("WebSocket error: " + e.Message);
    }

    // Triggered when the WebSocket is closed
    private void OnClose(object sender, CloseEventArgs e)
    {
        Debug.Log("WebSocket closed. Code: " + e.Code);
    }

    // Dispose of the WebSocket when done
    public void Dispose()
    {
        socket.Close();
        socket = null;
    }
}
