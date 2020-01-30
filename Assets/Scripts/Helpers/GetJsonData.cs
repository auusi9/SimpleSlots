using Newtonsoft.Json;
using UnityEngine;

public class GetJsonData<T>
{
    public static T GetJsonInfo(string path)
    {
        string json = Resources.Load(path).ToString();

        T myObject = JsonConvert.DeserializeObject<T>(json);

        return myObject;
    }
}

