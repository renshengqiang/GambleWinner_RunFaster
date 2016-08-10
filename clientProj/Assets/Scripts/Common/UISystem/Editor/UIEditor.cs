using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class UIEditor : Editor
{
    [MenuItem("Assets/UITool/NewWindow")]
    private static void CreateNewWindow()
    {
        string[] guids = Selection.assetGUIDs;
        string path = string.Empty;
        foreach (string guid in guids)
        {
            path = AssetDatabase.GUIDToAssetPath(guid);
            break;
        }
        if (false == Directory.Exists(path))
        {
            path = System.IO.Path.GetDirectoryName(path);
        }
        NewUIWndEditor.InitPath(path);
    }
}

public class NewUIWndEditor : EditorWindow
{
    private string path;
    private string namespaceName = "NewNS";
    private string wndName = "NewWindow";

    public static void InitPath(string path)
    {
        NewUIWndEditor window = (NewUIWndEditor)EditorWindow.GetWindow(typeof(NewUIWndEditor));
        window.path = path;
        window.title = "NewWindow";
        window.minSize = new Vector2(100, 40);
        window.namespaceName = "NewNS";
        window.wndName = "NewWindow";
    }
    void OnGUI()
    {
        EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            namespaceName = EditorGUILayout.TextField("NewNamespace", namespaceName);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            wndName = EditorGUILayout.TextField("NewWindowName", wndName);
            EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

        if (GUILayout.Button("OK") && !string.IsNullOrEmpty(wndName))
        {
            _CreateExtendBehavior(path, namespaceName, wndName);
            Close();
        }
    }
    private static void _CreateExtendBehavior(string path, string namespaceName, string wndName)
    {
        string contextTemplate = GetContextTemplate();
        string viewTemplate = GetViewTemplate();

        contextTemplate = contextTemplate.Replace(@"namespace UI", string.Format("namespace {0}", namespaceName));
        contextTemplate = contextTemplate.Replace(@"public class ExampleWnd : WndContext", string.Format("public class {0}Context : WndContext", wndName));

        viewTemplate = viewTemplate.Replace(@"namespace UI", string.Format("namespace {0}", namespaceName));
        viewTemplate = viewTemplate.Replace(@"public class ExampleView : WndView", string.Format("public class {0}View : WndView", wndName));

        File.WriteAllText(string.Format("{0}/{1}Context.cs", path, wndName), contextTemplate);
        File.WriteAllText(string.Format("{0}/{1}View.cs", path, wndName), viewTemplate);

        AssetDatabase.Refresh();
    }

    private static string GetContextTemplate()
    {
        return @"using Common;

namespace UI
{
    public class ExampleWnd : WndContext
    {
        public override WndType Type()
        {
            //todo: comment below line and return your window type
            throw new System.NotImplementedException();
        }
        public override string Name()
        {
            //todo: comment below line and return your window name
            throw new System.NotImplementedException();
        }

        public override string PrefabPath()
        {
            //todo:comment the below line and return your prefab path
            throw new System.NotImplementedException();
        }

        public override void Init()
        {
            //todo: add your Init work here
            base.Init();
        }
        public override void PrepareData(params object[] args)
        {
            //todo: add your data process here
            base.PrepareData(args);
        }
    }
}";
    }

    private static string GetViewTemplate()
    {
        return @"using Common;

namespace UI
{
    public class ExampleView : WndView
    {
        public override void Init(WndContext context)
        {
            //todo: add your view init work here
            base.Init(context);
        }

        public override void Open(WndContext context)
        {
            //todo: add your code for opening a window here
            base.Open(context);
        }

        public override void Close(WndContext context)
        {
            //todo: add your code for close a window here
            base.Close(context);
        }

        public override void Destory(WndContext context)
        {
            //todo: add your code for destroy a window here
            base.Destory(context);
        }
    }
}";
    }
}