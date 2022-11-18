// ================================================================================
//
//      作 者  :   G Q
//      时 间  :   2022年11月16日 15:40:47
//      类 名  :   SetCSTemplate
//      目 的  :   创建CS脚本时，自动增加该标题内容
//
// ================================================================================

using System;
using System.IO;

public class SetCSTemplate : UnityEditor.AssetModificationProcessor
{
    private static void OnWillCreateAsset(string path)
    {
        path = path.Replace(".meta", "");
        if (path.EndsWith(".cs"))
        {
            string allText = File.ReadAllText(path);
            allText = allText.Replace("#AuthorName#", "G Q")
                .Replace("#CreateTime#", DateTime.Now.ToString("F"));
            File.WriteAllText(path, allText);
        }
    }
}