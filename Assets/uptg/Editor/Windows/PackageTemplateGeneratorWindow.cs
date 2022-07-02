using System.IO;
using Nox7atra.UPTG.DataStructures;
using UnityEditor;
using UnityEngine;

namespace Nox7atra.UPTG
{
    public class PackageTemplateGeneratorWindow : EditorWindow
    {
        private string _CompanyName;
        private string _PackageName;
        private string _Version;
        private string _DisplayName;
        private string _Description;
        private string _Email;
        private string _Website;
        private bool _HasEditorDependency;
        private bool _HasRuntimeDependency;
        private bool _HasTestsDependency;
        private bool _HasSamples;
        private bool _HasDocumentation;
        private TextAsset _LicenseTemplate;
        private TextAsset _ReadmeTemplate;
        
        private PackageStructure _PackageFile;

        private string _RuntimeGUID;
        private string _EditorGUID;
        
        [MenuItem("PackageTemplateGenerator/Show")]
        static void Init()
        {
            var window = (PackageTemplateGeneratorWindow) EditorWindow.GetWindow(typeof(PackageTemplateGeneratorWindow), false,"UPTG");
            window.Show();
        }

        private void OnGUI()
        {
            _PackageName = EditorGUILayout.TextField("package-name*", _PackageName);
            
            if (GUILayout.Button("Try Load Package Data"))
            {
                LoadPackageData();
            }
            _CompanyName = EditorGUILayout.TextField("company-name*", _CompanyName);
            _Version = EditorGUILayout.TextField("version", _Version);
            _DisplayName = EditorGUILayout.TextField("display-name", _DisplayName);
            _Description = EditorGUILayout.TextField("description", _Description);
            _Email = EditorGUILayout.TextField("email", _Email);
            _Website = EditorGUILayout.TextField("website", _Website);
            _HasEditorDependency = EditorGUILayout.Toggle("Has Editor Dependency?", _HasEditorDependency);
            _HasRuntimeDependency = EditorGUILayout.Toggle("Has Runtime Dependency?", _HasRuntimeDependency);
            _HasTestsDependency = EditorGUILayout.Toggle("Has Tests Dependency?", _HasTestsDependency);
            _HasSamples = EditorGUILayout.Toggle("Has Samples?", _HasSamples);
            _HasDocumentation = EditorGUILayout.Toggle("Has Documentation?", _HasDocumentation);
            
            _LicenseTemplate = EditorGUILayout.ObjectField("License Template", _LicenseTemplate, typeof(TextAsset), false) as TextAsset;
            _ReadmeTemplate = EditorGUILayout.ObjectField("Readme Template",_ReadmeTemplate, typeof(TextAsset),false) as TextAsset;
            if (GUILayout.Button("Generate Structure"))
            {
                Generate();
            }
        }

        private void LoadPackageData()
        {
            var filePath = Path.Combine(GeneratorUtils.GetRoot(_PackageName), Constants.PackageManifestFilename);
            if (File.Exists(filePath))
            {
                _PackageFile = JsonUtility.FromJson<PackageStructure>(File.ReadAllText(filePath));

                _Version = _PackageFile.version;
                _DisplayName = _PackageFile.displayName;
                _Description = _PackageFile.description;
                if (_PackageFile.author != null)
                {
                    _CompanyName = _PackageFile.author.name;
                    _Email = _PackageFile.author.email;
                    _Website = _PackageFile.author.url;
                }
            }
        }
        private void Generate()
        {
            if (string.IsNullOrEmpty(_PackageName))
            {
                Debug.LogError("package-name can't be empty");
                return;
            }
            if (string.IsNullOrEmpty(_CompanyName))
            {
                Debug.LogError("company-name can't be empty");
                return;
            }

            if (_PackageFile == null)
            {
                _PackageFile = new PackageStructure();
            }
            GenerateFoldersAndFiles();
            GeneratePackageFile();
            GenerateFromTemplate(_ReadmeTemplate, Constants.ReadmeFilename);
            GenerateFromTemplate(_LicenseTemplate, Constants.LicenseFilename);
            AssetDatabase.Refresh();
        }
    
