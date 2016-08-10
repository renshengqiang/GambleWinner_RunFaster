using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class ImporterFileDataXml 
{
	public static string encode(List<string> file_list)
	{
		XmlDocument doc = new XmlDocument();
		XmlElement root = doc.CreateElement("ImportAsset");
		
		doc.AppendChild(root);
		
		foreach(string file in file_list)
		{
			XmlElement itemxml = doc.CreateElement("asset");
			
			itemxml.SetAttribute("path", file);
			
			root.AppendChild(itemxml);
		}
		
		return doc.OuterXml;
	}
	
	public static List<string> decode(string filexml)
	{
		List<string> filelist = new List<string> ();
		if(filexml != null && !filexml.Equals(""))
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(filexml);
			XmlElement root = doc.DocumentElement;
			XmlNodeList list = root.GetElementsByTagName("asset");
			int count = list.Count;
			for(int i = 0;i < count; i ++)
			{
				filelist.Add(((XmlElement)list.Item(i)).GetAttribute("path"));
			}
		}
		
		return filelist;
	}
}
