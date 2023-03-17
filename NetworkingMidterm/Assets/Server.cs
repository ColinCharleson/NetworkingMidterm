using System;
using System.Net;
using System.Text;
using System.Net.Sockets;
using UnityEngine;

public class Server : MonoBehaviour
{
	private Socket socket;
	IPAddress ip;

	public GameObject myCube;

	public void Start()
	{
		//Get the IP of the current computer
		IPHostEntry hostInfo = Dns.GetHostEntry(Dns.GetHostName());
		 ip = IPAddress.Parse("127.0.0.1");

		socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
		socket.Bind(new IPEndPoint(IPAddress.Any, 8888));


		//Display host name and IP of server
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

		// Recieves data from client
		byte[] buffer = new byte[512];
		int bytesReceived = socket.ReceiveFrom(buffer, ref remoteEP);
		// Converts the buffer to a string from bytes
		string data = Encoding.ASCII.GetString(buffer, 0, bytesReceived);

		float[] cubePos = new float[3];
		Buffer.BlockCopy(buffer, 0, cubePos, 0, 12);

		// Updates cubes position
		myCube.transform.position = new Vector3(cubePos[0], cubePos[1], cubePos[2]);
		Debug.Log("Recv from: " + remoteEP + " Data: " + new Vector3(cubePos[0], cubePos[1], cubePos[2]));
	}
}