using UnityEngine;
using System.Collections;

public class ItemGenerator : MonoBehaviour {

	public Controller controller;
	public GameObject[] items;

	// Use this for initialization
	void Start () {
		StartCoroutine(GenerateItem());
	}

	IEnumerator GenerateItem(){
		while(true){
			int modeOfCharacter = controller.characterMode;
			GameObject item = Instantiate(items[modeOfCharacter]) as GameObject;
			item.transform.parent = transform;
			item.SetActive(true);
			item.tag = controller.tagsForItems[modeOfCharacter];
			item.transform.position = controller.GetPositionAheadWithOffset(0.001f);
			
			yield return new WaitForSeconds(5.0f);
		}
	}
}
