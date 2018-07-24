using UnityEngine;
using System.Collections;
	
public class SightContrl : MonoBehaviour {
////////////////////////////////	SETING	/////////////////////////////////////
	SpriteRenderer sr;
	public Sprite up;
	public Sprite down;
	public Sprite left;
	public Sprite right;
	public Sprite leftup;
	public Sprite rightup;
	public Sprite leftdown;
	public Sprite rightdown;
	public Sprite B_up;
	public Sprite B_down;
	public Sprite B_left;
	public Sprite B_right;
	public Sprite B_leftup;
	public Sprite B_rightup;
	public Sprite B_leftdown;
	public Sprite B_rightdown;
	public Sprite None;
	//set value
	int answer = 0;
	int ShakeWay = 0;
	int Rand_One = 0;
	int Rand_Two = 0;
	//set time
	private float nextTime = 0.0f;
	float TimeRate = 0.0f;
	float ShowScoreTime = 1.5f;
	float TimeDelta = 0.0f;
	float TimeDeltaShow = 0.0f;
	float GamingTime = 60.0f;
	//open check shaking and 4way
	bool OpenCheck = false;
	bool DisplayScore = false;
	bool GameStart = false;
	bool GameOver = false;
	//check cleaning
	float Perfect = 0.0f;
	float Great = 0.0f;
	float Good = 0.0f;
	float Bad = 0.0f;
	float Miss = 0.0f;
	float Sun = 0.0f;
	//
	float Cle = 0.0f;
	float Cle2 = 0.0f;
	float Cle3 = 0.0f;
	float Cle4 = 0.0f;
	float Cle5 = 0.0f;
	//
	float Scale_X = 0.12f;
	float Scale_Y = 0.12f;
	float Scale_Xdelta = 0.0f;
	float Scale_Ydelta = 0.0f;
	float Scale_Z = 0.0f;
	//
	int Score = 0;
	//
	public float minSwipeDistY = 200.0f;	
	public float minSwipeDistX = 100.0f;	
	private Vector2 startPos;
	public int Way = 0;
////////////////////////////////	SETING	/////////////////////////////////////


	// Use this for initialization
	void Start () {
		Random.seed = System.Guid.NewGuid().GetHashCode();
		if (LevelChose.LevelChos <= 1) {
			gameObject.transform.position = new Vector3(0.03f,1.0f);
			PlayBand.On4WayTriggerEvent += DisplayWay;
			sr = GetComponent<SpriteRenderer> ();
			TimeRate = 6.0f;
			Invoke ("ChangeSprite", 5.0f);
		} else if (LevelChose.LevelChos == 2) {
			gameObject.transform.position = new Vector2(0.03f,1.0f);
			PlayBand.On8WayTriggerEvent += DisplayWay;
			sr = GetComponent<SpriteRenderer> ();
			TimeRate = 5.0f;
			Invoke ("ChangeSprite", 5.0f);
		} else if (LevelChose.LevelChos == 3) {
			PlayBand.On8WayTriggerEvent += DisplayWay;
			sr = GetComponent<SpriteRenderer> ();
			TimeRate = 4.0f;
			Invoke ("ChangeSprite", 5.0f);
		} else if (LevelChose.LevelChos == 4) {
			PlayBand.On8WayTriggerEvent += DisplayWay;
			sr = GetComponent<SpriteRenderer> ();
			TimeRate = 4.0f;
			Invoke ("ChangeSprite", 5.0f);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (LevelChose.LevelChos == 2) {
			float nValue = 1.5f * Mathf.Sin(Time.time)*1.5f * Mathf.Cos(Time.time);
			gameObject.transform.position = new Vector2(nValue,1.0f);
		}
		if (LevelChose.LevelChos == 3) {
			float nValue = 2.5f * Mathf.Sin(Time.time);
			float wValue = Mathf.PingPong(Time.time,1.0f);
			gameObject.transform.position = new Vector2(nValue,wValue);
		}
		if (LevelChose.LevelChos == 4) {
			float Ran_A = Random.Range(-1.0f,1.0f);
			float Ran_B = Random.Range(-1.0f,1.0f);
			float nValue = 2.5f * Mathf.Sin(Time.time)*2.5f * Mathf.Cos(Time.time);
			float wValue = 2.5f * Mathf.Sin(Time.time)*2.5f * Mathf.Cos(Time.time);
			gameObject.transform.position = new Vector2(Ran_A*nValue,Ran_B*wValue);
			
		}
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.Quit(); 
		}
		///check game per time 
		if (GameOver != true) {
			if (GameStart) {
				if (Time.time >= nextTime) {
					OpenCheck = false;
					GameStart = false;
					CheckAnswer ();
				}
			}
		}
		///show score time
		if (GameOver != true) {
			if (DisplayScore) {
				if (Time.time >= TimeDeltaShow) {
					Invoke ("ChangeSprite", 0.0f);
					ScoreShowerContrl.ShowNum = 0;
					DisplayScore = false;
				}
			}
		}
		///check timeout
		if (GameOver != true){
			if (Time.time >= GamingTime) {
						OpenCheck = false;
						GameStart = false;
						ScoreShowerContrl.ShowNum = 0;
						DisplayScore = false;
						GameOver = true;
						GameClearing();
			}
		}
	}

