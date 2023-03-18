using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Net;
using System.Text;
using System.Net.Sockets;

public class TCPServer : MonoBehaviour
{
    public Text textObject;
    private static byte[] inBuffer = new byte[512];
    private static List<Socket> clientSockets = new List<Socket>();

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            // Create a new TCP socket and bind it to the local IP address and port number
            Socket serverSoc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSoc.Bind(new IPEndPoint(IPAddress.Any, 8889));

            // Listen for incoming connections
            serverSoc.Listen(2);

            Debug.Log("Waiting for connections...");

            // Accept incoming connections
            for (int i = 0; i < 2; i++)
            {
                Socket clientSoc = serverSoc.Accept();
                clientSockets.Add(clientSoc);

                Debug.Log("Connected to client " + i + ": " + clientSoc.RemoteEndPoint.ToString());

                // Start receiving messages from the client
                clientSoc.BeginReceive(inBuffer, 0, inBuffer.Length, SocketFlags.None, ReceiveCallback, clientSoc);
            }
        }
        catch (Exception e)
        {
            Debug.Log("Exception: " + e.ToString());
        }
    }

    // Callback function that is called when a message is received
    private void ReceiveCallback(IAsyncResult result)
    {
        try
        {
            // Get the socket that received the message
            Socket clientSoc = (Socket)result.AsyncState;

            // Get the number of bytes received
            int bytesReceived = clientSoc.EndReceive(result);

            if (bytesReceived > 0)
            {
                // Convert the received bytes to a string and display it on the Text object
                string messageString = Encoding.ASCII.GetString(inBuffer, 0, bytesReceived);
                textObject.text += "Client: " + messageString + "\n";

                Debug.Log("Received " + bytesReceived.ToString() + " bytes from client: " + clientSoc.RemoteEndPoint.ToString());

                // Start receiving the next message
                clientSoc.BeginReceive(inBuffer, 0, inBuffer.Length, SocketFlags.None, ReceiveCallback, clientSoc);
            }
            else
            {
                // If the connection was closed by the client, close the socket
                clientSoc.Close();
                clientSockets.Remove(clientSoc);
                Debug.Log("Connection closed by client: " + clientSoc.RemoteEndPoint.ToString());
            }
        }
        catch (Exception e)
        {
            Debug.Log("Exception: " + e.ToString());
        }
    }

    void OnApplicationQuit()
    {
        // Close the TCP sockets when the application quits
        foreach (Socket clientSoc in clientSockets)
        {
            clientSoc.Close();
        }
    }
}