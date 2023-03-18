using System;
using System.Net;
using System.Text;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Server : MonoBehaviour
{
    private Socket socket;
    IPAddress ip;

    public GameObject myCube1, myCube2;

    private Dictionary<EndPoint, int> clients = new Dictionary<EndPoint, int>();

    public Text ipList;
    public void Start()
    {
        // Get the IP of the current computer
        IPHostEntry hostInfo = Dns.GetHostEntry(Dns.GetHostName());
        ip = IPAddress.Parse("127.0.0.1");

        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        socket.Bind(new IPEndPoint(IPAddress.Any, 8888));

        // Display host name and IP of server
        Debug.Log("Server name: " + hostInfo.HostName + " IP: " + ip);
    }

    public void Update()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        // Represents a network endpoint as an IP address and a port
        IPEndPoint localEP = new IPEndPoint(ip, 0);
        EndPoint remoteEP = (EndPoint)localEP;

        // Receives data from client
        byte[] buffer = new byte[512];
        int bytesReceived = socket.ReceiveFrom(buffer, ref remoteEP);

        if (!clients.ContainsKey(remoteEP))
        {
            clients.Add(remoteEP, clients.Count + 1);
            Debug.Log("Client " + clients.Count + " connected from: " + remoteEP);
            ipList.text += "\n" + remoteEP;
        }

        if (clients.ContainsKey(remoteEP))
        {
            int clientIndex = clients[remoteEP] - 1;
            float[] cubePos = new float[3];
            Buffer.BlockCopy(buffer, 0, cubePos, 0, 12);

            if (clientIndex == 0)
            {
                myCube1.transform.position = new Vector3(cubePos[0], cubePos[1], cubePos[2]);
                Debug.Log("Received from client 1: " + new Vector3(cubePos[0], cubePos[1], cubePos[2]));
            }
            else if (clientIndex == 1)
            {
                myCube2.transform.position = new Vector3(cubePos[0], cubePos[1], cubePos[2]);
                Debug.Log("Received from client 2: " + new Vector3(cubePos[0], cubePos[1], cubePos[2]));
            }
        }
    }
}
