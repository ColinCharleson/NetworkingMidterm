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
	public GameObject myCube;

	public InputField ipInput;
	public static IPAddress ip;

	private static byte[] outBuffer = new byte[512];
	private static IPEndPoint remoteEP;
	private static Socket clientSoc;

	private Vector3 lastPosition;

	public static void StartClient()
	{
		try
		{
			//represents a network endpoint as an IP address and a port
			remoteEP = new IPEndPoint(ip, 8888);

			// creates a new socket that can be used to send and recieve messages
			clientSoc = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

		}
		catch (Exception e)
		{
			Debug.Log("Exception: " + e.ToString());
		}
	}

	// Update is called once per frame
	void Update()
	{
		Vector3 pos = myCube.transform.position;

		if (lastPosition != pos)
		{
			byte[] floatToBytes = new byte[sizeof(float) * 3];
			Buffer.BlockCopy(new float[] { pos.x, pos.y, pos.z }, 0, floatToBytes, 0, sizeof(float) * 3);

			clientSoc.SendTo(floatToBytes, remoteEP);
			lastPosition = pos;
		}
	}

	// Start is called before the first frame update
	void Start()
	{
		myCube = GameObject.Find("Cube");
		lastPosition = myCube.transform.position;
		ip = GetIP();
		StartClient();
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
};