        private void GenerateFoldersAndFiles()
        {
            var root = GeneratorUtils.GetRoot(_PackageName);
            GeneratorUtils.CreateFolder(root);
            
                        
            if (_HasRuntimeDependency)
            {
                var folder = GeneratorUtils.CreateFolder(root, Constants.RuntimeFolderName);
                var asmdefName = $"{_CompanyName}.{_PackageName}.";
                GeneratorUtils.GenerateAsmdef(
                    Path.Combine(folder, $"{asmdefName}.asmdef"), 
                    asmdefName
                );

            }
            
            if (_HasEditorDependency)
            {
                var folder = GeneratorUtils.CreateFolder(root, Constants.EditorFolderName);
                var asmdefName = $"{_CompanyName}.{_PackageName}.{Constants.EditorFolderName}";
                GeneratorUtils.GenerateAsmdef(
                    Path.Combine(folder, $"{asmdefName}.asmdef"), 
                    asmdefName,
                    new []{Constants.EditorFolderName}
                );
            }

            
            if (_HasTestsDependency)
            {
                if (_HasRuntimeDependency)
                {
                    var folder =  GeneratorUtils.CreateFolder(root, Path.Combine(Constants.TestsFolderName, Constants.RuntimeFolderName));
                    var asmdefName = $"{_CompanyName}.{_PackageName}.{Constants.TestsFolderName}";
                    GeneratorUtils.GenerateAsmdef(
                        Path.Combine(folder, $"{asmdefName}.asmdef"), 
                        asmdefName
                    );
                }

                if (_HasEditorDependency)
                {
                    var folder =  GeneratorUtils.CreateFolder(root, Path.Combine(Constants.TestsFolderName, Constants.EditorFolderName));
                    var asmdefName = $"{_CompanyName}.{_PackageName}.{Constants.EditorFolderName}.{Constants.TestsFolderName}";
                    GeneratorUtils.GenerateAsmdef(
                        Path.Combine(folder, $"{asmdefName}.asmdef"), 
                        asmdefName,
                        new []{Constants.EditorFolderName}
                    );

                }
            }

            if (_HasSamples)
            {
                var folder = GeneratorUtils.CreateFolder(root, Constants.SamplesFolderName);
                
            }
            
            if (_HasDocumentation)
            {
                var folder =  GeneratorUtils.CreateFolder(root, Constants.DocumentationFolderName);
                var file = GeneratorUtils.CreateFile(folder, $"{_PackageName}.md");
            }
        }

        private void GenerateFromTemplate(TextAsset textAsset, string filename)
        {
            if(textAsset == null) return;
            File.WriteAllText(Path.Combine(GeneratorUtils.GetRoot(_PackageName),filename), 
                GeneratorUtils.ApplyTokensToText(
                    textAsset.text,
                    _CompanyName,
                    _Description,
                    _DisplayName,
                    _Email));
        } 
        private void GeneratePackageFile()
        {
            _PackageFile.name = $"com.{_CompanyName}.{_PackageName}";

            _PackageFile.version = _Version;
            _PackageFile.displayName = _DisplayName;
            _PackageFile.description = _Description;
            _PackageFile.unity = GeneratorUtils.GetUnityVersion();
            _PackageFile.unityRelease = GeneratorUtils.GetUnityReleaseVersion();
            
            _PackageFile.author = new Author();
            _PackageFile.author.name = _CompanyName;
            _PackageFile.author.email = _Email;
            _PackageFile.author.url = _Website;
            var filePath = Path.Combine(GeneratorUtils.GetRoot(_PackageName), Constants.PackageManifestFilename);
            var content = EditorJsonUtility.ToJson(_PackageFile, true);
            File.WriteAllText(filePath, content);
        }
    }
}
