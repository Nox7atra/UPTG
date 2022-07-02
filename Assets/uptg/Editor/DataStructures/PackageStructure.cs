using System;

namespace Nox7atra.UPTG.DataStructures
{
    [Serializable]
    public class Author
    {
        public string name;
        public string email;
        public string url;
    }
    [Serializable]
    public class PackageStructure
    {
        public string name;
        public string version;
        public string displayName;
        public string description;
        public string unity;
        public string unityRelease;
        public Author author;
    }
}