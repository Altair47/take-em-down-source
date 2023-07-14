using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour {
    public Canvas PauseBackground;
    public GameObject BossHud;
    public GameObject WinObject;
    public TextMeshProUGUI GameOverBackground;
    private PlayerController player;
    public bool gameIsPaused = false;

    public void Start() {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        UnpauseGame();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        PlayerPrefs.SetInt("wonGame", 0);
    }

    public void Update() {
        if (player.isDead) StartCoroutine(WaitForDeath());
        PauseMenu();
        CheckScene();
    }

    public void CheckScene() {
        if (PlayerPrefs.GetInt("wonGame") == 1) return;

        switch (SceneManager.GetActiveScene().name) {
            case "Revil":
                RevilProgression();
                break;
            case "Dock Thing":
                DockThingProgression();
                DockThingBoss();
                break;
        }
    }

    public void RevilProgression() {
        if (GameObject.FindGameObjectsWithTag("Enemy/Zombie").Length == 0) {
            LoadScene("Dock Thing");
        }
    }

    public void DockThingProgression() {
        if (GameObject.FindGameObjectsWithTag("Enemy/Zombie").Length == 0) {
            FenceGate fenceGate2 = GameObject.Find("fence_gate 2").GetComponent<FenceGate>();
            if (!fenceGate2.open) {
                fenceGate2.OpenFenceGate();
                if (Mathf.Abs(fenceGate2.GetZRotation()) < 0.005f) fenceGate2.open = true;
            }

            FenceGate fenceGate3 = GameObject.Find("fence_gate 3").GetComponent<FenceGate>();
            if (!fenceGate3.open) {
                fenceGate3.OpenFenceGate();
                if (Mathf.Abs(fenceGate3.GetZRotation()) < 0.005f) fenceGate3.open = true;
            }
        }
    }

    public void DockThingBoss() {
        if (player.bossBattle) {
            BossHud.SetActive(true);

            if (GameObject.Find("Human_Mutant").GetComponent<Cthulu>().isDead) {
                BossHud.SetActive(false);
                PlayerPrefs.SetInt("wonGame", 1);
                StartCoroutine(WaitForWin());
            }
        }
    }

    public void OnApplicationFocus(bool hasFocus) {
        if (hasFocus) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void PauseMenu() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (PauseBackground.gameObject.activeSelf) UnpauseGame();
            else PauseGame();
        }
    }

    public void PauseGame() {
        gameIsPaused = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        PauseBackground.gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    public void UnpauseGame() {
        gameIsPaused = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
        PauseBackground.gameObject.SetActive(false);
    }

    public void ExitGame() {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void LoadScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
        Time.timeScale = 1;
    }

    public IEnumerator WaitForDeath() {
        GameOverBackground.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        PauseGame();
    }

    public IEnumerator WaitForWin() {
        yield return new WaitForSeconds(5);
        WinObject.SetActive(true);
        yield return new WaitForSeconds(10);
        LoadScene("Menu");
    }
}
