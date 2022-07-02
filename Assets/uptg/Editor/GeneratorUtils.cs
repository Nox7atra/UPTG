using System;
using System.IO;
using Nox7atra.UPTG.DataStructures;
using UnityEditor;
using UnityEngine;

namespace Nox7atra.UPTG
{
    public static class GeneratorUtils
    {
        public static string CreateFolder(string folder)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            return folder;
        }
        public static string CreateFolder(string root, string subPath)
        {
            var folder = Path.Combine(root, subPath);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            return folder;
        }

        public static string CreateFile(string path, string filename)
        {
            var fullPath = Path.Combine(path, filename);
            if (!File.Exists(fullPath))
            {
                File.Create(fullPath);
            }

            return fullPath;
        }

        public static void GenerateAsmdef(string path, string name, string[] includePlatforms = null)
        {
            if(File.Exists(path)) return;
            AsmdefStructure structure = new AsmdefStructure();
            structure.name = name;
            structure.includePlatforms = includePlatforms;
            File.WriteAllText(path, EditorJsonUtility.ToJson(structure, true));
        }
        public static string GetRoot(string packageName)
        {
            return Path.Combine(Application.dataPath, packageName);
        }

        public static string GetUnityVersion()
        {
            var parts = Application.unityVersion.Split(".");
            return $"{parts[0]}.{parts[1]}";
        }
        
        
        public static string GetUnityReleaseVersion()
        {
            var parts = Application.unityVersion.Split(".");
            return parts[2];
        }

        public static string ApplyTokensToText(
            string inputText,
            string author, 
            string description, 
            string displayName, 
            string email)
        {
            return inputText
                .Replace(Constants.Templates.AuthorToken, author)
                .Replace(Constants.Templates.EmailToken, email)
                .Replace(Constants.Templates.YearToken, DateTime.Now.Year.ToString())
                .Replace(Constants.Templates.NameToken, displayName)
                .Replace(Constants.Templates.DescriptionToken, description);
        }
    }
}