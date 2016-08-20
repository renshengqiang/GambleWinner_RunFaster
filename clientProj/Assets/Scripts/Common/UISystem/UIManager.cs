using UnityEngine;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace Common
{
    public enum WndType
    {
        NORMAL,     /* normal fullscreen window, open & close like a stack */
        POPUP,      /* popup window, only one can open */
        MESSAGEBOX  /* message box */
    }

    public class Wnd
    {
        public WndType      type;
        public WndContext   context;
        public WndView      view;
    }

    public class UIManager : Singleton<UIManager>
    {
        private Stack<Wnd>                      stackOfWnds;        /* windows need to be open and close like a stack */
        private Dictionary<string, Wnd>         dicWnds;            /* opened windows */
        private Dictionary<string, WndContext>  dicContext;         /* dictionary of all windows */
        private Wnd                             topWnd;             /* the topmost window */
        private RectTransform                   normalWndContainer; /* normal fullscreen wnd's parent */
        private RectTransform                   popupWndContainer;  /* popup wnd's parent */
        private RectTransform                   messgeBoxContainer; /* messagebox wnd's parent */

        public override void Init()
        {
            stackOfWnds = new Stack<Wnd>();
            dicWnds = new Dictionary<string, Wnd>();
            dicContext = new Dictionary<string, WndContext>();
            InitAssembly();
            InitContainer();
        }

        public override void Release()
        {
            stackOfWnds.Clear();
            dicWnds.Clear();
            base.Release();
        }

        /// <summary>
        /// Open one Wnd with the name and parameters
        /// </summary>
        /// <param name="wndName">name of the window</param>
        /// <param name="args">parameters to open the window</param>
        public void Open(string wndName, params object[] args)
        {
            Wnd wnd;

            if(!dicWnds.TryGetValue(wndName, out wnd))
            {
                // not opened before
                wnd = new Wnd();
                WndContext context = dicContext[wndName];
                if(null == context)
                {
                    // must exist, because all wnd are registered when init
                    Logger.Error(string.Format("Wnd {0} not exist, please check"));
                    return;
                }
                else
                {
                    wnd.type = context.Type();
                    wnd.context = context;

                    if(null == wnd.view)
                    {
                        GameObject go = ResourceManager.GetInstance().GetResouce(context.PrefabPath());
                        if (go != null)
                        {
                            WndView view = go.GetComponent<WndView>();
                            if (null == view)
                            {
                                Logger.Error(string.Format("Wnd {0} has none view component attached, which is required, please check", wndName));
                            }
                            else
                            {
                                wnd.view = view;
                                RectTransform parent = null;
                                RectTransform child = view.transform as RectTransform;

                                switch (wnd.type)
                                {
                                    case WndType.NORMAL:
                                        parent = normalWndContainer;
                                        break;
                                    case WndType.POPUP:
                                        parent = popupWndContainer;
                                        break;
                                    case WndType.MESSAGEBOX:
                                        parent = messgeBoxContainer;
                                        break;
                                }
                                TransformUtil.AddChildAsLastSibling(parent, child);
                                TransformUtil.SetRectTransformStretch(child);

                                view.Init(wnd.context);
                            }
                        }
                        else
                        {
                            Logger.Error(string.Format("Wnd {0}'s prefab {1} not exist, please check", wndName, context.PrefabPath()));
                        }
                    }
                }
                dicWnds[wndName] = wnd;
            }//if
            else
            {
                Transform trans = wnd.view.transform;
                trans.SetAsLastSibling();
            }
            
            // if the window is the topmost window, just return
            if (topWnd == wnd) return;
            if(topWnd != null ) topWnd.view.gameObject.SetActive(false);

            // show the prefab
            wnd.context.PrepareData(args);
            wnd.view.Open(wnd.context);

            // todo: animation need be controlled under the UIManager, and need be configed
            wnd.view.gameObject.SetActive(true);

            if(wnd.type == WndType.NORMAL)
            {
                stackOfWnds.Push(wnd);
            }
            topWnd = wnd;
        }

        /// <summary>
        /// Close the topmost window
        /// If the topmost window is normal window, iterate a stack to get one top window.
        /// Else if it's a popup window, just close it.
        /// </summary>
        public void CloseTop()
        {
            if(stackOfWnds.Count < 1) return;

            Wnd stackTopWnd = stackOfWnds.Peek();

            if(topWnd.type == WndType.NORMAL)
            {
                if(stackOfWnds.Count > 1 && stackTopWnd == topWnd)
                {
                    stackOfWnds.Pop();
                    topWnd.view.gameObject.SetActive(false);
                    topWnd.view.Close(topWnd.context);

                    topWnd = stackOfWnds.Peek();
                    topWnd.view.gameObject.SetActive(true);
                    topWnd.view.Open(topWnd.context);
                }
            }
            else
            {
                topWnd.view.Close(topWnd.context);
                topWnd.view.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Utility function to show fasttips
        /// </summary>
        /// <param name="tips"></param>
        public void ShowFasttips(string tips)
        {

        }

        /// <summary>
        /// Utility function to show warning with a OK button
        /// </summary>
        /// <param name="content"></param>
        public void ShowWarning(string content)
        {

        }

        /// <summary>
        /// Utility function to show message box
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="okMsg"></param>
        /// <param name="cancelMsg"></param>
        /// <param name="okHandler"></param>
        /// <param name="cancelHandler"></param>
        public void ShowMessageBox(string title, string message, string okMsg, string cancelMsg,
                                    VoidHandler okHandler, VoidHandler cancelHandler = null)
        {

        }

        private static string baseContextName = "Common.WndContext";
        private void InitAssembly()
        {
            Type contextType = typeof(WndContext);
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for(int i=0; i<assemblies.Length; ++i)
            {
                Type[] types = assemblies[i].GetTypes();
                for(int j=0; j<types.Length; ++j)
                {
                    string fullname = types[j].FullName;

                    if (!fullname.Equals(baseContextName) && contextType.IsAssignableFrom(types[j]))
                    {
                        WndContext context = (WndContext)assemblies[i].CreateInstance(fullname);
                        if(context != null)
                        {
                            dicContext.Add(context.Name(), context);
                            //Logger.Info(string.Format("classname:{0} prefabPath:{1}", fullname, context.PrefabPath()));
                        }
                    }
                }//for j
            }//for i
        }//InitAssembly

        private void InitContainer()
        {
            normalWndContainer = transform.FindChild("UI/Canvas/NormalWndCtn") as RectTransform;
            popupWndContainer = transform.FindChild("UI/Canvas/PopupWndCtn") as RectTransform;
            messgeBoxContainer = transform.FindChild("UI/Canvas/MessageWndCtn") as RectTransform;

            if(null == normalWndContainer || null == popupWndContainer || null == messgeBoxContainer)
            {
                Logger.Error(string.Format("UIManager Init Container error: normalContainer:{0}, popupContainer:{1}, messageBoxContainer:{2}",
                    normalWndContainer, popupWndContainer, messgeBoxContainer));
            }
        }
    }

}

