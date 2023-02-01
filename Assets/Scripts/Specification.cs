using UnityEngine;
using System.Collections.Generic;

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
        public Material[] myMaterials;
    }
}
