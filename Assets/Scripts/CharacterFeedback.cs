using UnityEngine;
using System.Collections;

public class CharacterFeedback : MonoBehaviour {
	public Texture[] feedBack;
	private int enabledNum = 0;
	// Use this for initialization
	void Start () {
		StartCoroutine(showFeedback(0));
	}
	
	IEnumerator showFeedback(int num){
		enabledNum++;
		renderer.material.mainTexture = feedBack[num];
		renderer.enabled = true;
		yield return new WaitForSeconds(0.6f);
		enabledNum--;
		if(enabledNum==0){
			renderer.enabled = false;
		}
	}
	public void showFeedbackNumber(int num){
		num %= 5;
		StartCoroutine(showFeedback(num));
	}


}
