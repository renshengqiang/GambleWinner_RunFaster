using UnityEngine;
using System.Collections;

namespace Common
{
    [HideInInspector]
    public class PoolItemCom : MonoBehaviour
    {
        private string path;

        public string Path
        {
            get { return path; }
            set { path = value; }
        }
    }
}

