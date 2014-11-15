using UnityEngine;
using System.Collections;

public class BoosterExit : MonoBehaviour {

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
			controller.SetOnRollerCoaster(false);
			itemGenerator.SetIsOnRollerCoaster(false);
			ItemGenerator.itemCount = 0;
			ItemGenerator.obstacleCount = 0;
		}
	}
}
