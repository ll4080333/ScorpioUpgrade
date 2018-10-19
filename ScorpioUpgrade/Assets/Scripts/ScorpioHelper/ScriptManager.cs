using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Scorpio;

public class ScriptManager
{
    private static ScriptManager instance = null;

    public event System.Action<string> CatchInfoEvent;
    public event System.Action<string> PrintInfoEvent;

    //Dictionary<string, Uinnova.BaseObject> objectsBornInScript = new Dictionary<string,Uinnova. BaseObject>();

    public static ScriptManager GetInstance() { return instance ?? (instance = new ScriptManager()); }
    //pzx start 尝试解决js调用跟3d运行速度不同步的问题
    public delegate void DestroyAllObjectDelegate();
    public DestroyAllObjectDelegate DestroyAllObjectCallback = null;
    //pzx end
    string configPath;
    string scriptPath;

    private Script m_Script = null;         //脚本引擎
    public ScriptManager()
    {
        ScorpioDelegate.ScorpioDelegateFactory.Initialize();
        m_Script = new Script();
        m_Script.LoadLibrary();
        //m_Script.PushAssembly(typeof(VideoPanelsManager).Assembly); 
        //m_Script.PushAssembly(typeof(DG.Tweening.DOTween).Assembly);
        //m_Script.PushAssembly(typeof(TMPro.TextMeshProUGUI).Assembly);
        //m_Script.PushAssembly(typeof(TMPro.TextAlignmentOptions).Assembly);
        //m_Script.PushAssembly(typeof(iTween).Assembly); 
        //m_Script.PushAssembly(typeof(PathUtils).Assembly);
        //m_Script.PushAssembly(typeof(DevConfig).Assembly);
        m_Script.PushAssembly(typeof(WWW).Assembly);
        m_Script.PushAssembly(typeof(Time).Assembly);
        m_Script.PushAssembly(typeof(System.Text.Encoding).Assembly);
        m_Script.PushAssembly(typeof(LitJson.JsonMapper).Assembly);
        m_Script.PushAssembly(typeof(LitJson.JsonData).Assembly);
        //m_Script.PushAssembly(typeof(CommonUtil).Assembly);
        //m_Script.PushAssembly(typeof(Uinnova.ObjectUtil).Assembly);        
        m_Script.PushAssembly(typeof(System.Action).GetTypeInfo().Assembly);                        //System.Core.dll
        m_Script.PushAssembly(typeof(GameObject).GetTypeInfo().Assembly);                           //UnityEngine.dll
        m_Script.PushAssembly(typeof(UnityEngine.UI.CanvasScaler).GetTypeInfo().Assembly);          //UnityEngine.UI.dll
        m_Script.PushAssembly(typeof(UnityEngine.Networking.MsgType).GetTypeInfo().Assembly);       //UnityEngine.Networking.dll
        m_Script.SetObject("print",m_Script.CreateFunction(new ScriptPrint()));                    //载入print函数
        m_Script.SetObject("loadfile",m_Script.CreateFunction(new ScriptLoadScript(m_Script)));    //载入loadfile函数  根据自己需求自己修改，如果是普通路径可以查看 require 函数
        configPath = Application.streamingAssetsPath + "/Scripts/config.js";
        scriptPath = Application.streamingAssetsPath + "/Scripts/";
    }
    public Script GetScript() { return m_Script; }
    public string GetStackInfo() { return m_Script != null ? m_Script.GetStackInfo() : ""; }

    public void Start(ScriptLaunch.Value[] values)
    {
        ScriptTable objs = m_Script.CreateTable();
        foreach(var val in values)
        {
            objs.SetValue(val.name,m_Script.CreateObject(val.obj));
        }
        m_Script.SetObject("objs",objs);
    }

    public ScriptObject LoadString(string str)
    {
#if !UNITY_EDITOR || SCRIPTTEST
        try {
            return m_Script.LoadString(str);
        } catch (Scorpio.Exception.ScriptException e) {
            if (CatchInfoEvent != null)
                CatchInfoEvent(e.ToString());
        }

        return null;
#else
        return m_Script.LoadString(str);
#endif

    }

