using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Net;
using System.Text;
using System.Net.Sockets;

public class TCPClient : MonoBehaviour
{
    public InputField inputField;
    public Text textObject;

    private Socket clientSoc;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            // Create a new TCP socket and connect to the server
            clientSoc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSoc.Connect(new IPEndPoint(IPAddress.Loopback, 8889));

            Debug.Log("Connected to server: " + clientSoc.RemoteEndPoint.ToString());

            // Start receiving messages from the server
            byte[] inBuffer = new byte[512];
            clientSoc.BeginReceive(inBuffer, 0, inBuffer.Length, SocketFlags.None, ReceiveCallback, inBuffer);
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
            
            // Get the number of bytes received
            int bytesReceived = clientSoc.EndReceive(result);

            if (bytesReceived > 0)
            {
                // Convert the received bytes to a string and display it on the Text object
                string messageString = Encoding.ASCII.GetString((byte[])result.AsyncState, 0, bytesReceived);
                textObject.text = messageString;

                Debug.Log("Received " + bytesReceived.ToString() + " bytes from server.");

                // Start receiving the next message
                byte[] inBuffer = new byte[512];
                clientSoc.BeginReceive(inBuffer, 0, inBuffer.Length, SocketFlags.None, ReceiveCallback, inBuffer);
            }
            else
            {
                // If the connection was closed by the server, close the socket
                clientSoc.Close();
                Debug.Log("Connection closed by server.");
            }
        }
        catch (Exception e)
        {
            Debug.Log("Exception: " + e.ToString());
        }
    }

    // Send a message to the server when the "Send" button is clicked
    public void SendMessage()
    {
        string message = inputField.text;

        // Convert the message string to a byte array
        byte[] messageBytes = Encoding.ASCII.GetBytes(message);

        // Send the message to the server
        clientSoc.Send(messageBytes);
        inputField.text = null;
    }

    void OnApplicationQuit()
    {
        // Close the TCP socket when the application quits
        clientSoc.Close();
    }
}