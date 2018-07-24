using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

	// bottom set
	public GUIStyle PLAYGAME;
	public GUIStyle EXIT;
	
	public float GUIBottomPLAYX;
	public float GUIBottomPLAYY;
	public float GUIBottomExitX;
	public float GUIBottomExitY;
	
	public bool showGUIOutline = true;
	
	void Update(){
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.Quit(); 
		}
	}
	void OnGUI(){
		
		// display background
			//display bottom (without GUI outline)
			if (GUI.Button (new Rect (Screen.width * GUIBottomPLAYX, Screen.height * GUIBottomPLAYY, Screen.width * .3f,Screen.height * .2f), "", PLAYGAME)) {
				print ("Click Play Game");	
				Application.LoadLevel("LevelChoose");
			}
			if (GUI.Button (new Rect (Screen.width * GUIBottomExitX, Screen.height * GUIBottomExitY, Screen.width * .3f,Screen.height * .2f), "", EXIT)) {
				print ("Click Exit");
				Application.Quit();

		}
	}
}