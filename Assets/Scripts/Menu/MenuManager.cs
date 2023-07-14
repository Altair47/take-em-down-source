using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour {
    [Header("Objects")]
    public GameObject mainMenuCanvas = default;
    public GameObject creditsMenuCanvas = default;

    [Header("Audio")]
    public AudioSource sound = default;
    public AudioClip buttonClip = default;
    public AudioClip buttonBackClip = default;
    public AudioClip creditsClip = default;

    [Header("Generic")]
    public string selectedScene = default;

    public void Start() {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        ExitCredits();

        if (PlayerPrefs.HasKey("wonGame") && PlayerPrefs.GetInt("wonGame") == 1) {
            LoadCredits();
            PlaySound("credits");
            PlayerPrefs.SetInt("wonGame", 0);
        }
    }

    public void Update() {}

    public void ExitGame() {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void LoadScene() {
        SceneManager.LoadScene(selectedScene);
        Time.timeScale = 1;
    }

    public void LoadCredits() {
        creditsMenuCanvas.SetActive(true);
        mainMenuCanvas.SetActive(false);
    }

    public void ExitCredits() {
        mainMenuCanvas.SetActive(true);
        creditsMenuCanvas.SetActive(false);
    }

    public void PlaySound(string reason) {
        sound.Stop();

        switch (reason) {
            case "button":
                sound.PlayOneShot(buttonClip);
                break;
            case "button/back":
                sound.PlayOneShot(buttonBackClip);
                break;
            case "scene":
                sound.PlayOneShot(buttonClip);
                break;
            case "credits":
                sound.PlayOneShot(buttonClip);
                sound.PlayOneShot(creditsClip);
                break;
            default:
                break;
        }
    }
}
