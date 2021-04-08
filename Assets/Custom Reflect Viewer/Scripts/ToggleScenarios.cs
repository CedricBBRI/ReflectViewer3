using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Reflect;
using System.IO;

public class ToggleScenarios : MonoBehaviour
{
    public List<GameObject> listCustom;
    public GameObject selectedObject;
    public GameObject[] list1;
    public GameObject[] list2;
    public GameObject[] list3;
    public GameObject[] list4;
    public GameObject[] list5;
    public GameObject[] list6;
    List<GameObject> listAll1;
    List<GameObject> listAll2;
    List<Material> matPoss;
    List<string> namePoss;
    public Text textCosts;
    int curScenario1 = 1;
    int curScenario2 = 1;
    double curCost1 = 0.00;
    double curCost2 = 0.00;
    double totArea1 = 0;
    double totArea2 = 0;
    GameObject root;
    ChangeMaterial changeMatScript;
    List<GameObject[]> listOfLists1;
    public Material defMat;
    public Material defMatWhite;

    // Start is called before the first frame update
    void Start()
    {
        listAll1 = new List<GameObject>();
        listAll1.AddRange(list1);
        listAll1.AddRange(list2);
        listAll1.AddRange(list3);
        listAll2 = new List<GameObject>();
        listAll2.AddRange(list4);
        listAll2.AddRange(list5);
        listAll2.AddRange(list6);
        root = GameObject.Find("Root");
        changeMatScript = root.GetComponent<ChangeMaterial>();
        listOfLists1 = new List<GameObject[]>();
        listOfLists1.Add(list1);
        listOfLists1.Add(list2);
        listOfLists1.Add(list3);

        listCustom = new List<GameObject>();

    }

