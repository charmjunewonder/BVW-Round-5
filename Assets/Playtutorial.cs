using UnityEngine;
using System.Collections;
public class Playtutorial : MonoBehaviour {
	public MovieTexture movie;
	bool flag = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (flag && !movie.isPlaying) {
			Application.LoadLevel("Path");	
		}
		if (Input.GetButtonDown ("Fire2")) {
			Application.LoadLevel("Path");	
		}

	}

	public void PlayTutorial()
	{
		movie.Play();
		flag = true;
	}
}
