using System;
using System.Net;
using System.Net.Sockets;
using NaughtyAttributes;
using saxion_provided;
using UnityEngine;
using Random = UnityEngine.Random;

public class Client : MonoBehaviour
{
	public event Action<PlayerConnection.ConnectionType> connectionEvent;
	public event Action<float> oponnentDistanceRecievedEvent;

	public int id { get; private set; } = -1;
	public bool isInitialized => id >= 0;

	private bool isAccepted;
	private TcpClient client;

	public void Update()
	{
		if (client == null) { return; }

		try
		{
			if (client.Available > 0)
			{
				byte[] inBytes = StreamUtil.Read(client.GetStream());
				ProcessData(inBytes);
			}
		}
		catch (Exception e)
		{
			Debug.LogError(string.Concat(e.Message, '\n', e.StackTrace));
			Close();
		}
	}

	private void OnDestroy()
	{
		Debug.LogWarning("Destroying client instance...");
		Close();
	}

	public void Close()
	{
		if (client == null) { return; }

		isAccepted = false;
		client.Close();
		client = null;
		UnityEngine.SceneManagement.SceneManager.LoadScene(0);
	}

	public void Connect() => Connect(Settings.SERVER_IP, Settings.SERVER_PORT);

	public void Connect(IPAddress ip, int port, int attempts = 0)
	{
		if (isAccepted) { return; }

		if (attempts >= 5)
		{
			Destroy(this);
			throw new Exception("FAILED TO CONNECT TO SERVER");
		}

		try
		{
			Debug.Log("trying to connect to server");
			client ??= new TcpClient();
			client.Connect(ip, port);
		}
		catch (SocketException se)
		{
			if (se.SocketErrorCode == SocketError.ConnectionRefused)
			{
				Server.Instance.Initialize(ip, port);
				Connect(ip, port, attempts + 1);
				Debug.LogWarning($"Retrying connection attempt: {attempts}");
				return;
			}

			Debug.LogError($"Failed to connect to server!\n{se.Message}");
		}
	}

	public void SendData(Packet packet)
	{
		if (!isAccepted) { return; }

		if (packet == null)
		{
			Debug.LogWarning("Trying to send null");
			return;
		}

		// Debug.Log($"Client#{id} is sending data to the server!");
		try { StreamUtil.Write(client.GetStream(), packet.GetBytes()); }
		catch (Exception e)
		{
			Debug.LogWarning("Cannot send data to closed stream.");
			Destroy(this);
		}
	}

	private void ProcessData(byte[] dataInBytes)
	{
		Packet packet = new(dataInBytes);
		SeverObject severObject;
		try { severObject = packet.ReadObject(); }
		catch
		{
			Debug.LogError("object could not be read.");
			Close();
			return;
		}

		if (!isAccepted)
		{
			if (severObject is not AccessCallback callback) { return; }

			HandleAccessCallback(callback);
			return;
		}

		switch (severObject)
		{
			case PlayerDistance playerDistance:
				oponnentDistanceRecievedEvent?.Invoke(playerDistance.distance);
				break;
			case PlayerConnection playerConnection:
				connectionEvent?.Invoke(playerConnection.connectionType);
				break;
			case HeartBeat: break;
			default: throw new NotSupportedException($"Cannot process ISerializable type {severObject.GetType().Name}");
		}
	}

	private void HandleAccessCallback(AccessCallback callback)
	{
		isAccepted = callback.accepted;
		if (!isAccepted)
		{
			Destroy(this);
			return;
		}

		id = callback.id;
	}
}