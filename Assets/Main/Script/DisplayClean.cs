using UnityEngine;
using System.Collections;

public class DisplayClean : MonoBehaviour {
	SpriteRenderer sr;
	public Sprite S;
	public Sprite A;
	public Sprite B;
	public Sprite C;
	public Sprite D;
	public Sprite None;
	public static int DisplayC = 0;

	// Use this for initialization
	void Start () {
		sr = GetComponent<SpriteRenderer>();
		transform.localScale += new Vector3(0.6f,0.6f,0.0f);
	}
	
	// Update is called once per frame
	void Update () {
		switch (DisplayC) {
		case 0:
			sr.sprite = None;
			break;
		case 1:
			sr.sprite = S;
			break;
		case 2:
			sr.sprite = A;
			break;
		case 3:
			sr.sprite = B;
			break;
		case 4:
			sr.sprite = C;
			break;
		case 5:
			sr.sprite = D;
			break;
		}
	}
}
