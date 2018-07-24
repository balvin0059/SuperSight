public class PlayBandMobileSensor
{
	public bool Magnetometer=true;
	public bool Accelerometer=true;
	public bool Gyro=true;


	public PlayBandMobileSensor(){
	}

	public PlayBandMobileSensor(string data){
		Update(data);
	}

	public void Update(string data){
		string[] separators={","};
		string[] dataList=data.Split(separators,0);
		Magnetometer=(dataList[0]=="1");
		Accelerometer=(dataList[1]=="1");
		Gyro=(dataList[2]=="1");
	}
}
