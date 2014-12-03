//#define DEBUG
using UnityEngine;
using System.Collections;
using System.IO.Ports;

public class Controller : MonoBehaviour {

	public static SerialPort spsp;
	public static SerialPort spsp2;

	public int Number;

	public KeyCode key1;
	public KeyCode key2;
	public KeyCode jumpKey;

	public Transform[] controlPath;
	public Transform character;
	public GameObject camera;
	public GameObject camera2;

	public int collectedItemCount = 0;
	public GameObject[] models;
	public int characterMode = 0;
	public float pathPosition=0.001f;
	public float pathOffset = 0;
	public ParticleSystem TransitionEffect;
	public Texture[] speed;
	public Texture[] numbers;
	public Texture[] readyGo;
	public Texture[] progressBar;
	public GameObject readyGoGUI;
	public GameObject wheelChair;
	public GameObject hellgate;
	public bool rightPlayer;
	public SoundManager sm;
	public GameObject leaderBoard;
	public Animator jumpPadAnimator;
	public GameObject wheelChairFlare;
	public GameObject SeniorToBeLookedAt;
	public Font myFont;

	private RaycastHit hit;
	private float rayLength = 100;
	private Vector3 floorPosition;	
	private float lookAheadAmount = .001f;
	private float ySpeed=0;
	private float gravity=.1f;
	private float jumpForce=10f;
	private uint jumpState=0; //0=grounded 1=jumping
	private Vector3 previousNormal;
	private float velocity = 0;
	private float velocityDecrement = 0.00004f;
	private float velocityIncrement = 0.0006f;

	private float[] velocityUpperBounds;
	//private float cameraOffset = 0.01f;
	private Vector3 previousPosition;

	private Vector3 prevPosition;

	private Queue normals;

	private Animator animator;

	private bool walkable = false;
	private float waitingCount = 0;
	private bool lookingBack = false;
	private Vector3 offsetVector;

	private float rotAngle = -90;

	private bool isDropping;

	private int[] speedAngles = {30, 90, 190, 360};
	private int[] speedNums = {10, 30, 100, 200};

	private SerialPort spUnity;
	private SerialPort spUnity2;
	private bool isOnRollerCoaster;
	private bool isPadJumping;
	private bool isDropPath;

	private bool wheelchairRun;
	private bool arrivedOnTime;
	private bool lookAtSenior;
	private int lapCount = 0;

	private bool hellCamera = false;

	public int gameTime;
	private int startTime;
	public static int finalLapCount = -1;
	public static int leadingNum = -1;
	public static int winningNum = -1;
	private int countDown;
	private bool isFinished = false;
	public bool isPlayer;
	private GameObject player2;
	void OnDrawGizmos(){
		iTween.DrawPath(controlPath,Color.blue);	
	}
	
	
	void Start(){
		spUnity = Controller.spsp;
		spUnity2 = Controller.spsp2;
		
		player2 = GameObject.Find("Character2");
		player2.GetComponent<Controller>().isPlayer = true;
		startTime = (int)Time.time;
		//leaderBoard.SetActive (true);
		leaderBoard.GetComponent<RestartController>().deleteUnknown();
		//leaderBoard.SetActive (false);

		//set the model of the character
		characterMode = 0;
		models[0].SetActive(true);
		animator = models[0].GetComponent<Animator>();

		velocityUpperBounds = new float[4];
		velocityUpperBounds [0] = 0.00003f;
		velocityUpperBounds [1] = 0.00008f; 
		velocityUpperBounds [2] = 0.00025f;
		velocityUpperBounds [3] = 0.0005f;

		previousNormal = Vector3.up;
		//plop the character pieces in the "Ignore Raycast" layer so we don't have false raycast data:	
		foreach (Transform child in character) {
			child.gameObject.layer=2;
		}

		isOnRollerCoaster = false;
		wheelchairRun = false;
		arrivedOnTime = false;
		lookAtSenior = false;
		countDown = 10;

		normals = new Queue ();
		StartCoroutine(look());
		StartCoroutine("CheckCollectedItemCount");

		sm.PlayBGM (0);
	}

