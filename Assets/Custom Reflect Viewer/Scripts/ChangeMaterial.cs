﻿using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

namespace UnityEngine.Reflect
{
    public class ChangeMaterial : MonoBehaviour
    {
        public Toggle toggle;
        public Text showText;
        public GameObject selectedObject;

        public Material newMaterial;
        Material newMaterialCopy;
        public Image newMaterialCopyImage;

        public Text floatName;
        public Camera mainCam;
        public float[] floatNameOffset;
        public float[] floatImgOffset;
        private Vector3 hitPoint;

        public List<Material> matPoss;
        public List<Texture> texPossible;
        public List<Image> materialImages;

        float timeClick;

        GameObject root;// = GameObject.Find("Root");

        public int mortarWidth = 4;
        public Color mortarColor;

        public Color32[] pix;

        public RenderTexture rendTex;

        public Dropdown mortarSizeDrop;

        public GameObject replacementTest;

        public bool functionReplaceCalled;

        // Start is called before the first frame update
        void Start()
        {
            timeClick = Time.time;
            for (int i = 1; i < 300; i++)
            {
                Image tempImg = Instantiate(newMaterialCopyImage, newMaterialCopyImage.transform.parent);
                materialImages.Add(tempImg);
            }
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
            functionReplaceCalled = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (toggle.isOn)
            {
                if (selectedObject != null)
                {
                    //CreateUI(selectedObject);
                    matPoss = CreateUINew(selectedObject, 1);
                }
                // instead of GameObject, could use custom type like ControllableUnit
                if (Input.GetMouseButtonDown(1)) //right click
                {
                    timeClick = Time.time;
                }
                if ((Input.GetMouseButtonUp(1) && Time.time - timeClick < 0.3f) || (Input.touchCount > 2 && Input.touches[2].phase == TouchPhase.Began)) //right click
                {
                    selectedObject = ClickObjects();
                    //Debug.Log(selectedObject.name);
                    var selectedMeta = selectedObject.GetComponent<Metadata>();
                    //Dictionary<string, Metadata.Parameter> = selectedMeta.GetParameters;
                    string selectedCostString = selectedMeta.GetParameter("Cost");
                    //float selectedCost = float.Parse(selectedMeta.GetParameter("Cost"));
                    showText.text = selectedObject.name;// + " with cost: " + selectedCostString;
                    newMaterialCopy = new Material(selectedObject.GetComponent<Renderer>().material);
                    newMaterialCopy.shader = Shader.Find("Unlit/Texture");
                    newMaterialCopyImage.material = newMaterialCopy;


                }
                if (Input.GetMouseButtonUp(1) && Input.GetKey(KeyCode.LeftControl)) //right click and ctrl
                {
                    selectedObject = ClickObjects();
                    //ReplaceObject();
                }
                if ((Input.touchCount > 2 && Input.touches[2].phase == TouchPhase.Began)) //triple touch
                {
                    selectedObject = ClickObjects();
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
                if (Input.GetKeyDown("e"))
                {
                    ToggleLight();
                }
                if (Input.GetKeyDown("r"))
                {
                    ToggleAllLight();
                }
            }
        }

        GameObject ClickObjects()
        {
            Ray ray;
            GameObject target = null;
            if (Input.touchCount > 2 && Input.touches[2].phase == TouchPhase.Began)
            {
                ray = Camera.main.ScreenPointToRay(Input.touches[2].position); //touch
            }
            else
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition); //Mouse
            }
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

