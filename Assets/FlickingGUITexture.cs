using UnityEngine;
using System.Collections;
using System.IO.Ports;

public class FlickingGUITexture : MonoBehaviour {


	private SerialPort spUnity;
	private SerialPort spUnity2;
	
	private int player1;
	private int player2;
	// Use this for initialization
	void Start () {
	
		spUnity = Controller.spsp;
		spUnity2 = Controller.spsp2;
		StartCoroutine(flick());
	}

	void Update(){
		if(Input.GetButtonDown("Fire1")) {
			Application.LoadLevel("Path");	
		}
		
		if (spUnity != null && spUnity2 !=null) {
			if (spUnity.IsOpen) {
				try {
					player1 = spUnity.ReadByte();
				} catch (System.Exception) {
					
				}
			}
			
			if (spUnity2.IsOpen) {
				try {
					player2 = spUnity2.ReadByte();
				} catch (System.Exception) {
					
				}
			}
			//DetectKeys();
		}
		
		if(player1 == 1 && player2 ==4){
	
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
