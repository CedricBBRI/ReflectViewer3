using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Reflection;
using System.Linq;
using TMPro;

namespace UnityEngine.Reflect
{
    public class FindAllObjects : MonoBehaviour
    {

        Transform[] transformArr;
        public List<Transform> transformList;
        public List<GameObject> objList;
        public List<Metadata> metaList;
        public List<string> phases;
        public List<string> layers;
        public int numPhases = 0;
        public int numLayers = 0;
        public Dropdown dropDownPhases;

        public Slider slider;
        public Text sliderVal;
        public Toggle prevToggle;

        public InputField sortBy;
        public Dropdown sortByDrop;
        //public Text sortCats;
        public Dropdown showOnly;

        List<string> keyList;
        List<string> keyList2;
        GameObject root;

        public float alpha;

        public Text currentCostText;
        float currentCost;

        public void FindAll(InputField strInput)
        {
            if (transformList.Count == 0)
            {
                Initialize();
                string curPhase;
                currentCost = 0;
                foreach (Transform tr in transformList)
                {
                    GameObject go = tr.gameObject;
                    Debug.Log(go.name + "\n");
                    var meta = go.GetComponent<Metadata>();
                    if (go.transform.IsChildOf(root.transform) && meta != null && meta.GetParameters().Count() >= 1)// && go.GetComponent<MeshRenderer>() != null)
                    {
                        objList.Add(go);
                        if (!meta.GetParameter("Category").Contains("Door")){
                            go.AddComponent<MeshCollider>();
                        }
                        metaList.Add(meta);
                        if (go.name.Contains(strInput.text)) //Find all elements whose name includes...
                        {
                            Debug.Log(go.name + "\n");
                            //go.SetActive(false);
                        }
                        Dictionary<string, Metadata.Parameter> dict = meta.GetParameters();
                        //foreach (KeyValuePair<string, Metadata.Parameter> kvp in dict)
                        //{
                        //Debug.Log(kvp.Key);
                        //}
                        //currentCost += float.Parse(meta.GetParameter("Cost"));
                        curPhase = meta.GetParameter(dropDownPhases.captionText.text);
                        if (!phases.Contains(curPhase) && curPhase.Count() >= 1)
                        {
                            phases.Add(curPhase);
                        }
                        //phaseDict.Add(meta.GetParameter("Phase Created"), go);

                        //Debug.Log(meta.GetParameter("Phase Created"));
                    }
                    else
                    {
                        //transformList.Remove(tr);
                    }
                }
                phases.Sort();
                layers.Sort();
                numPhases = phases.Count;
                foreach (string str in phases)
                {
                    //Debug.Log(str);
                }
                //Debug.Log(numPhases);

                if (numPhases >= 1)
                {

                }
                else
                {
                    phases = layers;
                    numPhases = numLayers;

                }
                slider.maxValue = numPhases;
                slider.value = slider.maxValue;

                keyList = new List<string>(metaList[0].GetParameters().Keys);
                Debug.Log(metaList[0].GetParameters().Keys.Count());
                sortByDrop.ClearOptions();
                sortByDrop.AddOptions(keyList);

                UpdatePhasesShown();
            }
        }
        public void SortCategories()
        { 
            string sortVal = keyList[sortByDrop.value];
            List<string> categories = new List<string>();
            foreach (GameObject go in objList)
            {
                var meta = go.GetComponent<Metadata>();
                string sortValGo = meta.GetParameter(sortVal);
                Debug.Log(sortValGo);
                if (sortValGo.Length >= 1 && !categories.Contains(sortValGo)) //If the category doesn't exist yet
                {
                    categories.Add(sortValGo);
                    //catObj.name = sortValGo;
                }
            }

            showOnly.ClearOptions();
            keyList2 = categories;
            showOnly.AddOptions(keyList2);

            string sortCatsText = "list of " + sortVal + "\n";
            foreach(string str in categories)
            {
                Debug.Log(str);
                sortCatsText+= "\n" + str;
            }
            //sortCats.text = sortCatsText;

        }
        public void showOnlySelected()
        {
            string sortVal = keyList2[showOnly.value];
            string param = keyList[sortByDrop.value];
            foreach (GameObject go in objList)
            {
                var meta = go.GetComponent<Metadata>();
                string sortValGo = meta.GetParameter(param);
                Debug.Log(sortVal + " " + sortValGo);
                
                if (sortVal.Equals(sortValGo))
                {
                    go.SetActive(true);
                }
                else
                {
                    go.SetActive(false);
                }
            }
        }

        public void showAll()
        {
            foreach (GameObject go in objList)
            {
                go.SetActive(true);
            }
        }

        public void UpdatePhasesShown()
        {
            currentCost = 0;
            sliderVal.text = phases[(int)slider.value-1].ToString();
            float maxPhase = slider.value;
            foreach(GameObject go in objList)
            {
                var meta = go.GetComponent<Metadata>();
                int phase = phases.IndexOf(meta.GetParameter(dropDownPhases.captionText.text));
                if (prevToggle.isOn)
                {
                    if (phase >= maxPhase)
                    {
                        go.SetActive(false);
                    }
                    else
                    {
                        go.SetActive(true);
                        //currentCost += float.Parse(meta.GetParameter("Cost"));
                    }
                }
                else
                {
                    if (phase != maxPhase-1)
                    {
                        go.SetActive(false);
                    }
                    else
                    {
                        go.SetActive(true);
                        //currentCost += float.Parse(meta.GetParameter("Cost"));
                    }
                }
            }
            currentCostText.text = "Current cost: " + currentCost.ToString();
        }

        public void ClearLists()
        {
            transformList = new List<Transform>();
            objList = new List<GameObject>();
            metaList = new List<Metadata>();
            phases = new List<string>();
        }
        public void Initialize()
        {
            root = GameObject.Find("Root");
            transformArr = GameObject.FindObjectsOfType(typeof(Transform)) as Transform[];
            transformList = new List<Transform>(transformArr);
            objList = new List<GameObject>();
            metaList = new List<Metadata>();
            phases = new List<string>();
        }

        // Start is called before the first frame update
        void Start()
        {
            //Debug.Log("this is Start");
            slider.minValue = 1;
            slider.maxValue = 1;
            slider.value = 1;

        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}