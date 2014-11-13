using UnityEngine;
using System.Collections;

public class Obstacle : MonoBehaviour {

	public float obstaclePosition;

	private Controller controller;
	// Use this for initialization
	void Start () {
		controller = GameObject.Find ("Character1").GetComponent<Controller> ();
	}
	
	// Update is called once per frame
	void Update () {
		CheckValid ();
	}


	void CheckValid(){

		float pathPositionOfCharacter = controller.pathPosition;
		//transform.LookAt (iTween.PointOnPath(controller.controlPath, pathPositionOfCharacter - 0.001f));
		if(pathPositionOfCharacter - obstaclePosition > 0.005f){

			Destroy(gameObject);
			ItemGenerator.obstacleCount--;
		}
	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "RunMan") {
			controller.SetVelocity(controller.GetVelocity() / 3);
			ItemGenerator.obstacleCount--;
			Destroy(gameObject);
		}
	}
}
