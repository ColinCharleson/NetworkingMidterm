using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Lec04
using System;
using System.Net;
using System.Text;
using System.Net.Sockets;


public class Client : MonoBehaviour
{
	public GameObject myCube, myCube1, myCube2;

	public InputField ipInput;
	public static IPAddress ip;

	private static byte[] outBuffer = new byte[512];
	private static byte[] inBuffer = new byte[512];
	private static IPEndPoint remoteEP;
	private static Socket clientSoc;

	private Vector3 lastPosition;

	private static int clientId;

	public static void StartClient()
	{
		try
		{
			//represents a network endpoint as an IP address and a port
			remoteEP = new IPEndPoint(ip, 8889);

			// creates a new socket that can be used to send and recieve messages
			clientSoc = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

			// bind the client socket to any available port
			clientSoc.Bind(new IPEndPoint(IPAddress.Any, 0));

			// start listening for incoming data
			clientSoc.BeginReceive(inBuffer, 0, inBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);

		}
		catch (Exception e)
		{
			Debug.Log("Exception: " + e.ToString());
		}
	}

	private static void ReceiveCallback(IAsyncResult result)
	{
		try
		{
			// get the size of the incoming data
			int receivedSize = clientSoc.EndReceive(result);		

			// start listening for more incoming data
			clientSoc.BeginReceive(inBuffer, 0, inBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
		}
		catch (Exception e)
		{
			Debug.Log("Exception: " + e.ToString());
		}
	}


	// Update is called once per frame
	void Update()
	{

		// extract the position data from the incoming message
		float[] positionData = new float[6];
		Buffer.BlockCopy(inBuffer, 0, positionData, 0, sizeof(float) * 6);

		// update the position of the appropriate cube based on the client ID
		if (clientId == 0)
		{
			GameObject cubeToUpdate = GameObject.Find("Player2");
			Vector3 newPosition = new Vector3(positionData[3], positionData[4], positionData[5]);
			cubeToUpdate.transform.position = newPosition;
		}
		else if (clientId == 1)
		{
			GameObject cubeToUpdate = GameObject.Find("Player1");
			Vector3 newPosition = new Vector3(positionData[0], positionData[1], positionData[2]);
			cubeToUpdate.transform.position = newPosition;
		}

		Vector3 pos = myCube.transform.position;

		if (lastPosition != pos)
		{
			byte[] floatToBytes = new byte[(sizeof(float) * 3) + sizeof(int)];
			Buffer.BlockCopy(new float[] { pos.x, pos.y, pos.z }, 0, floatToBytes, 0, sizeof(float) * 3);
			Buffer.BlockCopy(BitConverter.GetBytes(clientId), 0, floatToBytes, sizeof(float) * 3, sizeof(int));
			clientSoc.SendTo(floatToBytes, remoteEP);
			lastPosition = pos;
		}
	}

	IPAddress GetIP()
	{
		if (ipInput.text == null)
		{
			return IPAddress.Parse("127.0.0.1");
		}
		else
		{

			return IPAddress.Parse(ipInput.text);
		}
	}

	public void JoinClient(int id)
	{
		clientId = id;

		if (clientId == 0)
		{
			myCube = myCube1;
			Debug.Log("Im Client 1)");
			lastPosition = myCube.transform.position;
			myCube2.GetComponent<Cube1>().enabled = false;
		}
		else if (clientId == 1)
		{
			myCube = myCube2;
			Debug.Log("Im Client 2)");
			lastPosition = myCube.transform.position;
			myCube1.GetComponent<Cube1>().enabled = false;
		}

		ip = GetIP();
		StartClient();
	}
};
