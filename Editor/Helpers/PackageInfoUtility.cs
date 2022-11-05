using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

public static class PackageInfoUtility
{
    public static PackageInfo GetPackageInfo(string packageName)
    {
        var path = $"Packages/{packageName}/package.json";
        
        var packageJsonAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
        
        Assert.IsNotNull(packageJsonAsset, $"Could not find package.json at path: {path}");
        
        return packageJsonAsset == null ? null : PackageInfo.FindForAssetPath(path);
    }
}