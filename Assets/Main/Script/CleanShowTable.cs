using UnityEngine;
using System.Collections;

public class CleanShowTable : MonoBehaviour {

	SpriteRenderer sr;
	public Sprite ObjSprite;

	// Use this for initialization
	void Start () {
		sr = GetComponent<SpriteRenderer>();
		//transform.localScale += new Vector3(0.6f,0.6f,0.0f);
	}
	
	// Update is called once per frame
	void Update () {

		if (CleanFather.GameEndToClean) {
			sr.sprite = ObjSprite;
		}
	}
}