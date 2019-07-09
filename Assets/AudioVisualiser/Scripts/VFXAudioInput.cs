using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.VFX;

public class VFXAudioInput : MonoBehaviour
{
	[SerializeField]
	private VisualEffect visualEffect;

	private SoundManager soundManager;

	private string[] vfxParameters = new string[8] 
	{
		"Sub-Bass",
		"Bass",
		"Low-Midrange",
		"Midrange",
		"Upper-Midrange",
		"Presence",
		"Brilliance",
		"Super-Brilliance"
	};

	// Start is called before the first frame update
	void Start()
    {
		soundManager = GetComponent<SoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
			VisualiseSound();
    }

	void VisualiseSound()
	{
		for (int i = 0; i < 8; i++)
		{
			visualEffect.SetFloat(vfxParameters[i], soundManager.audioBand[i]);
			visualEffect.SetFloat("Amplitude",soundManager.amplitude);
		}
	}
}
