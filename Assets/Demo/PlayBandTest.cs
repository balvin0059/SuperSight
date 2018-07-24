using UnityEngine;
using System.Collections;

public class PlayBandTest : MonoBehaviour
{
	public GUIStyle Back;
	public GUIStyle Again;
	
	public float BackX;
	public float BackY;
	public float AgainX;
	public float AgainY;
	void Start(){
		PlayBand.OnConnectResultEvent += ConnectResult;
		PlayBand.Connect();
	}

	public void ConnectResult(PlayBandConnectData result){}
	void OnGUI(){
		if (CleanFather.GameEndToClean) {
			if (GUI.Button (new Rect (Screen.width * BackX, Screen.height * BackY, Screen.width * .2f, Screen.height * .1f), "", Back)) {
				Application.LoadLevel ("LevelChoose");
			}
		}
		/*if (GUI.Button (new Rect (Screen.width * AgainX, Screen.height * AgainY, 100.0f, 50.0f), "", Again)) {
			Application.LoadLevel("LevelChoose");
		}*/
	}

}
