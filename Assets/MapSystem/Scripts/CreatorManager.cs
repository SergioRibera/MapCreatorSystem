using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CreatorManager : MonoBehaviour
{

    public static CreatorManager main;
    public List<PackageModel> imported;
    public List<PackageModel> creates;

    public PackageModel currentPackage;

    public TMP_InputField textNameMap;
    
    void Awake(){
        if(main == null)
            main = this;
    }

    void Start()
    {
        imported = Package.Load();
        creates = Package.Load(LoadPackage.Created);
        //currentPackage = creates[0];
        //Package.Import(currentPackage.name);
        //imported = Package.Load();
    }

    public void CreateMap(){
        string name = textNameMap.text;
        foreach(var c in creates)
            if(c.name == name)
                throw new Exception("Ya existe un mapa con este nombre en tu repositorio local");
        creates.Add(Package.Init(textNameMap.text));
    }
    public void Export(){
        Package.Make(currentPackage.name);
    }
}
