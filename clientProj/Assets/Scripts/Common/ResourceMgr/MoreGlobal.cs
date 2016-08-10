using MoreFun;
using UnityEngine;
using MoreFun.Utils;
using System;

/// <summary>
/// Global functions wihout namespace for easy access.
/// Do NOT add functions here UNLESS it's very useful.
/// </summary>
public static class MoreGlobal
{
    #region IsValid
    public static bool IsValid(this object obj)
    {
        if(null != obj)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public static bool IsValid(this UnityEngine.Object obj)
    {
        if(null != obj)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public static bool IsValid(this string obj)
    {
        return !(string.IsNullOrEmpty(obj));
    }

    public static bool IsValid(this Component obj)
    {
        if(null != obj && null != obj.gameObject)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    public static bool IsValid(this System.Collections.IList collection, int index)
    {
        return CollectionUtil.IsValidIndex(collection, index);
    }

    /// <summary>
    /// <para>Check a chain of objects for their validation.</para>
    /// <para>Note, when one parameters are NOT UnityEngine.Object, C# will call this function</para>
    /// <para>therefore, do NOT mix object and UnityEngine.Object in this call!!!</para>, 
    /// <para>Note, you can call <c>IsValid(obj1, obj2)</c>.  
    /// <para>But do NOT call like this: </para>
    /// <para><c>IsValid(obj1, obj1.obj2)</c></para>, 
    /// because when you call <c>obj1.obj2</c>, it's NOT guaranteed that <c>obj1</c> is not null, 
    /// and may cause exception!</para>
    /// </summary>
    /// <returns><c>true</c> if is valid the specified list; otherwise, <c>false</c>.</returns>
    /// <param name="list">List.</param>
    public static bool IsValid(params object[] list)
    {
        if(null != list)
        {
            for(int i = 0; i < list.Length; ++i)
            {
                if(null == list[i])
                {
                    return false;
                }
            }
            
            return true;
        }
        else
        {
            return false;
        }
    }
    
    /// <summary>
    /// <para>Check a chain of objects for their validation.</para>
    /// <para>Note, when all parameters are UnityEngine.Object, C# will call this function</para>
    /// <para>Note, you can call <c>IsValid(obj1, obj2)</c>.  
    /// <para>But do NOT call like this: </para>
    /// <para><c>IsValid(obj1, obj1.obj2)</c></para>, 
    /// because when you call <c>obj1.obj2</c>, it's NOT guaranteed that <c>obj1</c> is not null, 
    /// and may cause exception!</para>
    /// </summary>
    /// <returns><c>true</c> if is valid the specified list; otherwise, <c>false</c>.</returns>
    /// <param name="list">List.</param>
    public static bool IsValid(params UnityEngine.Object[] list)
    {
        if(null != list)
        {
            for(int i = 0; i < list.Length; ++i)
            {
                if(null == list[i])
                {
                    return false;
                }
            }
            
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool IsValidHandler(Delegate handler)
    {
        bool ret = true;
        if (null != handler && null != handler.Target)
        {
            object target = handler.Target;
            MonoBehaviour mono = handler.Target as MonoBehaviour;
            if (typeof(MonoBehaviour).IsAssignableFrom(target.GetType()))
            {
                if (mono == null || null == mono.gameObject)
                {
                    ret = false;
                }
            }
        }
        else if (null == handler)
        {
            ret = false;
        }
        return ret;
    }
    #endregion
}

