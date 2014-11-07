using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour {
	public Transform[] controlPath;
	public Transform character;
	
	private float pathPosition=0;
	private RaycastHit hit;
	private float rayLength = 100;
	private Vector3 floorPosition;	
	private float lookAheadAmount = .01f;
	private float ySpeed=0;
	private float gravity=.1f;
	private float jumpForce=2.15f;
	private uint jumpState=0; //0=grounded 1=jumping
	private Vector3 previousNormal;
	private float velocity = 0;
	private float velocityDecrement = 0.03f;
	private float velocityIncrement = 0.12f;
	private float cameraOffset = 0.01f;
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

		if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) {
			velocity += velocityIncrement;
		}

		velocity = Mathf.Clamp(velocity - velocityDecrement, 0, 1);
		pathPosition += Time.deltaTime * velocity;

		//jump:
		if (Input.GetKeyDown("space") && jumpState==0) {
			ySpeed+=jumpForce;
			jumpState=1;
		}
	}
	
	
	void FindFloorAndRotation(){
		float pathPercent = pathPosition%1;
		Vector3 coordinateOnPath = iTween.PointOnPath(controlPath,pathPercent);
		Vector3 lookTarget;
		Vector3 diretion;
		lookTarget = iTween.PointOnPath(controlPath,pathPercent+lookAheadAmount);
		diretion = lookTarget - coordinateOnPath;
		//calculate look data if we aren't going to be looking beyond the extents of the path:
		if(pathPercent-lookAheadAmount>=0 && pathPercent+lookAheadAmount <=1){
			
			//leading or trailing point so we can have something to look at:
			// lookTarget = iTween.PointOnPath(controlPath,pathPercent+lookAheadAmount);
			// diretion = lookTarget - coordinateOnPath;
			//Debug.DrawRay(transform.position, diretion.normalized * 10, Color.red);

			//look:
			//Debug.Log(lookTarget);
			//nullify all rotations but y since we just want to look where we are going:
			//float yRot = character.eulerAngles.y;
			//character.eulerAngles=new Vector3(0,yRot,0);
		}

		if (Physics.Raycast(coordinateOnPath,-previousNormal,out hit, rayLength)){
			previousNormal = hit.normal;
			Debug.DrawRay(coordinateOnPath, -previousNormal * hit.distance);
			//Debug.DrawRay(hit.point, hit.normal*10, Color.green);
			character.LookAt(transform.position + diretion.normalized, previousNormal);
			//character.LookAt(transform.position + diretion.normalized, previousNormal);
			//Debug.Log(transform.position + " " + diretion.normalized+ " " + previousNormal);

			//character.transform.rotation = Quaternion.LookRotation(hit.normal);
			floorPosition=hit.point;
		}
	}
	
	
	void MoveCharacter(){
		//add gravity:
		//ySpeed += gravity * Time.deltaTime;
		
		//apply gravity:
		character.position = floorPosition + previousNormal.normalized * ySpeed;

		ySpeed -=gravity;
		ySpeed = Mathf.Clamp(ySpeed, 0, jumpForce+1);
		jumpState=0;

		// character.position=new Vector3( Mathf.Round(floorPosition.x*100)/100,
		// 	Mathf.Round(floorPosition.y*100)/100,
		// 	Mathf.Round(floorPosition.z*100)/100);
		//Debug.Log(ySpeed + " " + previousNormal+ " " + previousNormal.normalized);
		// if(character.position.y<floorPosition.y){
		// 	//ySpeed=0;
		// 	character.position=new Vector3(floorPosition.x,floorPosition.y,floorPosition.z);
		// }		
	}
	
}