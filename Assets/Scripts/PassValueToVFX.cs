using UnityEngine;
using UnityEngine.VFX;

public class PassValueToVFX : MonoBehaviour
{
    public VisualEffect vineEffect;

    public RMSMonitor RmsMonitor;
    public PLBDetection PLBDetection;

    [SerializeField] private float timeUnderExhaleRemapped;
    [SerializeField] private float headSpeed;
    [SerializeField] private bool isEnd = false;
    
    [SerializeField] private GameObject FinalScene;

    // Start is called before the first frame update
    private void Start()
    {
        InvokeRepeating("ReduceHeadSpeed", 3.0f, 3.0f);
    }

    // Update is called once per frame
    private void Update()
    {
        headSpeed = ValueMapper(PLBDetection.GetComponent<PLBDetection>().maxTimeUnderExhalation);
        vineEffect.SetFloat("Head Speed", headSpeed);

        if (headSpeed>2.4)
        {
            if (!isEnd) {
               
                FinalScene.SetActive(true);
                isEnd = true;
            }
        }
    }

    private float ValueMapper(float timeUnderExhale)
    {
        timeUnderExhaleRemapped = Remap(timeUnderExhale, 0, 10, 0.11f, 3.95f);
        return timeUnderExhaleRemapped;
    }

    public static float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    private void ReduceHeadSpeed()
    {
        if (headSpeed > 0.11)
            headSpeed = headSpeed - 1;
    }
}