	IEnumerator look(){
		animator.SetTrigger("Look");

		yield return new WaitForSeconds(1.5f);
		sm.PlaySoundEffect (0, false);
		readyGoGUI.SetActive(true);
		readyGoGUI.guiTexture.texture = readyGo[0];
		yield return new WaitForSeconds(0.8f);
		readyGoGUI.guiTexture.texture = readyGo[1];
		yield return new WaitForSeconds(0.8f);
		readyGoGUI.SetActive(false);
		walkable = true;
	}
	
	
	void Update(){
		//--------------------------x----------------------------
		if (spUnity != null && spUnity2 != null) {
			if (spUnity.IsOpen ) {
				try {
					DetectKeysArduinoBrown ();
				} catch (System.Exception) {

				}
			}
			
			if (spUnity2.IsOpen ) {
				try {
					DetectKeysArduinoYellow ();
				} catch (System.Exception) {
					
				}
			}
			
			//DetectKeys();
		}
		//------------------------------------------------------
		//Debug.Log(isDropping);
		//characterMode = 3;
		animator = models[characterMode].GetComponent<Animator>();
		DetectKeys();
		FindFloorAndRotation();
		MoveCharacter();
		//CameraLook();
		//CheckCollectedItemCount();
	}

	IEnumerator seniorAutoWalk(){
		walkable = false;
		GameObject wheelChairClone = Instantiate(wheelChair) as GameObject;
		wheelChairClone.transform.localScale = new Vector3(100, 100, 100);
		wheelChairClone.AddComponent<WheelChairCollider>();
		BoxCollider bc = wheelChairClone.AddComponent<BoxCollider>();
		bc.size = new Vector3(0.05f, 0.05f, 0.05f);
		bc.isTrigger = true;

		ModifyLookAtDirection(wheelChairClone, pathPosition+0.01f, false);
		wheelChairClone.SetActive(true);
		walkable = false;
		while(true){
			walkable = false;
			velocity = Mathf.Clamp(velocity + velocityIncrement * Time.deltaTime, 0, 0.00003f);
			yield return new WaitForSeconds(0.1f);
		}
	}
	
	public void seniorSit(){
		StartCoroutine(seniorSitIEnumerator());
	}

	IEnumerator seniorSitIEnumerator(){
		walkable = false;
		wheelChair.SetActive(true);
		StopCoroutine("seniorAutoWalk");
		animator.SetTrigger("Sit");
		yield return new WaitForSeconds(7.15f);
		sm.PlayBGM (4);
		wheelChairFlare.SetActive (true);
		camera2.SetActive(false);
		camera.SetActive(true);
		walkable = true;
		models[3].collider.enabled = false;
//		StartCoroutine("wheelChairAutoRun");
	}

//	IEnumerator wheelChairAutoRun(){
//		while(true){
//			velocity = 0.0005f;
//			yield return new WaitForSeconds(0.1f);
//		}
//	}