    public ScriptObject LoadFile(byte[] text)
    {
        try
        {
            if(text != null)
            {
                //第一个参数是脚本摘要 有需求可以自己定义
                return m_Script.LoadBuffer("Load_www_File",text);
            }
        }
        catch(Scorpio.Exception.ScriptException e)
        {
            if(CatchInfoEvent != null)
                CatchInfoEvent(e.ToString());
        }

        return null;
    }

    public IEnumerator LoadScriptFilesInWeb()
    {
        string jsScriptConfigPath = configPath;
        if(jsScriptConfigPath.IndexOf("http") == -1)
        {
            jsScriptConfigPath = "file:///" + jsScriptConfigPath;
        }
        WWW www = new WWW(jsScriptConfigPath);
        yield return www;
        string configStr = www.text;
        List<string> scripts = new List<string>();
        if(!string.IsNullOrEmpty(configStr))
        {
            LitJson.JsonData jsonData = LitJson.JsonMapper.ToObject(configStr);
            if(jsonData.IsArray)
            {
                for(int i = 0; i < jsonData.Count; i++)
                {
                    string fileName = jsonData[i].ToString();
                    scripts.Add(fileName);
                }
            }

            string scriptStr;
            for(int j = 0; j < scripts.Count; j++)
            {

                string jsFile = Application.streamingAssetsPath + "/Scripts/" + scripts[j];
                if(jsFile.IndexOf("http") == -1)
                {
                    jsFile = "file:///" + jsFile;
                }
                www = new WWW(jsFile);
                yield return www;
                scriptStr = www.text;
                LoadString(scriptStr);
            }
        }

    }



    public ScriptObject LoadFile(string fileName)
    {

        //#if !UNITY_EDITOR || SCRIPTTEST

        //        //try {
        //        //    //Resource文件后缀改成txt  否则Unity不能识别TextAsset
        //        //    TextAsset text = Resources.Load<TextAsset>("Scripts/" + file);
        //        //    if (text != null) {
        //        //        //第一个参数是脚本摘要 有需求可以自己定义
        //        //        return m_Script.LoadBuffer(file, text.bytes);
        //        //    }
        //        //    string info = "找不到File : " + file;
        //        //    Debug.LogError(info);
        //        //    if(CatchInfoEvent!= null)
        //        //        CatchInfoEvent(info);
        //        //} catch (Scorpio.Exception.ScriptException e) {
        //        //    if (CatchInfoEvent != null)
        //        //        CatchInfoEvent(e.ToString());
        //        //}


        //#else
        string filePath = fileName;
        if(!Path.IsPathRooted(fileName))
            filePath = scriptPath + fileName;
        return m_Script.LoadFile(filePath);
        //#endif
    }

    public void PrintInfo(string info)
    {
        if(PrintInfoEvent != null)
            PrintInfoEvent(info);
    }

    public ScriptTable AddComponent(Component component,string name)
    {
        if(component == null || string.IsNullOrEmpty(name)) return null;
        return AddComponent(component.gameObject,name);
    }
    public ScriptTable AddComponent(GameObject gameObject,string name)
    {
        if(gameObject == null || string.IsNullOrEmpty(name)) return null;
        var table = m_Script.GetValue(name) as ScriptTable;
        if(table == null) return null;
        return AddComponent(gameObject,table,name);
    }
    public ScriptTable AddComponent(Component component,ScriptTable table)
    {
        if(component == null || table == null) return null;
        return AddComponent(component.gameObject,table,"");
    }
    public ScriptTable AddComponent(GameObject gameObject,ScriptTable table)
    {
        if(gameObject == null || table == null) return null;
        return AddComponent(gameObject,table,"");
    }
    public ScriptTable AddComponent(Component component,ScriptTable table,string name)
    {
        if(component == null || table == null) return null;
        return AddComponent(component.gameObject,table,name);
    }
    public ScriptTable AddComponent(GameObject gameObject,ScriptTable table,string name)
    {
        if(gameObject == null || table == null) return null;
        if(table.HasValue("Update") || table.HasValue("FixedUpdate"))
            gameObject.AddComponent<ScriptUpdateComponent>().Initialize(m_Script,table,name);
        else
            gameObject.AddComponent<ScriptComponent>().Initialize(m_Script,table,name);
        return table;
    }
    public ScriptTable GetComponent(Component component)
    {
        if(component == null) return null;
        return GetComponent(component.gameObject);
    }
    public ScriptTable GetComponent(GameObject gameObject)
    {
        if(gameObject == null) return null;
        ScriptComponent component = gameObject.GetComponent<ScriptComponent>();
        if(component == null) return null;
        return component.Table;
    }
    public ScriptTable GetComponent(Component component,string name)
    {
        if(component == null) return null;
        return GetComponent(component.gameObject,name);
    }
    public ScriptTable GetComponent(GameObject gameObject,string name)
    {
        if(gameObject == null) return null;
        ScriptComponent[] components = gameObject.GetComponents<ScriptComponent>();
        foreach(ScriptComponent component in components)
        {
            if(component.Name == name)
                return component.Table;
        }
        return null;
    }

