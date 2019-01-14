using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Client : MonoBehaviour
{
	public Transform cameraTransform;
    public Text ConnectState;
    public Text ServerIP;
    public Text CurrentLog;

	//config ip and port
	private const int SERVER_PORT = 8081;
    private string m_ServerIp = "";
	
	string Message = "disconnected";
	bool isConnecting = false;
	NetworkView networkView;

	// Use this for initialization
	void Start (){
		networkView = this.GetComponent<NetworkView> ();
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
        CurrentLog.text = ip + ":" + 8081;
    }

	// Update is called once per frame
    void Update()
    {
        if (MasterServer.PollHostList().Length != 0)
        {
            HostData[] hostDatas = MasterServer.PollHostList();
            m_ServerIp = hostDatas[0].ip[0];                       
        }
        ServerIP.text = m_ServerIp + ":" + SERVER_PORT;
        ConnectState.text = Message;

        switch (Network.peerType)
        {
            case NetworkPeerType.Disconnected:
                if (isConnecting == false && m_ServerIp != "")
                {
                    StartConnect();
                }
                break;
            case NetworkPeerType.Client:
                //Online();
                break;
            case NetworkPeerType.Connecting:
                break;
        }
    }

	public void Online ()
	{ 
		if (GUILayout.Button ("断开服务器")) {
			Network.Disconnect ();
		}
	}


	public void StartConnect ()
    {
        
        CurrentLog.text = "开始连接->" + m_ServerIp;
		isConnecting = true;
        NetworkConnectionError error = Network.Connect(m_ServerIp, SERVER_PORT);
        CurrentLog.text += "\n Status:" + error;
	}

	void OnFailedToConnect (NetworkConnectionError error)
	{
		isConnecting = false;
		Debug.Log ("OnFailedToConnect: " + error.ToString());
		StartConnect ();
	}

	void OnDisconnectedFromServer (NetworkDisconnection info)
	{
		isConnecting = false;
		Message = "disconnected";
		CurrentLog.text = "OnDisconnectedFromServer";
	}

	void OnConnectedToServer ()
	{
		isConnecting = true;
		Message = "connected";
		CurrentLog.text = "OnConnectedToServer";
	}


	[RPC]
	void RequestMessage (string command, string value, NetworkMessageInfo info)
	{
		//play  playing playing-pause pause replay
		Message = command + ":" + value;

		if (command.Equals ("p")) {
			float x=0.0f;
			float y=0.0f;
			float z=0.0f;
            float x1 = 0.0f;
            float y1 = 0.0f;
            float z1 = 0.0f;
            string[] ratations=value.Split(',');
			if(ratations.Length==6){
				x=float.Parse(ratations[0]);
				y=float.Parse(ratations[1]);
				z=float.Parse(ratations[2]);
                x1 = float.Parse(ratations[3]);
                y1 = float.Parse(ratations[4]);
                z1 = float.Parse(ratations[5]);
            }
			cameraTransform.eulerAngles=new Vector3(x,y,z);
            cameraTransform.transform.position = new Vector3(x1, y1, z1);
        }

	}
}