    // Update is called once per frame
    void Update()
    {
        selectedObject = changeMatScript.selectedObject;
        var test = selectedObject.GetComponent<Metadata>().GetParameter("Area");
        double curArrea = double.Parse(test.Split()[0], System.Globalization.CultureInfo.InvariantCulture);
        namePoss = new List<string>();
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            totArea1 = 0;
            curScenario1 = 1;
            foreach (GameObject obj in listAll1)
            {
                obj.SetActive(false);
            }
            foreach (GameObject obj in list1)
            {
                obj.SetActive(true);
                test = obj.GetComponent<Metadata>().GetParameter("Area");
                totArea1 += double.Parse(test.Split()[0], System.Globalization.CultureInfo.InvariantCulture);
            }
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            totArea1 = 0;
            curScenario1 = 2;
            foreach (GameObject obj in listAll1)
            {
                obj.SetActive(false);
            }
            foreach (GameObject obj in list2)
            {
                obj.SetActive(true);
                test = obj.GetComponent<Metadata>().GetParameter("Area");
                totArea1 += double.Parse(test.Split()[0], System.Globalization.CultureInfo.InvariantCulture);
            }
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            totArea1 = 0;
            curScenario1 = 3;
            foreach (GameObject obj in listAll1)
            {
                obj.SetActive(false);
            }
            foreach (GameObject obj in list3)
            {
                obj.SetActive(true);
                test = obj.GetComponent<Metadata>().GetParameter("Area");
                totArea1 += double.Parse(test.Split()[0], System.Globalization.CultureInfo.InvariantCulture);
            }
        }
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            totArea2 = 0;
            curScenario2 = 1;
            foreach (GameObject obj in listAll2)
            {
                obj.SetActive(false);
            }
            foreach (GameObject obj in list4)
            {
                obj.SetActive(true);
                test = obj.GetComponent<Metadata>().GetParameter("Area");
                totArea2 += double.Parse(test.Split()[0], System.Globalization.CultureInfo.InvariantCulture);
            }
        }
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            totArea2 = 0;
            curScenario2 = 2;
            foreach (GameObject obj in listAll2)
            {
                obj.SetActive(false);
            }
            foreach (GameObject obj in list5)
            {
                obj.SetActive(true);
                test = obj.GetComponent<Metadata>().GetParameter("Area");
                totArea2 += double.Parse(test.Split()[0], System.Globalization.CultureInfo.InvariantCulture);
            }
        }
        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            totArea2 = 0;
            curScenario2 = 3;
            foreach (GameObject obj in listAll2)
            {
                obj.SetActive(false);
            }
            foreach (GameObject obj in list6)
            {
                obj.SetActive(true);
                test = obj.GetComponent<Metadata>().GetParameter("Area");
                totArea2 += double.Parse(test.Split()[0], System.Globalization.CultureInfo.InvariantCulture);
            }
        }
        curCost1 = 0.0;
        totArea1 = 0.0;
        if (listCustom.Count >= 1)
        {
            foreach (GameObject go in listCustom)
            {
                matPoss = changeMatScript.CreateUINew(go, 0);
                if (matPoss.Count >= 1)
                {
                    foreach (Material mat in matPoss)
                    {
                        namePoss.Add(mat.name);
                        namePoss.Add(mat.name + " (Instance)");
                        //Debug.Log("namePoss: " + mat.name);
                    }
                    //Debug.Log("nameCurr: " + go.GetComponent<MeshRenderer>().sharedMaterial.name);
                    //if (matPoss.Contains(go.GetComponent<MeshRenderer>().sharedMaterial))
                    if (namePoss.Contains(go.GetComponent<MeshRenderer>().sharedMaterial.name))
                    { 
                        curCost1 += double.Parse(go.GetComponent<Metadata>().GetParameter("Area").Split()[0], System.Globalization.CultureInfo.InvariantCulture) * 20.0;
                    }
                }
                test = go.GetComponent<Metadata>().GetParameter("Area");
                totArea1 += double.Parse(test.Split()[0], System.Globalization.CultureInfo.InvariantCulture);
            }
        }
        textCosts.text = "Area is " + curArrea.ToString() + "\nThe price of scenario " + curScenario1 + " is " + curCost1.ToString() + "\nSelected area is " + totArea1;// + "\nThe price of scenario " + curScenario2 + " is " + curCost2.ToString() + "\nSelected area is " + totArea2 + "\nTotal area: " + (totArea1 + totArea2) +"\nTotal cost: " + (curCost1 + curCost2).ToString();

        if (Input.GetMouseButtonUp(1) && Input.GetKey(KeyCode.LeftControl)) //right click and ctrl
        {
            if (listCustom.Contains(selectedObject))
            {
                listCustom.Remove(selectedObject);
                changeMatScript.ChangeMaterialClick(defMatWhite, selectedObject);
            }
            else
            {
                listCustom.Add(selectedObject);
            }
            foreach (GameObject go in listCustom)
            {
                changeMatScript.ChangeMaterialClick(defMat, go);
            }
        }   
        
        if(changeMatScript.functionReplaceCalled == true)
        {
            if (listCustom.Contains(selectedObject))
            {
                foreach (GameObject go in listCustom)
                {
                    changeMatScript.ChangeMaterialClick(selectedObject.GetComponent<Renderer>().material,go);
                }
            }
            changeMatScript.functionReplaceCalled = false;
        }
    }

    void OnApplicationQuit()
    {
        Debug.Log("Application ending after " + Time.time + " seconds");
        CreateCSV("Test");
    }
    void CreateCSV(string fileName)
    {

        string path = "C:/Users/cdri/Documents" + "/" + fileName + ".csv";
        File.SetAttributes(path, File.GetAttributes(path) & ~FileAttributes.ReadOnly);
        if (File.Exists(path))
        {
            File.WriteAllText(path, String.Empty);
            //File.Delete(path);
        }

        var sr = File.CreateText(path);

        string data = textCosts.text;

        sr.WriteLine(data);

        FileInfo fInfo = new FileInfo(path);
        fInfo.IsReadOnly = true;

        sr.Close();

        Application.OpenURL(path);
    }

}
