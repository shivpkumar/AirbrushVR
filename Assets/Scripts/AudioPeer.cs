using UnityEngine;
using System.Collections;

[RequireComponent (typeof (AudioSource))]
public class AudioPeer : MonoBehaviour {
	AudioSource _audioSource;

	public static float[] _audioBand = new float[8];
	public static float[] _audioBandBuffer = new float[8];

	public static float[] _samples = new float[512];
	public static float[] _freqBand = new float[8];
	public static float[] _bandBuffer = new float[8];
	private float[] _bufferDecrease = new float[8];
	private float[] _freqBandHighest = new float[8];

	void Start () {
		_audioSource = GetComponent<AudioSource> ();
	}

	void Update () {
		GetSpectrumAudioSource ();
		MakeFrequencyBands ();
		BandBuffer ();
		CreateAudioBands ();
	}

	void CreateAudioBands() {
		for (int i = 0; i < 8; i++) {
			if (_freqBand [i] > _freqBandHighest [i]) {
				_freqBandHighest[i] = _freqBand[i];
			}

			_audioBand [i] = (_freqBand [i] / _freqBandHighest [i]);
			_audioBandBuffer [i] = (_bandBuffer [i] / _freqBandHighest [i]);
		}
	}

	void GetSpectrumAudioSource() {
		_audioSource.GetSpectrumData (_samples, 0, FFTWindow.Blackman);
	}

	void BandBuffer() {
		for (int i = 0; i < 8; i++) {
			if (_freqBand[i] > _bandBuffer[i]) {
				_bandBuffer[i] = _freqBand[i];
				_bufferDecrease[i] = 0.005f;
			}

			if (_freqBand[i] < _bandBuffer[i]) {
				_bandBuffer[i] -= _bufferDecrease[i];
				_bufferDecrease[i] *= 1.2f;
			}
		}
	}

	void MakeFrequencyBands() {
		/*
		 * 22050 hertz / 512 samples = 43 hertz per sample
		 * 
		 * 0: 2 samples = 86 hertz (0 - 86 hertz)
		 * 1: 4 samples = 172 hertz (87 - 258 hertz)
		 * 2: 8 samples = 344 hertz (259 - 602 hertz)
		 * 3: 16 samples = 688 hertz (603 - 1,290 hertz)
		 * 4: 32 samples = 1,376 hertz (1,291 - 2,666 hertz)
		 * 5: 64 samples = 2,752 hertz (2,667 - 5,418 hertz)
		 * 6: 128 samples = 5,504 hertz (5,419 - 10,922 hertz)
		 * 7: 256 samples = 11,008 hertz (10,923 - 21,930 hertz)
		 * 
		 * Total of 510 samples used
		 */

		int count = 0;

		for (int i = 0; i < 8; i++) {
			float average = 0;
			int sampleCount = (int)Mathf.Pow (2, i + 1);

			if (i == 7) {
				sampleCount += 2;
			}

			for (int j = 0; j < sampleCount; j++) {
				average += _samples [count] * (count + 1);
				count++;
			}

			average /= count;

			_freqBand [i] = average * 10;
		}
	}
}
