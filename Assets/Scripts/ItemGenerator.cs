using UnityEngine;
using System.Collections;

public class ItemGenerator : MonoBehaviour {

	public Controller controller;
	public float[] pathOffsets;

	public float[] ItemOffset;
	public GameObject[] items;
	public GameObject[] Obstacles;
	public float[] ObstacleOffset;
	public float ObstacleOffsetVariation;

	public static int itemCount;
	public static int obstacleCount;
	private float characterOffset;
	private Transform[] controlPath;
	// Use this for initialization
	void Start () {
		characterOffset = controller.pathOffset;
		itemCount = 0;
		obstacleCount = 0;
		GenerateItemAtFirstTime();
		controlPath = controller.controlPath;
	}

	void Update()
	{
		if (itemCount < 1) {
			itemCount++;
			Invoke ("GenerateItem", 0.1f);
		}
		if (controller.characterMode > 0 && obstacleCount < 1) {
			obstacleCount++;
			Invoke("GenerateObstacle", 0.1f);
		}
	}

	private void GenerateObstacle()
	{
		int modeOfCharacter = controller.characterMode;
		if (modeOfCharacter > 3) {
			return;
		}
		float characterPosition = controller.pathPosition;
		GameObject obstacleClone = Instantiate(Obstacles[modeOfCharacter]) as GameObject;
		float offset = Random.Range (-ObstacleOffsetVariation, ObstacleOffsetVariation);
		/*itemClone.transform.position = */ModifyLookAtDirection(obstacleClone, characterPosition + ObstacleOffset[modeOfCharacter] + offset, true);
		obstacleClone.transform.Rotate (0, 180, 0);

		obstacleClone.transform.parent = transform;

		obstacleClone.GetComponent<Obstacle>().obstaclePosition = characterPosition + ObstacleOffset[modeOfCharacter] + offset;

	}

	private void GenerateItem()
	{
		int modeOfCharacter = controller.characterMode;
		if (modeOfCharacter > 3) {
			return;
		}
		float characterPosition = controller.pathPosition;
		GameObject itemClone = Instantiate(items[modeOfCharacter]) as GameObject;

		/*itemClone.transform.position = */ModifyLookAtDirection(itemClone, characterPosition + ItemOffset[modeOfCharacter], false);
		itemClone.tag = "Item";
		itemClone.transform.parent = transform;
		itemClone.transform.Rotate (90, 0, 0);
		itemClone.GetComponent<Item> ().modeOfCharacter = modeOfCharacter;
		itemClone.GetComponent<Item>().itemPosition = characterPosition + ItemOffset[modeOfCharacter];


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

	public void ModifyLookAtDirection(GameObject other, float percent, bool onGround){
		Vector3 coordinateOnPath = iTween.PointOnPath(controlPath,percent);
		Vector3 lookTarget;
		Vector3 direction;
		lookTarget = iTween.PointOnPath(controlPath,percent+0.001f);
		direction = lookTarget - coordinateOnPath;
		
		int layerOfPath = 1 << 8;
		
		Vector3 directionA = new Vector3(-direction.y, direction.x, 0);
		float minDistance = Mathf.Infinity;
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
		//other.transform.up = vectorWithMinDistance;
		//Debug.DrawRay (coordinateOnPath, -vectorWithMinDistance, Color.red, 1000);
		if (!onGround) {
			other.transform.position = positionVector + 10 * vectorWithMinDistance.normalized
				+ offsetVector.normalized * characterOffset;
		}
		else
		{
			other.transform.position = positionVector + 2.5f * vectorWithMinDistance.normalized
				+ offsetVector.normalized * characterOffset;
		}
	}
}

