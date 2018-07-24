using UnityEngine;

public class PlayBandData
{
	public string BandMac="";
	public int BandNo=-1;

	public Quaternion Rotation;
	public Vector3 EulerAngles;
	public Vector3 Acceleration;
	public Vector3 FixedAcceleration;


	//
	//private Quaternion yupAngel=Quaternion.Euler(90f,0f,0f);
	//
	public PlayBandData(){
	}
	//
	public PlayBandData(PlayBandData data ){
		Update(data);
	}
	//
	public PlayBandData(float[] dataList){
		Update(dataList);
	}

	public void Update(PlayBandData data){
		Rotation.Set(data.Rotation.x,data.Rotation.y,data.Rotation.z,data.Rotation.w);
		EulerAngles.Set(data.EulerAngles.x,data.EulerAngles.y,data.EulerAngles.z);
		Acceleration.Set(data.Acceleration.x,data.Acceleration.y,data.Acceleration.z);
		FixedAcceleration.Set(data.FixedAcceleration.x,data.FixedAcceleration.y,data.FixedAcceleration.z);
	}

	public void Update(float[] dataList){
		//Axis:Z-UP
		Rotation.w=dataList[0];
		Rotation.x=-dataList[1];
		//Rotation.y=-dataList[2];
		//Rotation.z=dataList[3];
		//to Y-UP
		Rotation.y=-dataList[3];
		Rotation.z=-dataList[2];
		//Rotation=Rotation*yupAngel;
		//Axis:Y-UP pitch yaw roll
		EulerAngles.x=dataList[4]%360f;
		EulerAngles.y=dataList[5]%360f;
		EulerAngles.z=dataList[13]%360f;
		//Axis:Y-UP
		FixedAcceleration.x=dataList[7];
		FixedAcceleration.y=dataList[9];
		FixedAcceleration.z=dataList[8];
		Acceleration.x=dataList[10];
		Acceleration.y=dataList[12];
		Acceleration.z=dataList[11];
	}

	public PlayBandData Clone(){
		return new PlayBandData(this);
	}
	
	public override string ToString()
	{
		return string.Format("Rotation:[{0},{1},{2},{3}]\nEulerAngles:[{4},{5},{6}]\nAcceleration:[{7},{8},{9}]\nFixedAcceleration:[{10},{11},{12}]", Rotation.x, Rotation.y, Rotation.z, Rotation.w,EulerAngles.x,EulerAngles.y,EulerAngles.z, Acceleration.x,  Acceleration.y,  Acceleration.z,FixedAcceleration.x,FixedAcceleration.y,FixedAcceleration.z);
	}
	//==============================
	//ios
	//==============================
	#if UNITY_IPHONE && !UNITY_EDITOR
	private string[] separators={","};
	private float[] floatList=new float[14];
	//public PlayBandData(string TString){
	public void Update(string dataString){
		string[] stringList=dataString.Split(separators,0);
		floatList[0]=float.Parse(stringList[0]);
		floatList[1]=float.Parse(stringList[1]);
		floatList[2]=float.Parse(stringList[2]);
		floatList[3]=float.Parse(stringList[3]);
		floatList[4]=float.Parse(stringList[4]);
		floatList[5]=float.Parse(stringList[5]);
		floatList[6]=float.Parse(stringList[6]);
		floatList[7]=float.Parse(stringList[7]);
		floatList[8]=float.Parse(stringList[8]);
		floatList[9]=float.Parse(stringList[9]);
		floatList[10]=float.Parse(stringList[10]);
		floatList[11]=float.Parse(stringList[11]);
		floatList[12]=float.Parse(stringList[12]);
		floatList[13]=float.Parse(stringList[13]);
		Update(floatList);
	}
	#endif
}
