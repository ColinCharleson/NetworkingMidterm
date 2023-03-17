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
    public InputField ipInput;
    public InputField msgInput;

    private static byte[] outBuffer = new byte[512];
    private static IPEndPoint remoteEP;
    private static Socket clientSoc;

    // Start is called before the first frame update
    void Start()
    {
        StartClient();
    }

    // Update is called once per frame
    void Update()
    {
        // Check for input from user
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SendMsg();
        }
    }

    void StartClient()
    {
        try
        {
            // Get IP address from input field
            IPAddress ip = IPAddress.Parse(ipInput.text);

            // Create an endpoint that represents the server's IP and port number
            remoteEP = new IPEndPoint(ip, 8888);

            // Create a new TCP socket and connect to the server
            clientSoc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSoc.Connect(remoteEP);

            Debug.Log("Connected to server: " + remoteEP.ToString());
        }
        catch (Exception e)
        {
            Debug.Log("Exception: " + e.ToString());
        }
    }

    void SendMsg()
    {
        try
        {
            // Convert message to byte array
            byte[] messageByte = Encoding.ASCII.GetBytes(msgInput.text);

            // Send message to the server
            int bytesSent = clientSoc.Send(messageByte);

            Debug.Log("Sent " + bytesSent.ToString() + " bytes to server.");

            // Clear the input field
            msgInput.text = "";
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