	void DetectKeysArduinoBrown(){
		if(isPlayer == false){
		int arduinoValue = spUnity.ReadByte ();

		if (!isOnRollerCoaster) {
			if (walkable) {
				if (arduinoValue == 1) {
					if(velocity < velocityUpperBounds[characterMode % 4])
					{
						velocity += velocityIncrement * Time.deltaTime;
					}
					waitingCount = 0;
				} else{
					if(velocity == 0 && characterMode < 3)
						waitingCount += Time.deltaTime;
				} 
			}
			if (characterMode == 0) {
				if (waitingCount > 5) {
						waitingCount = 0;
						walkable = false;
						StartCoroutine (LookBack ());
				}
				if (lookingBack) {
						if (arduinoValue == 1) {
							waitingCount = 0;
							lookingBack = false;
							StartCoroutine (LookForward ());
						}
				}
			} else if (characterMode == 3) {

			} else {
				if (waitingCount > 5) {
						waitingCount = 0;
						walkable = false;
						StartCoroutine (Idle ());
				}
				//jump:
				if (walkable && arduinoValue == 2 && jumpState == 0) {
						animator.SetTrigger ("Jump");
						StartCoroutine (Jump ());
						jumpState = 1;
				}
			}
			if(velocity > velocityUpperBounds[characterMode % 4])
			{
				velocity = Mathf.Clamp(velocity - 5 * velocityDecrement * Time.deltaTime, 0, 1f);
			}
			else
			{
				velocity = Mathf.Clamp(velocity - velocityDecrement * Time.deltaTime, 0, 1f);
			}
		}
		else
		{
			if(characterMode != 3)
				velocity = 0.0005f;
		}
		pathPosition += velocity;
		animator.SetFloat ("Speed", velocity);
	}
	}
	void DetectKeysArduinoYellow(){
		if(isPlayer == true){
		int arduinoValue2 = spUnity2.ReadByte ();
		
			if (!isOnRollerCoaster) {
			if (walkable) {
				if (arduinoValue2 == 4) {
					if(velocity < velocityUpperBounds[characterMode % 4])
					{
						velocity += velocityIncrement * Time.deltaTime;
					}
					waitingCount = 0;
				} else{
					if(velocity == 0 && characterMode < 3)
						waitingCount += Time.deltaTime;
				} 
			}
			if (characterMode == 0) {
				if (waitingCount > 5) {
					waitingCount = 0;
					walkable = false;
					StartCoroutine (LookBack ());
				}
				if (lookingBack) {
					if (arduinoValue2 == 4) {
						waitingCount = 0;
						lookingBack = false;
						StartCoroutine (LookForward ());
					}
				}
			} else if (characterMode == 3) {
				
			} else {
				if (waitingCount > 5) {
					waitingCount = 0;
					walkable = false;
					StartCoroutine (Idle ());
				}
				//jump:
				if (walkable && arduinoValue2 == 3 && jumpState == 0) {
					animator.SetTrigger ("Jump");
					StartCoroutine (Jump ());
					jumpState = 1;
				}
			}
			if(velocity > velocityUpperBounds[characterMode % 4])
			{
				velocity = Mathf.Clamp(velocity - 5 * velocityDecrement * Time.deltaTime, 0, 1f);
			}
			else
			{
				velocity = Mathf.Clamp(velocity - velocityDecrement * Time.deltaTime, 0, 1f);
			}
		}
		else
		{
			if(characterMode != 3)
				velocity = 0.0005f;
		}
		pathPosition += velocity;
		animator.SetFloat ("Speed", velocity);
	}
	}
	void DetectKeys(){
		if(walkable){
			if(Input.GetKey(key1) || Input.GetKeyDown(key2)) {
				if(velocity < velocityUpperBounds[characterMode % 4])
				{
					velocity += velocityIncrement * Time.deltaTime;
				}
				
				waitingCount = 0;
			} else{
				if(velocity == 0 && characterMode < 3)
					waitingCount += Time.deltaTime;
			}
		}
		if(characterMode == 0){
			if(waitingCount > 5){
				waitingCount = 0;
				walkable = false;
				StartCoroutine(LookBack());
			}
			if(lookingBack){
				if(Input.GetKey(key1) || Input.GetKey(key2)) {
					waitingCount = 0;
					lookingBack = false;
					StartCoroutine(LookForward());
				}
			}
		} else if(characterMode == 3){

		} else{
			if(waitingCount > 5){
				waitingCount = 0;
				walkable = false;
				StartCoroutine(Idle());
			}
			//jump:
			if (walkable && Input.GetKeyDown(jumpKey) && jumpState==0) {
				animator.SetTrigger("Jump");
				StartCoroutine(Jump());
				jumpState=1;
			}
		}
		if(velocity > velocityUpperBounds[characterMode % 4])
		{
			velocity = Mathf.Clamp(velocity - 5 * velocityDecrement * Time.deltaTime, 0, 1f);
		}
		else
		{
			velocity = Mathf.Clamp(velocity - velocityDecrement * Time.deltaTime, 0, 1f);
		}


		pathPosition += velocity;
		animator.SetFloat("Speed", velocity);
	}



	IEnumerator Idle(){
		animator.SetTrigger("Idle");
		yield return new WaitForSeconds(0.1f);
		walkable = true;
	}

	IEnumerator LookBack(){
		animator.SetTrigger("LookBack");
		yield return new WaitForSeconds(0.6f);
		lookingBack = true;
	}

	IEnumerator LookForward(){
		animator.SetTrigger("LookForward");
		yield return new WaitForSeconds(0.95f);
		walkable = true;
	}

