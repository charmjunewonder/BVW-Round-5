using UnityEngine;
using System.Collections;

public class BoosterTrigger : MonoBehaviour {

	private Controller controller;
	private ItemGenerator itemGenerator;

	void Start () {
		controller = GameObject.Find ("Character1").GetComponent<Controller> ();
		itemGenerator = GameObject.Find ("ItemsGenerator").GetComponent<ItemGenerator> ();
	}
	
	void Update () {
	
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "RunMan") {
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
			controller.SetOnRollerCoaster(true);
			itemGenerator.SetIsOnRollerCoaster(true);
		}
	}
}
