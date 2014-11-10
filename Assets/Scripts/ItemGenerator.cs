using UnityEngine;
using System.Collections;

public class ItemGenerator : MonoBehaviour {

	public Controller controller;
	public float[] pathOffsets;

	public float[] ItemOffset;
	public GameObject[] items;

	public static int itemCount;
	// Use this for initialization
	void Start () {
		itemCount = 0;
		GenerateItemAtFirstTime();
	}

	void Update()
	{
		if (itemCount < 1) {
			GenerateItem();
		}
	}


	private void GenerateItem()
	{
		int modeOfCharacter = controller.characterMode;
		float characterPosition = controller.pathPosition;
		GameObject itemClone = Instantiate(items[modeOfCharacter]) as GameObject;
		itemClone.transform.position = iTween.PointOnPath(controller.controlPath, characterPosition + ItemOffset[modeOfCharacter]);
		itemClone.tag = "Item";
		itemClone.transform.parent = transform;
		itemClone.GetComponent<Item>().itemPosition = characterPosition + ItemOffset[modeOfCharacter];
		itemCount++;

	}

	void GenerateItemAtFirstTime(){
		for(int i = 0; i < 4; i++)
		{
			float characterPosition = 0;
			GameObject itemClone = Instantiate(items[0], iTween.PointOnPath(controller.controlPath, characterPosition + (i + 2) * ItemOffset[0]), transform.rotation) as GameObject;
			//itemClone.transform.position = iTween.PointOnPath(controller.controlPath, characterPosition + (i + 1) * ItemOffset[0]);
			itemClone.tag = "Item";
			itemClone.transform.parent = transform;
			itemClone.GetComponent<Item>().itemPosition = characterPosition + (i + 2) * ItemOffset[0];
			itemCount++;
		}

	}
}

