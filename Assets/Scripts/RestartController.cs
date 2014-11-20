using UnityEngine;
using System.Collections;

public class RestartController : MonoBehaviour {
	public bool rightPlayer;
	public Texture[] boardTexture;
	public bool isEnded = false;
	public int restartButtonIndex = 1;
	public int quitButtonIndex = 3;
	public Font myFont;
	private bool isStartedDisplay = false;
	private int[] times;
	private string[] names;
	private int newRecordTime1, newRecordTime2;
	private string newRecordName1 = "", newRecordName2 = "";
	private int newRecordIndex1 = int.MaxValue, newRecordIndex2 =int.MaxValue;
	private int newRecordCount = 0;
	// Use this for initialization
	void Start () {
		times = new int[5];
		names = new string[5];
		//PlayerPrefs.DeleteAll();
		//StartToDisplay(90, 90);
		//StartCoroutine(refreshAuto());
	}

	void Update(){
		if(!isStartedDisplay) return;
		bool clickRestart = false;
		bool clickQuit = false;

		int defaultWidth = 1600;
		float widthRatio = Screen.width * 1.0f/ defaultWidth;
		int rightOffset = 0;
		
		if(rightPlayer) rightOffset = Screen.width/2;
		float mouseX = Input.mousePosition.x;
		float mouseY = Screen.height - Input.mousePosition.y;

		//restart button
		float width = boardTexture[restartButtonIndex].width*0.7f*widthRatio;
		float height = boardTexture[restartButtonIndex].height*0.7f*widthRatio;
		if(mouseX > rightOffset + Screen.width * 0.09f
			&& mouseX < rightOffset + Screen.width * 0.09f + width
			&& mouseY > Screen.height * 0.69f
			&& mouseY < Screen.height * 0.69f + height){
			clickRestart = true;
			restartButtonIndex = 2;
		} else {
			restartButtonIndex = 1;
		}

		width = boardTexture[quitButtonIndex].width*0.6f*widthRatio;
		height = boardTexture[quitButtonIndex].height*0.6f*widthRatio;
		if(mouseX > rightOffset + Screen.width * 0.285f
			&& mouseX < rightOffset + Screen.width * 0.285f + width
			&& mouseY > Screen.height * 0.695f
			&& mouseY < Screen.height * 0.695f + height){
			clickQuit = true;
			quitButtonIndex = 4;
		} else {
			quitButtonIndex = 3;
		}
		if(Input.GetButtonDown("Fire1")) {
			if(clickRestart){
				Debug.Log("Restart");
				deleteUnknown();
				Application.LoadLevel("Path");
			}
			if(clickQuit){
				Debug.Log("Quit");
				deleteUnknown();
				Application.LoadLevel("Credit");
			}			
		}
		if(Input.GetKeyDown(KeyCode.Return)){
			Debug.Log("fjsdkl");
			if(newRecordName1.Length > 0 && newRecordIndex1 != int.MaxValue){
				PlayerPrefs.SetInt ("ScorePlayer" + newRecordIndex1, times[newRecordIndex1]);
				PlayerPrefs.SetString ("NamePlayer" + newRecordIndex1, newRecordName1);
				names[newRecordIndex1] = newRecordName1;
				newRecordCount--;
			}
			if(newRecordName2.Length > 0 && newRecordIndex2 != int.MaxValue){
				PlayerPrefs.SetInt ("ScorePlayer" + newRecordIndex2, times[newRecordIndex2]);
				PlayerPrefs.SetString ("NamePlayer" + newRecordIndex2, newRecordName2);
				names[newRecordIndex2] = newRecordName2;
				newRecordCount--;				
			}
		}
	}

	void OnGUI() {
		if(!isStartedDisplay) return;
		int defaultWidth = 1600;
		float widthRatio = Screen.width * 1.0f/ defaultWidth;
		int rightOffset = 0;
		
		if(rightPlayer) rightOffset = Screen.width/2;
		//Debug.Log(widthRatio);

		float width = boardTexture[0].width*0.6f*widthRatio, height = boardTexture[0].height*0.6f*widthRatio;
		GUI.DrawTexture(new Rect(rightOffset + Screen.width * 0.05f, Screen.height * 0.15f, width, height), boardTexture[0]);
		width = boardTexture[restartButtonIndex].width*0.7f*widthRatio;
		height = boardTexture[restartButtonIndex].height*0.7f*widthRatio;
		GUI.DrawTexture(new Rect(rightOffset + Screen.width * 0.09f, Screen.height * 0.69f, width, height), boardTexture[restartButtonIndex]);
		width = boardTexture[quitButtonIndex].width*0.6f*widthRatio;
		height = boardTexture[quitButtonIndex].height*0.6f*widthRatio;
		GUI.DrawTexture(new Rect(rightOffset + Screen.width * 0.285f, Screen.height * 0.695f, width, height), boardTexture[quitButtonIndex]);

		// Content
		GUIStyle style = new GUIStyle();
		style.font = myFont;
		style.fontSize = Mathf.FloorToInt(30*widthRatio);
		style.normal.textColor = Color.white;
		for(int i = 0; i < 5; ++i){
			if(names[i] == "Unknown"){
				if(newRecordIndex1 == i){
					newRecordName1 = GUI.TextField(new Rect(rightOffset + 150*widthRatio, (274+64.8f*i)*widthRatio, 250*widthRatio, 28*widthRatio), newRecordName1, 25);
				} else if(newRecordIndex2 == i){
					newRecordName2 = GUI.TextField(new Rect(rightOffset + 150*widthRatio, (274+64.8f*i)*widthRatio, 250*widthRatio, 28*widthRatio), newRecordName2, 25);
				}
			} else if(names[i] == "NotExisted"){
				continue;
			} else {
				GUI.Label(new Rect(rightOffset + 150*widthRatio, (270+66*i)*widthRatio, 300, 50), names[i], style);

			}
			int gameTime = times[i];
			int minutes = gameTime / 60;
			int seconds = gameTime - minutes * 60;
			int second1 = seconds / 10;
			int second2 = seconds - second1 * 10;
			int minute1 = minutes / 10;
			int minute2 = minutes - minute1 * 10;
			GUI.Label(new Rect(rightOffset + 530*widthRatio, (270+66*i)*widthRatio, 300, 50), 
				minute1 + "" + minute2 + " : " + second1 + "" + second2, style);
		}
		//Debug.Log(names[newRecordIndex1] + "|" + names[newRecordIndex2] + "|" + times[newRecordIndex1] + "|" + times[newRecordIndex2]);
    }

