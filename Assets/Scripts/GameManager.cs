using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAPI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject menu;
    public Text timer;
    public bool isOpen;
    
    // Start is called before the first frame update
    void Start()
    {
        menu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        CheckOpenMenu();
        if(isOpen == true)
        {
            menu.SetActive(true);
        }
        else
        {
            menu.SetActive(false);
        }                   
    }
    
    private void CheckOpenMenu()
    {       
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                menu.SetActive(!menu.activeSelf);
                if(menu.activeSelf)
                {
                    isOpen = true;
                }
                else
                {
                    isOpen = false;
                }
            }
        
        
    }
}
