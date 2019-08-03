using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public AudioSource audio;
    public AudioClip lol;
    public AudioClip lol2;
    public void PlayGame()
    {
        SceneManager.LoadScene("MainScene");
    }
    
    public void BackToMenu()
    {
        SceneManager.LoadScene("WelcomeScene");
    }

    public void PlaySound()
    {
        audio.GetComponent<AudioSource>().PlayOneShot(lol);
    }
    
    public void PlaySound2()
    {
        audio.GetComponent<AudioSource>().PlayOneShot(lol2);
    }
    public void Quit()
    {
        Application.Quit();
    }

    public void Volume()
    {
        GameObject slider = GameObject.Find("Slider");
        audio.volume = slider.GetComponent<Slider>().value;
    }
}
