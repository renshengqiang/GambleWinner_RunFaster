using System;
using System.Collections.Generic;
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
        public const int A = 15;
        public const int TWO = 14;
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
        STRAIGHT,           // 顺子
        FLUSH_STRAIGHT,     // 同花顺
        PAIR,               // 对子
        STRAIGHT_PAIR,      // 连对
        TRIP,               // 三个
        STRIGHT_TRIP,       // 三顺
        TRIP_PAIR,          // 三带二
        QUADS,              // 炸弹
        JOKER_QUADS         // 王炸
    }

    /// <summary>
    /// 出牌的类型
    /// </summary>
    public struct PokeCardsType : IComparable<PokeCardsType>
    {
        public PokeCmbType type;        // 组合类型，一般而言同种组合类型才能比较大小
        public Poke mainPoke;           // 主牌，连子之类的，这个就是首张牌
        public int continuousNum;       // 连续牌数量

        public int CompareTo(PokeCardsType other)
        {
            int ret = 0;

            if(type == other.type)
            {
                #region EQUAL
                switch (type)
                {
                    case PokeCmbType.SINGLE:
                    case PokeCmbType.PAIR:
                    case PokeCmbType.TRIP:
                    case PokeCmbType.TRIP_PAIR:
                        ret = mainPoke.CompareTo(other.mainPoke);
                        break;
                    case PokeCmbType.STRAIGHT:
                    case PokeCmbType.FLUSH_STRAIGHT:
                        // 如果是以A或者2开头，则需要另外进行单独处理
                        // 否则直接比较顺子的第一个数字即可
                        if(continuousNum == other.continuousNum)
                        {
                            if((mainPoke.number == Poke.A || mainPoke.number == Poke.TWO) &&
                            (other.mainPoke.number != Poke.A && other.mainPoke.number != Poke.TWO))
                            {
                                ret = -1;
                            }
                            else if((other.mainPoke.number == Poke.A || other.mainPoke.number == Poke.TWO) &&
                            (mainPoke.number != Poke.A && mainPoke.number != Poke.TWO))
                            {
                                ret = 1;
                            }
                            else
                            {
                                ret = mainPoke.CompareTo(other.mainPoke);
                            }
                        }
                        else
                        {
                            ret = 0;
                        }
                        break;
                    case PokeCmbType.STRAIGHT_PAIR:
                    case PokeCmbType.STRIGHT_TRIP:
                        if(continuousNum == other.continuousNum)
                        {
                            if(continuousNum > 2)
                            {
                                if((mainPoke.number == Poke.A || mainPoke.number == Poke.TWO) &&
                                    (other.mainPoke.number != Poke.A && other.mainPoke.number != Poke.TWO))
                                {
                                    ret = -1;
                                }
                                else if ((other.mainPoke.number == Poke.A || other.mainPoke.number == Poke.TWO) &&
                                    (mainPoke.number != Poke.A && mainPoke.number != Poke.TWO))
                                {
                                    ret = 1;
                                }
                                else
                                {
                                    ret = mainPoke.CompareTo(other.mainPoke);
                                }
                            }
                            else
                            {
                                ret = mainPoke.CompareTo(other.mainPoke);
                            }
                        }
                        else
                        {
                            ret = 0;
                        }
                        break;
                    case PokeCmbType.QUADS:
                        if(continuousNum != other.continuousNum)
                        {
                            ret = continuousNum.CompareTo(other.continuousNum);
                        }
                        else
                        {
                            ret = mainPoke.CompareTo(other.mainPoke);
                        }
                        break;
                    default:
                        ret = 0;
                        break;
                }
                #endregion
            }
            else
            {
                #region TYPE_UNEQUAL
                if (type == PokeCmbType.JOKER_QUADS)
                {
                    ret = 1;
                }
                else if(other.type == PokeCmbType.JOKER_QUADS)
                {
                    ret = -1;
                }
                else if(type == PokeCmbType.QUADS)
                {
                    ret = 1;
                }
                else if(other.type == PokeCmbType.QUADS)
                {
                    ret = -1;
                }
                else if(continuousNum == other.continuousNum &&
                        type == PokeCmbType.FLUSH_STRAIGHT &&
                        other.type == PokeCmbType.STRAIGHT)
                {
                    ret = 1;
                }
                else if(continuousNum == other.continuousNum &&
                    other.type == PokeCmbType.FLUSH_STRAIGHT &&
                    type == PokeCmbType.STRAIGHT)
                {
                    ret = -1;
                }
                else
                {
                    ret = 0;
                }
                #endregion
            }
            return ret;
        }
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

        public static PokeCardsType GetPockCarsdType(List<Poke> lstPokes)
        {
            PokeCardsType ret = new PokeCardsType();
            return ret;
        }
    }
}