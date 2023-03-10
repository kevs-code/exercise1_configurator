using System.Collections.Generic;
using UnityEngine;
using Garage.Specs;
using System;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public GameObject specificationParent;
    public GameObject cartParent;
    public GameObject importParent;
    public GameObject cameraCategory;
    public GameObject carCategory;
    public GameObject colourCategory;
    public GameObject carParent;
    public GameObject cameraParent;
    public GameObject rowPrefab;
    public GameObject importRowPrefab;
    public GameObject[] rotators;
    public Specification[] cars;
    public AudioList audioList;
    public Button audioButton;
    public float colorChangeDuration = 2.0f;

    private List<string> cameraList;
    private List<string> carList;
    private List<string> cartList;
    private List<string> specList;
    private List<string> statList;

    private Material[] myMaterials;
    private Transform myMesh;
    private Specification mySpecs;

    private TMPro.TextMeshProUGUI cameraText;
    private TMPro.TextMeshProUGUI carText;
    private TMPro.TextMeshProUGUI colourText;

    private int currentCar = 0;
    private int currentCamera = 0;
    private int currentColour = 0;
    private int refNumber = 0;
    

    private Vector3 currentPosition;
    private List<string> carsInScene;
    private List<string> carsToImport;
    private List<string> referenceCost;

    private List<String> bugatti = new List<string> { "Black", "Blue", "Gold", "Green", "Orange", "Red", "Rose Pink", "Soft Pink", "White" };
    private List<String> aston = new List<string> { "Dark Blue", "Green", "Orange", "White", "Dark Green", "Black", "Matte White", "Cyan" };
    private List<String> lamborghini = new List<string> { "Black", "Blue", "Green", "White", "Yellow" };
    private List<String> porsche = new List<string> { "White", "Black", "Blue", "Yellow", "Red" };
    private List<String> sport = new List<string> { "Yellow", "Blue", "Red", "Grey", "Purple" };
    private Dictionary<string, List<String>> colorNames = new Dictionary<string, List<String>>();

    public object ImportOnClick { get; private set; }

    void Start()
    {
        MusicPlayer();
        SoundPlayer();
        ColorDictionary();
        GetCars(carParent);
        EnableRotationScripts();
        GetGUILabels();
        GetMenuOptions();
        GetActiveSettings();
        GetSpecifications();
    }
    private void SoundPlayer()
    {
        AudioSource audioSource = carParent.GetComponent<AudioSource>();

        GameObject[] gameObjects = new GameObject[] { cameraCategory, carCategory, colourCategory };
        foreach (GameObject gameObject in gameObjects)
        {
            Button[] buttons = gameObject.GetComponentsInChildren<Button>();
            foreach (Button button in buttons)
            {
                button.onClick.AddListener(() =>
                {
                    SoundOnOff(audioSource);
                });

            }

        }
    }

    private void MusicPlayer()
    {
        AudioSource audioSource = cameraParent.GetComponent<AudioSource>();
        audioButton.onClick.AddListener(() =>
        {
            SoundOnOff(audioSource);
        });
    }

    public void SoundOnOff(AudioSource audioSource)
    {
        if (!audioSource.isPlaying)
        {
            if (audioSource.gameObject.name == "CarPool")
            {
               audioSource.clip = audioList.click;
            }
            audioSource.Play();
        }
        else
        {
            audioSource.Stop();
        }
    }

    private void ColorDictionary()
    {
        colorNames.Add("Bugatti", bugatti);
        colorNames.Add("Aston", aston);
        colorNames.Add("Porsche", porsche);
        colorNames.Add("Lamborghini", lamborghini);
        colorNames.Add("Sport", sport);
    }

    private void GetCars(GameObject parent)
    {
        carsToImport =  new List<string>();
        carsInScene = new List<string>();
        referenceCost = new List<string>();

        foreach (Transform child in parent.transform)
        {
            carsInScene.Add(child.name.ToString());
        }
        foreach (Specification car in cars)
        {
            if (!carsInScene.Contains(car.name))
            {
                carsToImport.Add(car.name);
            }
            referenceCost.AddRange(new string[] { car.name, car.cost.ToString() });
        }
        PopulateCarListFromParent(importParent, carsToImport);
    }
    
    private void PopulateCarListFromParent(GameObject parent, List<string> importCarList)
    {
        foreach (Transform child in parent.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < importCarList.Count; i++)
        {
            GameObject importRow = Instantiate<GameObject>(importRowPrefab, parent.transform);
            
            
            
            foreach (Transform child in importRow.transform)
            {
                if (child.name == "Car")
                {
                    TMPro.TextMeshProUGUI car = child.GetComponent<TMPro.TextMeshProUGUI>();
                    car.text = importCarList[i];
                }

                if (child.name == "Import")
                {
                    child.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        TMPro.TextMeshProUGUI carLabel = importRow.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();
                        ImportCarOnClick(carLabel.text);
                    });
                }
            }
            
        }
    }

    private void ImportCarOnClick(string carName)
    {
        foreach (Specification car in cars)
        {
            if (car.name == carName)
            {
                foreach (Transform child in carParent.transform)
                {
                    child.gameObject.SetActive(false);
                }
                carText.text = carName;
                ResetTransform();
                GameObject carAdded = Instantiate<GameObject>(car.parentCar, carParent.transform);//bool instantiateInWorldSpace not used
                carAdded.name = carName;
                carAdded.transform.position = car.myPosition;
                carAdded.transform.Rotate(car.myRotation.x, car.myRotation.y, car.myRotation.z, Space.World);//Space.Self both spaces work but y=-145 inspector
                GetMenuOptions();
                SetCurrentCarNum(carList.Count - 1);
                CarUpdater();
                ResetColor();
                carsToImport.Remove(carName);
                PopulateCarListFromParent(importParent, carsToImport);
            }

        }

    }

    private void ResetColor()
    {
        refNumber = 0;
        SetCurrentColNum(0);
        myMesh.GetComponent<MeshRenderer>().material = myMaterials[GetCurrentColNum()];
        ColorUpdater();
    }

    private void EnableRotationScripts()
    {
        currentPosition = rotators[0].transform.position;
        foreach (GameObject rotator in rotators)
        {
            RotateGO item = rotator.GetComponent<RotateGO>();
            item.enabled = true;
        }
    }

    private void ResetTransform()
    {
        rotators[0].transform.position = currentPosition;
        rotators[0].transform.rotation = new Quaternion(0, 0, 0, 1);
        rotators[1].transform.position = new Vector3(0, 0, 0);
        rotators[1].transform.rotation = new Quaternion(0, 0, 0, 1);
    }

    private void GetMenuOptions()
    {
        carList = PopulateListFromParent(carParent);
        PopulateSpecFromCarParent(carParent);
        cameraList = PopulateListFromParent(cameraParent);
    }
    private void GetSpecifications()
    {
        statList = PopulateStatList();
        specList = PopulateSpecificationsListFromParent(specificationParent);
        cartList = PopulateSpecificationsListFromParent(cartParent);
    }

    private void GetActiveSettings()
    {
        cameraText.text = cameraList[GetCurrentCamNum()];
        carText.text = carList[GetCurrentCarNum()];
        colourText.text = colorNames[carList[GetCurrentCarNum()]][GetCurrentColNum()];
        myMesh.GetComponent<MeshRenderer>().material = myMaterials[GetCurrentColNum()];
    }

    private void GetGUILabels()
    {
        cameraText = cameraCategory.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        carText = carCategory.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        colourText = colourCategory.GetComponentInChildren<TMPro.TextMeshProUGUI>();
    }

    private void UpdateCar()
    {
        foreach (Transform child in carParent.transform)
        {
            child.gameObject.SetActive(false);
        }

        foreach (Transform child in carParent.transform)
        {
            if (child.name == carText.text)
            {
                child.gameObject.SetActive(true);   
            }
        }
    }


    private void UpdateCamera()
    {
        foreach (Transform child in cameraParent.transform)
        {
            child.gameObject.SetActive(false);
        }

        foreach (Transform child in cameraParent.transform)
        {
            if (child.name == cameraText.text)
            {
                RotateMainCamera(child);
                child.gameObject.SetActive(true);
            }
        }
    }

    private void RotateMainCamera(Transform child)
    {
        if (child.name == "Main Rotate")
        {
            foreach (GameObject rotator in rotators)
            {
                RotateGO rotateItem = rotator.GetComponent<RotateGO>();
                rotateItem.enabled = true;
            }
        }
        else
        {
            foreach (GameObject rotator in rotators)
            {
                RotateGO rotateItem = rotator.GetComponent<RotateGO>();
                rotateItem.enabled = false;
            }
            ResetTransform();
        }
    }

    private List<string> PopulateStatList()
    {
        List<string> statLabel = new List<string>();
        statLabel.AddRange(new string[] { "ENGINE:", "CARTYPE:", "COOL:", "MODABLE:", "COST:"});
        return statLabel;
    }

    private void GetMaterialsAndSpecs(Transform child)
    {
        myMaterials = child.GetComponent<ActiveCar>().specification.myMaterials;
        myMesh = child.GetComponent<ActiveCar>().check;
        mySpecs = child.GetComponent<ActiveCar>().specification;
    }

    private List<string> PopulateSpecificationsListFromParent(GameObject parent)
    {
        List<string> specList = new List<string>();
        if (mySpecs == null) { return specList; }

        foreach (Transform child in parent.transform)
        {
            Destroy(child.gameObject);
        }

        specList.AddRange(new string[] { mySpecs.engineClass, mySpecs.carType, mySpecs.coolFactor, mySpecs.modibility, mySpecs.cost.ToString() });

        for (int i = 0; i < specList.Count; i++)
        {
            GameObject row = Instantiate<GameObject>(rowPrefab, parent.transform);

            foreach (Transform child in row.transform)
            {
                if (child.name == "Stat")
                {
                    TMPro.TextMeshProUGUI stat = child.GetComponent<TMPro.TextMeshProUGUI>();
                    stat.text = statList[i];
                }
                if (child.name == "Value")
                {
                    TMPro.TextMeshProUGUI value = child.GetComponent<TMPro.TextMeshProUGUI>();
                    value.text = specList[i];
                }
            }
        }
        return specList;
    }

    private void CameraUpdater()
    {
        cameraText.text = cameraList[GetCurrentCamNum()];
        UpdateCamera();
    }

    private void CarUpdater()
    {
        carText.text = carList[GetCurrentCarNum()];
        UpdateCar();
        PopulateSpecs();
        ResetColor();
    }

    private void PopulateSpecs()
    {
        PopulateSpecFromCarParent(carParent);
        specList = PopulateSpecificationsListFromParent(specificationParent);
        cartList = PopulateSpecificationsListFromParent(cartParent);
    }

    private void ColorUpdater()
    {
        colourText.text = colorNames[carList[GetCurrentCarNum()]][GetCurrentColNum()];
        LetLerpMesh();
        ShowHideUI[] myToggles = FindObjectsOfType(typeof(ShowHideUI), true) as ShowHideUI[];//toggles including inactive
        if (GetCurrentColNum() != 0)
        {
            AddOneThousand();

            foreach (ShowHideUI toggle in myToggles)
            {
                if (toggle.gameObject.name == "PriceGUI")
                {
                    toggle.gameObject.SetActive(true);
                }
            }
        }
        else// if == 0 set at reference cost on entering play mode
        {
            foreach (ShowHideUI toggle in myToggles)
            {
                if (toggle.gameObject.name == "PriceGUI")
                {
                    toggle.gameObject.SetActive(false);
                }
            }
            ResetPlayModeCost();
        }
    }




    private void LetLerpMesh()
    {
        if (refNumber == GetCurrentColNum())
        {
            // Debug.Log("car imported");
            // moved myMesh.GetComponent<MeshRenderer>().material = myMaterials[GetCurrentColNum()];
            return;
        }
        if (mySpecs.name == "Sport" | mySpecs.name == "Aston")
        {
            // myMesh.GetComponent<MeshRenderer>().material.color = Color.red;
            // myMesh.GetComponent<MeshRenderer>().material.color = myMaterials[GetCurrentColNum()].color;//then color.lerp
            myMesh.GetComponent<MeshRenderer>().material = myMaterials[GetCurrentColNum()];
        }
        else
        {
            colorChangeDuration = 3.0f;
            Material material1 = myMaterials[refNumber];
            Material material2 = myMaterials[GetCurrentColNum()];
            Renderer rend = myMesh.GetComponent<MeshRenderer>();
            StartCoroutine(materialLerpIn(material1, material2, rend));
        }
    }

    IEnumerator materialLerpIn(Material material1, Material material2, Renderer rend)
    {
        for (float t = 0.01f; t < colorChangeDuration; t += 0.05f)
        {
            rend.material.Lerp(material1, material2, t / colorChangeDuration);
            yield return null;
        }
    }


