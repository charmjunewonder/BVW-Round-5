using UnityEngine;
using System.Collections;

public class Obstacle : MonoBehaviour {

	public float obstaclePosition;
	private ItemGenerator itemGenerator;
	private Controller controller;
	// Use this for initialization
	void Start () {
		itemGenerator = GameObject.Find ("ItemsGenerator").GetComponent<ItemGenerator> ();
		controller = GameObject.Find ("Character1").GetComponent<Controller> ();
	}
	
	// Update is called once per frame
	void Update () {
		CheckValid ();
	}


	void CheckValid(){

		float pathPositionOfCharacter = controller.pathPosition;
		if(pathPositionOfCharacter - obstaclePosition > 0.005f)
		{
			if(itemGenerator.obstacleQueue.Count > 0)
			{
				itemGenerator.obstacleQueue.Dequeue();
			}
			ItemGenerator.obstacleCount--;
			Destroy(gameObject);
		}
	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "RunMan") {
			controller.SetVelocity(controller.GetVelocity() / 3);
			ItemGenerator.obstacleCount--;
			if(itemGenerator.obstacleQueue.Count > 0)
			{
				itemGenerator.obstacleQueue.Dequeue();
			}
			Destroy(gameObject);
		}
	}
}
