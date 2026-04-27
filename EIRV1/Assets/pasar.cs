using UnityEngine;
using UnityEngine.SceneManagement;

public class pasar : MonoBehaviour
{
    
    public void next()
    {
        SceneManager.LoadScene("Intro");
    }
}
