using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Cthulu cthulu=default;
    public Image healthbar=default;

    void Start()
    {
        cthulu = GameObject.Find("Human_Mutant").GetComponent<Cthulu>();
        healthbar = GetComponent<Image>();
    }

    void Update()
    {
        healthbar.fillAmount = Mathf.Clamp(cthulu.cHealth / 1000f, 0, 1f);
    }
}
