using UnityEngine;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace Common
{
    public enum WndType
    {
        NORMAL,     /* normal fullscreen window */
        POPUP,      /* popup window */
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

        public override void Init()
        {
            stackOfWnds = new Stack<Wnd>();
            dicWnds = new Dictionary<string, Wnd>();
            dicContext = new Dictionary<string, WndContext>();
            InitAssembly();
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
                wnd = new Wnd();
                WndContext context = dicContext[wndName];
                if(null == context)
                {
                    Logger.Error(string.Format("Wnd {0} not exist, please check"));
                    return;
                }
                else
                {
                    wnd.type = context.Type();
                    wnd.context = context;

                    GameObject go = ResourceManager.GetInstance().GetResouce(context.PrefabPath());
                    if(go != null)
                    {
                        WndView view = go.GetComponent<WndView>();
                        if(null == view)
                        {
                            Logger.Error(string.Format("Wnd {0} has none view component attached, which is required, please check", wndName));
                        }
                        else
                        {
                            wnd.view = view;
                        }
                        // todo: some ui initilise job, it's relevant to resources
                    }
                    else
                    {
                        Logger.Error(string.Format("Wnd {0}'s prefab {1} not exist, please check", wndName, context.PrefabPath()));
                    }
                }
            }
            
            // if the window is the topmost window, just return
            if (topWnd == wnd) return;

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
                    topWnd.view.Close(topWnd.context);

                    topWnd = stackOfWnds.Peek();
                    topWnd.view.Open(topWnd.context);
                }
            }
            else
            {
                topWnd.view.Close(topWnd.context);
            }
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
                }
            }
        }

    }

}

