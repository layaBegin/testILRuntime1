using UnityEngine;
using System.Collections;
using UnityEditor;

public class CreateAssetBundle : Editor {

    [MenuItem("AssetBundle/Create")]
    static void CreateAB()
    {
        Debug.Log("streamingAssetsPath:" + Application.streamingAssetsPath);
        BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath + "/AssetBundle", 
            BuildAssetBundleOptions.ForceRebuildAssetBundle, BuildTarget.StandaloneWindows64);
    }

}
