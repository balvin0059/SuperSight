using UnityEngine;
using System.Collections;

public class ScoreShowerContrl : MonoBehaviour {
	SpriteRenderer sr;
	public Sprite Perfect;
	public Sprite Miss;
	public Sprite None;
	public static int ShowNum = 0;

	// Use this for initialization
	void Start () {
		sr = GetComponent<SpriteRenderer>();
		transform.localScale = new Vector3(0.5f,0.5f,0.0f);
	}
	
	// Update is called once per frame
	void Update () {
		switch (ShowNum) {
		case 0:
			sr.sprite = None;
			break;
		case 1:
			sr.sprite = Perfect;
			break;
		case 2:
			sr.sprite = Miss;
			break;
		}
	}
}
