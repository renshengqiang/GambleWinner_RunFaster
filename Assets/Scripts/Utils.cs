using System;
using System.Collections;
using UnityEngine;
namespace RunFaster
{
    public enum PokeColor
    {
        CLUB = 1,   // 草花
        SPADE,      // 黑桃
        DIAMOND,    // 方片
        HEART,      // 红桃
        BLACK,      // 黑色（Joker专用）
        RED         // 红色（Joker专用）
    }

    public struct Poke : IComparable<Poke>
    {
        public const int MIN = 3;
        public const int MAX = 16;
        public int number;          // 牌的数值（3:3, ..., J:11, Q: 12, K:13, A: 14:, 2: 15, Joker: 16)
        public PokeColor color;     // 花色

        public int CompareTo(Poke right)
        {
            if(number != right.number)
            {
                return number.CompareTo(right.number);
            }
            else
            {
                if(number == MAX)
                {
                    int leftColor = (int)color;
                    int rightColor = (int)right.color;
                    return leftColor.CompareTo(rightColor);
                }
                else
                {
                    return 0;
                }
            }
        }

        public static int CompareForDisplay(Poke left, Poke right)
        {
            if (left.number != right.number)
            {
                return left.number.CompareTo(right.number);
            }
            else
            {
                int leftColor = (int)left.color;
                int rightColor = (int)right.color;
                return leftColor.CompareTo(rightColor);
            }
        }

        public override string ToString()
        {
            string numStr;
            switch(number)
            {
                case 11:
                    numStr = "J";
                    break;
                case 12:
                    numStr = "Q";
                    break;
                case 13:
                    numStr = "K";
                    break;
                case 14:
                    numStr = "A";
                    break;
                case 15:
                    numStr = "2";
                    break;
                case 16:
                    numStr = "Joker";
                    break;
                default:
                    numStr = number.ToString();
                    break;
            }

            string colorStr;
            switch(color)
            {
                case PokeColor.CLUB:
                    colorStr = "草花";
                    break;
                case PokeColor.SPADE:
                    colorStr = "黑桃";
                    break;
                case PokeColor.DIAMOND:
                    colorStr = "方片";
                    break;
                case PokeColor.HEART:
                    colorStr = "红桃";
                    break;
                case PokeColor.BLACK:
                    colorStr = "小";
                    break;
                default:
                    colorStr = "大";
                    break;
            }
            return string.Format("({0} {1})", numStr, colorStr);
        }
    }

    /// <summary>
    /// 扑克组合类型
    /// </summary>
    public enum PokeCmbType
    {
        SINGLE = 1,         // 单支
        COUPLE,             // 对子
        
    }

    public class Utils
    {
        public static System.Random random = null;
        public static System.Random GetRandom()
        {
            if(null == random)
            {
                random = new System.Random(DateTime.Now.Second);
            }

            return random;
        }

        public static Poke GenerateOnePoke()
        {
            Poke poke = new Poke();
            poke.number = GetRandom().Next(Poke.MIN, Poke.MAX+1);

            if(poke.number == Poke.MAX)
            {
                int color = random.Next((int)PokeColor.BLACK, (int)PokeColor.RED+1);
                poke.color = (PokeColor)color;
            }
            else
            {
                int color = random.Next((int)PokeColor.CLUB, (int)PokeColor.HEART+1);
                poke.color = (PokeColor)color;
            }
            Debug.Log(string.Format("{0} {1}", poke.number, poke.color));
            return poke;
        }
    }
}