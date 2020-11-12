using System.Collections.Generic;
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

        public List<Material> matPossible;
        public List<Image> materialImages;

        float timeClick;

        GameObject root;// = GameObject.Find("Root");

        // Start is called before the first frame update
        void Start()
        {
            timeClick = Time.time;
            for (int i = 1; i < 100; i++)
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
        }

        // Update is called once per frame
        void Update()
        {
            if (toggle.isOn)
            {
                if (selectedObject != null)
                {
                    //CreateUI(selectedObject);
                    CreateUINew(selectedObject);
                }
                // instead of GameObject, could use custom type like ControllableUnit
                if (Input.GetMouseButtonDown(1)) //right click
                {
                    timeClick = Time.time;
                }
                if ((Input.GetMouseButtonUp(1) && Time.time - timeClick < 0.3f) || (Input.touchCount > 2 && Input.touches[2].phase == TouchPhase.Began)) //right click
                {
                    selectedObject = clickObjects();
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
                if ((Input.touchCount > 2 && Input.touches[2].phase == TouchPhase.Began)) //triple touch
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

        GameObject clickObjects()
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

        public void CreateUI(GameObject go)
        {

            Vector3 nameOffset = new Vector3(floatNameOffset[0], floatNameOffset[1], floatNameOffset[2]);
            Vector3 imOffset = new Vector3(floatImgOffset[0], floatImgOffset[1], floatImgOffset[2]);
            floatName.text = go.name;
            floatName.transform.position = mainCam.WorldToScreenPoint(hitPoint) + nameOffset;
            newMaterialCopyImage.transform.position = mainCam.WorldToScreenPoint(hitPoint) + imOffset;
            Material tempMat = new Material(newMaterial);
            newMaterialCopyImage.material = tempMat;
            tempMat.shader = Shader.Find("Unlit/Texture");
            newMaterialCopyImage.GetComponent<Button>().onClick.AddListener(() => ChangeMaterialClick(newMaterial));
        }

        public void CreateUINew(GameObject go)
        {
            Vector3 imOffset = new Vector3(floatImgOffset[0], floatImgOffset[1], floatImgOffset[2]);
            var meta = go.GetComponent<Metadata>();
            matPossible = new List<Material>();
            if (go.name.Contains("Wall") || meta.GetParameter("Category").Contains("Wall")) //If it's a wall, show wall material options
            {
                matPossible = Resources.LoadAll("Materials/Wall", typeof(Material)).Cast<Material>().ToList();
            }
            if (go.name.Contains("Floor") || meta.GetParameter("Category").Contains("Floor"))
            {
                matPossible = Resources.LoadAll("Materials/Floor", typeof(Material)).Cast<Material>().ToList();
            }
            if (go.name.Contains("Window") || meta.GetParameter("Category").Contains("Window"))
            {
                matPossible = Resources.LoadAll("Materials/Window", typeof(Material)).Cast<Material>().ToList();
            }
            if (go.name.Contains("Ceiling") || meta.GetParameter("Category").Contains("Ceiling"))
            {
                matPossible = Resources.LoadAll("Materials/Wall", typeof(Material)).Cast<Material>().ToList();
            }
            for (int i = 0; i < materialImages.Count(); i++)
            {
                materialImages[i].transform.position = new Vector3(0f, -10000f, 0f);
            }
            if(matPossible.Count() >= 1)
            {
                //Debug.Log("possible materials found");
                //Debug.Log(materialImages.Count().ToString() + " " + matPossible.Count().ToString());
                for (int i = 0; i < matPossible.Count(); i++)// Material mat in matPossible)
                {
                    Material mat = new Material(matPossible[i]);
                    Material mat3D = new Material(matPossible[i]);
                    Image tempImg = materialImages[i];
                    mat.shader = Shader.Find("UI/Default");
                    tempImg.material = mat;
                    RectTransform tempRect = (RectTransform)tempImg.transform;
                    tempImg.transform.position = mainCam.WorldToScreenPoint(hitPoint) + imOffset + new Vector3(0f, -(tempRect.rect.height+40f) * i, 0f);
                    tempImg.GetComponent<Button>().onClick.AddListener(() => ChangeMaterialClick(mat3D));
                    materialImages[i] = tempImg;
                }
            }
        }

        public void ChangeMaterialClick(Material mat)
        {

            foreach (Renderer rend in selectedObject.GetComponents<Renderer>())
            {
                var mats = new Material[rend.materials.Length];
                for (var j = 0; j < rend.materials.Length; j++)
                {
                    mats[j] = mat;
                }
                rend.materials = mats;
            }
            selectedObject.GetComponent<MeshRenderer>().material = mat;
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
        public void ReplaceMaterials(Material newMat)
        {

        }
    }
}