        public List<Material> CreateUINew(GameObject go, int draw)
        {
            Vector3 imOffset = new Vector3(floatImgOffset[0], floatImgOffset[1], floatImgOffset[2]);
            var meta = go.GetComponent<Metadata>();
            List<Material> matPoss = new List<Material>();
            List<Texture> texPoss = new List<Texture>();
            if (go.name.Contains("Wall") || meta.GetParameter("Category").Contains("Wall")) //If it's a wall, show wall material options
            {
                matPoss.AddRange(Resources.LoadAll("Materials/Wall", typeof(Material)).Cast<Material>().ToList());
                //texPossible.AddRange(Resources.LoadAll("Materials/Tiles", typeof(Texture)).Cast<Texture>().ToList());
            }
            if (go.name.Contains("Floor") || meta.GetParameter("Category").Contains("Floor"))
            {
                matPoss.AddRange(Resources.LoadAll("Materials/Floor", typeof(Material)).Cast<Material>().ToList());
            }
            if (go.name.Contains("Window") || meta.GetParameter("Category").Contains("Window"))
            {
                matPoss.AddRange(Resources.LoadAll("Materials/Window", typeof(Material)).Cast<Material>().ToList());
            }
            if (go.name.Contains("Ceiling") || meta.GetParameter("Category").Contains("Ceiling"))
            {
                matPoss.AddRange(Resources.LoadAll("Materials/Wall", typeof(Material)).Cast<Material>().ToList());
            }
            float[] mortarWidthArray = { 0.01f, 0.03f, 0.1f };
            foreach (Texture tex in texPoss)
            {
                Material tempMat = new Material(Shader.Find("Custom/TileShader"));
                tempMat.mainTexture = tex;
                tempMat.SetFloat("_MortarSize", mortarWidthArray[mortarSizeDrop.value]);
                matPoss.Add(tempMat);
            }
            if (draw >= 1)
            {
                for (int i = 0; i < materialImages.Count(); i++)
                {
                    materialImages[i].transform.position = new Vector3(0f, -10000f, 0f);
                }
                if (matPoss.Count() >= 1)
                {
                    //Debug.Log("draw >= 1\n");
                    for (int i = 0; i < this.matPoss.Count(); i++)// Material mat in matPossible)
                    {
                        Material mat = new Material(this.matPoss[i]);
                        Material mat3D = new Material(this.matPoss[i]);
                        Image tempImg = materialImages[i];
                        int maxSqrt = Mathf.FloorToInt(Mathf.Sqrt(this.matPoss.Count()));
                        mat.shader = Shader.Find("UI/Default");
                        tempImg.material = mat;
                        RectTransform tempRect = (RectTransform)tempImg.transform;
                        tempImg.transform.position = mainCam.WorldToScreenPoint(hitPoint) + imOffset + new Vector3(0f + Mathf.Floor(i / maxSqrt) * (tempRect.rect.width + 40f), -(tempRect.rect.height + 40f) * (i - Mathf.Floor(i / maxSqrt) * maxSqrt), 0f);
                        tempImg.GetComponent<Button>().onClick.AddListener(() => ChangeMaterialClick(mat3D, selectedObject));
                        materialImages[i] = tempImg;
                    }
                }
            }
            return matPoss;
        }

        public void ChangeMaterialClick(Material mat, GameObject selectedObject)
        {
            functionReplaceCalled = true;
            Texture2D texMort = (Texture2D) mat.mainTexture;
            mat.mainTexture = texMort;
            foreach (Renderer rend in selectedObject.GetComponents<Renderer>())
            {
                var mats = new Material[rend.sharedMaterials.Length];
                for (var j = 0; j < rend.sharedMaterials.Length; j++)
                {
                    mats[j] = mat;
                }
                rend.sharedMaterials = mats;
            }
            selectedObject.GetComponent<MeshRenderer>().material = mat;
        }

        public void ReplaceObject()
        {
            GameObject go = selectedObject;
            float sizeX = go.GetComponent<Renderer>().bounds.size.x;
            float sizeY = go.GetComponent<Renderer>().bounds.size.y;
            float sizeZ = go.GetComponent<Renderer>().bounds.size.z;
            Vector3 loc = go.GetComponent<Renderer>().bounds.center;
            Debug.Log(sizeX.ToString() + " " + sizeY.ToString() + " " + sizeZ.ToString());

            GameObject replGo = (GameObject) Instantiate(replacementTest, root.transform);
            replGo.transform.position = loc;// go.transform.position;
            //replGo.transform.rotation = go.transform.rotation;
            replGo.transform.localScale = new Vector3(sizeX, sizeY, sizeZ);
            //replGo.transform.position += new Vector3(0f, sizeY/2, 0f);
            Destroy(go);
            functionReplaceCalled = true;
        }

        public void ToggleLight()
        {
            GameObject go = selectedObject;
            var meta = go.GetComponent<Metadata>();
            if (meta.GetParameter("Category").Contains("Light"))
            {
                foreach (Transform child in go.transform)
                {
                    Light light = child.gameObject.GetComponent(typeof(Light)) as Light;
                    if (light.enabled)
                    {
                        light.enabled = false;
                    }
                    else
                    {
                        light.enabled = true;
                    }
                }
            }
        }

        public void ToggleAllLight()
        {
            GameObject root = GameObject.Find("Root");
            Transform[] transList = root.GetComponentsInChildren<Transform>();
            foreach (Transform allObj in transList)
            {
                GameObject go = allObj.gameObject;
                Debug.Log(go.name);
                var meta = go.GetComponent<Metadata>();
                if (meta != null && meta.GetParameter("Category").Contains("Light"))
                {
                    foreach (Transform child in go.transform)
                    {
                        Light light = child.gameObject.GetComponent(typeof(Light)) as Light;
                        if (light.enabled)
                        {
                            light.enabled = false;
                        }
                        else
                        {
                            light.enabled = true;
                        }
                    }
                }
            }
        }
    }
}