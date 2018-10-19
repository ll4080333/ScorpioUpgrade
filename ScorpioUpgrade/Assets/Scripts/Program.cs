using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Program : MonoBehaviour 
{
	private void Awake()
	{
		ScriptManager scriptManager = ScriptManager.GetInstance(); // 初始化engine、loading
        scriptManager.GetScript().PushAssembly(typeof(Program).Assembly);
        scriptManager.LoadDefaultFile();
	}
}
