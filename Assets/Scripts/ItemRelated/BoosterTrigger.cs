using UnityEngine;
using System.Collections;

public class BoosterTrigger : MonoBehaviour {
	public CharacterLeftFeedback leftFeedback;

	private Controller controller;
	private ItemGenerator itemGenerator;

	private Controller controller2;
	private ItemGenerator2 itemGenerator2;

	void Start () {
		controller = GameObject.Find ("Character1").GetComponent<Controller> ();
		itemGenerator = GameObject.Find ("ItemsGenerator").GetComponent<ItemGenerator> ();

		controller2 = GameObject.Find ("Character2").GetComponent<Controller> ();
		itemGenerator2 = GameObject.Find ("ItemsGenerator2").GetComponent<ItemGenerator2> ();
	}
	
	void Update () {
	
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.name == "Character1") {
			if(itemGenerator.obstacleQueue.Count > 0)
			{
				GameObject temp = itemGenerator.obstacleQueue.Dequeue() as GameObject;
				Destroy(temp);
			}

			if(itemGenerator.itemQueue.Count > 0)
			{
				GameObject temp = itemGenerator.itemQueue.Dequeue() as GameObject;
				Destroy(temp);
			}
			leftFeedback.showGoodFeedback();
			controller.SetOnRollerCoaster(true);
			itemGenerator.SetIsOnRollerCoaster(true);
		}

		if (col.name == "Character2") {
			if(itemGenerator2.obstacleQueue.Count > 0)
			{
				GameObject temp = itemGenerator2.obstacleQueue.Dequeue() as GameObject;
				Destroy(temp);
			}
			
			if(itemGenerator2.itemQueue.Count > 0)
			{
				GameObject temp = itemGenerator2.itemQueue.Dequeue() as GameObject;
				Destroy(temp);
			}
			leftFeedback.showGoodFeedback();
			controller2.SetOnRollerCoaster(true);
			itemGenerator2.SetIsOnRollerCoaster(true);
		}
	}
}