	IEnumerator Jump(){
		float jumpIncrement = jumpForce/5;
		float temp = velocityDecrement;
		velocityDecrement = 0;
		for(int i = 0; i < 22; i++){
			ySpeed += jumpIncrement / 2.2f;
			yield return new WaitForSeconds(0.01f);
		}
		//yield return new WaitForSeconds(0.01f);
		for(int i = 0; i < 18; i++){
			ySpeed -= jumpIncrement / 1.8f;
			yield return new WaitForSeconds(0.01f);
		}
		velocityDecrement = temp;
		ySpeed = Mathf.Clamp(ySpeed, 0, jumpForce+1);	
		jumpState=0;
	}
	
	
	void FindFloorAndRotation(){
		float pathPercent = pathPosition%1f;
		Vector3 coordinateOnPath = iTween.PointOnPath(controlPath,pathPercent);
		Vector3 lookTarget;
		Vector3 diretion;
		lookTarget = iTween.PointOnPath(controlPath,pathPercent+lookAheadAmount);
		diretion = lookTarget - coordinateOnPath;

		int layerOfPath = 1 << 8;
//
		// if (Physics.Raycast(coordinateOnPath,-previousNormal,out hit, rayLength, layerOfPath)){
		// 	previousNormal = GetNormal(hit.normal);

		// 	if(Vector3.Distance(previousPosition, hit.point) > 0.5f){
		// 		floorPosition=hit.point;
		// 		//Debug.DrawRay(coordinateOnPath+ offsetVector.normalized * pathOffset, previousNormal.normalized * 10, Color.red, 10); 
		// 		if(isDropPath || isPadJumping){
		// 			//Debug.Log("fssfsdfsdfsdfsd " + pathPosition);
		// 			Vector3 v1 = Vector3.Cross(previousNormal, diretion);
		// 			Vector3 v2 = Vector3.Cross(v1, previousNormal);

		// 			Debug.DrawRay(coordinateOnPath+ offsetVector.normalized * pathOffset, v2.normalized * 10, Color.red, 10); 

		// 			character.transform.LookAt(transform.position + v2.normalized, previousNormal);
		// 			//character.transform.up = previousNormal.normalized;
		// 		} else{
		// 			character.transform.LookAt(transform.position + diretion.normalized, previousNormal);
		// 			//Debug.Log("fjlas " + pathPosition);
		// 		}
		// 		offsetVector = Vector3.Cross(previousNormal, diretion);
		// 	}
		// }
		//Test
		offsetVector = Vector3.Cross(previousNormal, diretion);
		Vector3 testNormal = previousNormal;
		if (Physics.Raycast(coordinateOnPath+offsetVector.normalized * pathOffset + previousNormal.normalized * 10,-previousNormal,out hit, rayLength, layerOfPath)){
			previousNormal = GetNormal(hit.normal);

			if(Vector3.Distance(previousPosition, hit.point) > 0.5f){
				floorPosition=hit.point;
				Debug.DrawRay(coordinateOnPath+ offsetVector.normalized * pathOffset + testNormal.normalized * 10, -testNormal.normalized * 10, Color.red, 10); 
				if(isDropPath || isPadJumping){
					//Debug.Log("fssfsdfsdfsdfsd " + pathPosition);
					Vector3 v1 = Vector3.Cross(previousNormal, diretion);
					Vector3 v2 = Vector3.Cross(v1, previousNormal);

					Debug.DrawRay(coordinateOnPath+ offsetVector.normalized * pathOffset, v2.normalized * 10, Color.red, 10); 
					//Debug.DrawRay(coordinateOnPath+ offsetVector.normalized * pathOffset, v2.normalized * 10, Color.red, 10); 

					character.transform.LookAt(transform.position + v2.normalized, previousNormal);
					//character.transform.up = previousNormal.normalized;
				}else{
		 			character.transform.LookAt(transform.position + diretion.normalized, previousNormal);
		 			//Debug.Log("fjlas " + pathPosition);
		 		}
			}
		}
	}
	
	
	void MoveCharacter(){
		if (!lookAtSenior) 
		{
			prevPosition = character.position;
			// set offset for each player
			//character.position = floorPosition + previousNormal.normalized * ySpeed;
			if(isPadJumping){
				pathPosition += 0.001f;
				float pathPercent = pathPosition%1f;
				Vector3 coordinateOnPath = iTween.PointOnPath(controlPath,pathPercent);
				character.position = coordinateOnPath + offsetVector.normalized * pathOffset;
				return;
			}
			
			Vector3 nextPosition = calculateNextPosition();
			if(character.position.y - nextPosition.y > 40 && !isDropping){
				walkable = false;
				isDropping = true;
			} 
			else if(isDropping){
				character.position = new Vector3(nextPosition.x, character.position.y - 3 , nextPosition.z);
				if(character.position.y - nextPosition.y < 0.1f){
					isDropping = false;
					walkable = true;
				}
			}
			else{
				character.position = nextPosition;
			}
		}
	}

