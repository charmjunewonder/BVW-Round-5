using UnityEngine;
using System.Collections;

public class LeaderBoard : MonoBehaviour {

	public GameObject timeNotify;
	public GameObject[] currentScore;
	public Texture[] numbersTexture = new Texture[10];

	// Use this for initialization
	void Start () {
		for(int i = 0; i < 4; ++i){
			currentScore[i].renderer.material.mainTexture = numbersTexture[0];
		}

	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnGUI(){
		GUI.DrawTexture(new Rect(Screen.width * 0.5f, Screen.height * 0.5f, 76*0.75f, 92*0.75f), numbersTexture[0]);
		GUI.DrawTexture(new Rect(Screen.width * 0.5f, Screen.height * 0.5f, 76*0.75f, 92*0.75f), numbersTexture[0]);

	}

	IEnumerator recordTime(){
		while(true){
			// gameTime = (int)Time.time - startTime;
			// int minutes = gameTime / 60;
			// int seconds = gameTime - minutes * 60;
			// int second1 = seconds / 10;
			// int second2 = seconds - second1 * 10;
			// int minute1 = minutes / 10;
			// int minute2 = minutes - minute1 * 10;
			// currentScore[0].renderer.material.mainTexture = numbersTexture[minute1];
			// currentScore[1].renderer.material.mainTexture = numbersTexture[minute2];
			// currentScore[2].renderer.material.mainTexture = numbersTexture[second1];
			// currentScore[3].renderer.material.mainTexture = numbersTexture[second2];
			// yield return new WaitForSeconds(1.0f);
		}
	}
}
