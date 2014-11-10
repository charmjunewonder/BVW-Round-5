using UnityEngine;
using System.Collections;
using System.IO.Ports;

public class Arduino_Unity : MonoBehaviour {

	public string stringToEdit = "Serial Port";
	public SerialPort sp;

	void OnGUI(){
		
		stringToEdit = GUI.TextField (new Rect (60, 45, 100, 25), stringToEdit);
		
		if (GUI.Button (new Rect (190, 45, 100, 25), "Connect")) {
			Controller.spsp = new SerialPort (stringToEdit, 9600);
			
			if (Controller.spsp.IsOpen) {
				Controller.spsp.Close ();
				print ("Serial port is opened, so closed!");
			} else {
				Controller.spsp.Open ();
				Controller.spsp.ReadTimeout = 1;
			}

			Application.LoadLevel ("Path");
		}
		else {
			Debug.Log("WRONG PORT");
		}

		if (GUI.Button (new Rect (190, 80, 120, 25), "Anyway Play")) {

			Application.LoadLevel("Path");
				}
		
	}




//	void Update(){
//
//		
//	}

}
