using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PLBDetection : MonoBehaviour
{
    [SerializeField]
    private float rmsValue;
    public GameObject RMSMeter;

    public double rmsThreshold = 0.03f;
   // [SerializeField] private bool IsExhaling = false;
    [SerializeField] private RMSMonitor rmsMonitor;
    [SerializeField] public float breathPerMinute;
    [SerializeField] public float breathPerSecond;
    [SerializeField] private float timeUnderExhalation;
    [SerializeField]
    private bool _IsExhaling ;

    [SerializeField] private float timeUnderExhalationMinutes;
    [SerializeField] private int exhalationCounter;
    
    [SerializeField] private bool prev_IsExhalingValue;
    [SerializeField] private float TimeElapsed;
    [SerializeField] public float maxTimeUnderExhalation;


    public bool IsExhaling
    {
        get { return _IsExhaling ; }
        set
        {
            _IsExhaling = value;
            
            if (timeUnderExhalation>0)
                breathPerMinute = 60 / (timeUnderExhalation * 2);
            
            if (_IsExhaling && !prev_IsExhalingValue)
                exhalationCounter++;
            
            if (_IsExhaling)
            {
                prev_IsExhalingValue = true;
            }
            else 
                prev_IsExhalingValue = false;
            

        }    
    }

    // Start is called before the first frame update
    void Start()
    { 
        rmsMonitor = RMSMeter.GetComponent<RMSMonitor>();
        
        InvokeRepeating("CalculateBreathingRate", 10.0f, 10.0f);
        
        maxTimeUnderExhalation = 0;
    }

    // Update is called once per frame
    void Update()
    {
       
        TimeElapsed += Time.deltaTime;
        rmsValue = rmsMonitor.m_rms;
       
        if (rmsValue > rmsThreshold)
        {
            timeUnderExhalation  += Time.deltaTime;
            IsExhaling = true;
            if (timeUnderExhalation > maxTimeUnderExhalation)
                maxTimeUnderExhalation = timeUnderExhalation;
        }
        else 
        {
            timeUnderExhalation = 0;
            IsExhaling = false;
        }
        

        timeUnderExhalationMinutes = timeUnderExhalation % 60;
        
        //if(timeUnderExhalationSeconds)
        
       
       
        
    }
    
    void CalculateBreathingRate()
    {
        
        float minutes = Mathf.Floor(TimeElapsed / 60);
        // if(minutes!=0)
        //     breathPerMinute = (exhalationCounter) / minutes; 
        
        //breathPerSecond = (exhalationCounter) / TimeElapsed; 

        //instance.velocity = Random.insideUnitSphere * 5;
    }
}
