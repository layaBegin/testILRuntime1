using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ABTools
{
    private static ABTools instance;

    public static ABTools Instance
    {
        get {
            if (null == instance)
            {
                instance = new ABTools();
            }
            return instance;
        }
    }


    private ABTools()
    {
        abDic = new Dictionary<string, AssetBundle>();//初始化字典
        abPath = Application.streamingAssetsPath + "/AssetBundle/";

        //如果要加载所有依赖项，首先要获取总的AB包，从AB包中加载所有的依赖信息
        if (singleAB == null)//如果没有总AB包，那么加载，如有，直接用
        {
            singleAB = AssetBundle.LoadFromFile(abPath + "AssetBundle");
        }

        if (singleManifest == null)
        {
            singleManifest = singleAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }
    }

    private Dictionary<string, AssetBundle> abDic;//存储加载的AB包，key就是ab包的名字，value就是加载进来的ab包

    private string abPath;//AB包的路径

    private AssetBundle singleAB;//总的ab包

    private AssetBundleManifest singleManifest;//存有所有依赖的文件

    /// <summary>
    /// 通过ab包的名字获取ab包
    /// </summary>
    /// <param name="abName">ab包的名字</param>
    /// <returns>返回ab包</returns>
    public AssetBundle GetABByName(string abName)
    {
        //先判断字典里有没有这个AB包，如果有加载所有的依赖项，直接用，如果没有，需要加载

        if (abDic.ContainsKey(abName))//就是判断这个字典里有没有这个abName的键
        {
            return abDic[abName];//如果有，直接返回字典的东西
        }
        else
        {
            return LoadAB(abName);//如果没有，那么重新去加载
        }
    }


    /// <summary>
    /// 卸载ab包
    /// </summary>
    /// <param name="abName"></param>
    public void RemoveAB(string abName)
    {
        //1.卸载执行的名字的ab包，从字典里
        //2.从字典里删除这个文件
    }



    /// <summary>
    /// 加载ab包
    /// </summary>
    /// <param name="abName">ab包的名字</param>
    /// <returns></returns>
    AssetBundle LoadAB(string abName)
    {
        //所有加载进来的AB包都存在字典里

        string[] deps = singleManifest.GetAllDependencies(abName);

        //循环加载依赖项
        for (int i = 0; i < deps.Length; i++)
        {
            //通过依赖信息加载依赖项

            //递归加载多层依赖
            LoadAB(deps[i]);
        }

        if (abDic.ContainsKey(abName))//判断字典里是否有这个ab包，如果有，直接用
        {
            return abDic[abName];
        }
        else
        {
            AssetBundle ab = AssetBundle.LoadFromFile(abPath + abName);//如果没有这个ab包，那么加载ab包，并添加到字典中
            abDic.Add(abName, ab);
            return ab;
        }
    }


    /*
     * 
     *   AB1 依赖于 AB2  AB2依赖于AB3    1：加载依赖项   2.加载自己
     *    
     *   Load(AB1) --  1:加载AB2  2.加载AB1
     *   
     *   1:加载AB2  2.加载AB1 -- 1加载AB3  2.加载AB2  3.加载AB1
     * 
     *   1加载AB3  2.加载AB2  3.加载AB1
     * 
     * 
     * */


    public void DebugLog()
    {
        foreach (var item in abDic.Keys)//变量字典的所有的键
        {
            Debug.Log(item);
        }
    }


    //斐波那契数列    1  1  2  3  5  8  13 21 34 55 89。。。。。。
    //根据斐波那契数列计算第几个是什么数
    public int Get(int num)
    {
        if (num == 0 || num == 1)
        {
            return 1;
        }
        else
        {
            //num = 2
            return Get(num - 2) + Get(num - 1);
        }
    }

    /// num 2
    /// Get(0) + Get(1)  ==   1 + 1

    /// num 3
    /// Get(1) + Get(2) ==  1 + Get(2)
    /// 1 + Get(2)  == 1 + Get(0) + Get(1);
    /// 1 + 1 + 1


}
