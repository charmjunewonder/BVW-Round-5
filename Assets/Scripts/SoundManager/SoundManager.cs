using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {
	
	public AudioSource BGMPlayer;
	public AudioSource soundEffectPlayer;

	public AudioClip[] BGMs;
	public AudioClip[] soundEffects;

	private int BGMCount = 0;

	public void PlayBGM(int index)
	{
		if (BGMCount < index) {
			BGMPlayer.clip = BGMs[index];
			BGMPlayer.Play();
			BGMCount++;
		}

	}

	public void PlaySoundEffect(int index, bool loop)
	{
		soundEffectPlayer.clip = soundEffects [index];
		soundEffectPlayer.loop = loop;
		soundEffectPlayer.Play ();
	}
}
