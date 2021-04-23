using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

namespace UnityEngine.Reflect
{
    public class FindAllObjects : MonoBehaviour  //Finds objects and makes them ready for editing and stuff?
    {

        Transform[] transformArr;
        public List<Transform> transformList; //List of all the transforms of all objects imported by Reflect
        public List<GameObject> objList; //List of all objects imported
        public List<Metadata> metaList; //List of all metadata
        public List<string> phases; //List of all phasing info
        public int numPhases = 0; //Number of phases
        public Dropdown dropDownPhases; //

        public Slider slider; //Slider to go between phases
        public Text sliderVal; //Name of current phase shown
        public Toggle prevToggle; //To select if to show phase alone or include previous phases as well

        public InputField sortBy;
        public Dropdown sortByDrop;
        //public Text sortCats;
        public Dropdown showOnly;

        List<string> keyList;
        List<string> keyList2;
        GameObject root; //GameObject under which all imported gameobjects are stored

        public float alpha;


        public void FindAll(InputField strInput)
        {
            if (transformList.Count == 0)
            {
                Initialize();
                string curPhase;
                foreach (Transform tr in transformList)
                {
                    GameObject go = tr.gameObject;
                    Debug.Log(go.name + "\n");
                    var meta = go.GetComponent<Metadata>();
                    if (go.transform.IsChildOf(root.transform) && meta != null && meta.GetParameters().Count() >= 1)
                    {
                        objList.Add(go);
                        if (!meta.GetParameter("Category").Contains("Door")){ //Adds collision boxes to all objects except those labeled as door
                            go.AddComponent<MeshCollider>();
                        }
                        metaList.Add(meta);
                        if (go.name.Contains(strInput.text)) //Find all elements whose name includes...
                        {
                            Debug.Log(go.name + "\n");
                        }
                        Dictionary<string, Metadata.Parameter> dict = meta.GetParameters();
                        curPhase = meta.GetParameter(dropDownPhases.captionText.text);
                        if (!phases.Contains(curPhase) && curPhase.Count() >= 1)
                        {
                            phases.Add(curPhase);
                        }
                    }
                }
                phases.Sort();
                numPhases = phases.Count; //number of phases

                if (numPhases == 0)
                {
                    numPhases = 1;
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