	Vector3 calculateNextPosition(){
		return 	floorPosition + previousNormal.normalized * ySpeed;
		//return 	floorPosition + previousNormal.normalized * ySpeed + offsetVector.normalized * pathOffset;
	}

	IEnumerator DropWithGravityIEnumerator(){
		walkable = false;
		isDropping = true;
		Vector3 destination = calculateNextPosition();
		Debug.Log(destination);
		while(character.position.y - destination.y > 0.1f){
			//Debug.Log(++count + " " + character.position + " " + destination);
			Vector3 position = character.position;
			position.y -= 3f;
			character.position = position;
			yield return new WaitForSeconds(0.01f);
		}

		character.position = destination;
		isDropping = false;
		walkable = true;
	}

	IEnumerator CheckCollectedItemCount(){
		while(true){
			if(collectedItemCount >= 4 && characterMode <= 3){

				collectedItemCount = 0;
				models[characterMode].SetActive(false);

				TransitionEffect.gameObject.SetActive(true);

				characterMode = ++characterMode % 4;
				animator = models[characterMode].GetComponent<Animator>();

				sm.PlaySoundEffect(1, false);
				sm.PlayBGM(characterMode);

				Invoke("SetNextModelActive", 1);
				Invoke("SetWalkableTrue", 1.7f);
			}
			if(characterMode == 3){
				if(leadingNum == -1)
				{
					sm.PlayBGM(3);
					leadingNum = Number;
					finalLapCount = lapCount + 1;
				}

				if(Number == 0)
				{
					GameObject.Find("ItemsGenerator").GetComponent<ItemGenerator>().Clear();
				}
				else
				{
					GameObject.Find("ItemsGenerator2").GetComponent<ItemGenerator2>().Clear();
				}
				camera2.SetActive(true);
				camera.SetActive(false);
				StartCoroutine("seniorAutoWalk");
				break;
			}
			yield return new WaitForSeconds(0.1f);
		}
	}

	private void SetNextModelActive()
	{
		models[characterMode].SetActive(true);
		animator = models[characterMode].GetComponent<Animator>();
	}

	public void SetWalkableTrue()
	{
		TransitionEffect.gameObject.SetActive(false);
		walkable = true;
	}

	public void SetWalkableFalse()
	{
		walkable = false;
	}

	public void SetVelocity(float vol)
	{
		velocity = vol;
	}
	public float GetVelocity()
	{
		return velocity;
	}

	public Vector3 GetPositionWithPercent(float percent){
		return iTween.PointOnPath(controlPath,percent);
	}

	public void ModifyLookAtDirection(GameObject other, float percent, bool t){
		float pathPercent = percent%1f;
		Vector3 coordinateOnPath = iTween.PointOnPath(controlPath,pathPercent);
		Vector3 lookTarget;
		Vector3 direction;
		lookTarget = iTween.PointOnPath(controlPath,pathPercent+lookAheadAmount);
		direction = lookTarget - coordinateOnPath;

		int layerOfPath = 1 << 8;
		Debug.DrawRay(coordinateOnPath+ offsetVector.normalized * pathOffset, direction.normalized * 100, Color.yellow, 100); 

		Vector3 directionA = new Vector3(-direction.y, direction.x, 0);
		float minDistance = Mathf.Infinity;
		Vector3 vectorWithMinDistance = directionA;
		Vector3 positionVector = other.transform.position;
		Debug.DrawRay(coordinateOnPath+ offsetVector.normalized * pathOffset, directionA.normalized * 100, Color.green, 100); 

		for(int i = 0; i < 8; i++){
			directionA = Quaternion.AngleAxis(-45, direction) * directionA;

			if (Physics.Raycast(coordinateOnPath,-directionA,out hit, 10.0f, layerOfPath)){

				if(hit.distance < minDistance){
					minDistance = hit.distance;
					vectorWithMinDistance = directionA;
					positionVector = hit.point;
					
				}
			}
			Debug.DrawRay(coordinateOnPath+ offsetVector.normalized * pathOffset, directionA.normalized * 10, Color.red, 100); 

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
			Debug.DrawRay(coordinateOnPath+ offsetVector.normalized * pathOffset, directionA.normalized * 10, Color.red, 100); 

		}
		Vector3 offsetVector1 = Vector3.Cross(vectorWithMinDistance, direction);
		other.transform.LookAt(other.transform.position + direction.normalized, vectorWithMinDistance);
		if (!t) {
			other.transform.position = positionVector + 1f * vectorWithMinDistance.normalized
				+ offsetVector1.normalized * pathOffset;
		}
		else
		{
			other.transform.position = positionVector + 3f * vectorWithMinDistance.normalized
				+ offsetVector1.normalized * pathOffset;
		}


		Debug.Log (positionVector + " " + vectorWithMinDistance + " " + offsetVector1);
	}