private void ResetPlayModeCost()
    {
        for (int i = 0; i < referenceCost.Count; i++)
        {
            if (referenceCost[i] == mySpecs.name)
            {
                mySpecs.cost = int.Parse(referenceCost[i + 1]);// should be base cost
                PopulateSpecs();
            }
        }
    }

    private void AddOneThousand()
    {
        for (int i = 0; i < referenceCost.Count; i++)
        {
            if (referenceCost[i] == mySpecs.name)
            {
                if (mySpecs.cost == int.Parse(referenceCost[i + 1]))//no TryParse
                {
                    mySpecs.cost += 1000;
                }
                PopulateSpecs();
            }
        }
    }

    public List<string> PopulateListFromParent(GameObject parent)
    {
        List<string> options = new List<string>();
        int tempNum = 0;

        foreach (Transform child in parent.transform)
        {
            child.gameObject.SetActive(false);
        }

        foreach (Transform child in parent.transform)
        {
            if (tempNum == 0)
            {
                child.gameObject.SetActive(true);
            }
            tempNum++;
        }
        //
        foreach (Transform child in parent.transform)
        {
            options.Add(child.name);
        }
        return options;
    }

    public void PopulateSpecFromCarParent(GameObject parent)
    {
        foreach (Transform child in parent.transform)
        {
            if (child.gameObject.activeSelf)
            {
                if (parent != carParent) { continue; }
                GetMaterialsAndSpecs(child);
            }
        }
    }

    public int GetCurrentCarNum()
    {
        return currentCar;
    }

    public int SetCurrentCarNum(int count)
    {
        currentCar = count;
        return currentCar;
    }
    public int GetCurrentCamNum()
    {
        return currentCamera;
    }

    public int SetCurrentCamNum(int count)
    {
        currentCamera = count;
        return currentCamera;
    }
    public int GetCurrentColNum()
    {
        return currentColour;
    }

    public int SetCurrentColNum(int count)
    {
        currentColour = count;
        return currentColour;
    }

    public void PreviousCam()
    {
        int tempNumber = GetCurrentCamNum() - 1;

        if (tempNumber < 0)
        {
            SetCurrentCamNum(cameraList.Count - 1);
        }
        else
        {
            SetCurrentCamNum(tempNumber);
        }
        CameraUpdater();
    }

    public void NextCam()
    {
        int tempNumber = GetCurrentCamNum() + 1;
        if (tempNumber > cameraList.Count - 1)
        {
            SetCurrentCamNum(0);
        }
        else
        {
            SetCurrentCamNum(tempNumber);
        }
        CameraUpdater();
    }

    public void PreviousCar()
    {
        int tempNumber = GetCurrentCarNum() - 1;

        if (tempNumber < 0)
        {
            SetCurrentCarNum(carList.Count - 1);
        }
        else
        {
            SetCurrentCarNum(tempNumber);
        }
        CarUpdater();
    }

    public void NextCar()
    {
        int tempNumber = GetCurrentCarNum() + 1;
        if (tempNumber > carList.Count - 1)
        {
            SetCurrentCarNum(0);
        }
        else
        {
            SetCurrentCarNum(tempNumber);
        }
        CarUpdater();
    }
    public void PreviousColour()
    {
        refNumber = GetCurrentColNum();
        int tempNumber = GetCurrentColNum() - 1;

        if (tempNumber < 0)
        {
            SetCurrentColNum(myMaterials.Length - 1);
        }
        else
        {
            SetCurrentColNum(tempNumber);
        }
        ColorUpdater();
    }
    public void NextColour()
    {
        refNumber = GetCurrentColNum();
        int tempNumber = GetCurrentColNum() + 1;
        if (tempNumber > myMaterials.Length - 1)
        {
            SetCurrentColNum(0);
        }
        else
        {
            SetCurrentColNum(tempNumber);
        }
        ColorUpdater();
    }
}