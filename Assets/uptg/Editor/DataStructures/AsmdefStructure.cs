using System;
using System.Collections.Generic;

namespace Nox7atra.UPTG.DataStructures
{
    [Serializable]
    public class AsmdefStructure
    {
        public string name;
        public string[] references;
        public string[] optionalUnityReferences;
        public string[] includePlatforms;
        public string[] excludePlatforms;
        public bool allowUnsafeCode;
        public bool overrideReferences;
        public string[] precompiledReferences;
        public bool autoReferenced;
        public string[] defineConstraints;
    }
}