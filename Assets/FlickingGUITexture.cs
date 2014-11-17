using UnityEngine;
using System.Collections;

public class FlickingGUITexture : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine(flick());
	}

	void Update(){
		if(Input.GetButtonDown("Fire1")) {
			Application.LoadLevel("Path");	
		}
	}
	
	IEnumerator flick(){
		while(true){
			guiTexture.enabled = true;
			yield return new WaitForSeconds(0.8f);			
			guiTexture.enabled = false;
			yield return new WaitForSeconds(0.4f);
		}
	}
}
