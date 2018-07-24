using UnityEngine;
using System.Collections;

public class LevelChose : MonoBehaviour {

	// bottom set
	public GUIStyle Level1;
	public GUIStyle Level2;
	public GUIStyle Level3;
	public GUIStyle Level4;
	public GUIStyle Level5;
	public GUIStyle ChangeL;
	public GUIStyle ChangeR;

	public float Level1X;
	public float Level1Y;
	public float ChangeLX;
	public float ChangeLY;
	public float ChangeRX;
	public float ChangeRY;

	public static int LevelChos = 1;


	void Update(){
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.Quit(); 
		}
	}
	void OnGUI(){

		switch(LevelChos){
		case 1:
			if (GUI.Button (new Rect (Screen.width * Level1X, Screen.height * Level1Y, Screen.width * .2f,Screen.height * .1f), "", Level1)) {
					Application.LoadLevel("Gaming");
				}
			break;
		case 2:
			if (GUI.Button (new Rect (Screen.width * Level1X, Screen.height * Level1Y, Screen.width * .2f,Screen.height * .1f), "", Level2)) {
				Application.LoadLevel("Gaming");		
			}
			break;
		case 3:
			if (GUI.Button (new Rect (Screen.width * Level1X, Screen.height * Level1Y, Screen.width * .2f,Screen.height * .1f), "", Level3)) {
				Application.LoadLevel("Gaming");
			}
			break;
		case 4:
			if (GUI.Button (new Rect (Screen.width * Level1X, Screen.height * Level1Y, Screen.width * .2f,Screen.height * .1f), "", Level4)) {
				Application.LoadLevel("Gaming");
			}
			break;
		case 5:
			if (GUI.Button (new Rect (Screen.width * Level1X, Screen.height * Level1Y, Screen.width * .2f,Screen.height * .1f), "", Level5)) {

			}
			break;
		}
		if (GUI.Button (new Rect (Screen.width * ChangeLX, Screen.height * ChangeLY, Screen.width * .1f,Screen.height * .05f), "", ChangeL)) {
			if(LevelChos == 1){
				LevelChos = 5;
			}else{	LevelChos -= 1;	}
		}
		if (GUI.Button (new Rect (Screen.width * ChangeRX, Screen.height * ChangeRY, Screen.width * .1f,Screen.height * .05f), "", ChangeR)) {
			if(LevelChos == 5){
				LevelChos = 1;
			}else{	LevelChos += 1;	}
		}

	}
}
