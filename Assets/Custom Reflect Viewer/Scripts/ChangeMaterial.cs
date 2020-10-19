using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.Reflect
{
    public class ChangeMaterial : MonoBehaviour
    {
        public Toggle toggle;
        public Text showText;
        GameObject selectedObject = null;

        public Material newMaterial;
        Material newMaterialCopy;
        public Image newMaterialCopyImage;
        

        GameObject root;// = GameObject.Find("Root");

        // Start is called before the first frame update
        void Start()
        {
            root = GameObject.Find("Root");
            newMaterialCopy = newMaterial;
            if (root != null)
            {
                Debug.Log("success");
            }
            else
            {
                Debug.Log("root is null");
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (toggle.isOn)
            {
                // instead of GameObject, could use custom type like ControllableUnit
                if (Input.GetMouseButtonUp(0)) //left click
                {
                    selectedObject = clickObjects();
                    Debug.Log(selectedObject.name);
                    var selectedMeta = selectedObject.GetComponent<Metadata>();
                    //Dictionary<string, Metadata.Parameter> = selectedMeta.GetParameters;
                    string selectedCostString = selectedMeta.GetParameter("Cost");
                    //float selectedCost = float.Parse(selectedMeta.GetParameter("Cost"));
                    showText.text = selectedObject.name + " with cost: " + selectedCostString;
                    newMaterialCopy = selectedObject.GetComponent<Renderer>().material;
                    newMaterialCopyImage.material = newMaterialCopy;
                }
                if (Input.GetMouseButtonUp(1) && Input.GetKey(KeyCode.LeftControl)) //right click
                {
                    selectedObject = clickObjects();
                    Debug.Log(selectedObject.name);
                    var selectedMeta = selectedObject.GetComponent<Metadata>();
                    string selectedCostString = selectedMeta.GetParameter("Cost");
                    //float selectedCost = float.Parse(selectedMeta.GetParameter("Cost"));
                    //showText.text = selectedObject.name + " with cost: " + selectedCostString;
                    if(Input.GetKey(KeyCode.LeftAlt))
                    {
                        selectedObject.GetComponent<MeshRenderer>().material = newMaterialCopy;
                    }
                    else
                    {
                        selectedObject.GetComponent<MeshRenderer>().material = newMaterial;
                    }
                }
            }
        }

        GameObject clickObjects()
        {
            GameObject target = null;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            //Debug.Log("Shooting.");
            if (Physics.Raycast(ray, out hit)) // you can also only accept hits to some layer and put your selectable units in this layer
            {
                //Debug.Log("Shooting.2");
                if (hit.transform != null && hit.transform.IsChildOf(root.transform))
                {
                    target = hit.transform.gameObject; 
                    //Debug.Log(target.name);
                }
            }
            return target;
        }
    }
}