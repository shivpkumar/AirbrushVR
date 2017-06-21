using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guitar : MonoBehaviour
{

	void Start()
	{
		AudioSource audio = GetComponent<AudioSource>();
		string microphone = "Rocksmith USB Guitar Adapter";
		audio.clip = Microphone.Start(microphone, true, 10, 44100);
		audio.loop = true;

		if (Microphone.IsRecording(microphone))
		{
			while (!(Microphone.GetPosition(microphone) > 0)) { }
			audio.Play();
		}
	}
}
