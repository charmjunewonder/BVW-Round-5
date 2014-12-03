using UnityEngine;
using System.Collections;

public class RestartController : MonoBehaviour {
	public bool rightPlayer;
	public Texture[] boardTexture;
	public bool isEnded = false;
	public int restartButtonIndex = 1;
	public int quitButtonIndex = 3;
	public Font myFont;
	public bool isWinning;
	private bool isStartedDisplay = false;
	private int[] times;
	private string[] names;
	private int newRecordTime1;
	private int newRecordIndex1 = int.MaxValue;
	private string newRecordName1 = "";
	// Use this for initialization
	void Start () {
		times = new int[5];
		names = new string[5];
		//PlayerPrefs.DeleteAll();
		//if(!isWinning) return;
		//StartToDisplay(60);
		//StartCoroutine(refreshAuto());
		// if(isWinning)
		// 	Invoke("deleteUnknown", 5);
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
				Debug.Log("Record");
				PlayerPrefs.SetInt ("ScorePlayer" + newRecordIndex1, times[newRecordIndex1]);
				PlayerPrefs.SetString ("NamePlayer" + newRecordIndex1, newRecordName1);
				names[newRecordIndex1] = newRecordName1;
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
				if(!isWinning) continue;
				newRecordName1 = GUI.TextField(new Rect(rightOffset + 150*widthRatio, (274+64.8f*i)*widthRatio, 250*widthRatio, 28*widthRatio), newRecordName1, 25);
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

    public void StartToDisplay(int playertime1){

    	int count = 0;
    	//get all the previous scores
		for(int i = 0; i < 5; i++){
			int leaderTime = PlayerPrefs.GetInt ("ScorePlayer" + i, int.MaxValue);
			string leaderName = PlayerPrefs.GetString ("NamePlayer" + i, "NotExisted");
			names[i] = "NotExisted";
			times[i] = int.MaxValue;
			if(leaderTime != int.MaxValue && leaderName != "NotExisted" && leaderName != "Unknown"){
				names[count] = leaderName;
				times[count] = leaderTime;
				count++;
			}
		}
		if(isWinning){
			for(int i = 0; i < 5; i++){
				if(playertime1 < times[i]){
					for(int j = 4; j > i; j--){
						times[j] = times[j-1];
						names[j] = names[j-1];
						PlayerPrefs.SetInt ("ScorePlayer" + j, times[j]);
						PlayerPrefs.SetString ("NamePlayer" + j, names[j]);
					}
					times[i] = playertime1;
					names[i] = "Unknown";
					newRecordIndex1 = i;
					PlayerPrefs.SetInt ("ScorePlayer" + i, playertime1);
					PlayerPrefs.SetString ("NamePlayer" + i, "Unknown");
					break;
				}
			}
		}
    	isStartedDisplay = true;
    	if(!isWinning)
    		StartCoroutine(refreshAuto());
    }

    public void refresh(){
		int count = 0;
    	for(int i = 0; i < 5; i++){
			int leaderTime = PlayerPrefs.GetInt ("ScorePlayer" + i, int.MaxValue);
			string leaderName = PlayerPrefs.GetString ("NamePlayer" + i, "NotExisted");
			if(leaderTime != int.MaxValue && leaderName != "NotExisted"){
				names[count] = leaderName;
				times[count] = leaderTime;
				count++;
			} else{
				names[i] = "NotExisted";
				times[i] = int.MaxValue;
			}
		}
		
    }

    IEnumerator refreshAuto(){
    	while(true){
    		refresh();
    		yield return new WaitForSeconds(0.2f);
    	}
    }
    
	public void deleteUnknown(){
		
		for(int i = 0; i < 5; i++){
			string leaderName = PlayerPrefs.GetString ("NamePlayer" + i, "NotExisted");
			if(leaderName == "Unknown"){
				PlayerPrefs.SetInt ("ScorePlayer" + i, int.MaxValue);
				PlayerPrefs.SetString ("NamePlayer" + i, "NotExisted");
				
			}
		}
		Debug.Log("delete");
    }
}
