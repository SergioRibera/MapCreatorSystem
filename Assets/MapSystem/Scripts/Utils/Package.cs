using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LoadPackage {
    Created,
    Imported
}

public static class Package
{
#region Paths
    static string _path = Application.persistentDataPath + "/maps/creates/";
    static string _pathBuild = Application.persistentDataPath + "/maps/builds/";
    static string _pathImports = Application.persistentDataPath + "/maps/imports/";
    static string _pathTemp = Application.persistentDataPath + "/tmp/";
#endregion


    static string[] _folders = new string[]{
        "models",
        "textures"
    };
    static string _ext = ".rat";

    public static PackageModel Init(string name){
        string path = _path + name + "/";
        Utils.DirExistsCreate(path);
        Utils.DirExistsCreate(_pathBuild);
        for(var i = 0;i < _folders.Length;i++)
            _folders[i] = path + _folders[i];
        Utils.MakeDirs(_folders);
        PackageModel pm = new PackageModel();
        pm.name = name;
        pm.Save(path);
        return pm;
    }
    public static List<PackageModel> Load(LoadPackage loaded = LoadPackage.Imported){
        string path = loaded == LoadPackage.Imported ? _pathImports : _path;
        List<PackageModel> pList = new List<PackageModel>();
        Utils.DirExistsCreate(path);
        Utils.ReadIndexs(path, (pm) => {
            pList.Add(pm);
        });
        return pList;
    }

    public static void Make(string name){
        string path = _path + name + "/";
        string outputFile = _pathBuild + name + _ext;
        Utils.ExistsDelete(outputFile);
        CreatorManager.main.StartCoroutine(MakePackage(path, outputFile));
    }
    public static void Import(string name){
        Utils.DirExistsCreate(_pathTemp);
        string path = _pathTemp + name + _ext;
        string outputDir = _pathImports + name + "/";
        Utils.DirExistsCreate(outputDir);
        CreatorManager.main.StartCoroutine(UnPackage(path, outputDir));
    }

    static IEnumerator MakePackage(string input, string output){
        yield return null;
        try{
            Utils.CompressFolder(input, output);
        }catch(Exception e){
            Debug.LogWarning(e.Message);
        }
    }
    static IEnumerator UnPackage(string input, string output){
        yield return null;
        try{
            Utils.DecompressToDir(input, output);
        }catch(Exception e){
            Debug.LogWarning(e.Message);
        }finally{
            Utils.DeleteFile(input);
        }
    }
}