	// 4way
	void DisplayWay(PlayBandDirection dir,PlayBandData data){
		int ptr=(int)dir;
		if (OpenCheck) {
			if(Time.time < nextTime){
			switch (ptr) {
			case 1:
				ShakeWay = 1;
				break;
			case 2:
				ShakeWay = 2;
				break;
			case 3:
				ShakeWay = 3;
				break;
			case 4:
				ShakeWay = 4;
				break;			
			case 5:
				ShakeWay = 5;
				break;
			case 6:
					ShakeWay = 6;
					break;
			case 7:
					ShakeWay = 7;
					break;
			case 8:
					ShakeWay = 8;
					break;		
			}
				OpenCheck = false;
				CheckAnswer();
			}
		}
	}
	//
	void OnSwipe(){//滑動
		if (OpenCheck) {
			Touch touch = Input.touches [0];
			switch (touch.phase) {				
			case TouchPhase.Began:				
				startPos = touch.position;				
				break;
			case TouchPhase.Ended:				
				float swipeDistVertical = (new Vector3 (0, touch.position.y, 0) - new Vector3 (0, startPos.y, 0)).magnitude;				
				if (swipeDistVertical > minSwipeDistY) {					
					float swipeValue = Mathf.Sign (touch.position.y - startPos.y);					
					if (swipeValue > 0) {
						ShakeWay = 0;
					}//up swipe
			else if (swipeValue < 0) {
						ShakeWay = 1;
					}//down swipe
				}				
				float swipeDistHorizontal = (new Vector3 (touch.position.x, 0, 0) - new Vector3 (startPos.x, 0, 0)).magnitude;				
				if (swipeDistHorizontal > minSwipeDistX) {
					float swipeValue = Mathf.Sign (touch.position.x - startPos.x);
					if (swipeValue > 0) {
						ShakeWay = 3;
					}//right swipe
			else if (swipeValue < 0) {
						ShakeWay = 2;
					}//left swipe	
				}
				OpenCheck = false;
				CheckAnswer ();
				break;
			}
		}

}

