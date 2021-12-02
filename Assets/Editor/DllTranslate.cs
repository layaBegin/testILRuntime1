using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class DllTranslate : Editor

{
    static string NormalPath = "";

    //[MenuItem("MyMenu/ILRuntime/DLL To byte[]")]
    //public static void DLLToBytes()
    //{
    //    DLLToBytes(true);
    //}
    [MenuItem("MyMenu/ILRuntime/DLL To byte[] (Choose Folder)")]
    public static void DLLToBytes_Choose()
    {
        DLLToBytes(false);
    }

    private static void DLLToBytes(bool autoChoosePath)
    {
        string folderPath;
        if (autoChoosePath)
            folderPath = NormalPath;
        else
            folderPath = EditorUtility.OpenFolderPanel("ѡ�� DLL ���ڵ��ļ���", Application.dataPath  , string.Empty);
        if (string.IsNullOrEmpty(folderPath)) return;

        DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
        if (directoryInfo == null) return;

        FileInfo[] fileInfos = directoryInfo.GetFiles();

        List<FileInfo> listDLL = new List<FileInfo>();
        List<FileInfo> listPDB = new List<FileInfo>();

        for (int i = 0; i < fileInfos.Length; i++)
        {
            if (fileInfos[i].Extension == ".dll")
            {
                listDLL.Add(fileInfos[i]);
            }
            else if (fileInfos[i].Extension == ".pdb")
            {
                listPDB.Add(fileInfos[i]);
            }
        }

        if (listDLL.Count + listPDB.Count <= 0)
        {
            Debug.Log("�ļ�����û��dll�ļ�");
            return;
        }
        else
        {
            Debug.Log("ѡ��·��Ϊ:" + folderPath);
        }

        string savePath;
        if (autoChoosePath)
            savePath = NormalPath;
        else
            savePath = EditorUtility.OpenFolderPanel("ѡ�� DLL ת���󱣴���ļ���", Application.dataPath + "/Addressable/ILRuntime", string.Empty);
        if (string.IsNullOrEmpty(savePath)) return;

        Debug.Log("---��ʼת�� DLL �ļ�------------------");
        string path = string.Empty;
        for (int i = 0; i < listDLL.Count; i++)
        {
            path = $"{savePath}/{Path.GetFileNameWithoutExtension(listDLL[i].Name)}_dll_res.bytes";
            BytesToFile(path, FileToBytes(listDLL[i]));
        }
        Debug.Log("---DLL �ļ�ת������------------------");

        Debug.Log("---��ʼת�� PDB �ļ�------------------");
        for (int i = 0; i < listPDB.Count; i++)
        {
            path = $"{savePath}/{Path.GetFileNameWithoutExtension(listPDB[i].Name)}_pdb_res.bytes";
            BytesToFile(path, FileToBytes(listPDB[i]));
        }
        Debug.Log("---PDB �ļ�ת������------------------");
        Debug.Log("����·��Ϊ:" + savePath);

        AssetDatabase.Refresh();
    }

    private static byte[] FileToBytes(FileInfo fileInfo)
    {
        return File.ReadAllBytes(fileInfo.FullName);
    }
    private static void BytesToFile(string path, byte[] bytes)
    {
        Debug.Log($"Path:{path}\nlength:{bytes.Length}");
        File.WriteAllBytes(path, bytes);
    }




   

}
