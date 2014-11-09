using UnityEngine;
using System.Collections;

public class ItemGenerator : MonoBehaviour {

	public Controller controller;
	public GameObject item;

	// Use this for initialization
	void Start () {
		GenerateItemAtFirstTime();
	}

	void GenerateItemAtFirstTime(){
		int modeOfCharacter = controller.characterMode;
		float emptyPercent = 0.01f;
		float increment = (1 - emptyPercent) / 30;
		float previousPercent = emptyPercent;
		for(int i = 0; i < 12; i++){
			GameObject itemClone = Instantiate(item) as GameObject;
			itemClone.transform.parent = transform;
			itemClone.SetActive(true);
			itemClone.GetComponent<Item>().models[modeOfCharacter].SetActive(true);
			itemClone.tag = "Item";
			float percent = Random.Range(previousPercent+increment, previousPercent+increment*2);
			previousPercent = previousPercent+increment*2;
			itemClone.transform.position = controller.GetPositionWithPercent(percent);
			controller.ModifyLookAtDirection(itemClone, percent);
		}
	}
}
