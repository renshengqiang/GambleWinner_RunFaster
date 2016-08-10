using UnityEngine;
using System.Collections;

namespace Common
{
    /// <summary>
    /// window model logic
    /// when you have a window  to be shown, you must implement a WndContext
    /// </summary>
    public abstract class WndContext
    {
        /// <summary>
        /// The type of the wnd
        /// </summary>
        /// <returns></returns>
        public abstract WndType Type();
        /// <summary>
        /// The Name of the wnd, used to open one wnd
        /// </summary>
        /// <returns></returns>
        public abstract string Name();
        /// <summary>
        /// Prefab path, used to get prefab path
        /// </summary>
        /// <returns></returns>
        public abstract string PrefabPath();
        /// <summary>
        /// Init the WndContext
        /// </summary>
        public virtual void Init()
        {

        }
        /// <summary>
        /// prepare data for the view component
        /// </summary>
        /// <param name="args"></param>
        public virtual void PrepareData(params object[] args)
		{
			
		}
    }
}