	private Vector3 GetNormal(Vector3 v)
	{
		normals.Enqueue(v);
		if(normals.Count >= 20)
		{
			normals.Dequeue();
		}

		Vector3 average = new Vector3(0, 0, 0);
		foreach(Vector3 temp in normals)
		{
			average += temp;
		}
		average /= normals.Count;
		return average;
	}

	void OnGUI() {
		int defaultWidth = 1600;
		float widthRatio = Screen.width * 1.0f/ defaultWidth;
		int rightOffset = 0;
		
		if(rightPlayer) rightOffset = Screen.width/2;
		//Debug.Log(widthRatio);

		float width = speed[0].width*0.4f*widthRatio, height = speed[0].height*0.4f*widthRatio;
		GUI.DrawTexture(new Rect(rightOffset + Screen.width * 0.475f - width, Screen.height * 0.8f, width, height), speed[0]);

		float ratio = velocity / (velocityUpperBounds[characterMode]- velocityDecrement * Time.deltaTime);
		rotAngle = -90 + ratio * speedAngles[characterMode];
		int speedNum =  Mathf.CeilToInt(speedNums[characterMode] * ratio);
		speedNum = Mathf.Clamp (speedNum, 0, 300);
		int num1 = speedNum / 100;
		int remainder = speedNum - num1 * 100;
		int num2 = remainder / 10;
		remainder = remainder - num2 * 10;
		width = numbers[0].width*0.4f*widthRatio;
      	height = numbers[0].height*0.40f*widthRatio;
		if(num1 != 0){
			GUI.DrawTexture(new Rect(rightOffset + Screen.width * 0.427f - width, Screen.height * 0.885f, width, height), numbers[num1]);
		}
		if(num1 != 0 || num2 != 0){
			GUI.DrawTexture(new Rect(rightOffset + Screen.width * 0.439f - width, Screen.height * 0.885f, width, height), numbers[num2]);
		}

		GUI.DrawTexture(new Rect(rightOffset + Screen.width * 0.451f - width, Screen.height * 0.885f, width, height), numbers[remainder]);
		width = speed[2].width*0.47f*widthRatio;
      	height = speed[2].height*0.47f*widthRatio;
		GUI.DrawTexture(new Rect(rightOffset + Screen.width * 0.469f - width, Screen.height * 0.9f, width, height), speed[2]);

		//progress bar
      	width = progressBar[0].width*0.47f*widthRatio;
      	height = progressBar[0].height*0.47f*widthRatio;
        GUI.DrawTexture(new Rect(rightOffset + Screen.width * 0.03f, Screen.height * 0.87f, width, height), 
        	progressBar[(characterMode * 4 + collectedItemCount)%13]);

        //Time
        if(!isFinished)
    		gameTime = (int)Time.time - startTime;
		int minutes = gameTime / 60;
		int seconds = gameTime - minutes * 60;
		int second1 = seconds / 10;
		int second2 = seconds - second1 * 10;
		int minute1 = minutes / 10;
		int minute2 = minutes - minute1 * 10;
        GUIStyle style = new GUIStyle();
		style.font = myFont;
		style.fontSize = Mathf.FloorToInt(50*widthRatio);
		style.normal.textColor = Color.white;
		GUI.Label(new Rect(rightOffset + 310*widthRatio, 10*widthRatio, 300, 50), 
			minute1 + "" + minute2 + " : " + second1 + "" + second2, style);

		//Rotate
		width = speed[1].width*0.4f*widthRatio;
      	height = speed[1].height*0.4f*widthRatio;
        Vector3 pivotPoint = new Vector2(rightOffset + Screen.width * 0.390f, Screen.height * 0.872f);
        GUIUtility.RotateAroundPivot(rotAngle, pivotPoint);
        //GUI.DrawTexture(new Rect(Screen.width * 0.665f, Screen.width * 0.665f, speed[1].width*0.65f, speed[1].height*0.65f), speed[1]);
        GUI.DrawTexture(new Rect(rightOffset + Screen.width * 0.392f - width, Screen.height * 0.868f, width, height), speed[1]);
    }

