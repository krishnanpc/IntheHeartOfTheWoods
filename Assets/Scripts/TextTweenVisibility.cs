using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;
using TMPro;

public class TextTweenVisibility : MonoBehaviour
{
    [SerializeField] private TextMeshPro currentText;
    [SerializeField] private float duration;
    [SerializeField] private float onScreenTime;

    // Make a text appear slowly
    // Start is called before the first frame update
    private void Start()
    {
        Invoke("DissappearSlowly", duration + onScreenTime);
    }

    void Awake()
    {
        currentText = this.GetComponent<TextMeshPro>();
        AppearSlowly();
    }

    private void AppearSlowly()
    {
        Tween.Value (0.0f, 1.0f, ChangeTransparency, duration, 0.0f, Tween.EaseInOut, Tween.LoopType.None);
    }
    
    private void DissappearSlowly()
    {
        Tween.Value (1.0f, 0.0f, ChangeTransparency, duration, 0.0f, Tween.EaseInOut, Tween.LoopType.None);
    }

    private void ChangeTransparency(float value)
    {
        currentText.color = new Vector4(currentText.color.r, currentText.color.g, currentText.color.b, value);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    

    
}
