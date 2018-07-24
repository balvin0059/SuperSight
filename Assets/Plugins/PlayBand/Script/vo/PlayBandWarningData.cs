public class PlayBandWarningData:PlayBandID
{
	public bool lowBattery=false;
	public bool hightTemperature=false;
	public bool otherWarning=false;
	public bool willReset5s=false;

	public PlayBandWarningData():base("",0){
	}

	public PlayBandWarningData(string data,string deviceMac,int deviceNo): base(deviceMac,deviceNo){
		Update(data);
	}

	public PlayBandWarningData(string data):base("",0){
		Update(data);
	}

	public void Update(string data){
		string[] separators={","};
		string[] dataList=data.Split(separators,0);
		lowBattery=(dataList[0]=="1");
		hightTemperature=(dataList[1]=="1");
		otherWarning=(dataList[2]=="1");
		willReset5s=(dataList[3]=="1");
	}
}
