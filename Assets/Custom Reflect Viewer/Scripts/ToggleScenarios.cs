using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleScenarios : MonoBehaviour
{
    public GameObject[] list1;
    public GameObject[] list2;
    public GameObject[] list3;
    public List<GameObject> listAll;
    // Start is called before the first frame update
    void Start()
    {
        listAll = new List<GameObject>();
        listAll.AddRange(list1);
        listAll.AddRange(list2);
        listAll.AddRange(list3);
        foreach(GameObject obj in listAll)
        {
            //obj.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            foreach (GameObject obj in listAll)
            {
                obj.SetActive(false);
            }
            foreach (GameObject obj in list1)
            {
                obj.SetActive(true);
            }
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            foreach (GameObject obj in listAll)
            {
                obj.SetActive(false);
            }
            foreach (GameObject obj in list2)
            {
                obj.SetActive(true);
            }
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            foreach (GameObject obj in listAll)
            {
                obj.SetActive(false);
            }
            foreach (GameObject obj in list3)
            {
                obj.SetActive(true);
            }
        }
    }
}
