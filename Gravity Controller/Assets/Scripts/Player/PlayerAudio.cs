using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
	public static AudioSource audioSource;

	public static AudioClip fireSound; 
	public static AudioClip reloadSound;
	public static AudioClip hitSound;
	public static AudioClip globalGravityHighSound;
	public static AudioClip globalGravityLowSound;
	public static AudioClip localGravitySound;
	public static AudioClip footstepSound;

	[SerializeField] private AudioClip _fireSound; 
	[SerializeField] private AudioClip _reloadSound;
	[SerializeField] private AudioClip _hitSound;
	[SerializeField] private AudioClip _globalGravityHighSound;
	[SerializeField] private AudioClip _globalGravityLowSound;
	[SerializeField] private AudioClip _localGravitySound;
	[SerializeField] private AudioClip _footstepSound;

	public const float FireSoundMaxVolume = 1f;
	public const float ReloadSoundMaxVolume = 1f;
	public const float HitSoundMaxVolume = 1f;
	public const float GlobalGravityHighSoundMaxVolume = 1f;
	public const float GlobalGravityLowSoundMaxVolume = 1f;
	public const float LocalGravitySoundMaxVolume = 1f;
	public const float FootstepMaxVolume = 1f;

	private void Start() {
		audioSource = GetComponent<AudioSource>();
		LoadClips();
	}

	private void LoadClips() {
		fireSound = _fireSound;
		reloadSound = _reloadSound;
		hitSound = _hitSound;
		globalGravityHighSound = _globalGravityHighSound;
		globalGravityLowSound = _globalGravityLowSound;
		localGravitySound = _localGravitySound;
		footstepSound = _footstepSound;
	}

	public static void PlaySfxOnce(AudioClip clip, float maxVolume) {
		audioSource.volume = maxVolume * GameManager.Instance.GetSFXVolume();
		audioSource.PlayOneShot(clip);
	}

	public static void PlaySfx(AudioClip clip, float maxVolume) {
		audioSource.volume = maxVolume * GameManager.Instance.GetSFXVolume();
		audioSource.clip = clip;
		audioSource.Play();
	}
	public static void StopSfx() {
		audioSource.Stop();
	}
}
