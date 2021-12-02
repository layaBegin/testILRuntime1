using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoadCube : MonoBehaviour {

    public AssetBundle single;
    public AssetBundleManifest manifest;
    public AssetBundle ab;//cube.ab
	// Use this for initialization
	void Awake () {
        //加载总的ab包
        single = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/AssetBundle/AssetBundle");
        //加载总ab里的manifest文件
        manifest = single.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        LoadDep("cube.ab");
        ab = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/AssetBundle/cube.ab");
        GameObject cubePrefab = ab.LoadAsset<GameObject>("Cube");
        Instantiate(cubePrefab);
    }

    List<string> list = new List<string>();
    void LoadDep(string abName)
    {
        string[] deps = manifest.GetAllDependencies(abName);
        for (int i = 0; i < deps.Length; i++)
        {
            if (!list.Contains(deps[i]))//只有没加载过才加载
            {
                Debug.Log(deps[i]);
                AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/AssetBundle/" + deps[i]);
                list.Add(deps[i]);
                LoadDep(deps[i]);//递归加载所有的多层依赖想
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
      
    }
}
