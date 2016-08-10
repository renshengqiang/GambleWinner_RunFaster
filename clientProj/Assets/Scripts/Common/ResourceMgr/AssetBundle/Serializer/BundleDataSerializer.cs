using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class BundleListXML
{
    static public List<BundleInfo> decode(string bundleXML)
    {
        List<BundleInfo> list_bundle = new List<BundleInfo>();

        if (bundleXML != null && !bundleXML.Equals(""))
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(bundleXML);
            XmlElement root = doc.DocumentElement;
            XmlNodeList list = root.GetElementsByTagName("asset");
            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                XmlElement element = (XmlElement)list.Item(i);
                BundleInfo bundle = BunldeInfoXml.decode(element);

                list_bundle.Add(bundle);
            }
        }

        return list_bundle;
    }

    static public string encode(List<BundleInfo> list_bundle)
    {
        XmlDocument doc = new XmlDocument();
        XmlElement root = doc.CreateElement("assetbundles");

        doc.AppendChild(root);

        foreach (BundleInfo data in list_bundle)
        {
            root.AppendChild(BunldeInfoXml.encode(doc, data));
        }

        return doc.OuterXml;
    }
}

class BunldeInfoXml
{
    static public BundleInfo decode(XmlElement xml)
    {
        BundleInfo bundle = new BundleInfo();

        bundle.bundle_id = uint.Parse(xml.GetAttribute("id"));
        bundle.bundle_name = xml.GetAttribute("name");
        bundle.bundle_md5 = xml.GetAttribute("md5");
        bundle.size = uint.Parse(xml.GetAttribute("size"));
        string strDepBundle = xml.GetAttribute("dep");

        if (strDepBundle.Length > 0)
        {
            string[] dep_bundles = strDepBundle.Split(';');
            foreach (string depbundlename in dep_bundles)
            {
                if (depbundlename != "")
                {
                    uint bundleid = uint.Parse(depbundlename);
                    bundle.dep_bundle_list.Add(bundleid);
                }
            }
        }

        return bundle;
    }

    static public XmlElement encode(XmlDocument doc, BundleInfo data)
    {
        XmlElement xml = doc.CreateElement("asset");

        xml.SetAttribute("id", data.bundle_id.ToString());
        xml.SetAttribute("name", data.bundle_name);
        xml.SetAttribute("md5", data.bundle_md5.ToString());
        xml.SetAttribute("size", data.size.ToString());

        string strDepBundle = "";
        int count = data.dep_bundle_list.Count;
        if (count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                if (i > 0)
                {
                    strDepBundle += ";";
                }

                strDepBundle += data.dep_bundle_list[i].ToString();
            }
        }
        xml.SetAttribute("dep", strDepBundle);

        return xml;
    }
}