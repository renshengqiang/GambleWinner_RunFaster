using UnityEngine;
using RunFaster;

namespace Common
{
    public class SpriteManager : Singleton<SpriteManager>
    {
        PokeSpriteSelector pokeSpriteSelector;

        public override void Init()
        {
            GameObject go = ResourceManager.GetInstance().GetResouce("PokeSpriteSelector");
            if(go != null)
            {
                pokeSpriteSelector = go.GetComponent<PokeSpriteSelector>();
                if(pokeSpriteSelector == null)
                {
                    Logger.Error("SpriteManager Init error( get component error)");
                }
            }
            else
            {
                Logger.Error("SpriteManager Init error( load reource error)");
            }
        }

        public Sprite GetSprite(Poke poke)
        {
            int index = 0;

            if (poke.color == PokeColor.BLACK)
            {
                index = 52;
            }
            else if(poke.color == PokeColor.RED)
            {
                index = 53;
            }
            else
            {
                index = (poke.number - 3) * 4 + ((int)poke.color - 1);
            }


            if(pokeSpriteSelector != null)
            {
                return pokeSpriteSelector.GetSprite(index);
            }
            else
            {
                return null;
            }
        }
    }
}

