using UnityEngine;
using System.Collections;

public class BoosterTrigger : MonoBehaviour {


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
			itemGenerator.Clear ();
			controller.SetOnRollerCoaster(true);
		}

		if (col.name == "Character2") {
			controller2.SetOnRollerCoaster(true);
			itemGenerator2.Clear ();
		}

		GameObject.Find ("SoundManager").GetComponent<SoundManager>().PlaySoundEffect(2, false);
	}
}