	public Animator GetAnimator()
	{
		return animator;
	}

	public void SetOnRollerCoaster(bool t)
	{
		isOnRollerCoaster = t;
		if (t) {
			velocity = 0.0005f;
		}
	}
	public void SetIsJumpingTrue(){
		isPadJumping = true;
		walkable = false;
	}
	public void SetIsJumpingFalse(){
		isPadJumping = false;
		walkable = true;
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.tag == "FallingEnter") {
			isDropPath = true;
		} else if (col.gameObject.tag == "FallingExit") {
			isDropPath = false;
		} 
		else if (col.gameObject.tag == "JumpPadTrigger"){
			sm.PlaySoundEffect(6, false);
			jumpPadAnimator.SetTrigger("Jump");
			SetIsJumpingTrue();
		}
		else if (col.gameObject.tag == "JumpPadExit"){
			SetIsJumpingFalse();
		}
		else if (col.gameObject.tag == "Tomb") 
		{
			velocity = 0;
			walkable = false;
			if(winningNum == -1)
			{
				isFinished = true;
				sm.PlayBGM(5);
				//sm.PlaySoundEffect(3, true);
				winningNum = Number;
				if(Number == 0)
				{
					GameObject.Find("Character2").GetComponent<Controller>().Dance();
					GameObject.Find("Character2").GetComponent<Controller>().velocity = 0;
					GameObject.Find("Character2").GetComponent<Controller>().walkable = false;
					GameObject.Find ("Character2").GetComponent<Controller>().SwitchCamera();
					GameObject.Find("Camera1").GetComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer("UI"));

				}
				else
				{
					GameObject.Find("Camera2").GetComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer("UI"));
					GameObject.Find("Character1").GetComponent<Controller>().Dance();
					GameObject.Find("Character1").GetComponent<Controller>().velocity = 0;
					GameObject.Find("Character1").GetComponent<Controller>().walkable = false;
					GameObject.Find ("Character1").GetComponent<Controller>().SwitchCamera();
					GameObject.Find("Camera11").SetActive (true);
				}
				Invoke ("showLeaderBoard", 2);
			}
			models[characterMode].SetActive(false);
		}
	}

	public void Dance()
	{
		Debug.Log ("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
		animator.SetBool("standup", true);
	}

//	public void CountDown(){
//		StartCoroutine("CountDownIEnumerator");
//	}
//
//	IEnumerator CountDownIEnumerator()
//	{
//		readyGoGUI.SetActive(true);
//		for (int i = 10; i >= 1; i--) 
//		{
//			countDown--;
//			readyGoGUI.guiTexture.texture = numbers[countDown];
//			yield return new WaitForSeconds(1f);
//		}
//		readyGoGUI.SetActive (false);
//		if (!arrivedOnTime) {
//			tomb.SetActive (false);
//
//			if(pathPosition % 1 <= 0.96f)
//			{
//				ModifyLookAtDirection(hellgate, (pathPosition + 0.02f) % 1, true);
//				hellgate.transform.Rotate (0, 180, 0, Space.Self);
//				hellgate.transform.Translate (5, 0, 0, Space.Self);
//				hellCamera = true;
//			}
//			hellgate.SetActive (true);
//		}
//	}

	public void SwitchCamera()
	{
		camera2.SetActive(true);
		camera.SetActive(false);
	}

	private void showLeaderBoard(){
		leaderBoard.SetActive(true);
		Controller c1 = GameObject.Find ("Character1").GetComponent<Controller> ();
		Controller c2 = GameObject.Find ("Character2").GetComponent<Controller> ();
		int time1 = c1.gameTime;
		int time2 = c2.gameTime;
		if(time1 > time2){
			//2 is the winner
			c2.leaderBoard.GetComponent<RestartController>().isWinning = true;
			c2.leaderBoard.GetComponent<RestartController>().StartToDisplay(time2);
			c1.leaderBoard.GetComponent<RestartController>().StartToDisplay(time2);
		} else{
			//1 is the winner
			c1.leaderBoard.GetComponent<RestartController>().isWinning = true;
			c1.leaderBoard.GetComponent<RestartController>().StartToDisplay(time1);
			c2.leaderBoard.GetComponent<RestartController>().StartToDisplay(time1);
		}
	}
}