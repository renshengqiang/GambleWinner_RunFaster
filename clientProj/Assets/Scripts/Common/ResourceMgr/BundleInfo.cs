using UnityEngine;
using System.Collections.Generic;

public class BundleInfo 
{
    public uint bundle_id;
    public string bundle_name;
    public string bundle_md5;
    public uint size;
    public List<uint> dep_bundle_list;
}

public class BundleInfoArray : global::ProtoBuf.IExtensible
{
    public BundleInfoArray() { }

    private readonly global::System.Collections.Generic.List<BundleInfo> _lstBundleInfo = new global::System.Collections.Generic.List<BundleInfo>();

    public global::System.Collections.Generic.List<BundleInfo> lstBundleInfo
    {
        get { return _lstBundleInfo; }
    }

    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
    { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
}
