using UnityEngine;
using System.Collections;
	
public class PlayBandTest2 : MonoBehaviour
{
		private enum PAGE
		{
				OPERATION = 0,
				BATTERY,
				LED,
				VIBRATE,
				DATETIME,
				SYSTEM,
				MAGNETIC
		};
		
		private string m_ConnectedDeviceName = "";
		private string m_MgParam1 = "0";
		private string m_OpParam1 = "3";
		private string m_OpParam2 = "50";
		private string m_ViParam1 = "100";
		private string m_ViParam2 = "100";
		private string m_ViParam3 = "100";
		private string m_LEDParam1 = "0";
		private string m_LEDParam2 = "100";
		private string m_LEDParam3 = "100";
		private string m_LEDParam4 = "100";
		//
		private PAGE m_CurPage;
		private int leftSpace = 20;
		private int topSpace = 100;
		//
		private string res_Connect = "Disconnect";
		private string res_Data = "BandData";
		private string res_Calibrated = "Calibrated:false";
		private string res_Click = "Button Click:0";
		private int clicked = 0;
		private string res_Battery = "Battery Life:";
	//
	  private bool menuOpen=true;
		//
		public Transform obj3d;
	//
		public void ConnectResult (PlayBandConnectData connectData)
		{
				if (connectData.success) {
						res_Connect = "Connected:" + connectData.address;
		            	menuOpen=false;
				} else {
						res_Connect = connectData.status.ToString ();
				}
			
		}
		
	public void ReceiveData (PlayBandData data)
		{
				obj3d.rotation = data.Rotation;
				res_Data = data.ToString ();
		}
		
		public void ButtonClicked (PlayBandID data)
		{
		clicked++;
		res_Click ="Player "+(data.playerID+1)+ " Button Click:" + clicked.ToString ();
		}
		
	   public void OnCalibrated (PlayBandID data)
		{
				res_Calibrated = "Calibrated:true";
		}
		
		public void BatteryStatus (PlayBandBatteryData data)
		{
		res_Battery ="Player "+(data.playerID+1)+ " Battery Life:" + data.life.ToString ();
		}
		
		//
		void Start ()
		{
				PlayBand.OnConnectResultEvent += ConnectResult;
	        	PlayBand.OnIncomingDataEventP1 += ReceiveData;
				PlayBand.OnButtonClickedEvent += ButtonClicked;
				PlayBand.OnBatteryStatusEvent += BatteryStatus;
				PlayBand.OnCalibratedEvent += OnCalibrated;
			
				m_CurPage = PAGE.OPERATION;
		}
		
		void Update ()
		{
				if (Input.GetKeyDown (KeyCode.Escape)) {
						Application.Quit ();
				}
		}
		
