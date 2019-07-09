using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
	//publics
	[Header("Play button")]
	[SerializeField]
	private KeyCode playButton;

	[Header("Audio Input")]
	[SerializeField]
	private AudioSource audioSource;
	[SerializeField]
	private AudioClip audioClip;

	float[] samples = new float[512];
	float[] freqBand = new float[8];
	float[] bandBuffer = new float[8];

	float [] bufferDecrease = new float[8];

	float [] freqBandHighest = new float[8];
	public float [] audioBand = new float[8];
	public float [] audioBandBuffer = new float[8];

	public float[] audioPulse = new float[8];
	public float audioPulseConstraint;

	public float amplitude, amplitudeBuffer, amplitudePulse;
	float amplitudeHighest;

	public float energy, energyMax;

	//privates
	private bool audioPlaying = false;

	private LoopbackAudio loopBackAudio;

    // Start is called before the first frame update
    void Start()
    {
		audioSource = GetComponent<AudioSource>();
		loopBackAudio = GetComponent<LoopbackAudio>();
    }

    // Update is called once per frame
    void Update()
    {
		/*if (Input.GetKeyDown(playButton) && !audioPlaying)
		{
			audioSource.Play();
			audioPlaying = true;
		}
		else if (Input.GetKeyDown(playButton) && audioPlaying)
		{
			audioSource.Stop();
			audioPlaying = false;
		}
		if (audioPlaying)
		{
			GetSamples();
			MakeFrequencyBands();
			BandBuffer();
			CreateAudioBands();
			GetAmplitude();
			GetPulse();

		}*/
		GetSamples();
		MakeFrequencyBands();
		BandBuffer();
		CreateAudioBands();
		GetAmplitude();
		GetPulse();
		GetEnergy();

	}
	void GetEnergy()
	{
		energy = loopBackAudio.PostScaledEnergy;
		energyMax = loopBackAudio.PostScaledMax;
	}

	void GetPulse()
	{
		for (int i = 0; i < 8; i++)
		{
			if(audioBand[i] > audioPulseConstraint)
			{
				audioPulse[i] = audioBandBuffer[i];
			}
			else
			{
				audioPulse[i] = 0;
			}
		}
	}

	void GetAmplitude()
	{
		float currentAmplitude = 0;
		float currentAmplitudeBuffer = 0;
		for (int i = 0; i < 8; i++)
		{
			currentAmplitude += audioBand[i];
			currentAmplitudeBuffer += audioBandBuffer[i];
		}
		if(currentAmplitude > amplitudeHighest)
		{
			amplitudeHighest = currentAmplitude;
		}
		amplitude = currentAmplitude / amplitudeHighest;
		amplitudeBuffer = currentAmplitudeBuffer / amplitudeHighest;
	}

	void CreateAudioBands()
	{
		for (int b = 0; b < 8; b++)
		{
				if(freqBand[b] > freqBandHighest[b])
				{
					freqBandHighest[b] = freqBand[b];
				}
				audioBand[b] = (freqBand[b] / freqBandHighest[b]);
				audioBandBuffer[b] = (bandBuffer[b] / freqBandHighest[b]);

		}
	}
	void GetSamples()
	{
		
		samples = loopBackAudio.PostScaledSpectrumData;
	
		/*for (int i = 0; i < 512; i++)
		{
			samples[i] = loopBackAudio.SpectrumData[i];
			Debug.Log("samples[i] = " + samples[i]);
		}*/
	}

	void BandBuffer()
	{
		for (int g = 0; g < 8; g++)
		{
			if(freqBand[g] > bandBuffer[g])
			{
				bandBuffer[g] = freqBand[g];
				bufferDecrease[g] = 0.005f;
			}
			if (freqBand[g] < bandBuffer[g])
			{
				bandBuffer[g] -= bufferDecrease[g];
				bufferDecrease[g] *= 1.2f;
			}
		}
	}

	void MakeFrequencyBands()
	{

		/* 22050 / 512 = 43 hertz per sample 
		* 
		* 20 - 60
		* 60 - 250
		* 250 - 500
		* 500 - 2000
		* 2000 - 4000
		* 4000 - 6000
		* 6000 - 20000 
		* 
		* 0- 2 = 86
		* 1- 4 = 172
		* 2- 8 = 344
		* 3- 16 = 688
		* 4- 32 = 1376
		* 5- 64 = 2752
		* 6- 128 = 5504
		* 7- 256 = 11008
		* 
		*/

		int count = 0;

		for (int i = 0; i < 8; i++)
		{
			float average = 0;
			int sampleCount = (int)Mathf.Pow(2, i) * 2;

			if(i == 7)
			{
				sampleCount += 2;
			}

			for (int j = 0; j < sampleCount; j++)
			{
				average += samples[count] * (count + 1);
				count++;
			}
			average /= count;
			freqBand[i] = average * 10;
		}

	}
}
