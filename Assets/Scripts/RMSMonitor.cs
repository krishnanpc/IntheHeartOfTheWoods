using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Assertions;


[AddComponentMenu("RMSMonitor")]
[RequireComponent(typeof(AudioSource))]
public class RMSMonitor : MonoBehaviour
{
	public AudioSource m_unityAudioSource = null;

	[ReadOnly] public string m_name = null;
	[ReadOnly] public int m_id = 0;
	public bool m_drawHUD = false;

	[Header("Audio Settings")]

	int m_minSampleRate = 0;
	int m_maxSampleRate = 0;
	[ReadOnly] public int m_channels = 0;
	[ReadOnly] public int m_sampleRate = 0;

	public int m_activeChannel = 0;
	

	[Tooltip("Seconds of input that will be buffered. Too short = lost data")]
	public int m_micRecordLength = 1;
	[Tooltip("Seconds until mic input is played through audio source. Too short = artifacts")]
	[ReadOnly] public float m_delay = 0.001f;
	[ReadOnly] public int m_readPosition = 0;
	[ReadOnly] public int m_maxDelay = 0;
	int m_oldWritePosition = 0;
	bool m_bReload = false;

	public bool m_recording = false;
	
	[Header("Analysis")]
	
	[Header("Analysis")] public float m_spectrumWidth = 1024;

	//public ixAudioAsnalyser m_audioAnalyser = new ixAudioAnalyser();
	//[ReadOnly] public ixHUDDrawer m_ixHUDDrawer;
	//[ReadOnly] public ixHUDDrawer m_ixHUDDrawer;
	public float[] m_audioSamples = null;
	private float[] m_audioSamplesFirstPart = null;
	private float[] m_audioSamplesSecondPart = null;
	public float[] m_audioSpectrum = null; // new float[m_spectrumWidth];

	
	private float sampleRate = 44100;
	[Header("Display")]
	public Material m_lineMaterial = null;

	[SerializeField] private float rmsValue;
	[SerializeField] public float m_rms;


	//------- MonoBehaviour callbacks ---------
	private void Awake()
	{
	}
	public void OnEnable()
	{
		
		_Activate();
	}

	public void OnDisable()
	{

		//Deactivate();
	}
	public void Start()
	{
		
	
		Initialise();
		_Activate();
		//m_ixHUDDrawer.InitializeDrawer((int) m_history);
	}

	// -------------------------------
	// Called after creation from ixAudio...

	public void Initialise()
	{
		m_name = Microphone.devices[0];
		Microphone.GetDeviceCaps(m_name, out m_minSampleRate, out m_maxSampleRate); // These numbers are often equal and seemingly meaningless. Ignore.

		m_unityAudioSource = this.GetComponent<AudioSource>();
		//bypass to try and speed things up
		m_unityAudioSource.bypassEffects = true;
		m_unityAudioSource.bypassListenerEffects = true;
		m_unityAudioSource.bypassReverbZones = true;
		m_unityAudioSource.priority = 0;
		m_unityAudioSource.pitch = 1;
		m_unityAudioSource.loop = true;
		//m_unityAudioSource.outputAudioMixerGroup = ixAudio.I.m_audioMixerMicGroup;

		// if (m_lineMaterial == null)
		// 	m_lineMaterial = ixAudio.I.m_lineMaterial;
		
		

	}

	void _Activate()
	{
		
	

		StartMicrophone();
	}

	public void Activate()
	{
		gameObject.SetActive(true);
	}

	void StartMicrophone()
	{
		//Start recording and store the audio captured from the microphone at the AudioClip in the AudioSource
		m_unityAudioSource.clip = Microphone.Start(m_name, true, m_micRecordLength, (int)sampleRate);
		m_channels = m_unityAudioSource.clip.channels;

		if (!(Microphone.GetPosition(m_name) > 0)) { }      //wait for mic ready
		m_unityAudioSource.loop = true;                     //continual output
		m_unityAudioSource.mute = true;                    //Hack for bug

		m_sampleRate = m_unityAudioSource.clip.frequency;
		
		m_unityAudioSource.Play();

		m_recording = Microphone.IsRecording(m_name);

		

		}
	// public void Deactivate()
	// {
	// 	StopMicrophone();
	//
	// 	gameObject.SetActive(false);
	// 	ixAudio.ActiveAudioSources().Remove(this);
	// 	Print(1, "Deactivate " + m_name);
	// }

	void StopMicrophone()
	{
		Microphone.End(m_name);
		m_recording = Microphone.IsRecording(m_name);

		m_unityAudioSource.clip = null;
		m_unityAudioSource.loop = false;
	}

	
    private void Update()
    {
        if (m_recording)
        {
            if (m_audioSamples == null || m_audioSamples.Length != (int) m_spectrumWidth)
                m_audioSamples = new float[(int) m_spectrumWidth];

            if (m_audioSpectrum == null || m_audioSpectrum.Length != (int) m_spectrumWidth)
                m_audioSpectrum = new float[(int) m_spectrumWidth];
            
           // try
            {
	            // int micGetPosition = Microphone.GetPosition(m_name);
             //    int offset = micGetPosition - m_audioSamples.Length;
                
                //m_unityAudioSource.clip.GetData(m_audioSamples, offset);

                //m_unityAudioSource.GetOutputData(m_audioSamples, 0);
                //float[] samples = theSource.GetOutputData(1024, 0);

                
                int micGetPosition = Microphone.GetPosition(m_name);
                int offset = micGetPosition - m_audioSamples.Length;
                
                if (offset >= 0)
	                m_unityAudioSource.clip.GetData(m_audioSamples, offset);
                else 
                {
	                Array.Resize(ref m_audioSamplesFirstPart, -offset);
	                m_unityAudioSource.clip.GetData(m_audioSamplesFirstPart, m_micRecordLength * m_sampleRate + (offset));
	                Array.Resize(ref m_audioSamplesSecondPart, micGetPosition);
	                if(m_audioSamplesSecondPart.Length!=0)
		                m_unityAudioSource.clip.GetData(m_audioSamplesSecondPart, 0);
	                Assert.IsTrue((m_audioSamplesFirstPart.Length+m_audioSamplesSecondPart.Length)==m_audioSamples.Length);
	                Array.Copy(m_audioSamplesFirstPart,m_audioSamples, m_audioSamplesFirstPart.Length);
	                Array.Copy(m_audioSamplesSecondPart, 0, m_audioSamples, m_audioSamplesFirstPart.Length,m_audioSamplesSecondPart.Length);
                }


            }
            // catch (Exception e)
            // {
            //     Debug.Log("exception caught "+e +"at" + Time.time); 
            // }



            rmsValue =  GetRMS(m_audioSamples);
        }

        if (m_drawHUD)
        {
            //m_ixHUDDrawer.Draw(audioSpectrum,audioSamples,this);
        }
    }


    float GetRMS(float[] audiosamples)
    {
	    var rmsValue = 0;
	    float sample = audiosamples[0];
	    float sumSqr = 0;
	    
	    for (var i = 1; i < audiosamples.Length; i++)
	    {
		    sample = audiosamples[i];
		    sumSqr += sample * sample;
	    }


	   
	    // return the square root of the mean of squared samples
	    m_rms = (float) Math.Sqrt(sumSqr / (float)audiosamples.Length);

	    
	    return m_rms;
    }
}

