using UnityEngine;
using System.Collections;

public class AIItemGenerator : MonoBehaviour {

	public AICharacter controller;
	public float[] pathOffsets;

	public float[] ItemOffset;
	public GameObject[] items;

	public static int itemCount;
	private float characterOffset;
	private Transform[] controlPath;
	// Use this for initialization
	void Start () {
		characterOffset = controller.pathOffset;
		itemCount = 0;
		controlPath = controller.controlPath;
		
		GenerateItemAtFirstTime();
	}

	void Update()
	{
		if (itemCount < 1) {
			itemCount++;
			Invoke ("GenerateItem", 0.5f);
		}
	}


	private void GenerateItem()
	{
		int modeOfCharacter = controller.characterMode;
		float characterPosition = controller.pathPosition;
		GameObject itemClone = Instantiate(items[modeOfCharacter]) as GameObject;
		/*itemClone.transform.position = */ModifyLookAtDirection(itemClone, characterPosition + ItemOffset[modeOfCharacter]);
		itemClone.tag = "Item";
		itemClone.transform.parent = transform;
		itemClone.GetComponent<AIItem>().itemPosition = characterPosition + ItemOffset[modeOfCharacter];


	}

	void GenerateItemAtFirstTime(){
		int modeOfCharacter = controller.characterMode;
		
		for(int i = 0; i < 4; i++)
		{
			float characterPosition = 0;
			GameObject itemClone = Instantiate(items[0], iTween.PointOnPath(controller.controlPath, characterPosition + (i + 2) * ItemOffset[0]), transform.rotation) as GameObject;
			ModifyLookAtDirectionForMilk(itemClone, characterPosition + + (i + 2) * ItemOffset[0]);
			//itemClone.transform.position = iTween.PointOnPath(controller.controlPath, characterPosition + (i + 1) * ItemOffset[0]);
			itemClone.tag = "Item";
			itemClone.transform.parent = transform;
			itemClone.GetComponent<AIItem>().itemPosition = characterPosition + (i + 2) * ItemOffset[0];
			itemCount++;
		}

	}

	public void ModifyLookAtDirection(GameObject other, float percent){
		Vector3 coordinateOnPath = iTween.PointOnPath(controlPath,percent);
		Vector3 lookTarget;
		Vector3 direction;
		lookTarget = iTween.PointOnPath(controlPath,percent+0.001f);
		direction = lookTarget - coordinateOnPath;
		
		int layerOfPath = 1 << 8;
		
		Vector3 directionA = new Vector3(-direction.y, direction.x, 0);
		float minDistance = Mathf.Infinity;
		//normal of the floor
		Vector3 vectorWithMinDistance = directionA;
		Vector3 positionVector = other.transform.position;
		
		RaycastHit hit;
		for(int i = 0; i < 8; i++){
			directionA = Quaternion.AngleAxis(-45, direction) * directionA;
			if (Physics.Raycast(coordinateOnPath,-directionA,out hit, 10.0f, layerOfPath)){
				
				if(hit.distance < minDistance){
					minDistance = hit.distance;
					vectorWithMinDistance = directionA;
					positionVector = hit.point;
					
				}
			}
		}
		for(int i = 0; i < 9; i++){
			directionA = Quaternion.AngleAxis(-45+10*i, direction) * vectorWithMinDistance;
			
			if (Physics.Raycast(coordinateOnPath,-directionA,out hit, 10.0f, layerOfPath)){
				if(hit.distance < minDistance){
					minDistance = hit.distance;
					vectorWithMinDistance = directionA;
					positionVector = hit.point;
				}
			}
		}
		Vector3 offsetVector = Vector3.Cross(vectorWithMinDistance, direction);
		other.transform.LookAt(other.transform.position + direction.normalized, vectorWithMinDistance);
		other.transform.position = positionVector + 10 * vectorWithMinDistance.normalized
			 + offsetVector.normalized * characterOffset;
	}
	
	public void ModifyLookAtDirectionForMilk(GameObject other, float percent){
		Vector3 coordinateOnPath = iTween.PointOnPath(controlPath,percent);
		Vector3 lookTarget;
		Vector3 direction;
		lookTarget = iTween.PointOnPath(controlPath,percent+0.001f);
		direction = lookTarget - coordinateOnPath;
		
		int layerOfPath = 1 << 8;
		
		Vector3 directionA = new Vector3(-direction.y, direction.x, 0);
		float minDistance = Mathf.Infinity;
		//normal of the floor
		Vector3 vectorWithMinDistance = directionA;
		Vector3 positionVector = other.transform.position;
		
		RaycastHit hit;
		for(int i = 0; i < 8; i++){
			directionA = Quaternion.AngleAxis(-45, direction) * directionA;
			if (Physics.Raycast(coordinateOnPath,-directionA,out hit, 10.0f, layerOfPath)){
				
				if(hit.distance < minDistance){
					minDistance = hit.distance;
					vectorWithMinDistance = directionA;
					positionVector = hit.point;
					
				}
			}
		}
		for(int i = 0; i < 9; i++){
			directionA = Quaternion.AngleAxis(-45+10*i, direction) * vectorWithMinDistance;
			
			if (Physics.Raycast(coordinateOnPath,-directionA,out hit, 10.0f, layerOfPath)){
				if(hit.distance < minDistance){
					minDistance = hit.distance;
					vectorWithMinDistance = directionA;
					positionVector = hit.point;
				}
			}
		}
		Vector3 offsetVector = Vector3.Cross(vectorWithMinDistance, direction);
		other.transform.LookAt(other.transform.position + direction.normalized, vectorWithMinDistance);
		other.transform.position = coordinateOnPath + offsetVector.normalized * characterOffset;
	}
}
