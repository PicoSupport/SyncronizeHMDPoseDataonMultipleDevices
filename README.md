
## Unity_Demo_SyncronizeHMDPoseDataonMultipleDevices

## Unity Versions：
- 2017.1.0f3 and later

## Description：

- It realizes the function of synchronously displaying the application screen running in Pico device on PC or tablet.

- Main idea of the scheme: use the NetworkView provided by Unity to implement the Server side and the Client side respectively. The scene contents of the two sides are consistent. The Server side is running on the Pico device, while the Client side is running on PC or tablet.
Server calls MasterServer to create a room, and then the Client finds the IP through the room name to connect to the Server.
The Server sends the transform data of the Head to the Client every frame,
Remote procedure call protocol (RPC) is used to synchronize the Client and Server images.

Note: the client-side Camera can be written using an ordinary Camera, instead of **Pvr_UnitySDK Prefab**.

Main Code：
1．The server creates Host：
```C#
MasterServer.RegisterHost("PicoNet","Speech");
```
2．The client gets the Host list：
```C#
MasterServer.ClearHostList();
MasterServer.RequestHostList("PicoNet");
```
3．The client gets **Server IP**：
```C#
if(MasterServer.PollHostList().Length != 0)
{
  HostData[] hostDatas = MasterServer.PollHostList();
  m_ServerIp = hostDatas[0].ip[0];
}
```
Update the code：
1．Increase the parameters passed by the server
```C#
string sendString = 
  cameraTransform.rotation.eulerAngles.x + "," +
  cameraTransform.rotation.eulerAngles.y + "," +
  cameraTransform.rotation.eulerAngles.z + "," +
  cameraTransform.transform.position.x + ","+
  cameraTransform.transform.position.y + "," +
  cameraTransform.transform.position.z ;
networkView.RPC ("RequestMessage", RPCMode.Others, "p",sendString);
```
2．Increase client receive parameters
```C#
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
```
Parameter specification：
[0][1][2] This is the Rotation of the camera X, Y and Z
[3][4][5] Represents the degree to which the Position X, Y and Z axes of the camera Position change

## Usage：

- ClientScene： Assets -> Scenes -> Scene_Client.unity
- ServerScene： Assets -> Scenes -> Scene_Server.unity

- Build two scenes seperately. The server runs on a Pico device and the client runs on a PC or Pad.
