using System.Collections.Generic;
using UnityEngine;
using Garage.Specs;
using System;

public class UIManager : MonoBehaviour
{
    public GameObject specificationParent;
    public GameObject cartParent;
    public GameObject cameraCategory;
    public GameObject carCategory;
    public GameObject colourCategory;
    public GameObject carParent;
    public GameObject cameraParent;
    public GameObject rowPrefab;

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


    // Let's have a look at this rusty code #1
    void Start()
    {
        GetGUILabels();
        GetMenuOptions();
        GetActiveSettings();
        GetSpecifications();
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
        colourText.text = myMaterials[GetCurrentColNum()].ToString();
        myMesh.GetComponent<MeshRenderer>().material = myMaterials[GetCurrentColNum()];
    }

    private void GetGUILabels()
    {
        cameraText = cameraCategory.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        carText = carCategory.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        colourText = colourCategory.GetComponentInChildren<TMPro.TextMeshProUGUI>();
    }

    // this is the same method so add call parameters uses carparent and cartext
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
                child.gameObject.SetActive(true);
            }
        }
    }
// method ends

    private List<string> PopulateStatList()
    {
        List<string> statLabel = new List<string>();
        statLabel.AddRange(new string[] { "ENGINE:", "CARTYPE:", "COOL:", "MODABLE:", "COST:"});
        return statLabel;
    }

//
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
        PopulateSpecFromCarParent(carParent);
        specList = PopulateSpecificationsListFromParent(specificationParent);
        cartList = PopulateSpecificationsListFromParent(cartParent);
    }

    private void ColourUpdater()
    {
        colourText.text = myMaterials[GetCurrentColNum()].ToString();
        myMesh.GetComponent<MeshRenderer>().material = myMaterials[GetCurrentColNum()];
    }

    public List<string> PopulateListFromParent(GameObject parent)
    {
        List<string> options = new List<string>();
        int tempNum = 0;
        // same method new tempNum exists a signature maybe and polymorphism overide nah
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

    // there is a better way, so much getters and setters code for such a simple thing as tracking 3 integers!
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
        int tempNumber = GetCurrentColNum() - 1;

        if (tempNumber < 0)
        {
            SetCurrentColNum(myMaterials.Length - 1);
        }
        else
        {
            SetCurrentColNum(tempNumber);
        }
        ColourUpdater();
    }
    public void NextColour()
    {
        int tempNumber = GetCurrentColNum() + 1;
        if (tempNumber > myMaterials.Length - 1)
        {
            SetCurrentColNum(0);
        }
        else
        {
            SetCurrentColNum(tempNumber);
        }
        ColourUpdater();
    }
}