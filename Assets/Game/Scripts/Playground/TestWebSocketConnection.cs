using System;
using System.Collections;
using System.Collections.Generic;
using BestHTTP.WebSocket;
using UnityEngine;

public class TestWebSocketConnection : MonoBehaviour
{
    private const string ROOT_PATH = "127.0.0.1:9000";
    private const string LOBBY_NAME = "lobby";
    void Start()
    {
        //var path = "wss://demo.piesocket.com/v3/channel_123?api_key=VCXCEuvhGcBDP7XhiJJUDvR1e1D3eiVjgZ9VRiaV&notify_self";
        var path = string.Format("ws://{0}/ws/rooms/{1}/",ROOT_PATH, LOBBY_NAME);
        var webSocket = new WebSocket(new Uri(path), "http://127.0.0.1:9000", string.Empty);
        Debug.Log(path);
        webSocket.OnOpen += OnWebSocketOpen;
        webSocket.OnMessage += OnMessageReceived;
        webSocket.OnBinary += OnBinaryMessageReceived;
        webSocket.OnClosed += OnWebSocketClosed;
        webSocket.OnError += OnError;
        webSocket.Open();
    }

    private void OnWebSocketOpen(WebSocket webSocket)
    {
        Debug.Log("WebSocket is now Open!");
    }
    private void OnMessageReceived(WebSocket webSocket, string message)
    {
        Debug.Log("Text Message received from server: " + message);
    }
    private void OnBinaryMessageReceived(WebSocket webSocket, byte[] message)
    {
        Debug.Log("Binary Message received from server. Length: " + message.Length);
    }
    private void OnWebSocketClosed(WebSocket webSocket, UInt16 code, string message)
    {
        Debug.Log("WebSocket is now Closed!");
    }
    void OnError(WebSocket ws, string error)
    {
        Debug.LogError("Error: " + error);
    }
}
