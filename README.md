
###  [ `Return | 首页` ](https://github.com/PicoSupport/PicoSupport)
* [AndroidDemo | 安卓](https://github.com/PicoSupport/PicoSupport/blob/master/android.md)
* [UnityDemo | Unity3d](https://github.com/PicoSupport/PicoSupport/blob/master/unity.md)

## Unity_Demo_PicoUnityProjectorDemo

## Unity_Versions：
- 2017.1.0f3 Or UP

## Explain 说明：

- 功能：实现了将Pico 设备中运行的应用画面同步显示在PC或平板端的功能。

- 方案主要思路：使用Unity自带的NetworkView，分别实现Server端和Client端，二者场景内容一致，Server端运行在Pico 设备上，Client端运行在PC或平板上。 Server调用MasterServer创建一个房间，然后Client通过房间名找到IP去连接Server，Server每帧向Client发送头戴（Head）的transform数据，通过远程过程调用协议（RPC）实现Client端与Server端画面的同步。

注：Client端的Camera使用普通Camera即可，不可用Pvr_UnitySDK Prefab。

Demo主要C#代码 BatteryManager.cs文件

主要代码：
1．Server端创建Host：
```
MasterServer.RegisterHost("PicoNet","Speech");
```
2．Client端获取Host列表：
	```
MasterServer.ClearHostList();
MasterServer.RequestHostList("PicoNet");
```
3．Client端获取Server IP：
```
if(MasterServer.PollHostList().Length != 0)
{
  HostData[] hostDatas = MasterServer.PollHostList();
  m_ServerIp = hostDatas[0].ip[0];
}
```
更新代码：
1．增加Server端传递参数
```
string sendString = 
  cameraTransform.rotation.eulerAngles.x + "," +
  cameraTransform.rotation.eulerAngles.y + "," +
  cameraTransform.rotation.eulerAngles.z + "," +
  cameraTransform.transform.position.x + ","+
  cameraTransform.transform.position.y + "," +
  cameraTransform.transform.position.z ;
networkView.RPC ("RequestMessage", RPCMode.Others, "p",sendString);
```
2．增加Client端接收参数
```
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
参数说明：
[0][1] [2] 代表了摄像机Rotation X，Y，Z轴的旋转程度
[3][4][5] 代表了摄像机Position X，Y，Z轴位置变化的程度

## Use 使用：

- 场景位置： Assets -> Scenes -> Scene_Client.unity
- 场景位置： Assets -> Scenes -> Scene_Server.unity

打包两个场景，Server端运行在Pico 设备上，Client端运行在PC或平板上。

## Announcements 注意事项：
- 在Goblin上使用时，在Pvr_UnitySDK中的Head Pose和Hand Pose选择Three Dof，取消勾选Show SafePanel
- 在CV上使用时，在Pvr_UnitySDK中的Head Pose和Hand Pose选择Six Dof

## Pico技术支持
欢迎更多地了解我们，如果您有任何问题，请联系我们。
Learn about us, and contact us if you have any questions. 

- Email:  support@picovr.com
- Web:  [https://www.picovr.com/](https://www.picovr.com/)
