public class PlayBandID
{
	public string address="";
	public int playerID=0;

	public PlayBandID(string deviceMac,int deviceNo){
		SetID(deviceMac,deviceNo);
	}

	public virtual void SetID(string deviceMac,int deviceNo){
		address=deviceMac;
		playerID=deviceNo;
	}
}
