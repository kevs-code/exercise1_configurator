using UnityEngine;
using System.Collections.Generic;
using System;

namespace Garage.Specs
{
    [CreateAssetMenu(fileName = "Specification", menuName = "New Specification", order = 0)]
    public class Specification : ScriptableObject
    {
        public string engineClass;
        public string carType;
        public string coolFactor;
        public string modibility;
        public int cost = 2000;
        public GameObject parentCar;
        public Material[] myMaterials;
        public Vector3 myPosition = new Vector3((float)-0.129999995, (float)-5.84588194, (float)-0.219999999);
        public Vector3 myRotation = new Vector3(0, (float)214.999985, 0);
        //Vector3(0,214.999985,0)
        //Quaternion(0,0.953716993,0,-0.300705761) //polar
    }
}