		void OnGUI ()
		{
	
		if(!menuOpen){
			if (GUI.Button (new Rect (leftSpace, 20 , 80, 60), "Menu")) {
				menuOpen=true;
			}
			return;
		}else{
			if (GUI.Button (new Rect (leftSpace, 20 , 80, 60), "Menu")) {
				menuOpen=false;
			}
		}

				GUI.TextArea (new Rect (leftSpace + 0, topSpace + 260, 180, 40), res_Connect);
				GUI.TextArea (new Rect (leftSpace + 0, topSpace + 320, 180, 40), res_Calibrated);
				GUI.TextArea (new Rect (leftSpace + 0, topSpace + 380, 180, 40), res_Click);
				GUI.TextArea (new Rect (leftSpace + 0, topSpace + 440, 180, 40), res_Battery);
				GUI.TextArea (new Rect (leftSpace + 0, topSpace + 500, 280, 80), res_Data);
			
		if (GUI.Button (new Rect (leftSpace, topSpace + 0, 80, 60), "Connect\nOperation")) {
						m_CurPage = PAGE.OPERATION;
				}
			
				if (GUI.Button (new Rect (leftSpace + 100, topSpace + 0, 80, 60), "Magnetic\nMode")) {
						m_CurPage = PAGE.MAGNETIC;
				}
			
				if (GUI.Button (new Rect (leftSpace + 200, topSpace + 0, 80, 60), "Vibrate")) {
						m_CurPage = PAGE.VIBRATE;
				}
			
			
				if (GUI.Button (new Rect (leftSpace + 300, topSpace + 0, 80, 60), "LED")) {
						m_CurPage = PAGE.LED;
				}
			
			
				if (GUI.Button (new Rect (leftSpace + 400, topSpace + 0, 80, 60), "Battery")) {
						//PlayBand.InquireBatteryStatus ();
			            m_CurPage = PAGE.BATTERY;
				}
			
				if (m_CurPage == PAGE.OPERATION) {
						m_OpParam1 = GUI.TextField (new Rect (leftSpace + 0, topSpace + 80, 80, 60), m_OpParam1);
						m_OpParam2 = GUI.TextField (new Rect (leftSpace + 100, topSpace + 80, 180, 60), m_ConnectedDeviceName);
				
			if(!PlayBand.BandConnectList[0].success){
						if (GUI.Button (new Rect (leftSpace + 0, topSpace + 160, 80, 80), "Connect")) {
			                	//int _Ret = PlayBandBinding.Connect(m_ConnectedDeviceName, int.Parse(m_OpParam1), m_OpParam2);
								int _Ret = PlayBand.Connect ((PlayBandPowerMode)int.Parse (m_OpParam1), "");
								//
								res_Calibrated = "Calibrated:false";
								res_Battery = "Life:";
								res_Click = "Button Click:0";
								clicked = 0;
						}
			}else{
			if (GUI.Button (new Rect (leftSpace + 0, topSpace + 160, 80, 80), "Disconnect")) {
								int _Ret = PlayBand.Disconnect ();
								//
					           PlayBand.BandConnectList[0].success=false;
								res_Connect = "Disconnect";
				}

				if( !PlayBand.BandConnectList[1].success){
					if (GUI.Button (new Rect (leftSpace + 100, topSpace + 160, 80, 80), "2P\nConnect")) {
						//int _Ret = PlayBandBinding.Connect(m_ConnectedDeviceName, int.Parse(m_OpParam1), m_OpParam2);
						int _Ret = PlayBand.AddPlayer(1);
						//
					}
				}else{	
					if (GUI.Button (new Rect (leftSpace + 100, topSpace + 160, 80, 80), "2P\nDisconnect")) {
						PlayBand.BandConnectList[1].success=false;
						int _Ret = PlayBand.RemovePlayer (1);

						//
					}
				}
			}

				} else if (m_CurPage == PAGE.MAGNETIC) {
						m_MgParam1 = GUI.TextField (new Rect (leftSpace + 0, topSpace + 80, 80, 60), m_MgParam1);
				
						if (GUI.Button (new Rect (leftSpace + 0, topSpace + 160, 80, 80), "Magnetic\nMode")) {
								int magnetic = 0;
								m_MgParam1 = magnetic.ToString ();
								PlayBand.SetMagenticMode ((PlayBandMagenticMode)magnetic);
								//PlayBand.VibrateTwice(50,500,500);
						}
				
						if (GUI.Button (new Rect (leftSpace + 100, topSpace + 160, 80, 80), "Gyro\nMode")) {
								int magnetic = 1;
								m_MgParam1 = magnetic.ToString ();
								PlayBand.SetMagenticMode ((PlayBandMagenticMode)magnetic);
						}
				
				
						if (GUI.Button (new Rect (leftSpace + 200, topSpace + 160, 80, 80), "No Move\nMode")) {
								int magnetic = 2;
								m_MgParam1 = magnetic.ToString ();
								PlayBand.SetMagenticMode ((PlayBandMagenticMode)magnetic);
						}
				
				} else if (m_CurPage == PAGE.LED) {	
						m_LEDParam1 = GUI.TextField (new Rect (leftSpace + 0, topSpace + 80, 80, 60), m_LEDParam1);
						m_LEDParam2 = GUI.TextField (new Rect (leftSpace + 100, topSpace + 80, 80, 60), m_LEDParam2);
						m_LEDParam3 = GUI.TextField (new Rect (leftSpace + 200, topSpace + 80, 80, 60), m_LEDParam3);
						m_LEDParam4 = GUI.TextField (new Rect (leftSpace + 300, topSpace + 80, 80, 60), m_LEDParam4);
				

			if (GUI.Button (new Rect (leftSpace + 0, topSpace + 160, 80, 80), "1P\nLED\nOff")) {
								int _Ret = PlayBand.LEDOff ((PlayBandLEDColor)(int.Parse (m_LEDParam1)),0);
						}
				
			if (GUI.Button (new Rect (leftSpace + 100, topSpace + 160, 80, 80), "1P\nLED\nOn")) {
				int _Ret = PlayBand.LEDOn ((PlayBandLEDColor)(int.Parse (m_LEDParam1)), int.Parse (m_LEDParam2),0);
						}
				
			if (GUI.Button (new Rect (leftSpace + 200, topSpace + 160, 80, 80), "1P\nLED\nFlash")) {
				int _Ret = PlayBand.LEDFlash ((PlayBandLEDColor)(int.Parse (m_LEDParam1)), int.Parse (m_LEDParam2), int.Parse (m_LEDParam3), int.Parse (m_LEDParam4),0);
					}

			if(PlayBand.BandConnectList[1].success){

				if (GUI.Button (new Rect (leftSpace + 300, topSpace + 160, 80, 80), "2P\nLED\nOff")) {
					int _Ret = PlayBand.LEDOff ((PlayBandLEDColor)(int.Parse (m_LEDParam1)),1);
				}
				
				if (GUI.Button (new Rect (leftSpace + 400, topSpace + 160, 80, 80), "2P\nLED\nOn")) {
					int _Ret = PlayBand.LEDOn ((PlayBandLEDColor)(int.Parse (m_LEDParam1)), int.Parse (m_LEDParam2),1);
				}
				
				if (GUI.Button (new Rect (leftSpace + 500, topSpace + 160, 80, 80), "2P\nLED\nFlash")) {
					int _Ret = PlayBand.LEDFlash ((PlayBandLEDColor)(int.Parse (m_LEDParam1)), int.Parse (m_LEDParam2), int.Parse (m_LEDParam3), int.Parse (m_LEDParam4),1);
				}
			}

				} else if (m_CurPage == PAGE.VIBRATE) {	
						m_ViParam1 = GUI.TextField (new Rect (leftSpace + 0, topSpace + 80, 80, 60), m_ViParam1);
						m_ViParam2 = GUI.TextField (new Rect (leftSpace + 100, topSpace + 80, 80, 60), m_ViParam2);
						m_ViParam3 = GUI.TextField (new Rect (leftSpace + 200, topSpace + 80, 80, 60), m_ViParam3);
				

			if (GUI.Button (new Rect (leftSpace + 0, topSpace + 160, 80, 80), "1P\nVibrate\nOnce")) {
				int _Ret = PlayBand.VibrateOnce (int.Parse (m_ViParam1), int.Parse (m_ViParam2),0);
						}
				
			if (GUI.Button (new Rect (leftSpace + 100, topSpace + 160, 80, 80), "1P\nVibrate\nTwice")) {
				int _Ret = PlayBand.VibrateTwice (int.Parse (m_ViParam1), int.Parse (m_ViParam2), int.Parse (m_ViParam3),0);
						}

			if(PlayBand.BandConnectList[1].success){

				if (GUI.Button (new Rect (leftSpace + 200, topSpace + 160, 80, 80), "2P\nVibrate\nOnce")) {
					int _Ret = PlayBand.VibrateOnce (int.Parse (m_ViParam1), int.Parse (m_ViParam2),1);
				}
				
				if (GUI.Button (new Rect (leftSpace + 300, topSpace + 160, 80, 80), "2P\nVibrate\nTwice")) {
					int _Ret = PlayBand.VibrateTwice (int.Parse (m_ViParam1), int.Parse (m_ViParam2), int.Parse (m_ViParam3),1);
				}

			}

		}else if (m_CurPage == PAGE.BATTERY) {	
			if (GUI.Button (new Rect (leftSpace + 0, topSpace + 160, 80, 80), "1P\nBattery")) {
				int _Ret = PlayBand.InquireBatteryStatus(0);
			}
			if(PlayBand.BandConnectList[1].success){
				if (GUI.Button (new Rect (leftSpace + 100, topSpace + 160, 80, 80), "2P\nBattery")) {
					int _Ret = PlayBand.InquireBatteryStatus(1);
				}
			}
		}
		}
}
