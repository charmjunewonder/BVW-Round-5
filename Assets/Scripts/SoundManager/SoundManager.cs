using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {
	
	public AudioSource BGMPlayer;
	public AudioSource soundEffectPlayer;
	public AudioSource[] voiceEffectPlayer;

	public AudioClip[] BGMs;
	public AudioClip[] soundEffects;

	public AudioClip[] BabyItemSound;
	public AudioClip[] BoyItemSound;
	public AudioClip[] BoyObstacleSound;
	public AudioClip[] AdultItemSound;
	public AudioClip[] AdultObstacleSound;

	private int BGMCount = 0;

	public void PlayBGM(int index)
	{
		if (BGMCount <= index) {
			BGMPlayer.clip = BGMs[index];
			BGMPlayer.Play();
			BGMCount++;
		}

	}

	public void PlayVoiceEffect(int index, int num, bool PoN)
	{
		if (index == 0) {
			voiceEffectPlayer[num].clip = BabyItemSound[Random.Range (0, BabyItemSound.Length - 1)];
			voiceEffectPlayer[num].Play ();
		}
		else if(index == 1)
		{
			if(PoN)
			{
				voiceEffectPlayer[num].clip = BoyItemSound[Random.Range (0, BoyItemSound.Length - 1)];
				voiceEffectPlayer[num].Play ();
			}
			else
			{
				voiceEffectPlayer[num].clip = BoyObstacleSound[Random.Range (0, BoyObstacleSound.Length - 1)];
				voiceEffectPlayer[num].Play ();
			}
		}
		else if(index == 2)
		{
			if(PoN)
			{
				voiceEffectPlayer[num].clip = AdultItemSound[Random.Range (0, AdultItemSound.Length - 1)];
				voiceEffectPlayer[num].Play ();
			}
			else
			{
				voiceEffectPlayer[num].clip = AdultObstacleSound[Random.Range (0, AdultObstacleSound.Length - 1)];
				voiceEffectPlayer[num].Play ();
			}
		}

	}

	public void PlaySoundEffect(int index, bool loop)
	{
		soundEffectPlayer.clip = soundEffects[index];
		soundEffectPlayer.loop = loop;
		soundEffectPlayer.Play ();
	}

	public void StopSoundEffect()
	{
		soundEffectPlayer.Stop ();
	}
}
