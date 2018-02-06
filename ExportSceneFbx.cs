
// 支持unity5.0之前的版本

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class ExportSceneFbx
{
    [MenuItem("Fanyoy/ExportSceneFbx")]
    public static void Export()
    {
        var exportpath = Application.dataPath + "/../export-fbx";
        if (Directory.Exists(exportpath))
        {
            Directory.Delete(exportpath, true);
        }
        Directory.CreateDirectory(exportpath);

        UnityEngine.Debug.Log("开始检索");
        var list = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (var iit in list)
        {
            //UnityEngine.Debug.Log("go.name=" + iit.gameObject.name);
            //continue;
            var allrenderer = iit.GetComponentsInChildren<Renderer>(true);
            foreach(var it in allrenderer)
            {
                if (it != null)
                {
                    var prefab = PrefabUtility.GetPrefabParent(it.transform.gameObject);
                    if (prefab == null)
                    {
                        // UnityEngine.Debug.LogError("can't find prefab:" + it.gameObject.name);
                        continue;
                    }
                    var prefabPath = AssetDatabase.GetAssetPath(prefab);

                    if (!prefabPath.ToLower().EndsWith(".fbx"))
                    {
                        continue;
                    }

                    var foldername = Path.GetFileNameWithoutExtension(prefabPath);
                    Directory.CreateDirectory(exportpath + "/" + foldername);
                    File.Copy(prefabPath, exportpath + "/" + foldername + "/" + Path.GetFileName(prefabPath), true);

                    var depends = AssetDatabase.GetDependencies(new string[] { prefabPath });

                    foreach (var mat in it.sharedMaterials)
                    {
                        // 检索出所有的贴图
                        List<Texture> allTexture = new List<Texture>();
                        Shader shader = mat.shader;
                        for (int i = 0; i < ShaderUtil.GetPropertyCount(shader); i++)
                        {
                            if (ShaderUtil.GetPropertyType(shader, i) == ShaderUtil.ShaderPropertyType.TexEnv)
                            {
                                Texture texture = mat.GetTexture(ShaderUtil.GetPropertyName(shader, i));
                                allTexture.Add(texture);
                            }
                        }

                        foreach (var t in allTexture)
                        {
                            var tex = AssetDatabase.GetAssetPath(t);
                            if (!tex.StartsWith("Resources") && tex.Length > 0)
                            {
                                File.Copy(tex, exportpath + "/" + foldername + "/" + Path.GetFileName(tex), true);
                            }
                        }
                    }
                }
            }
        }
        UnityEngine.Debug.Log("检索成功");
    }
}
