using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
//using ICSharpCode.SharpZipLib.Zip;
using System.Linq;


public static class EngineUtil {
    public static GameObject FindChild(Component com, string str) {
        if (com == null) return null;
        return FindChild(com.gameObject, str);
    }
    public static GameObject FindChild(GameObject go, string str) {
        if (go == null) return null;
        if (string.IsNullOrEmpty(str)) return go;
        Transform trans = go.transform.Find(str);
        if (trans == null) return null;
        return trans.gameObject;
    }
    public static object FindChild(Component com, string str, Type type) {
        if (com == null) return null;
        return FindChild(com.gameObject, str, type);
    }
    public static object FindChild(GameObject go, string str, Type type) {
        GameObject obj = FindChild(go, str);
        if (obj == null) return null;
        return obj.GetComponent(type);
    }
    public static Component GetComponent(Component com, Type type) {
        if (com == null) return null;
        return GetComponent(com.gameObject, type);
    }
    public static Component GetComponent(GameObject obj, Type type) {
        if (obj == null) return null;
        return obj.GetComponent(type);
    }
    public static GameObject GetGameObject(Component com) {
        return com == null ? null : com.gameObject;
    }
    public static GameObject GetGameObject(GameObject obj) {
        return obj;
	}

	// ---------------------------------------------------------------------------------
	// larrow
	public static void DestroyGameObject(GameObject obj) 
	{
		MonoBehaviour.Destroy (obj);
	}

	public static Material CreateTransparentMaterial()
	{
		Material mat = new Material (Shader.Find ("Standard"));
		mat.SetFloat("_Mode", 3);
		mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
		mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
		mat.SetInt("_ZWrite", 0);
		return mat;
	}

	public static Dictionary<string, Texture2D> UnzipTexturePackege(byte[] data, string encodeingName)
	{
		Dictionary<string, Texture2D> result = new Dictionary<string, Texture2D> ();

		if (data == null)
			return result;

        /*
		 * http://www.66acg.com/?post=174
		错误: CodePage 437 not supported

		原因:
		ICSharpCode.SharpZipLib.Zip.ZipConstants.DefaultCodePage默认为437(美国/加拿大英语)，如果被解压的文件不是437编码将报CodePage 437 not supported错误。

		解决方案:
		//根据项目中使用的编码，重设ZipConstants.DefaultCodePage的值。例如我的项目使用的是UTF8编码。
		ICSharpCode.SharpZipLib.Zip.ZipConstants.DefaultCodePage = System.Text.Encoding.UTF8.CodePage;
		*/
        // http://community.sharpdevelop.net/forums/t/21795.aspx

        System.Text.Encoding encoding = encoding = System.Text.Encoding.UTF8;
        if (!string.IsNullOrEmpty(encodeingName))
        {
            try
            {
                var tmp = System.Text.Encoding.GetEncoding(encodeingName);
                if (tmp != null)
                {
                    encoding = tmp;
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
        //ZipConstants.DefaultCodePage = encoding.CodePage;

		MemoryStream zipStream = new MemoryStream (data);
		//ZipInputStream zipInputStream = new ZipInputStream(zipStream);

		//ZipEntry zipEntry = zipInputStream.GetNextEntry();

		//while (zipEntry != null) {
		//	byte[] buf = new byte[zipEntry.Size];
		//	zipInputStream.Read(buf, (int)zipEntry.Offset, (int)zipEntry.Size);

		//	Texture2D tex = new Texture2D(0, 0);
		//	tex.LoadImage (buf);
		//	result.Add (zipEntry.Name, tex);

		//	zipEntry = zipInputStream.GetNextEntry();
		//}

        result = result.OrderBy(p => p.Key).ToDictionary(p => p.Key, o => o.Value);

        return result;
	}


	public static void ExternalCall(string funcName, string jsonString)
	{
		object[] param = new object[1];
		param [0] = jsonString;

		Application.ExternalCall(funcName, param);
	}
}
