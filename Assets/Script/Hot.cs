using UnityEngine;
using System.Collections;
using System.IO;

public class Hot : MonoBehaviour {


    private void Awake()
    {
        //1.打包
        //2.上传服务器，模拟，把新的ab包放在服务器的文件夹了，改变版本号，也就是改变txt的内容


        //3.判断版本号,不同就下载新的ab包
        
        if (CheckHot())
        {
            UpdateAsset();
            Debug.Log("需要更新");
        }
        else
        {
            Debug.Log("不需要更新");
        }
        
    }

    // Use this for initialization
    void Start () {

        /*
        string str = "abcdef";

        str = str.Replace("cd","dc");//参数一，要替换下去的字符串，参数二是要替换上来的字符串

        Debug.Log(str);
        */

      

        //4.解析ab包。使用ab包的内容
        AssetBundle ab = ABTools.Instance.GetABByName("hot.ab");

        GameObject prefab = ab.LoadAsset<GameObject>("Hot");

        GameObject obj = Instantiate(prefab, transform) as GameObject;
        obj.transform.localPosition = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
	
	}



    /// <summary>
    /// 判断版本号是否一致
    /// </summary>
    /// <returns></returns>
    bool CheckHot()
    {
        string current = "";
        string server = "";
        //获取当前的版本号
        StreamReader sr = new StreamReader(Application.streamingAssetsPath + "/AssetBundle/v.txt");
        try
        {
            current = sr.ReadToEnd();
        }
        catch (System.Exception)
        {

        }
        sr.Close();

        //获取服务器的版本号
        StreamReader sr1 = new StreamReader(Application.streamingAssetsPath + "/Server/v.txt");
        try
        {
            server = sr1.ReadToEnd();
        }
        catch (System.Exception)
        {

        }
        sr1.Close();

        if (current.Equals(server))//如果版本号一致，不需要更新
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// 更新资源
    /// </summary>
    void UpdateAsset()
    {
        //拷贝文件到本地
        //本地的所有文件
        
        string[] loacalFiles = Directory.GetFiles(Application.streamingAssetsPath +"/AssetBundle");

        for (int i = 0; i < loacalFiles.Length; i++)
        {
            Debug.Log(loacalFiles[i]);
            File.Delete(loacalFiles[i]);
        }

        //服务器的所有文件
        string[] serverFiles = Directory.GetFiles(Application.streamingAssetsPath + "/Server");

        for (int i = 0; i < serverFiles.Length; i++)
        {
            string newFilePath = serverFiles[i].Replace("Server", "AssetBundle");
            File.Copy(serverFiles[i], newFilePath);
        }

        //streamingAssets/AssetBundle/panel.ab

        //streamingAssets/Server/panel.ab

        //serverFiles[i].Replace("Server", "AssetBundle");

    }
}
