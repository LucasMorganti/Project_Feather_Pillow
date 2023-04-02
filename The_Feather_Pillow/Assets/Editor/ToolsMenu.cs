using UnityEngine;
using UnityEditor;
using System.IO;
using static System.IO.Directory;
using static System.IO.Path;
using static UnityEngine.Application;
using static UnityEditor.AssetDatabase;
using System.Security.Permissions;

namespace SoloDev_FeatherPillow
{
    public static class ToolsMenu
    {
        [MenuItem("Tools/Setup/Create Default Folders")]
        public static void CreateDefaultFolders()
        {
            Debug.Log(dataPath);

            CreateDirectories("_Project", "Scripts", "Art");
            Refresh();
        }

        public static void CreateDirectories(string root, params string[] dir)
        {
            var fullpath = Combine(dataPath, root);
            foreach (var newDirectory in dir)
            {
                CreateDirectory(Combine(fullpath, newDirectory));
            }
        }
    }
}
