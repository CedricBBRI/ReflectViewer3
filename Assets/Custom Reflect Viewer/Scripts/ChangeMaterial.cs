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
        public List<Texture> texPossible;
        public List<Image> materialImages;

        float timeClick;

        GameObject root;// = GameObject.Find("Root");

        public int mortarWidth = 4;
        public Color mortarColor;

        public Color32[] pix;

        public RenderTexture rendTex;

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

        public void CreateUINew(GameObject go)
        {
            Vector3 imOffset = new Vector3(floatImgOffset[0], floatImgOffset[1], floatImgOffset[2]);
            var meta = go.GetComponent<Metadata>();
            matPossible = new List<Material>();
            texPossible = new List<Texture>();
            if (go.name.Contains("Wall") || meta.GetParameter("Category").Contains("Wall")) //If it's a wall, show wall material options
            {
                matPossible.AddRange(Resources.LoadAll("Materials/Wall", typeof(Material)).Cast<Material>().ToList());
                texPossible.AddRange(Resources.LoadAll("Materials/Tiles", typeof(Texture)).Cast<Texture>().ToList());
            }
            if (go.name.Contains("Floor") || meta.GetParameter("Category").Contains("Floor"))
            {
                matPossible.AddRange(Resources.LoadAll("Materials/Floor", typeof(Material)).Cast<Material>().ToList());
            }
            if (go.name.Contains("Window") || meta.GetParameter("Category").Contains("Window"))
            {
                matPossible.AddRange(Resources.LoadAll("Materials/Window", typeof(Material)).Cast<Material>().ToList());
            }
            if (go.name.Contains("Ceiling") || meta.GetParameter("Category").Contains("Ceiling"))
            {
                matPossible.AddRange(Resources.LoadAll("Materials/Wall", typeof(Material)).Cast<Material>().ToList());
            }
            for (int i = 0; i < materialImages.Count(); i++)
            {
                materialImages[i].transform.position = new Vector3(0f, -10000f, 0f);
            }
            foreach(Texture tex in texPossible)
            {
                Material tempMat = new Material(Shader.Find("Standard"));
                tempMat.mainTexture = tex;
                matPossible.Add(tempMat);
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
                    int maxSqrt = Mathf.FloorToInt(Mathf.Sqrt(matPossible.Count()));
                    mat.shader = Shader.Find("UI/Default");
                    tempImg.material = mat;
                    RectTransform tempRect = (RectTransform)tempImg.transform;
                    tempImg.transform.position = mainCam.WorldToScreenPoint(hitPoint) + imOffset + new Vector3(0f + Mathf.Floor(i/maxSqrt)* (tempRect.rect.width + 40f), -(tempRect.rect.height+40f) * (i- Mathf.Floor(i / maxSqrt)*maxSqrt), 0f);
                    tempImg.GetComponent<Button>().onClick.AddListener(() => ChangeMaterialClick(mat3D));
                    materialImages[i] = tempImg;
                }
            }
        }

        public void ChangeMaterialClick(Material mat)
        {
            Texture2D texMort = (Texture2D) mat.mainTexture;
            //texMort = AddMortar(texMort); //VERY SLOW WHY?
            //Graphics.Blit(texMort, rendTex, new Vector2(1.2f, 1.2f), new Vector2(0.05f, 0.05f));
            mat.mainTexture = texMort;
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


        public Texture2D AddMortarN(Texture2D tex)
        {
            Texture2D newTex = new Texture2D(tex.width+mortarWidth, tex.height+mortarWidth);// + mortarWidth, tex.height + mortarWidth);
            int i, j;
            newTex.SetPixels(tex.GetPixels());
            for (i = 0; i < newTex.width; i++)
            {
                for (j = 0; j < newTex.height; j++)
                {
                    if (i >= tex.width-mortarWidth || j >= tex.height-mortarWidth) //(i < (int)Mathf.Round(mortarWidth / 2f) || j < (int)Mathf.Round(mortarWidth / 2f) || i >= newTex.width - (int)Mathf.Round(mortarWidth / 2f) || j >= newTex.height - (int)Mathf.Round(mortarWidth / 2f))
                    {
                       newTex.SetPixel(i, j, mortarColor);
                    }
                    else
                    {
                        //newTex.SetPixels(tex.GetPixels());//(i, j, tex.GetPixel(i - (int)Mathf.Round(mortarWidth / 2f), j - (int)Mathf.Round(mortarWidth / 2f)));
                    }
                }
            }
            newTex.Apply();
            return newTex;
        }
        public Texture2D AddMortarN2(Texture2D tex)
        {
            Texture2D newTex = new Texture2D(tex.width, tex.height);// + mortarWidth, tex.height + mortarWidth);
            //pix = tex.GetPixels32();
            //newTex.SetPixels32(pix);
            //newTex.Apply();
            for (int i = 0; i < newTex.width; i++)
            {
                for (int j = 0; j < newTex.height; j++)
                {
                    if (i >= tex.width-mortarWidth || j >= tex.height-mortarWidth) //(i < (int)Mathf.Round(mortarWidth / 2f) || j < (int)Mathf.Round(mortarWidth / 2f) || i >= newTex.width - (int)Mathf.Round(mortarWidth / 2f) || j >= newTex.height - (int)Mathf.Round(mortarWidth / 2f))
                    {
                        newTex.SetPixel(i, j, mortarColor);
                    }
                    else
                    {
                        newTex.SetPixel(i, j, tex.GetPixel(i*(tex.width-mortarWidth)/tex.width, j* (tex.height - mortarWidth) / tex.height));
                    }
                }
            }
            newTex.Apply();
            return newTex;
        }
        public Texture2D AddMortar(Texture2D tex)
        {
            Texture2D newTex = new Texture2D(tex.width + mortarWidth, tex.height + mortarWidth);
            for (int i = 0; i < newTex.width; i++)
            {
                for (int j = 0; j < newTex.height; j++)
                {
                    if (i < (int)Mathf.Round(mortarWidth / 2f) || j < (int)Mathf.Round(mortarWidth / 2f) || i >= newTex.width - (int)Mathf.Round(mortarWidth / 2f) || j >= newTex.height - (int)Mathf.Round(mortarWidth / 2f))
                    {
                        newTex.SetPixel(i, j, mortarColor);
                    }
                    else
                    {
                        newTex.SetPixel(i, j, tex.GetPixel(i - (int)Mathf.Round(mortarWidth / 2f), j - (int)Mathf.Round(mortarWidth / 2f)));
                    }
                }
            }
            newTex.Apply();
            return newTex;
        }
        public Texture2D AddMortarCopyTexture(Texture2D tex)
        {
            Texture2D newTex = new Texture2D(tex.width + mortarWidth, tex.height + mortarWidth);
            Graphics.CopyTexture(tex, newTex);
            for (int i = 0; i < newTex.width; i++)
            {
                for (int j = 0; j < newTex.height; j++)
                {
                    if (i < (int)Mathf.Round(mortarWidth / 2f) || j < (int)Mathf.Round(mortarWidth / 2f) || i >= newTex.width - (int)Mathf.Round(mortarWidth / 2f) || j >= newTex.height - (int)Mathf.Round(mortarWidth / 2f))
                    {
                        newTex.SetPixel(i, j, mortarColor);
                    }
                    else
                    {
                        //newTex.SetPixel(i, j, tex.GetPixel(i - (int)Mathf.Round(mortarWidth / 2f), j - (int)Mathf.Round(mortarWidth / 2f)));
                    }
                }
            }
            newTex.Apply();
            return newTex;
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