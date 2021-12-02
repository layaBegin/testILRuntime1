using UnityEngine;
using System.Collections;

public class LoadPanel : MonoBehaviour {

    public AssetBundle abDep;
    public AssetBundle ab;

    public GameObject prefab;

    public AssetBundle abSingle;
    public AssetBundleManifest manifestSingle;

    private void Awake()
    {
        //如果要使用prefab.ab，那么必须把prefab.ab的依赖的ab包加载进来

        //abDep = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/AssetBundle/scene.normal");
        abSingle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/AssetBundle/AssetBundle");

        manifestSingle = abSingle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

        string[] dep = manifestSingle.GetAllDependencies("prefab.ab");

        foreach (var item in dep)
        {
            abDep = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/AssetBundle/" + item);
        }

        ab = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/AssetBundle/prefab.ab");

       

        prefab = ab.LoadAsset<GameObject>("Panel");

        GameObject obj = Instantiate(prefab, transform) as GameObject;

        obj.transform.localPosition = Vector3.zero;
    }

    // Use this for initialization
    void Start () {

       


        

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("卸载ab包");
            ab.Unload(false);
            abDep.Unload(false);
        }
    }
}
