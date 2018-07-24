public enum PlayBandBatteryStatus
{
	Low = 1,
	Normal=2,
	Good=3
};

public class PlayBandBatteryData:PlayBandID
{
	//battery life 0%~100%
	public int life;
	//status 3= 60%~100% , 2=10%~60% , 1=0%~10%
	public PlayBandBatteryStatus status;
	//
	public PlayBandBatteryData(string data,string deviceMac,int deviceNo):base(deviceMac,deviceNo){
		Update(data);
	}

	public PlayBandBatteryData(string data):base("",0){
		Update(data);
	}

	public void Update(string data){
		string[] separators={","};
		string[] dataList=data.Split(separators,0);
		life=int.Parse(dataList[0]);
		status=(PlayBandBatteryStatus)int.Parse(dataList[1]);
	}
}
