using UnityEngine;
using System.Collections;

public class CleanText : MonoBehaviour {
	public GameObject Father;
	public GameObject[] Child;
	public GUIText ObjNum_text;
	public float ObjNum;
	int Num_Check;
	string Objsrting;

	// Use this for initialization
	void Start () {
		ObjNum = 0.0f;
			Child = GameObject.FindGameObjectsWithTag ("Num");
		for ( int i=0 ; i < Child.Length ; i++ ) {
			if(gameObject.name == Child[i].name){
				Num_Check = i;
				print (Child[i].name);
				print (Num_Check);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		switch(Num_Check){
		case 0:
			ObjNum = CleanFather.Grn;
			break;
		case 1:
			ObjNum = CleanFather.Gon;
			break;
		case 2:
			ObjNum = CleanFather.Pn;
			break;
		case 3:
			ObjNum = CleanFather.HitRate;
			break;
		case 4:
			ObjNum = CleanFather.Mn;
			break;
		case 5:
			ObjNum = CleanFather.Bn;
			break;
		}
			if (CleanFather.GameEndToClean) {
			if(gameObject.name == "HitRate"){GetComponent<GUIText> ().text = ObjNum.ToString ("0")+"%";}
				GetComponent<GUIText> ().text = ObjNum.ToString ("0");	
			}

	}
}
