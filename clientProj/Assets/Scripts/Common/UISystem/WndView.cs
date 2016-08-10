using UnityEngine;
using System.Collections;

namespace Common
{
    /// <summary>
    /// window view logic
    /// when you have a window to be shown and set pariticual widget, you need to implement a WndView
    /// </summary>
    public class WndView : MonoBehaviour
    {
        /// <summary>
        /// Init is called once on a gameobect.
        /// And it's called before Open.
        /// You can do some widget init work here.
        /// </summary>
        /// <param name="context"></param>
        public virtual void Init(WndContext context)
		{
		}
		/// <summary>
		/// Open is called whenever a window comes to foreground
		/// </summary>
		/// <param name="context"></param>
        public virtual void Open(WndContext context)
		{
		}
		/// <summary>
		/// Close is called whenever a window comes to background
		/// </summary>
		/// <param name="context"></param>
        public virtual void Close(WndContext context)
		{	
		}
		/// <summary>
		/// Destroy is called when a window gameobect is destroyed
		/// </summary>
		/// <param name="context"></param>
		public virtual void Destory(WndContext context)
		{
		}
    }
}