	// gaming for change sprite
	void ChangeSprite(){
		if(LevelChose.LevelChos == 1){
			GameStart = true;
			//random change
			Rand_One = Random.Range (0, 4) + 1;
			// set sprite and answer
			switch (Rand_One) {
			case 1:
				sr.sprite = up;
				answer = 1;
				break;
			case 2:
				sr.sprite = down;
				answer = 2;
				break;
			case 3:
				sr.sprite = left;
				answer = 3;
				break;
			case 4:
				sr.sprite = right;
				answer = 4;
				break;
			}
			// open checking and time set
			nextTime = Time.time + TimeRate;
			OpenCheck = true;
		}else if(LevelChose.LevelChos == 2){
			GameStart = true;
			//random change
			Rand_One = Random.Range (0, 8) + 1;
			// set sprite and answer
			switch (Rand_One) {
			case 1:
				sr.sprite = up;
				answer = 1;
				break;
			case 2:
				sr.sprite = down;
				answer = 2;
				break;
			case 3:
				sr.sprite = left;
				answer = 3;
				break;
			case 4:
				sr.sprite = right;
				answer = 4;
				break;
			case 5:
				sr.sprite = leftup;
				answer = 5;
				break;
			case 6:
				sr.sprite = rightup;
				answer = 6;
				break;
			case 7:
				sr.sprite = leftdown;
				answer = 7;
				break;
			case 8:
				sr.sprite = rightdown;
				answer = 8;
				break;
			}
			// open checking and time set
			nextTime = Time.time + TimeRate;
			OpenCheck = true;
		}else if(LevelChose.LevelChos == 3){
			GameStart = true;
			//random change
			Rand_One = Random.Range (0, 8) + 1;
			Rand_Two = Random.Range (0, 10) + 1;
			// set sprite and answer
			if(Rand_Two <= 4 ){
				switch (Rand_One) {
				case 1:
					sr.sprite = B_up;
					answer = 2;
					break;
				case 2:
					sr.sprite = B_down;
					answer = 1;
					break;
				case 3:
					sr.sprite = B_left;
					answer = 4;
					break;
				case 4:
					sr.sprite = B_right;
					answer = 3;
					break;
				case 5:
					sr.sprite = B_leftup;
					answer = 8;
					break;
				case 6:
					sr.sprite = B_rightup;
					answer = 7;
					break;
				case 7:
					sr.sprite = B_leftdown;
					answer = 6;
					break;
				case 8:
					sr.sprite = B_rightdown;
					answer = 5;
					break;
				}
			}else{
				switch (Rand_One) {
				case 1:
					sr.sprite = up;
					answer = 1;
					break;
				case 2:
					sr.sprite = down;
					answer = 2;
					break;
				case 3:
					sr.sprite = left;
					answer = 3;
					break;
				case 4:
					sr.sprite = right;
					answer = 4;
					break;
				case 5:
					sr.sprite = leftup;
					answer = 5;
					break;
				case 6:
					sr.sprite = rightup;
					answer = 6;
					break;
				case 7:
					sr.sprite = leftdown;
					answer = 7;
					break;
				case 8:
					sr.sprite = rightdown;
					answer = 8;
					break;
				}
			}

			// open checking and time set
			nextTime = Time.time + TimeRate;
			OpenCheck = true;
		}else if(LevelChose.LevelChos == 4){
			GameStart = true;
			//random change
			Rand_One = Random.Range (0, 8) + 1;
			Rand_Two = Random.Range (0, 10) + 1;
			// set sprite and answer
			if(Rand_Two <= 5 ){
				switch (Rand_One) {
				case 1:
					sr.sprite = B_up;
					answer = 2;
					break;
				case 2:
					sr.sprite = B_down;
					answer = 1;
					break;
				case 3:
					sr.sprite = B_left;
					answer = 4;
					break;
				case 4:
					sr.sprite = B_right;
					answer = 3;
					break;
				case 5:
					sr.sprite = B_leftup;
					answer = 8;
					break;
				case 6:
					sr.sprite = B_rightup;
					answer = 7;
					break;
				case 7:
					sr.sprite = B_leftdown;
					answer = 6;
					break;
				case 8:
					sr.sprite = B_rightdown;
					answer = 5;
					break;
				}
			}else{
				switch (Rand_One) {
				case 1:
					sr.sprite = up;
					answer = 1;
					break;
				case 2:
					sr.sprite = down;
					answer = 2;
					break;
				case 3:
					sr.sprite = left;
					answer = 3;
					break;
				case 4:
					sr.sprite = right;
					answer = 4;
					break;
				case 5:
					sr.sprite = leftup;
					answer = 5;
					break;
				case 6:
					sr.sprite = rightup;
					answer = 6;
					break;
				case 7:
					sr.sprite = leftdown;
					answer = 7;
					break;
				case 8:
					sr.sprite = rightdown;
					answer = 8;
					break;
				}
			}
			// open checking and time set
			nextTime = Time.time + TimeRate;
			OpenCheck = true;
		}

	}
	// end of shake to check answer
	void CheckAnswer(){
		TimeDelta = nextTime-Time.time;
		sr.sprite = None;

		Sun += 1.0f;
		if (ShakeWay == answer) {
			if(TimeDelta<6.0f && TimeDelta > 0.0f){
				ShowScore(1);
			}
			ShowScore(1);
			Scale_Xdelta += Scale_X;
			if(Scale_Xdelta < 0.9f)
			transform.localScale -= new Vector3(Scale_X,Scale_Y,0.0f);

		} else {
			Miss += 1.0f;
			CleanFather.Mn += 1;
			ShowScore(2);

			Scale_Xdelta -= Scale_X;
			if(Scale_Xdelta > -0.5f)
			transform.localScale += new Vector3(Scale_X,Scale_Y,0.0f);

		}
		ShakeWay = 0;
	}
	//show score
	void ShowScore(int a){
		TimeDeltaShow = Time.time + ShowScoreTime;
		switch (a) {
		case 1:
			ScoreShowerContrl.ShowNum = 1;
			break;
		case 2:
			ScoreShowerContrl.ShowNum = 2;
			break;
		}
		DisplayScore = true;
	}
	// cleaning
	void GameClearing(){
		float hitrate = 0.0f;
		sr.sprite = None;
		ScoreShowerContrl.ShowNum = 0;
		CleanFather.GameEndToClean = true;
		hitrate = ((Perfect + Great + Good + Bad) / Sun) * 100;
		CleanFather.HitRate = hitrate;
		//
		if (Perfect > 0) {
			Cle = (Perfect / Sun) * 100;
		}
		if (Great > 0) {
			Cle2 = (Great / Sun) * 100;
		}
		if (Good > 0) {
			Cle3 = (Good / Sun) * 100;
		}
		if (Bad > 0) {
			Cle4 = (Bad / Sun) * 100;
		}
		if (Miss > 0) {
			Cle5 = (Miss / Sun) * 100;
		}
		//////////////////////////
		if (Cle > 75.0f && Cle <= 100.0f && Cle5+Cle4 < 20.0f) {
			DisplayClean.DisplayC = 1;
		} else if (Cle > 50.0f && Cle <= 75.0f && Cle2 > 15.0f && Cle5+Cle4 < 25.0f) {
			DisplayClean.DisplayC = 2;
		} else if (Cle > 25.0f && Cle <= 50.0f && Cle2 > 30.0f && Cle5+Cle4 < 30.0f) {
			DisplayClean.DisplayC = 3;
		} else if (Cle <= 25.0f && Cle2 > 15.0f && Cle3 > 15.0f && Cle5+Cle4 <= 50.0f) {
			DisplayClean.DisplayC = 4;
		} else {
			DisplayClean.DisplayC = 5;
		}
		///RESET
		answer = 0;
		ShakeWay = 0;
		Rand_One = 0;
		Rand_Two = 0;
		//set time
		nextTime = 0.0f;
		TimeRate = 0.0f;
		ShowScoreTime = 1.5f;
		TimeDelta = 0.0f;
		TimeDeltaShow = 0.0f;
		GamingTime = 60.0f;
		//open check shaking and 4way
		OpenCheck = false;
		DisplayScore = false;
		GameStart = false;
		GameOver = false;
		//check cleaning
		Perfect = 0.0f;
		Great = 0.0f;
		Good = 0.0f;
		Bad = 0.0f;
		Miss = 0.0f;
		Sun = 0.0f;
		//
		Cle = 0.0f;
		Cle2 = 0.0f;
		Cle3 = 0.0f;
		Cle4 = 0.0f;
		Cle5 = 0.0f;
		//
		Scale_X = 0.12f;
		Scale_Y = 0.12f;
		Scale_Xdelta = 0.0f;
		Scale_Ydelta = 0.0f;
		Scale_Z = 0.0f;
		//
		Score = 0;


	}
}
