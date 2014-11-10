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
		transform.Rotate (0, 3f, 0, Space.Self);
		CheckValid ();
	}


	void CheckValid(){
		if(true){
			Debug.Log (controller.pathPosition + " " + obstaclePosition);
			float pathPositionOfCharacter = controller.pathPosition;
			if(pathPositionOfCharacter - obstaclePosition > 0.001f){
				
				Destroy(gameObject);
				ItemGenerator.obstacleCount--;
			}
		}
	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "RunMan") {

			controller.SetBouncedBackTrue();
			controller.SetVelocity(-0.00008f);
		}
	}
}
