using UnityEngine;
using System.Collections;

public class CharacterLeftFeedback : MonoBehaviour {
	public Texture[] feedBack;
	private int enabledNum = 0;

	IEnumerator showFeedback(int num){
		enabledNum++;
		renderer.material.mainTexture = feedBack[num];
		renderer.enabled = true;
		yield return new WaitForSeconds(1f);
		enabledNum--;
		if(enabledNum==0){
			renderer.enabled = false;
		}
	}

	public void showBadFeedback(){
		StartCoroutine(showFeedback(0));
	}

	public void showGoodFeedback(){
		StartCoroutine(showFeedback(1));
	}
}
