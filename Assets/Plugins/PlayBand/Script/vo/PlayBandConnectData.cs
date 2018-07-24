public enum PlayBandConnectStatus
{
	Success =0,
	Connecting,
	Failed,
	DeviceFailed,
	Disconnect
}

public class PlayBandConnectData:PlayBandID
{
	public bool success=false;
	public PlayBandConnectStatus status=PlayBandConnectStatus.Disconnect;

	public PlayBandConnectData():base("",0){
	}
	//
	public PlayBandConnectData(string data,string deviceMac,int deviceNo): base(deviceMac,deviceNo){
		status=(PlayBandConnectStatus)int.Parse(data);
		success=(status == PlayBandConnectStatus.Success);;
	}
	//
	public PlayBandConnectData(string data):base("",0){
		string[] separators={","};
		string[] dataList=data.Split(separators,0);
		status=(PlayBandConnectStatus)int.Parse(dataList[0]);
		success=(status == PlayBandConnectStatus.Success);
		if(success){
		address=dataList[1];
		}
	}
}