using UnityEngine;
using System.Collections;

public class TimeRecorder : MonoBehaviour {

	public int startTime;
	public int gameTime;
	public GameObject timeNotify;
	public GameObject[] currentScore;
	public Texture[] numbersTexture = new Texture[10];

	// Use this for initialization
	void Start () {
		StartCoroutine (recordTime ());
		for(int i = 0; i < 4; ++i){
			currentScore[i].renderer.material.mainTexture = numbersTexture[0];
		}

	}
	
	// Update is called once per frame
	void Update () {
	}

	IEnumerator recordTime(){
		while(true){
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
