using UnityEngine;
using System.Collections;

public class Obstacle2 : MonoBehaviour {
	
	public float obstaclePosition;
	private ItemGenerator2 itemGenerator;
	private Controller controller;
	// Use this for initialization
	void Start () {
		itemGenerator = GameObject.Find ("ItemsGenerator2").GetComponent<ItemGenerator2> ();
		controller = GameObject.Find ("Character2").GetComponent<Controller> ();
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
			itemGenerator.obstacleCount--;
			Destroy(gameObject);
		}
	}
	
	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "RunMan") {
			controller.SetVelocity(controller.GetVelocity() / 3);
			itemGenerator.obstacleCount--;
			if(itemGenerator.obstacleQueue.Count > 0)
			{
				itemGenerator.obstacleQueue.Dequeue();
			}
			Destroy(gameObject);
		}
	}
}
