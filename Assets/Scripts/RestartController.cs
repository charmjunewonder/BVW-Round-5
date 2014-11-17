using UnityEngine;
using System.Collections;

public class RestartController : MonoBehaviour {
	public bool rightPlayer;
	public Texture[] boardTexture;
	public bool isEnded = false;
	public int restartButtonIndex = 1;
	public int quitButtonIndex = 3;
	public Font myFont;
	// Use this for initialization
	void Start () {
	
	}

	void Update(){
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
		if (Input.GetButtonDown("Fire1")) {
			if(clickRestart){
				Debug.Log("Restart");
			}
			if(clickQuit){
				Debug.Log("Quit");
			}			
		}
	}

	void OnGUI() {
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
		width = 150*widthRatio;
		height = 150*widthRatio;

		for(int i = 0; i < 5; ++i){
			GUI.Label(new Rect(rightOffset + 150*widthRatio, (270+66*i)*widthRatio, 300, 50), "Elizabeth", style);
			GUI.Label(new Rect(rightOffset + 530*widthRatio, (270+66*i)*widthRatio, 300, 50), "01 : 30", style);
		}
    }
}
