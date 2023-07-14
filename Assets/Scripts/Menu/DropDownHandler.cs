using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DropDownHandler : MonoBehaviour {
    public TMP_Dropdown selector;
    private MenuManager menuManager;
    public GameObject dockThing;
    public GameObject Revil;

    public void Start() {
        menuManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<MenuManager>();

        selector.onValueChanged.AddListener(delegate {
            switch (selector.value) {
                case 0:
                    menuManager.selectedScene = "Revil";
                    dockThing.SetActive(false);
                    Revil.SetActive(true);
                    break;
                case 1:
                    menuManager.selectedScene = "Dock Thing";
                    dockThing.SetActive(true);
                    Revil.SetActive(false);
                    break;
                default:
                    menuManager.selectedScene = "Revil";
                    dockThing.SetActive(false);
                    Revil.SetActive(true);
                    break;
            }
        });
    }
}
