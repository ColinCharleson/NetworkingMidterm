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
    private static Socket clientSoc;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            // Create a new TCP socket and bind it to the local IP address and port number
            clientSoc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSoc.Bind(new IPEndPoint(IPAddress.Any, 8888));

            // Listen for incoming connections
            clientSoc.Listen(1);

            Debug.Log("Waiting for connection...");

            // Accept the first incoming connection
            Socket serverSoc = clientSoc.Accept();

            Debug.Log("Connected to client: " + serverSoc.RemoteEndPoint.ToString());

            // Start receiving messages from the client
            serverSoc.BeginReceive(inBuffer, 0, inBuffer.Length, SocketFlags.None, ReceiveCallback, serverSoc);
        }
        catch (Exception e)
        {
            Debug.Log("Exception: " + e.ToString());
        }
    }

    // Callback function that is called when a message is received
    private void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            // Get the socket that received the message
            Socket serverSoc = (Socket)ar.AsyncState;

            // Get the number of bytes received
            int bytesReceived = serverSoc.EndReceive(ar);

            if (bytesReceived > 0)
            {
                // Convert the received bytes to a string and display it on the Text object
                string messageString = Encoding.ASCII.GetString(inBuffer, 0, bytesReceived);
                textObject.text = messageString;

                Debug.Log("Received " + bytesReceived.ToString() + " bytes from client.");

                // Start receiving the next message
                serverSoc.BeginReceive(inBuffer, 0, inBuffer.Length, SocketFlags.None, ReceiveCallback, serverSoc);
            }
            else
            {
                // If the connection was closed by the client, close the socket
                serverSoc.Close();
                clientSoc.Close();
                Debug.Log("Connection closed by client.");
            }
        }
        catch (Exception e)
        {
            Debug.Log("Exception: " + e.ToString());
        }
    }

    void OnApplicationQuit()
    {
        // Close the TCP socket when the application quits
        clientSoc.Close();
    }
}
