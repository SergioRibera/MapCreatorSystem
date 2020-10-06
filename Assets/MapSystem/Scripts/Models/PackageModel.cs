using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[Serializable]
public class PackageModel
{
    public string name;
    public string idCreator;
    public string idPackage;
    public List<ObjectMap> objectsMap;
    public List<string> importedFiles;
    
    public PackageModel(){
        this.idCreator = "";
        this.idPackage = "";
        this.objectsMap = new List<ObjectMap>();
        this.importedFiles = new List<string>();
    }

    public static PackageModel Load(string filePath) =>
        JsonConvert.DeserializeObject<PackageModel>(Utils.ReadFile(filePath));

    public void Save(string folder){
        string json = JsonConvert.SerializeObject(this);
        Utils.WriteFile(folder + "/index.json", json);
    }
}
