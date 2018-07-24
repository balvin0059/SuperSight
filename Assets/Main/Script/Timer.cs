using UnityEngine;
using System.Collections;

public class Timer : MonoBehaviour {
	public GUIText time_text;
	public float total_time;
	
	// Use this for initialization
	void Start () {
		total_time = 5.0f;
	}
	
	// Update is called once per frame
	void Update () {
		if (total_time <= 0) {
			total_time = 0;
			Destroy(gameObject);
		} else {
			total_time -= Time.deltaTime;	
			GetComponent<GUIText>().text = total_time.ToString("0");	
		}
	}
}