    public void DelComponent(Component component)
    {
        if(component == null) return;
        DelComponent(component.gameObject);
    }
    public void DelComponent(GameObject gameObject)
    {
        if(gameObject == null) return;
        ScriptComponent component = gameObject.GetComponent<ScriptComponent>();
        if(component == null) return;
        Object.Destroy(component);
    }
    public void DelComponent(Component component,string name)
    {
        if(component == null) return;
        DelComponent(component.gameObject,name);
    }

    public void DelComponent(GameObject gameObject,string name)
    {
        if(gameObject == null) return;
        ScriptComponent[] components = gameObject.GetComponents<ScriptComponent>();
        foreach(ScriptComponent component in components)
        {
            if(component.Name == name)
            {
                Object.Destroy(component);
                return;
            }
        }
    }

    // ------------------------------------------------------------------
    // larrow  modified
    public void LoadDefaultFile()
    {
#if UNITY_STANDALONE
        string configStr = System.IO.File.ReadAllText(configPath);
        LitJson.JsonData jsonData = LitJson.JsonMapper.ToObject(configStr);
        //List<string> list = new List<string>();
        if(jsonData.IsArray)
        {
            for(int i = 0; i < jsonData.Count; i++)
            {
                string fileName = jsonData[i].ToString();
                ScriptManager.GetInstance().LoadFile(fileName);
            }
        }
#elif UNITY_WEBPLAYER

#endif
    }

    public void ClearStackInfo()
    {
        m_Script.ClearStackInfo();
    }

    public void DelComponentAll(GameObject gameObject,string name)
    {
        if(gameObject == null) return;
        ScriptComponent[] components = gameObject.GetComponents<ScriptComponent>();
        foreach(ScriptComponent component in components)
        {
            if(component.Name == name)
            {
                Object.Destroy(component);
            }
        }
    }

    //public void AddObjectBornInScript(Uinnova.BaseObject bo)
    //{
    //       //因为脚本创建的物体都是在预览下，为了可选，需要加入预览可选列表
    //       //Uinnova.SceneLoader.Instance.GetDisplaySetting().SetPickable(bo.ID, true);
    //       if (!objectsBornInScript.ContainsKey (bo.ID))
    //		objectsBornInScript [bo.ID] = bo;
    //}

    //public void RemoveObjectBornInScript(Uinnova.BaseObject bo)
    //{
    //	if (objectsBornInScript.ContainsKey (bo.ID))
    //		objectsBornInScript.Remove (bo.ID);
    //}

    public void RemoveAllObjectsBornInScript()
    {
        //objectsBornInScript.Clear ();
    }

    public void DestroyAllObjectsBornInScript()
    {
        DestroyAllObjectsBornInScript(null);

    }

    //pzx start
    public void DestroyAllObjectsBornInScript(DestroyAllObjectDelegate callback)
    {

        //objectsBornInScript.Clear();
        if(callback != null)
            callback();
    }
    //pzx end

    public void EmploryCSharpClass(string scriptValueName,string scriptAssemblyName,System.Reflection.Assembly classObj)
    {
        //它是这样压入 类的（程序集）
        m_Script.PushAssembly(classObj);

        if(scriptValueName != "")
        {
            //在脚本里注册全局的
            LoadString(scriptValueName + " = import_type( \"" + scriptAssemblyName + "\" )");
        }
    }
}
