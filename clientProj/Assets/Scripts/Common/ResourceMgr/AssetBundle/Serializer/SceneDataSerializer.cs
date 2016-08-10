using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class SceneBundleInfoXML
{
    public static List<StreamSceneBuildState> decode(string bundleXML)
    {
        List<StreamSceneBuildState> list_bundle = new List<StreamSceneBuildState>();

        if (bundleXML != null && !bundleXML.Equals(""))
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(bundleXML);
            XmlElement root = doc.DocumentElement;
            XmlNodeList list = root.GetElementsByTagName("scene");
            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                XmlElement element = (XmlElement)list.Item(i);
                StreamSceneBuildState state = new StreamSceneBuildState();
                state.bundle_name = element.GetAttribute("bundle");
                state.size = long.Parse(element.GetAttribute("size"));
                state.md5 = element.GetAttribute("md5");
                list_bundle.Add(state);
            }
        }
        return list_bundle;
    }

    public static string encode(List<StreamSceneBuildState> lstSceneInfo)
    {
        XmlDocument doc = new XmlDocument();
        XmlElement root = doc.CreateElement("scenes");

        doc.AppendChild(root);

        foreach (StreamSceneBuildState data in lstSceneInfo)
        {
            XmlElement xml = doc.CreateElement("scene");

            xml.SetAttribute("bundle", data.bundle_name);
            xml.SetAttribute("size", data.size.ToString());
            xml.SetAttribute("md5", data.md5);
            root.AppendChild(xml);
        }

        return doc.OuterXml;
    }
}