    public void StartToDisplay(int playertime1, int playertime2){

    	int time1, time2;
    	if(playertime1 < playertime2){
    		time1 = playertime1;
    		time2 = playertime2;
    	} else{
    		time1 = playertime2;
    		time2 = playertime1;
    	}
    	int count = 0;
		for(int i = 0; i < 5; i++){
			int leaderTime = PlayerPrefs.GetInt ("ScorePlayer" + i, int.MaxValue);
			names[i] = "NotExisted";
			times[i] = leaderTime;
			if(leaderTime != int.MaxValue){
				names[i] = PlayerPrefs.GetString ("NamePlayer" + i);
			}
		}
		newRecordIndex1 = PlayerPrefs.GetInt ("newRecordIndex1", int.MaxValue);
		if(newRecordIndex1 == int.MaxValue){
		for(int i = 0; i < 5; i++){
			if(time1 < times[i]){
				for(int j = 4; j > i; j--){
					times[j] = times[j-1];
					names[j] = names[j-1];
				}
				times[i] = time1;
				names[i] = "Unknown";
				newRecordIndex1 = i;
				PlayerPrefs.SetInt ("ScorePlayer" + newRecordIndex1, time1);
				PlayerPrefs.SetString ("NamePlayer" + newRecordIndex1, "Unknown");
				PlayerPrefs.SetInt ("newRecordIndex1", i);
				newRecordCount++;
				break;
			}
		}
		}
		newRecordIndex2 = PlayerPrefs.GetInt ("newRecordIndex2", int.MaxValue);
		if(newRecordIndex2 == int.MaxValue){
		for(int i = 0; i < 5; i++){
			if(time2 < times[i]){
				for(int j = 4; j > i; j--){
					times[j] = times[j-1];
					names[j] = names[j-1];
				}
				times[i] = time2;
				names[i] = "Unknown";
				newRecordIndex2 = i;
				PlayerPrefs.SetInt ("ScorePlayer" + newRecordIndex2, time2);
				PlayerPrefs.SetString ("NamePlayer" + newRecordIndex2, "Unknown");
				PlayerPrefs.SetInt ("newRecordIndex2", i);
				
				newRecordCount++;
				break;
			}
			
		}
		}
//    	for(int i = 0; i < 5; i++){
//    		int leaderTime = PlayerPrefs.GetInt ("ScorePlayer" + i, int.MaxValue);
//    		names[i] = "NotExisted";
//    		if(leaderTime <= time1 || count > 1){
//    			if(leaderTime == int.MaxValue) continue;
//    			times[i] = leaderTime;
//    			names[i] = PlayerPrefs.GetString ("NamePlayer" + i);
//    		} else {
//    			times[i] = time1;
//    			names[i] = "Unknown";
//    			time1 = time2;
//    			if(count == 0){
//    				newRecordIndex1 = i;
//    			} else{
//    				newRecordIndex2 = i;
//    			}
//    			count++;
//    		}
//    	}
    	isStartedDisplay = true;
    	StartCoroutine(refreshAuto());
    }

    public void refresh(){
    	for(int i = 0; i < 5; i++){
			int leaderTime = PlayerPrefs.GetInt ("ScorePlayer" + i, int.MaxValue);
			names[i] = "NotExisted";
			times[i] = leaderTime;
			if(leaderTime != int.MaxValue){
				names[i] = PlayerPrefs.GetString ("NamePlayer" + i);
			}
		}
		newRecordIndex1 = PlayerPrefs.GetInt ("newRecordIndex1");
		newRecordIndex2 = PlayerPrefs.GetInt ("newRecordIndex2");
		
    }

    IEnumerator refreshAuto(){
    	while(true){
    		refresh();
    		yield return new WaitForSeconds(0.2f);
    	}
    }
    
	public void deleteUnknown(){
		for(int i = 0; i < 5; i++){
			int leaderTime = PlayerPrefs.GetInt ("ScorePlayer" + i, int.MaxValue);
			names[i] = "NotExisted";
			times[i] = leaderTime;
			if(leaderTime != int.MaxValue){
				names[i] = PlayerPrefs.GetString ("NamePlayer" + i);
				if(names[i] == "Unknown"){
					PlayerPrefs.SetInt ("ScorePlayer" + i, int.MaxValue);
					PlayerPrefs.SetString ("NamePlayer" + i, "NotExisted");
					names[i] = "NotExisted";
					times[i] = int.MaxValue;
				}
			}
		}
		PlayerPrefs.SetInt ("newRecordIndex1", int.MaxValue);
		PlayerPrefs.SetInt ("newRecordIndex2", int.MaxValue);
		Debug.Log("delete");
    }
}
