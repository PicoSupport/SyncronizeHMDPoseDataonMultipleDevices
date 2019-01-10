using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Server : MonoBehaviour
{
	public Transform cameraTransform;
    public Text OnlineNum;
    public Text CurrentState;
    public Text Log;

	PlayStatus currentStatus = PlayStatus.Ready;  
	float alreadyPlayedTime = 0.0f;
	int port = 8081;
	NetworkView networkView;
	int length = 0;
	
	void Start ()
	{
		networkView = this.GetComponent<NetworkView> ();

		if (Network.peerType == NetworkPeerType.Disconnected) {
			bool useNat = !Network.HavePublicAddress ();
			NetworkConnectionError error = Network.InitializeServer (1000, port, useNat);
			MasterServer.RegisterHost ("PicoNet","Speech");
		}

        Invoke("RequestServerIP", 5f);        
	}

    void RequestServerIP()
    {
        MasterServer.ClearHostList();
        MasterServer.RequestHostList("PicoNet");

        Invoke("GetServerIP", 5f);
    }

    void GetServerIP()
    {
        HostData[] hostDatas = MasterServer.PollHostList();
        string ip = hostDatas[0].ip[0];
        Log.text = ip + ":" + 8081;
    }

	void OnGUI ()
	{
		switch (Network.peerType) {
		case NetworkPeerType.Disconnected:
			StartServer ();
			break;
		case NetworkPeerType.Server:
			OnServer ();
			break;
		case NetworkPeerType.Client:
			break;
		case NetworkPeerType.Connecting:
			break;
		}
	}

	void FixedUpdate ()
	{
//		if (currentStatus==PlayStatus.Playing) {
//			alreadyPlayedTime++;
//		}

		string sendString = cameraTransform.rotation.eulerAngles.x + "," +
			cameraTransform.rotation.eulerAngles.y + "," +
			cameraTransform.rotation.eulerAngles.z + "," +
            cameraTransform.transform.position.x + ","+
            cameraTransform.transform.position.y + "," +
            cameraTransform.transform.position.z ;
		networkView.RPC ("RequestMessage", RPCMode.Others, "p",sendString);
//		Debug.Log (sendString);
	}

	void StartServer ()
	{
		if (GUILayout.Button ("启动服务器")) {
			bool useNat = !Network.HavePublicAddress ();
			NetworkConnectionError error = Network.InitializeServer (1000, port, useNat);
			Log.text = "Status:" + error;
		}
	}


	void OnServer ()
	{
		length = Network.connections.Length;
		OnlineNum.text = "在线人数：" + length;
		CurrentState.text = "当前状态：" + currentStatus.ToString() + "\n 播放时间：" + alreadyPlayedTime;


//		for (int i=0; i<length; i++) {
//			NetworkPlayer player=Network.connections[i];
//			GUILayout.Label("IP:"+player.ipAddress+"    Port:"+player.port);
//		}

		if (GUILayout.Button ("断开服务器")) {
			Network.Disconnect ();
			alreadyPlayedTime = 0;
			currentStatus=PlayStatus.Ready;
		}

		if (GUILayout.Button ("播放")) {
			currentStatus = PlayStatus.Playing;
			networkView.RPC ("RequestMessage", RPCMode.All, "play", "");
		}
		
		if (GUILayout.Button ("暂停")) {
			currentStatus = PlayStatus.Pause;
			networkView.RPC ("RequestMessage", RPCMode.All, "pause", "");
		}
		if (GUILayout.Button ("重置")) {
			currentStatus = PlayStatus.Ready;
			alreadyPlayedTime = 0;
			networkView.RPC ("RequestMessage", RPCMode.All, "reset", "");
		}
	}

	void OnPlayerConnected (NetworkPlayer player)
	{
		string command = "ready";		
		switch (currentStatus) {
		case PlayStatus.Playing:
			command = "playing";
			break;
		case PlayStatus.Pause:
			command = "playing-pause";
			break;
		default:
			command = "ready";
			break;
		}
		networkView.RPC ("RequestMessage",
		                 player,
		                 command,
		                 alreadyPlayedTime.ToString ());
	}

	[RPC]
	void RequestMessage (string command, string value, NetworkMessageInfo info)
	{
		Log.text = "command:" + command + "   value:" + value;
	}
}

public enum PlayStatus
{
	Ready,
	Playing,
	Pause
}
