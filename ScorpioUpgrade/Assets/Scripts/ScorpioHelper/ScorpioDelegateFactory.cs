using System;
using Scorpio;
using Scorpio.Userdata;
using System.Collections.Generic;
using UnityEngine;
//using Uinnova;

namespace ScorpioDelegate {
    public class ScorpioDelegateFactory : DelegateTypeFactory {
        public static void Initialize() {
            ScriptUserdataDelegateType.SetFactory(new ScorpioDelegateFactory());
        }
        public Delegate CreateDelegate(Script script, Type type, ScriptFunction func) {
            if (type == typeof(System.Action))
                return new System.Action(() => { func.call(); });
            //else if (type == typeof(UIEventListener.UIEventProxy))
            //    return new UIEventListener.UIEventProxy((arg0) => func.call(arg0));
            ////else if (type == typeof(System.Action<FloorPageSubSysEnum>))
            ////    return new System.Action<FloorPageSubSysEnum>((arg0) => func.call(arg0));
            //else if (type == typeof(BBS_MouseEventFunction))
            //    return new BBS_MouseEventFunction(() => func.call());
            //else if (type == typeof(Uinnova.BuildingEffects.EffectCallback))
            //    return new Uinnova.BuildingEffects.EffectCallback(() => func.call());
            //else if (type == typeof(DG.Tweening.Core.DOGetter<UnityEngine.Color>))
            //    return new DG.Tweening.Core.DOGetter<UnityEngine.Color>(() => (UnityEngine.Color)func.call());
            //else if (type == typeof(DG.Tweening.Core.DOSetter<UnityEngine.Color>))
            //    return new DG.Tweening.Core.DOSetter<UnityEngine.Color>((arg0) => func.call(arg0));
            else if (type == typeof(System.Action<System.String>))
                return new System.Action<System.String>((arg0) => { func.call(arg0); });
            else if (type == typeof(System.Comparison<UnityEngine.Transform>))
                return new System.Comparison<UnityEngine.Transform>((arg0, arg1) => { return (System.Int32)Convert.ChangeType(script.CreateObject(func.call(arg0, arg1)).ObjectValue, typeof(System.Int32)); });
            else if (type == typeof(TimerCallBack))
                return new TimerCallBack((arg0, arg1) => { func.call(arg0, arg1); });
            else if (type == typeof(UnityEngine.Application.LogCallback))
                return new UnityEngine.Application.LogCallback((arg0, arg1, arg2) => { func.call(arg0, arg1, arg2); });
            else if (type == typeof(UnityEngine.Events.UnityAction))
                return new UnityEngine.Events.UnityAction(() => { func.call(); });
            else if (type == typeof(UnityEngine.Events.UnityAction<System.Boolean>))
                return new UnityEngine.Events.UnityAction<System.Boolean>((arg0) => { func.call(arg0); });
            else if (type == typeof(UnityEngine.Events.UnityAction<System.Int32>))
                return new UnityEngine.Events.UnityAction<System.Int32>((arg0) => { func.call(arg0); });
            else if (type == typeof(UnityEngine.Events.UnityAction<System.Single>))
                return new UnityEngine.Events.UnityAction<System.Single>((arg0) => { func.call(arg0); });
            else if (type == typeof(UnityEngine.Events.UnityAction<System.String>))
                return new UnityEngine.Events.UnityAction<System.String>((arg0) => { func.call(arg0); });
            else if (type == typeof(UnityEngine.Events.UnityAction<UnityEngine.Vector2>))
                return new UnityEngine.Events.UnityAction<UnityEngine.Vector2>((arg0) => { func.call(arg0); });
            else if (type == typeof(UnityEngine.Events.UnityAction<UnityEngine.GameObject>))
                return new UnityEngine.Events.UnityAction<UnityEngine.GameObject>((arg0) => { func.call(arg0); });
            else if (type == typeof(UnityEngine.Events.UnityAction<UnityEngine.Texture2D>))
                return new UnityEngine.Events.UnityAction<UnityEngine.Texture2D>((arg0) => { func.call(arg0); });
            else if (type == typeof(UnityEngine.Networking.NetworkMessageDelegate))
                return new UnityEngine.Networking.NetworkMessageDelegate((arg0) => { func.call(arg0); });
            else if (type == typeof(YieldCallback))
                return new YieldCallback(() => { func.call(); });

            // uinnova
            //else if (type == typeof(HttpClient.DelegateString))
            //    return new HttpClient.DelegateString((arg0) => { func.call(arg0); });
            //else if (type == typeof(HttpClient.DelegateBool))
            //    return new HttpClient.DelegateBool((arg0) => { func.call(arg0); });
            //else if (type == typeof(Uinnova.OrbitCamera.FlyEndDelegate))
            //    return new Uinnova.OrbitCamera.FlyEndDelegate(() => { func.call(); });
            //else if (type == typeof(WeatherManager.HourChanged))
            //    return new WeatherManager.HourChanged((arg0) => { func.call(arg0); });
            //else if (type == typeof(WeatherManager.WeatherChanged))
            //    return new WeatherManager.WeatherChanged((arg0) => { func.call(arg0); });
            else if (type == typeof(System.Action<object>))
                return new System.Action<object>((arg0) => { func.call(arg0); });
            //else if (type == typeof(Action<List<Uinnova.BaseObject>>))
            //    return new Action<List<Uinnova.BaseObject>>((List<Uinnova.BaseObject> arg0) => { func.call(arg0); });
            //else if (type == typeof(Uinnova.UIMenu.ShowPosDelegate))
            //    return new Uinnova.UIMenu.ShowPosDelegate((arg0) => { return (UnityEngine.Vector3)func.call(arg0); });
            //else if(type == typeof(Action<CurveLine>))
            //    return new Action<CurveLine>((arg0) => { func.call(arg0); });
            //else if (type == typeof(Action<Text3D>))
            //    return new Action<Text3D>((arg0) => { func.call(arg0); });
            //else if (type == typeof(UIListTable.PageDelegate))
            //    return new UIListTable.PageDelegate((arg0) => { func.call(arg0); });
            //else if (type == typeof(EventTriggerListener.VoidDelegate))
            //    return new EventTriggerListener.VoidDelegate((arg0) => { func.call(arg0);  });
            //else if (type == typeof(IBV.UIEventListener.VoidDelegate))
            //    return new IBV.UIEventListener.VoidDelegate((arg0) => { func.call(arg0); });
            //else if (type == typeof(UIPopupPanel.onClickCloseBtn))
            //    return new UIPopupPanel.onClickCloseBtn(() => { func.call(); });
            //else if (type == typeof(UIPopupPanel.onPanelFolded))
            //    return new UIPopupPanel.onPanelFolded((arg0) => { func.call(arg0); });
            throw new Exception("Delegate Type is not found : " + type + "  func : " + func);
        }
    }
}