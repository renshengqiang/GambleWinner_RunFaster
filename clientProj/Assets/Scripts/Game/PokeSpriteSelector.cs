using UnityEngine;
using System.Collections;

namespace RunFaster
{
    public class PokeSpriteSelector : MonoBehaviour
    {
        [SerializeField]
        private Sprite[] lstPokeSprites;

        public Sprite GetSprite(int index)
        {
            if(lstPokeSprites != null && index < lstPokeSprites.Length)
            {
                return lstPokeSprites[index];
            }
            else
            {
                return null;
            }
        }
    }
}

