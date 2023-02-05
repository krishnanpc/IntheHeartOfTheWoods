using UnityEngine;

public class SceneController : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private GameObject A1;
    [SerializeField] private GameObject A2;
    [SerializeField] private GameObject A3;
    [SerializeField] private GameObject A4;
    
    [SerializeField] private GameObject B1;
    [SerializeField] private GameObject B2;
    [SerializeField] private GameObject B3;
   

    [SerializeField] private GameObject TitleScene;
    [SerializeField] private GameObject MainScene;

    private void Start()
    {
        Invoke("Method_A2", 15.0f);
        Invoke("Method_A3", 20.0f);
        Invoke("Method_A4", 30.0f);
        Invoke("Method_MainScene", 200.0f);
      
    }


    // Update is called once per frame
    private void Update()
    {
        
    }

    private void Method_A2()
    {
        A1.SetActive(false);
        A2.SetActive(true);
    }

    private void Method_A3()
    {
        A2.SetActive(false);
        A3.SetActive(true);
    }

    private void Method_A4()
    {
        A3.SetActive(false);
        A4.SetActive(true);
        TitleScene.SetActive(false);
        MainScene.SetActive(true);
    }
    
    private void Method_B2()
    {
      
        B2.SetActive(true);
        
    }
    
    
}