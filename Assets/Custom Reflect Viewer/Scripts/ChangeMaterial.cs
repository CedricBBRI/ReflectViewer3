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

        public Text floatName;
        public Camera mainCam;
        public float[] floatNameOffset;
        public float[] floatImgOffset;
        private Vector3 hitPoint;
        

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
                if (selectedObject != null)
                {
                    CreateUI(selectedObject);
                }
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
                    newMaterialCopy = new Material(selectedObject.GetComponent<Renderer>().material);
                    newMaterialCopy.shader = Shader.Find("Unlit/Texture");
                    newMaterialCopyImage.material = newMaterialCopy;
                }
                if (Input.GetMouseButtonUp(1) && Input.GetKey(KeyCode.LeftControl)) //right click
                {
                    GameObject selectedObjectLocal = clickObjects();
                    Debug.Log(selectedObjectLocal.name);
                    var selectedMeta = selectedObjectLocal.GetComponent<Metadata>();
                    string selectedCostString = selectedMeta.GetParameter("Cost");
                    //float selectedCost = float.Parse(selectedMeta.GetParameter("Cost"));
                    //showText.text = selectedObject.name + " with cost: " + selectedCostString;
                    if(Input.GetKey(KeyCode.LeftAlt))
                    {
                        selectedObjectLocal.GetComponent<MeshRenderer>().material = newMaterialCopy;
                    }
                    else
                    {
                        selectedObjectLocal.GetComponent<MeshRenderer>().material = newMaterial;
                    }
                }
            }
        }

        GameObject clickObjects()
        {
            GameObject target = null;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) // you can also only accept hits to some layer and put your selectable units in this layer
            {
                if (hit.transform != null && hit.transform.IsChildOf(root.transform))
                {
                    target = hit.transform.gameObject;
                    hitPoint = hit.point;// - ray.GetPoint(0.01f);
                }
            }
            return target;
        }

        public void CreateUI(GameObject go)
        {
            Vector3 nameOffset = new Vector3();
            nameOffset.x = floatNameOffset[0];
            nameOffset.y = floatNameOffset[1];
            nameOffset.z = floatNameOffset[2];
            Vector3 imOffset = new Vector3(floatImgOffset[0], floatImgOffset[1], floatImgOffset[2]);
            floatName.text = go.name;
            floatName.transform.position = mainCam.WorldToScreenPoint(hitPoint) + nameOffset;
            newMaterialCopyImage.transform.position = mainCam.WorldToScreenPoint(hitPoint) + imOffset;
            //newMaterialCopy.shader = ;
            Material tempMat = new Material(newMaterial);
            newMaterialCopyImage.material = tempMat;
            tempMat.shader = Shader.Find("Unlit/Texture");
            newMaterialCopyImage.GetComponent<Button>().onClick.AddListener(() => OnClick(newMaterial));
        }

        public void ChangeMaterialClick(Material mat)
        {
            selectedObject.GetComponent<MeshRenderer>().material = mat;
        }

        public void OnClick(Material mat)
        {
            ChangeMaterialClick(mat);
        }
    }
}