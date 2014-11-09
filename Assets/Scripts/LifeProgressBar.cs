using UnityEngine;
using System.Collections;

public class LifeProgressBar : MonoBehaviour {

	public Texture[] progressImages;
	private int currentState = 0;

	// Use this for initialization
	void Start () {
		guiTexture.texture = progressImages[currentState];
	}

	public void changeToNextState(){
		currentState++;
		guiTexture.texture = progressImages[currentState];
	}

}
