//=====================================
//20150428
//danco2009@gmail.com
//=====================================
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class PlayBand : MonoBehaviour
{
	public static string ConnectAddress = "";
	public static PlayBandConnectStatus  ConnectStatus = PlayBandConnectStatus.Disconnect;
	public static bool Calibrated = false;
	public static string LastConnectAddress = "";
	public static PlayBandMobileSensor MobileSensor = new PlayBandMobileSensor ();
	//--------------------------------------------------------------------
	public static event Action<PlayBandConnectData> OnConnectResultEvent;
	public static event Action<PlayBandData[]> OnIncomingDataEvent;
	public static event Action<PlayBandID> OnButtonClickedEvent;
	public static event Action<PlayBandBatteryData> OnBatteryStatusEvent;
	public static event Action<PlayBandWarningData> OnWarningDataEvent;
	//cancel
	public static event Action<PlayBandID> OnCalibratedEvent;
	//
	public static event Action<PlayBandData> OnIncomingDataEventP1;
	public static event Action<PlayBandData> OnIncomingDataEventP2;
	public static event Action<PlayBandData> OnIncomingDataEventP3;
	public static event Action<PlayBandData> OnIncomingDataEventP4;
	public static event Action OnButtonClickedEventP1;
	public static event Action OnButtonClickedEventP2;
	public static event Action OnButtonClickedEventP3;
	public static event Action OnButtonClickedEventP4;
	//--------------------------------------------------------------------
	public PlayBandPowerMode powerMode = PlayBandPowerMode.Normal;
	public PlayBandSensorRate sensorDataRate = PlayBandSensorRate.Normal_25HZ;
	//
	public static PlayBandPowerMode PowerMode {
		get { return instance.powerMode; }
		set { instance.powerMode = value;}
	}
	//
	public static PlayBandSensorRate SensorDataRate {
		get { return instance.sensorDataRate; }
		set { instance.sensorDataRate = value;}
	}
	//
	private float sensorDataPerSecond = 0.03f;
	private float pauseUpdateTime = 0f;
	private string UPDATE_SENSOR_DATA = "UpdateSensorData";
	private string BAND_MAC_SAVE = "bandMac";
	private bool alreadyCalculate = false;
	//--------------------------------------------------------------------
	private static PlayBandConnectData[] bandConnectList=new PlayBandConnectData[4];
	public static PlayBandConnectData[]  BandConnectList {
		get { return bandConnectList; }
		//set {bandConnectList = value;}
	}
	//--------------------------------------------------------------------
	private static PlayBandData bandData = new PlayBandData ();
	private static PlayBandData[] bandDataList=new PlayBandData[4]{new PlayBandData (),new PlayBandData (),new PlayBandData (),new PlayBandData ()};
	//--------------------------------------------------------------------
	public static PlayBand instance;
	
	public static PlayBand GetInstance ()
	{
		return instance;
	}
	//--------------------------------------------------------------------
	void Awake ()
	{
		instance = this;
		//gameObject.name = this.GetType().ToString();
		gameObject.name = "PlayBand";
		DontDestroyOnLoad (this);
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		Input.compensateSensors = true;
		//Input.compass.enabled=true;
		//Input.gyro.enabled=true;
		//
		SetSensorRate ((int)sensorDataRate);
		LastConnectAddress = PlayerPrefs.GetString (BAND_MAC_SAVE, "");
		//
		dataStream.Add (new PlayBandData ());
		dataStreamZ.Add (new PlayBandData ());
		//
		dataCacheStream.Add (new PlayBandDataCache ());
		dataCacheStream.Add (new PlayBandDataCache ());
		dataCacheStream.Add (new PlayBandDataCache ());
		//
		bandConnectList[0]=new PlayBandConnectData();
		bandConnectList[1]=new PlayBandConnectData();
		bandConnectList[2]=new PlayBandConnectData();
		bandConnectList[3]=new PlayBandConnectData();
	}
	//
	public void SetSensorRate (int dataRate)
	{
		sensorDataPerSecond = (float)(1000 / dataRate) * 0.001f;
		if (IsInvoking (UPDATE_SENSOR_DATA)) {
			StartDataListener ();
		}
	}
	
	public static void SetDataRate (int dataRate)
	{
		instance.SetSensorRate (dataRate);
	}
	//--------------------------------------------------------------------
	void Update ()
	{
		if (Time.timeScale == 0f) {
			if (IsInvoking (UPDATE_SENSOR_DATA)) {
				pauseUpdateTime += Time.unscaledDeltaTime;
				if (pauseUpdateTime >= sensorDataPerSecond) {
					UpdateSensorData ();
					pauseUpdateTime = 0;
				}
			}
		}
	}
	//
	private void StartDataListener ()
	{
		CancelInvoke (UPDATE_SENSOR_DATA);
		InvokeRepeating (UPDATE_SENSOR_DATA, 0.1f, sensorDataPerSecond);
	}
	
	private void StopDataListener ()
	{
		CancelInvoke (UPDATE_SENSOR_DATA);
	}
	//
	private void UpdateSensorData ()
	{
		GetSensorData ();
		
		if (OnIncomingDataEvent != null) {
			OnIncomingDataEvent (bandDataList);
		}

		if (OnIncomingDataEventP1 != null) {
			OnIncomingDataEventP1 (bandDataList[0]);
		}

		if (OnIncomingDataEventP2 != null) {
			OnIncomingDataEventP2 (bandDataList[1]);
		}

		if (OnIncomingDataEventP3 != null) {
			OnIncomingDataEventP3 (bandDataList[2]);
		}

		if (OnIncomingDataEventP4 != null) {
			OnIncomingDataEventP4 (bandDataList[3]);
		}
		//------------------------------
		alreadyCalculate = false;
		
		if (haveWayTrigger ()) {
			if (triggerLockOff) {
				CalculateForceXY ();
				PushDataCache (bandData, _currentForce, _forceDirection, _forceAngel);
				alreadyCalculate = true;
				//
				DecideWayTrigger (bandData, _currentForce, _forceDirection, _forceAngel);
			}
		}
		
		if (havePeakTrigger ()) {
			if (peakLockOff) {
				if (!alreadyCalculate) {
					CalculateForceXY ();
					PushDataCache (bandData, _currentForce, _forceDirection, _forceAngel);
				}
				DecidePeakTrigger (bandData, _currentForce, _forceDirection, _forceAngel); 
			}
		}
		//------------------------------
		alreadyCalculate = false;
		
		if (OnZWayTriggerEvent != null) {
			if (triggerLockOffZ) {
				CalculateForceXZ ();
				alreadyCalculate = true;
				DecideWayTriggerZ (bandData, _currentForce, _forceDirection);
			}
		}
		
		if (OnZWayPeakEvent != null) {
			if (peakLockOffZ) {
				if (!alreadyCalculate) {
					CalculateForceXZ ();
				}
				DecidePeakTriggerZ (bandData, _currentForce, _forceDirection);
			}
		}
		//------------------------------
		UpdateMouseCursor (bandData);
	}
	//
	private void CalculateForceXY ()
	{
		_forceDirection.x = bandData.Acceleration.x;
		_forceDirection.y = bandData.Acceleration.y;
		_currentForce = Mathf.Abs (Vector2.Distance (Vector2.zero, _forceDirection));
		_forceAngel = Vector2.Angle (Vector2.up, _forceDirection) % 360;
		if (_forceAngel > 180f) {
			_forceAngel = 360f - _forceAngel;
		}
	}
	
	private void CalculateForceXZ ()
	{
		_forceDirection.x = bandData.Acceleration.z;
		_forceDirection.y = bandData.Acceleration.x;
		_currentForce = Mathf.Abs (Vector2.Distance (Vector2.zero, _forceDirection));
	}


	//=====================================
	//JNI Android Multi
	//=====================================
	#if UNITY_ANDROID && !UNITY_EDITOR
	private string[] separators={"@"};
	private string[] stringList=new string[3];
	private string deviceMac="";
	private int deviceNo=-1;

	public void OnJniConnectResult (string result)
	{
		stringList=result.Split(separators,0);
		deviceNo=int.Parse(stringList[0]);
		deviceMac=stringList[1];
		result=stringList[2];

		PlayBandConnectData resultData = new PlayBandConnectData (result,deviceMac,deviceNo);
		bandConnectList[deviceNo]=resultData;
		ConnectAddress = resultData.address;
		OnConnectResultEvent (resultData);
		
		if (resultData.success) {
			StartDataListener ();
			LastConnectAddress = ConnectAddress;
			PlayerPrefs.SetString (BAND_MAC_SAVE, LastConnectAddress);
			PlayerPrefs.Save ();
		} else {
			Calibrated = false;
			StopDataListener ();
		}
		//
	}
	
	public void OnJniButtonClicked (string result)
	{
		stringList=result.Split(separators,0);
		deviceNo=int.Parse(stringList[0]);
		deviceMac=stringList[1];
		result=stringList[2];
		UpdateMouseClick (1);
		
		if (OnButtonClickedEvent != null) {
			OnButtonClickedEvent (new PlayBandID (deviceMac,deviceNo));
		}

		if (deviceNo==0&&OnButtonClickedEventP1 != null) {
			OnButtonClickedEventP1 ();
		}

		if (deviceNo==1&&OnButtonClickedEventP2 != null) {
			OnButtonClickedEventP2 ();
		}

		if (deviceNo==2&&OnButtonClickedEventP3 != null) {
			OnButtonClickedEventP3 ();
		}

		if (deviceNo==3&&OnButtonClickedEventP4 != null) {
			OnButtonClickedEventP4 ();
		}

	}
	//
	public void OnJniBatteryStatus (string result)
	{
		stringList=result.Split(separators,0);
		deviceNo=int.Parse(stringList[0]);
		deviceMac=stringList[1];
		result=stringList[2];

		if (OnBatteryStatusEvent != null) {
			OnBatteryStatusEvent (new PlayBandBatteryData (result,deviceMac,deviceNo));
		}
		
	}

	public void OnJniWarningStatus (string result)
	{
		stringList=result.Split(separators,0);
		deviceNo=int.Parse(stringList[0]);
		deviceMac=stringList[1];
		result=stringList[2];

		if (OnWarningDataEvent != null) {
			OnWarningDataEvent (new PlayBandWarningData (result,deviceMac,deviceNo));
		}
	}
	
	public void OnJniMagnetCalibrated (string result)
	{
		stringList=result.Split(separators,0);
		deviceNo=int.Parse(stringList[0]);
		deviceMac=stringList[1];
		result=stringList[2];

		Calibrated = (result == "1");
		if (OnCalibratedEvent != null) {
			OnCalibratedEvent (new PlayBandID (deviceMac,deviceNo));
			//OnCalibratedEvent ();
		}	
	}
	//
	public void OnJniSensorStatus (string result)
	{
		stringList=result.Split(separators,0);
		deviceNo=int.Parse(stringList[0]);
		deviceMac=stringList[1];
		result=stringList[2];

		MobileSensor.Update (result);
	}

	//=====================================
	//Sender
	//=====================================
	private static AndroidJavaObject _CurrentActivity;
	
	public static AndroidJavaObject GetActivity()
	{
		if(_CurrentActivity==null){
			AndroidJavaClass _UnityPlayer=new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			_CurrentActivity = _UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
		}
		return _CurrentActivity;
	}
	//--------------------------------------------------------------------
	/*
	public static PlayBandData GetSensorData()
	{
		AndroidJavaObject  _CurActivity=GetActivity();
		bandData.Update(_CurActivity.Call<float[]>("OnUnityGetSensor"));
		return bandData;
	}
   */

	private static float[] data1 = new float[14];
	private static float[] data2 = new float[14];
	private static float[] data3 = new float[14];
	private static float[] data4 = new float[14];
	//private static PlayBandData bandData1 = new PlayBandData ();
	private static PlayBandData bandData2 = new PlayBandData ();
	private static PlayBandData bandData3 = new PlayBandData ();
	private static PlayBandData bandData4 = new PlayBandData ();

	public static PlayBandData GetSensorData()
	{
		AndroidJavaObject  _CurActivity=GetActivity();
		//bandData.Update(_CurActivity.Call<float[]>("OnUnityGetSensor"));
		float[] combineBandData=_CurActivity.Call<float[]>("OnUnityGetSensor");
		//TODO
		bandData.Update(combineBandData);
		bandDataList[0]=bandData;
		//bandDataList[0].Update(combineBandData);
		//Array.Copy(combineBandData, data1, 14);
		if(bandConnectList[1].success){
		Array.Copy(combineBandData,14, data2, 0, 14);
			bandDataList[1].Update(data2);
		}
		if(bandConnectList[2].success){
		Array.Copy(combineBandData,28, data3, 0, 14);
			bandDataList[2].Update(data3);
		}
		if(bandConnectList[3].success){
		Array.Copy(combineBandData,42, data4, 0, 14);
			bandDataList[3].Update(data4);
		}

		//Debug.Log("GetSensorData:"+combineBandData);
		return bandData;
	}

	public static int AddPlayer(int deviceNo)
	{
		AndroidJavaObject  _CurActivity=GetActivity();
		
		object[] _Param = new object[1];
		_Param[0] = deviceNo;
		
		int _Ret = 0;
		_Ret = _CurActivity.Call<int>("OnUnityAddPlayer", _Param);
		return _Ret;
	}
	
	public static int RemovePlayer(int deviceNo)
	{
		AndroidJavaObject  _CurActivity=GetActivity();
		
		object[] _Param = new object[1];
		_Param[0] = deviceNo;
		
		int _Ret = 0;
		_Ret = _CurActivity.Call<int>("OnUnityRemovePlayer", _Param);
		return _Ret;
	}
	
	public static int InquireBatteryStatus(int deviceNo)
	{
		AndroidJavaObject  _CurActivity=GetActivity();

		object[] _Param = new object[1];
		_Param[0] = bandConnectList[deviceNo].address;

		int _Ret = 0;
		_Ret = _CurActivity.Call<int>("OnUnityInquireBatteryStatus", _Param);
		return _Ret;
	}

	public static int LEDOff(PlayBandLEDColor ledColor,int deviceNo)
	{
		AndroidJavaObject  _CurActivity=GetActivity();
		object[] _Param = new object[6];
		_Param[0] = (int)ledColor;
		_Param[1] = 0;
		_Param[2] = 1;
		_Param[3] = 0;
		_Param[4] = 0;
		_Param[5] = bandConnectList[deviceNo].address;
		int _Ret = 0;
		_Ret = _CurActivity.Call<int>("OnUnityControlLED", _Param);
		_Param = null;
		return _Ret;
	}

	public static int LEDOn(PlayBandLEDColor ledColor, int brightness,int deviceNo)
	{
		AndroidJavaObject  _CurActivity=GetActivity();
		object[] _Param = new object[6];
		_Param[0] = (int)ledColor;
		_Param[1] = 1;
		_Param[2] = brightness;
		_Param[3] = 0;
		_Param[4] = 0;
		_Param[5] = bandConnectList[deviceNo].address;
		int _Ret = 0;
		_Ret = _CurActivity.Call<int>("OnUnityControlLED", _Param);
		_Param = null;
		return _Ret;
	}

	public static int LEDFlash(PlayBandLEDColor ledColor, int brightness, int onPeriod, int offPeriod,int deviceNo)
	{
		AndroidJavaObject  _CurActivity=GetActivity();
		object[] _Param = new object[6];
		_Param[0] = (int)ledColor;
		_Param[1] = 2;
		_Param[2] = brightness;
		_Param[3] = onPeriod;
		_Param[4] = offPeriod;
		_Param[5] = bandConnectList[deviceNo].address;
		int _Ret = 0;
		_Ret = _CurActivity.Call<int>("OnUnityControlLED", _Param);
		_Param = null;
		return _Ret;
	}

	public static int VibrateOnce(int power, int period,int deviceNo)
	{
		AndroidJavaObject  _CurActivity=GetActivity();
		object[] _Param = new object[5];
		_Param[0] = 1;
		_Param[1] = power;
		_Param[2] = period;
		_Param[3] = 0;
		_Param[4] = bandConnectList[deviceNo].address;
		int _Ret = 0;
		_Ret = _CurActivity.Call<int>("OnUnityDoVibrate", _Param);
		_Param = null;
		return _Ret;
	}

	public static int VibrateTwice(int power, int onPeriod, int offPeriod,int deviceNo)
	{
		AndroidJavaObject  _CurActivity=GetActivity();
		object[] _Param = new object[5];
		_Param[0] = 2;
		_Param[1] = power;
		_Param[2] = onPeriod;
		_Param[3] = offPeriod;
		_Param[4] = bandConnectList[deviceNo].address;
		int _Ret = 0;
		_Ret = _CurActivity.Call<int>("OnUnityDoVibrate", _Param);
		_Param = null;
		return _Ret;
	}
	//--------------------------------------------------------------------
	public static int Connect(PlayBandPowerMode mode,string deviceName,PlayBandSensorRate sensorRate)
	{
		PowerMode=mode;
		SensorDataRate=sensorRate;
		AndroidJavaObject _CurActivity=GetActivity();
		object[] _Param = new object[3];
		_Param[0] = deviceName;
		_Param[1] = (int) mode;
		_Param[2] = (int)sensorRate;
		int _Ret = 0;
		_Ret = _CurActivity.Call<int>("OnUnityConnect", _Param);
		_Param = null;
		if(_Ret==0){
			Calibrated=false;
		}
		return _Ret;
	}
	//
	public static int Connect(PlayBandPowerMode mode,string deviceName)
	{
		return Connect(mode,deviceName,SensorDataRate);
	}
	//
	public static int Connect(PlayBandPowerMode mode)
	{
		PowerMode=mode;
		return Connect(mode,"",SensorDataRate);
	}
	
	public static int Connect()
	{
		return Connect(PowerMode,"",SensorDataRate);
	}
	
	public static int Reconnect()
	{
		return Connect(PowerMode,LastConnectAddress,SensorDataRate);
	}
	//
	public static int SetPowerMode(PlayBandPowerMode mode,PlayBandSensorRate sensorRate)
	{
		AndroidJavaObject  _CurActivity=GetActivity();
		object[] _Param = new object[2];
		_Param[0] =(int)mode;
		_Param[1] = sensorRate;
		int _Ret = 0;
		_Ret = _CurActivity.Call<int>("OnUnityChangeMode", _Param);
		_Param = null;
		return _Ret;
	}
	
	public static int Disconnect()
	{
		AndroidJavaObject  _CurActivity=GetActivity();
		_CurActivity.Call("OnUnityDisconnect");
		return 0;
	}
	
	public static int SetMagenticMode(PlayBandMagenticMode mode)
	{
		AndroidJavaObject  _CurActivity=GetActivity();
		object[] _Param = new object[1];
		_Param[0] = (int) mode;
		int _Ret=0;
		_Ret =_CurActivity.Call<int>("OnUnityMagneticMode",_Param);
		_Param = null;
		return _Ret;
	}
	
	public static void SetNoMoveMode()
	{
		SetMagenticMode(PlayBandMagenticMode.NoMove);
	}
	
	public static void SetNoMagneticMode()
	{
		SetMagenticMode(PlayBandMagenticMode.Gyro);
	}
	
	public static void SetMagneticMode()
	{
		SetMagenticMode(PlayBandMagenticMode.Magentic);
	}
	
	public static string GetCountry()
	{
		//CN TW HK
		AndroidJavaObject  _CurActivity=GetActivity();
		string _Ret =_CurActivity.Call<string>("OnUnityGetCountry");
		return _Ret;
	}
	
	#elif UNITY_EDITOR
	public static int Connect(PlayBandPowerMode mode,string deviceName,PlayBandSensorRate sensorRate )
	{
		return 0;
	}
	
	public static int Connect(PlayBandPowerMode mode,string deviceName )
	{
		return 0;
	}
	
	public static int Connect(PlayBandPowerMode mode)
	{
		return 0;
	}
	
	public static int Connect()
	{
		return 0;
	}
	
	public static int Reconnect()
	{
		return 0;
	}
	//
	public static int SetPowerMode(PlayBandPowerMode mode,PlayBandSensorRate sensorRate)
	{
		return 0;
	}
	
	public static int SetMagenticMode(PlayBandMagenticMode mode)
	{
		return 0;
	}
	
	public static void SetNoMoveMode()
	{
	}
	
	public static int Disconnect()
	{
		return 0;
	}
	
	public static PlayBandData GetSensorData()
	{
		return bandData;
	}
	
	public static string GetCountry()
	{
		return "CN";
	}

	public static int InquireBatteryStatus(int deviceNo)
	{
		return 0;
	}
	
	public static int LEDOff(PlayBandLEDColor ledColor,int deviceNo)
	{
		return 0;
	}
	
	public static int LEDOn(PlayBandLEDColor ledColor, int brightness,int deviceNo)
	{
		return 0;
	}
	
	public static int LEDFlash(PlayBandLEDColor ledColor, int brightness, int onPeriod, int offPeriod,int deviceNo)
	{
		return 0;
	}
	
	public static int VibrateOnce(int power, int period,int deviceNo)
	{
		return 0;
	}
	
	public static int VibrateTwice(int power, int onPeriod, int offPeriod,int deviceNo)
	{
		return 0;
	}
	//
	public static int AddPlayer(int deviceNo)
	{
		return 0;
	}
	
	public static int RemovePlayer(int deviceNo)
	{
		return 0;
	}

	#endif
	//--------------------------------------------------------------------
	//for v1.0 single band
	//--------------------------------------------------------------------
	public static int InquireBatteryStatus()
	{
		return InquireBatteryStatus(0);
	}
	
	public static int LEDOff(PlayBandLEDColor ledColor)
	{
		return  LEDOff(ledColor,0);
	}
	
	public static int LEDOn(PlayBandLEDColor ledColor, int brightness)
	{
		return LEDOn(ledColor, brightness,0);
	}
	
	public static int LEDFlash(PlayBandLEDColor ledColor, int brightness, int onPeriod, int offPeriod)
	{
		return LEDFlash( ledColor,  brightness,  onPeriod,  offPeriod,0);
	}
	
	public static int VibrateOnce(int power, int period)
	{
		return VibrateOnce(power,period,0);
	}
	
	public static int VibrateTwice(int power, int onPeriod, int offPeriod)
	{
		return VibrateTwice(power,onPeriod,offPeriod,0);
	}
	//=====================================
	//Decide Way
	//=====================================
	public bool exactAngleMode = false;
	//
	public static bool ExactAngleMode {
		get { return instance.exactAngleMode; }
		set { instance.exactAngleMode = value;}
	}
	//
	private List<PlayBandDataCache> dataCacheStream = new List<PlayBandDataCache> (3);
	//--------------------------------------------------------------------
	private PlayBandDirection DecideXWay (Vector2 forceDirection)
	{
		PlayBandDirection directionWay = (forceDirection.x >= 0) ? PlayBandDirection.Right : PlayBandDirection.Left;
		return directionWay;
	}
	
	private PlayBandDirection DecideYWay (Vector2 forceDirection)
	{
		PlayBandDirection directionWay = (forceDirection.y >= 0) ? PlayBandDirection.Up : PlayBandDirection.Down;
		return directionWay;
	}
	
	private PlayBandDirection DecideZWay (Vector2 forceDirection)
	{
		PlayBandDirection directionWay = (forceDirection.x >= 0) ? PlayBandDirection.Forward : PlayBandDirection.Back;
		return directionWay;
	}
	
	private PlayBandDirection Decide4Way (Vector2 forceDirection, float forceAngel)
	{
		PlayBandDirection directionWay = PlayBandDirection.Up;
		if (forceAngel <= 45f) {
			directionWay = PlayBandDirection.Up;
		} else if (forceAngel > 45f && forceAngel <= 135f) {
			directionWay = (forceDirection.x >= 0) ? PlayBandDirection.Right : PlayBandDirection.Left;
		} else if (forceAngel > 135f) {
			directionWay = PlayBandDirection.Down;
		}
		return directionWay;
	}
	
	private PlayBandDirection Decide8Way (Vector2 forceDirection, float forceAngel)
	{
		PlayBandDirection directionWay = PlayBandDirection.Up;
		
		if (forceAngel <= 22.5f) {
			directionWay = PlayBandDirection.Up;
		} else if (forceAngel > 22.5f && forceAngel <= 67.5f) {
			directionWay = (forceDirection.x >= 0) ? PlayBandDirection.RightUp : PlayBandDirection.LeftUp;
		} else if (forceAngel > 67.5f && forceAngel <= 112.5f) {
			directionWay = (forceDirection.x >= 0) ? PlayBandDirection.Right : PlayBandDirection.Left;
		} else if (forceAngel > 112.5f && forceAngel <= 157.5f) {
			directionWay = (forceDirection.x >= 0) ? PlayBandDirection.RightDown : PlayBandDirection.LeftDown;
		} else if (forceAngel > 157.5f) {
			directionWay = PlayBandDirection.Down;
		}
		return directionWay;
	}
	//
	//--------------------------------------------------------------------
	private void PushDataCache (PlayBandData data, float currentForce, Vector2 forceDirection, float forceAngel)
	{
		dataCacheStream [2].Update (dataCacheStream [1]);
		dataCacheStream [1].Update (dataCacheStream [0]);
		dataCacheStream [0].Update (data, currentForce, forceDirection, forceAngel);
	}
	
	private bool CorrectAngleDecide (PlayBandDataCache currentCache, PlayBandDataCache prevCache)
	{
		if (currentCache.direction.x > 0) {
			if (currentCache.data.EulerAngles.y < prevCache.data.EulerAngles.y) {
				return false;
			}
		} else {
			if (currentCache.data.EulerAngles.y > prevCache.data.EulerAngles.y) {
				return false;
			}
		}

		return true;
	}
	//=====================================
	//N WayTrigger
	//=====================================
	public static event Action<PlayBandDirection,PlayBandData> On4WayTriggerEvent;
	public static event Action<PlayBandDirection,PlayBandData> On8WayTriggerEvent;
	public static event Action<PlayBandDirection,PlayBandData> OnYWayTriggerEvent;
	public static event Action<PlayBandDirection,PlayBandData> OnXWayTriggerEvent;
	public static event Action<PlayBandDirection,PlayBandData> OnZWayTriggerEvent;
	public static event Action<float,Vector2,PlayBandData> OnXYAngleTriggerEvent;
	public static event Action<PlayBandData> OnShakeTriggerEvent;
	//
	public float triggerForce = 0.3f;
	public float triggerLockTime = 0.7f;   
	//
	public static float TriggerForce {
		get { return instance.triggerForce; }
		set { instance.triggerForce = value;}
	}
	//
	public static float TriggerLockTime {
		get { return instance.triggerLockTime; }
		set { instance.triggerLockTime = value;}
	}
	//
	private bool triggerLockOff = true;
	private bool triggerLockOffZ = true;
	//
	private float _currentForce = 0f;
	private float _forceAngel = 0f;
	private Vector2 _forceDirection = Vector2.zero;
	//--------------------------------------------------------------------
	private  bool haveWayTrigger ()
	{
		if (On4WayTriggerEvent != null) {
			return true;
		} else if (On8WayTriggerEvent != null) {
			return true;
		} else if (OnYWayTriggerEvent != null) {
			return true;
		} else if (OnXWayTriggerEvent != null) {
			return true;
		} else if (OnXYAngleTriggerEvent != null) {
			return true;
		} else if (OnShakeTriggerEvent != null) {
			return true;
		}


		return false;
	}
	//
	private void DecideWayTrigger (PlayBandData data, float currentForce, Vector2 forceDirection, float forceAngel)
	{
		//
		if (currentForce >= triggerForce) {
			//
			if (exactAngleMode) {
				if (!CorrectAngleDecide (dataCacheStream [0], dataCacheStream [2])) {
					return;
				}
			}
			//
			triggerLockOff = false;
			
			if (On4WayTriggerEvent != null) {
				On4WayTriggerEvent (Decide4Way (forceDirection, forceAngel), data);
			}
			if (On8WayTriggerEvent != null) {
				On8WayTriggerEvent (Decide8Way (forceDirection, forceAngel), data);
			}
			if (OnYWayTriggerEvent != null) {
				OnYWayTriggerEvent (DecideYWay (forceDirection), data);
			}
			if (OnXWayTriggerEvent != null) {
				OnXWayTriggerEvent (DecideXWay (forceDirection), data);
			}
			if (OnXYAngleTriggerEvent != null) {
				OnXYAngleTriggerEvent (forceAngel,forceDirection, data);
			}
			if (OnShakeTriggerEvent != null) {
				OnShakeTriggerEvent (data);
			}

			
			Invoke ("UnlockWayTrigger", triggerLockTime);
		}
		//
	}
	
	private void UnlockWayTrigger ()
	{
		triggerLockOff = true;
	}
	//--------------------------------------------------------------------
	private void DecideWayTriggerZ (PlayBandData data, float currentForce, Vector2 forceDirection)
	{
		if (currentForce >= triggerForce) {
			triggerLockOffZ = false;
			OnZWayTriggerEvent (DecideZWay (forceDirection), data);
			Invoke ("UnlockWayTriggerZ", triggerLockTime);
		}
	}
	
	private void UnlockWayTriggerZ ()
	{
		triggerLockOffZ = true;
	}
	//=====================================
	//PeakTrigger
	//=====================================
	public static event Action<PlayBandDirection,PlayBandData[]> On4WayPeakEvent;
	public static event Action<PlayBandDirection,PlayBandData[]> On8WayPeakEvent;
	public static event Action<PlayBandDirection,PlayBandData[]> OnYWayPeakEvent;
	public static event Action<PlayBandDirection,PlayBandData[]> OnXWayPeakEvent;
	public static event Action<PlayBandDirection,PlayBandData[]> OnZWayPeakEvent;
	public static event Action<float,Vector2,PlayBandData[]> OnXYAnglePeakEvent;
	public static event Action<PlayBandData[]> OnShakePeakEvent;
	
	//public bool enablePeakTrigger = false;
	public float minPeakGate = 0.2f;
	public float maxPeakGate = 0.7f;
	public float peakLockTime = 0.5f;
	//
	public static float MinPeakGate {
		get { return instance.minPeakGate; }
		set { instance.minPeakGate = value;}
	}
	//
	public static float MaxPeakGate {
		get { return instance.maxPeakGate; }
		set { instance.maxPeakGate = value;}
	}
	//
	public static float PeakLockTime {
		get { return instance.peakLockTime; }
		set { instance.peakLockTime = value;}
	}
	
	//private ArrayList dataStream = new ArrayList ();
	//private ArrayList dataStreamZ = new ArrayList ();
	private List<PlayBandData> dataStream = new List<PlayBandData> ();
	private List<PlayBandData> dataStreamZ = new  List<PlayBandData> ();
	private bool peakLockOff = true;
	private bool peakLockOffZ = true;
	private float lastForce = 0f;
	private float lastForceZ = 0f;
	
	private  bool havePeakTrigger ()
	{
		if (On4WayPeakEvent != null) {
			return true;
		} else if (On8WayPeakEvent != null) {
			return true;
		} else if (OnYWayPeakEvent != null) {
			return true;
		} else if (OnXWayPeakEvent != null) {
			return true;
		} else if (OnXYAnglePeakEvent != null) {
			return true;
		} else if (OnShakePeakEvent != null) {
			return true;
		}

		return false;
	}
	
	private void DecidePeakTrigger (PlayBandData data, float currentForce, Vector2 forceDirection, float forceAngel)
	{
		if (currentForce < minPeakGate) {
			return;
		}
		if (currentForce < lastForce) {
			if (lastForce >= maxPeakGate) {
				OnPeak (forceDirection, forceAngel);
			} 
			OnPeakEnd ();
		} else {
			dataStream.Add (data.Clone ());
			lastForce = currentForce;
		}
	}
	
	private void OnPeak (Vector2 forceDirection, float forceAngel)
	{
		//
		if (exactAngleMode) {
			if (!CorrectAngleDecide (dataCacheStream [1], dataCacheStream [2])) {
				return;
			}
		}
		//
		//PlayBandData[] dataList = (PlayBandData[])dataStream.ToArray (typeof(PlayBandData));
		PlayBandData[] dataList = dataStream.ToArray ();
		//
		if (On4WayPeakEvent != null) {
			On4WayPeakEvent (Decide4Way (forceDirection, forceAngel), dataList);
		}
		if (On8WayPeakEvent != null) {
			On8WayPeakEvent (Decide8Way (forceDirection, forceAngel), dataList);
		}
		if (OnYWayPeakEvent != null) {
			OnYWayPeakEvent (DecideYWay (forceDirection), dataList);
		}
		if (OnXWayPeakEvent != null) {
			OnXWayPeakEvent (DecideXWay (forceDirection), dataList);
		}
		if (OnXYAnglePeakEvent != null) {
			OnXYAnglePeakEvent (forceAngel,forceDirection, dataList);
		}
		if (OnShakePeakEvent != null) {
			OnShakePeakEvent (dataList);
		}

	}
	
	private void OnPeakEnd ()
	{
		dataStream.Clear ();
		dataStream.Add (new PlayBandData ());
		lastForce = 0f;
		peakLockOff = false;
		Invoke ("UnlockPeakTrigger", peakLockTime);
	}
	
	private void UnlockPeakTrigger ()
	{
		peakLockOff = true;
	}
	//--------------------------------------------------------------------
	private void DecidePeakTriggerZ (PlayBandData data, float currentForce, Vector2 forceDirection)
	{
		if (currentForce < minPeakGate) {
			return;
		}
		if (currentForce < lastForceZ) {
			if (lastForceZ >= maxPeakGate) {
				//OnZWayPeakEvent (DecideZWay (forceDirection), (PlayBandData[])dataStreamZ.ToArray (typeof(PlayBandData)));
				OnZWayPeakEvent (DecideZWay (forceDirection), dataStreamZ.ToArray ());
			} 
			OnPeakEndZ ();
		} else {
			dataStreamZ.Add (data.Clone ());
			lastForceZ = currentForce;
		}
		//
	}
	
	private void OnPeakEndZ ()
	{
		dataStreamZ.Clear ();
		dataStreamZ.Add (new PlayBandData ());
		lastForceZ = 0f;
		peakLockOffZ = false;
		Invoke ("UnlockPeakTriggerZ", peakLockTime);
	}
	
	private void UnlockPeakTriggerZ ()
	{
		peakLockOffZ = true;
	}
	//=====================================
	//Mouse
	//=====================================
	public static event Action<Collider> OnMousePressEvent;
	//
	public Transform mouseCursor;
	public Vector2 mouseAngleMax = new Vector2 (30f, 30f);
	public float raycastLength = 10f;
	public Camera mouseCamera;
	//
	public static Vector2 MouseAngleMax {
		get { return instance.mouseAngleMax; }
		set { instance.mouseAngleMax = value;}
	}
	
	public static float RaycastLength {
		get { return instance.raycastLength; }
		set { instance.raycastLength = value;}
	}
	
	public static Transform MouseCursor {
		get { return instance.mouseCursor; }
		set { instance.mouseCursor = value;}
	}
	
	public static Camera MouseCamera {
		get { return instance.mouseCamera; }
		set { instance.mouseCamera = value;}
	}
	
	[HideInInspector]
	public bool
		enableMouse = false;
	private Vector3 anglePosition = Vector3.zero;
	private Vector3 anglePositionCache = Vector3.zero;
	private Vector2 correctAngle = Vector2.zero;
	//
	public static void StartMouseListener ()
	{
		//custom camera
		if (instance.mouseCamera == null) {
			instance.mouseCamera = instance.gameObject.GetComponent<Camera> ();
		}
		//create camera
		if (instance.mouseCamera == null) {
			Camera mouseCamera = instance.gameObject.AddComponent<Camera> ();
			mouseCamera.transform.position = new Vector3 (0, 0, -1000);
			mouseCamera.clearFlags = CameraClearFlags.Depth;
			mouseCamera.orthographic = true;
			mouseCamera.depth = 100;
			mouseCamera.nearClipPlane = 0f;
			mouseCamera.farClipPlane = 1f;
			instance.mouseCamera = mouseCamera;
		}
		//
		if (instance.mouseCursor == null) {
			instance.enableMouse = false;
		} else {
			instance.enableMouse = true;
			instance.mouseCursor.gameObject.SetActive (true);
		}
	}
	//
	public static void StopMouseListener ()
	{
		instance.enableMouse = false;
		if (instance.mouseCursor != null) {
			instance.mouseCursor.gameObject.SetActive (false);
		}
	}
	
	private void UpdateMouseClick (int press)
	{
		if (!enableMouse) {
			return;
		}
		
		if (OnMousePressEvent != null) {
			Ray ray = Camera.main.ViewportPointToRay (anglePosition);
			//Debug.DrawRay (ray.origin, ray.direction.normalized * raycastLength, Color.yellow, 1f);
			RaycastHit hit = new RaycastHit ();
			if (Physics.Raycast (ray, out hit, raycastLength)) {
				OnMousePressEvent (hit.collider);
			}
		}
	}
	
	private void UpdateMouseCursor (PlayBandData data)
	{
		if (!enableMouse) {
			return;
		}
		//
		correctAngle.x = (bandData.EulerAngles.x <= 180f) ? bandData.EulerAngles.x : bandData.EulerAngles.x - 360f;
		correctAngle.y = (bandData.EulerAngles.y <= 180f) ? bandData.EulerAngles.y : bandData.EulerAngles.y - 360f;
		anglePosition.x = (mouseAngleMax.y + correctAngle.y) / (mouseAngleMax.y + mouseAngleMax.y);
		anglePosition.y = (mouseAngleMax.x + correctAngle.x) / (mouseAngleMax.x + mouseAngleMax.x);
		anglePosition.y = 1f - anglePosition.y;
		anglePosition.x = Mathf.Clamp (anglePosition.x, 0.02f, 0.98f);
		anglePosition.y = Mathf.Clamp (anglePosition.y, 0.02f, 0.98f);
		anglePosition.z = mouseCamera.nearClipPlane;
		anglePosition = Vector3.Lerp (anglePosition, anglePositionCache, 0.5f);
		mouseCursor.position = mouseCamera.ViewportToWorldPoint (anglePosition);
		anglePositionCache = anglePosition;
	}

	//end
}
