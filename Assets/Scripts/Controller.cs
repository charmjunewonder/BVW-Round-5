using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour {
	public Transform[] controlPath;
	public Transform character;
	
	private float pathPosition=0;
	private RaycastHit hit;
	private float speed = 0.12f;
	private float rayLength = 100;
	private Vector3 floorPosition;	
	private float lookAheadAmount = .01f;
	private float ySpeed=0;
	private float gravity=.5f;
	private float jumpForce=.15f;
	private uint jumpState=0; //0=grounded 1=jumping
	private Vector3 previousNormal;
	void OnDrawGizmos(){
		iTween.DrawPath(controlPath,Color.blue);	
	}	
	
	
	void Start(){
		previousNormal = Vector3.up;
		//plop the character pieces in the "Ignore Raycast" layer so we don't have false raycast data:	
		foreach (Transform child in character) {
			child.gameObject.layer=2;
		}
	}
	
	
	void Update(){
		DetectKeys();
		FindFloorAndRotation();
		MoveCharacter();
	}
	
	
	void DetectKeys(){
		//forward path movement:

		if(Input.GetKey(KeyCode.A)) {
			pathPosition += Time.deltaTime * speed;
		}
		if(Input.GetKey(KeyCode.D)) {
			pathPosition += Time.deltaTime * speed;
		}
		//jump:
		if (Input.GetKeyDown("space") && jumpState==0) {
			ySpeed-=jumpForce;
			jumpState=1;
		}
	}
	
	
	void FindFloorAndRotation(){
		float pathPercent = pathPosition%1;
		Vector3 coordinateOnPath = iTween.PointOnPath(controlPath,pathPercent);
		Vector3 lookTarget;
		
		//calculate look data if we aren't going to be looking beyond the extents of the path:
		if(pathPercent-lookAheadAmount>=0 && pathPercent+lookAheadAmount <=1){
			
			//leading or trailing point so we can have something to look at:
			lookTarget = iTween.PointOnPath(controlPath,pathPercent+lookAheadAmount);
			Vector3 diretion = lookTarget - coordinateOnPath;
			//Debug.DrawRay(transform.position, diretion.normalized * 10, Color.red);

			//look:
			character.LookAt(transform.position + diretion.normalized, previousNormal);
			//Debug.Log(lookTarget);
			//nullify all rotations but y since we just want to look where we are going:
			//float yRot = character.eulerAngles.y;
			//character.eulerAngles=new Vector3(0,yRot,0);
		}

		if (Physics.Raycast(coordinateOnPath,-previousNormal,out hit, rayLength)){
			previousNormal = hit.normal;
			Debug.DrawRay(coordinateOnPath, -previousNormal * hit.distance);
			//Debug.DrawRay(hit.point, hit.normal*10, Color.green);


			//character.transform.rotation = Quaternion.LookRotation(hit.normal);
			floorPosition=hit.point;
		}
	}
	
	
	void MoveCharacter(){
		//add gravity:
		ySpeed += gravity * Time.deltaTime;
		
		//apply gravity:
		character.position=new Vector3(floorPosition.x,character.position.y-ySpeed,floorPosition.z);
		
		//floor checking:
		if(character.position.y<floorPosition.y){
			ySpeed=0;
			jumpState=0;
			character.position=new Vector3(floorPosition.x,floorPosition.y,floorPosition.z);
		}		
	}
	
}