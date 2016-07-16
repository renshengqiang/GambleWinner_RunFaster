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
        public const int K = 13;
        public const int A = 14;
        public const int TWO = 15;
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

        public Poke(string number, PokeColor color)
        {
            switch(number)
            {
                case "3":
                    this.number = 3;
                    break;
                case "4":
                    this.number = 4;
                    break;
                case "5":
                    this.number = 5;
                    break;
                case "6":
                    this.number = 6;
                    break;
                case "7":
                    this.number = 7;
                    break;
                case "8":
                    this.number = 8;
                    break;
                case "9":
                    this.number = 9;
                    break;
                case "10":
                    this.number = 10;
                    break;
                case "J":
                    this.number = 11;
                    break;
                case "Q":
                    this.number = 12;
                    break;
                case "K":
                    this.number = 13;
                    break;
                case "A":
                    this.number = 14;
                    break;
                case "2":
                    this.number = 15;
                    break;
                case "JO":
                    this.number = 16;
                    break;
                default:
                    this.number = 0;
                    break;
            }
            this.color = color;
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
        STRIGHT_TRIP_PAIR,  // 飞机
        QUADS,              // 炸弹
        JOKER_QUADS,        // 王炸
        ILLEGAL
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
                    case PokeCmbType.STRIGHT_TRIP_PAIR:
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

        public static bool operator > (PokeCardsType left, PokeCardsType right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator < (PokeCardsType left, PokeCardsType right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >= (PokeCardsType left, PokeCardsType right)
        {
            return !(left<right);
        }

        public static bool operator <= (PokeCardsType left, PokeCardsType right)
        {
            return !(left>right);
        }

        public static bool operator == (PokeCardsType left, PokeCardsType right)
        {
            return left.CompareTo(right) == 0;
        }

        public static bool operator != (PokeCardsType left, PokeCardsType right)
        {
            return !(left == right);
        }
        public override string ToString()
        {
            string ret = string.Empty;
            switch(type)
            {
                case PokeCmbType.SINGLE:
                    ret = "单只";
                    break;
                case PokeCmbType.STRAIGHT:
                    ret = "连子";
                    break;
                case PokeCmbType.FLUSH_STRAIGHT:
                    ret = "同花顺";
                    break;
                case PokeCmbType.PAIR:
                    ret = "对子";
                    break;
                case PokeCmbType.STRAIGHT_PAIR:
                    ret = "连对";
                    break;
                case PokeCmbType.TRIP:
                    ret = "三支";
                    break;
                case PokeCmbType.STRIGHT_TRIP:
                    ret = "三连";
                    break;
                case PokeCmbType.TRIP_PAIR:
                    ret = "三带二";
                    break;
                case PokeCmbType.STRIGHT_TRIP_PAIR:
                    ret = "飞机";
                    break;
                case PokeCmbType.QUADS:
                    ret = "炸弹";
                    break;
                case PokeCmbType.JOKER_QUADS:
                    ret = "王炸";
                    break;
                default:
                    ret = "错误";
                    break;
            }

            ret = string.Format("{0} {1} {2}", ret, mainPoke.ToString(), continuousNum);
            return ret;
        }
    }

    public class Utils
    {
        public static System.Random random = null;
        public const int CARDS_NUM = 14;            // 只有14种类型的卡牌
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

        public static List<Poke> GeneratePokeList(string[] lstNumber, PokeColor[] lstColor)
        {
            List<Poke> lstRet = new List<Poke>();

            if(lstNumber.Length != lstColor.Length)
            {
                Debug.LogError(string.Format("number Count: {0} color Count: {1}", lstNumber.Length, lstColor.Length));
            }
            else
            {
                for(int i=0; i<lstNumber.Length; ++i)
                {
                    Poke poke = new Poke(lstNumber[i], lstColor[i]);
                    lstRet.Add(poke);
                }
            }
            return lstRet;
        }

        /// <summary>
        /// 判断当前出牌的类型
        /// </summary>
        /// <param name="lstPokes">当前牌的序列</param>
        /// <param name="isOnePeckOfCards">是否是一副牌</param>
        /// <returns></returns>
        public static PokeCardsType GetPokeCarsdType(List<Poke> lstPokes, bool isOnePeckOfCards = true)
        {
            PokeCardsType ret = new PokeCardsType();
            int[] pokeCounts = new int[CARDS_NUM+3];        // 多出三个，因为点数从3开始
            int[] pokeTypeCounts = new int[9];              // 最多只能是8张连续相同的
            int maxSamePokeNum = 0;                         // 相同大小牌的最多数量（三代两就是3， 对子就是2等等）
            int maxSamePos = 0;                             // 数量最多的牌在lstPokes中的下标
            int maxSamePosNo = 0;                           // 数量最多的牌在 pokeCount 中的下标（例如 3条3 就是 3）
            bool continues = true;                          // 这些牌是否是连续的
            bool crossA23 = false;                          // 如果是连子或者连对等，是否跨越了A23，这种情况需要单独处理
            bool cross23 = false;                           // 同上，是否跨越了2,3

            for (int i = 0; i < pokeCounts.Length; ++i)
            {
                pokeCounts[i] = 0;
            }
            for (int i = 0; i < pokeTypeCounts.Length; ++i)
            {
                pokeTypeCounts[i] = 0;
            }

            // 第一步，给所有的牌进行排序
            lstPokes.Sort();

            // 第二步，统计每张手牌的数量
            for (int i = 0; i < lstPokes.Count; ++i)
            {
                if(lstPokes[i].number >= Poke.MIN && lstPokes[i].number <= Poke.MAX)
                {
                    pokeCounts[lstPokes[i].number]++;
                }
                else
                {
                    Debug.Log(string.Format("Encounter error poke whose number is {0}", lstPokes[i].number));
                }
            }

            // 第三步，统计哪张牌出现最多
            int sum = 0;
            for (int i = 0; i < pokeCounts.Length; ++i )
            {
                if(pokeCounts[i] > 0)
                {
                    pokeTypeCounts[pokeCounts[i]]++;
                    if (i == Poke.MAX && pokeCounts[i] > maxSamePokeNum &&
                        lstPokes.Count > 4 && 
                        pokeCounts[Poke.MAX] == 4)  // 如果是四个王，为可能的飞机进行考虑
                    {
                        pokeTypeCounts[4]--;
                        pokeTypeCounts[2] += 2;
                    }
                    else if (pokeCounts[i] > maxSamePokeNum)
                    {
                        maxSamePokeNum = pokeCounts[i];
                        maxSamePos = sum;
                        maxSamePosNo = i;
                    }
                    sum += pokeCounts[i];
                }
            }

            // 第四步，分析手牌是否是连续的
            bool isStraightTripPair = false;
            int count = lstPokes.Count/maxSamePokeNum;

            if (maxSamePokeNum == 3 &&
                pokeTypeCounts[3] > 1 && 
                pokeTypeCounts[2] == pokeTypeCounts[3])
            {
                count = lstPokes.Count / 5;
                isStraightTripPair = true;
            }

            sum = 0;
            for(int i=0; i<count; ++i)
            {
                if (pokeCounts[(i + maxSamePosNo) % Poke.MAX] == maxSamePokeNum)
                {
                    sum += pokeCounts[(i + maxSamePosNo) % Poke.MAX];
                    if (isStraightTripPair)
                    {
                        sum += 2;
                    }
                }
            }
            for(int i=1; i<count;++i)
            {
                if (pokeCounts[(maxSamePosNo + Poke.MAX - i - 3) % Poke.MAX] == maxSamePokeNum)
                {
                    sum += pokeCounts[(maxSamePosNo + Poke.MAX - i - 3) % Poke.MAX];
                    if (isStraightTripPair)
                    {
                        sum += 2;
                    }
                }
            }

            if(sum == lstPokes.Count)
            {
                // K, A, 2, 3 不算作是连续的
                if(pokeCounts[Poke.MIN] == maxSamePokeNum && 
                   pokeCounts[Poke.MIN] == pokeCounts[Poke.K] &&
                   pokeCounts[Poke.MIN] == pokeCounts[Poke.A] &&
                   pokeCounts[Poke.MIN] == pokeCounts[Poke.TWO])
                {
                    continues = false;
                }
                else
                {
                    continues = true;

                    if(pokeCounts[Poke.MIN] == maxSamePokeNum && 
                   pokeCounts[Poke.MIN] == pokeCounts[Poke.A] &&
                   pokeCounts[Poke.MIN] == pokeCounts[Poke.TWO])
                    {
                        crossA23 = true;
                    }
                    else if (pokeCounts[Poke.MIN] == maxSamePokeNum &&
                   pokeCounts[Poke.MIN] == pokeCounts[Poke.TWO])
                    {
                        cross23 = true;
                    }
                }
            }
            else
            {
                continues = false;
            }

            switch(maxSamePokeNum)
            {
                case 1:
                    #region CASE1
                    if (lstPokes.Count == 1)
                    {
                        ret.type = PokeCmbType.SINGLE;
                        ret.mainPoke = lstPokes[0];
                    }
                    else if(continues && lstPokes.Count >= 5)
                    {
                        // 判断是否是同花顺
                        bool sameColor = true;
                        for(int i=0; i<lstPokes.Count; ++i)
                        {
                            if (lstPokes[i].color != lstPokes[maxSamePos].color)
                            {
                                sameColor = false;
                                break;
                            }
                        }
                        if(sameColor)
                        {
                            ret.type = PokeCmbType.FLUSH_STRAIGHT;
                        }
                        else
                        {
                            ret.type = PokeCmbType.STRAIGHT;
                        }

                        if(crossA23)
                        {
                            ret.mainPoke = lstPokes[lstPokes.Count - 2];
                        }
                        else if(cross23)
                        {
                            ret.mainPoke = lstPokes[lstPokes.Count - 1];
                        }
                        else
                        {
                            ret.mainPoke = lstPokes[maxSamePos];
                        }
                        
                        ret.continuousNum = lstPokes.Count;
                    }
                    else
                    {
                        ret.type = PokeCmbType.ILLEGAL;
                    }
                    #endregion
                    break;
                case 2:
                    #region CASE2
                    if (lstPokes.Count % 2 != 0)
                    {
                        ret.type = PokeCmbType.ILLEGAL;
                    }
                    else if(lstPokes.Count == 2)
                    {
                        if (maxSamePosNo == Poke.MAX && isOnePeckOfCards)
                        {
                            ret.type = PokeCmbType.JOKER_QUADS;
                        }
                        else if (maxSamePosNo == Poke.MAX && lstPokes[0].color != lstPokes[1].color)
                        {
                            ret.type = PokeCmbType.ILLEGAL;
                        }
                        else
                        {
                            ret.type = PokeCmbType.PAIR;
                        }
                        
                        ret.mainPoke = lstPokes[0];
                    }
                    else if (continues && lstPokes.Count >= 6)
                    {
                        ret.type = PokeCmbType.STRAIGHT_PAIR;
                        if(crossA23)
                        {
                            ret.mainPoke = lstPokes[lstPokes.Count - 4];
                        }
                        else if(cross23)
                        {
                            ret.mainPoke = lstPokes[lstPokes.Count - 2];
                        }
                        else
                        {
                            ret.mainPoke = lstPokes[maxSamePos];
                        }
                        ret.continuousNum = lstPokes.Count / 2;
                    }
                    else
                    {
                        ret.type = PokeCmbType.ILLEGAL;
                    }
                    #endregion
                    break;
                case 3:
                    #region CASE3
                    if (lstPokes.Count == 3)
                    {
                        if(lstPokes[maxSamePos].number != Poke.MAX)     // 不能是三只王
                        {
                            ret.type = PokeCmbType.TRIP;
                            ret.mainPoke = lstPokes[maxSamePos];
                        }
                        else
                        {
                            ret.type = PokeCmbType.ILLEGAL;
                        }
                    }
                    else if(lstPokes.Count == 5 && pokeTypeCounts[2] == 1)
                    {
                        if (lstPokes[maxSamePos].number != Poke.MAX)
                        {
                            ret.type = PokeCmbType.TRIP_PAIR;
                            ret.mainPoke = lstPokes[maxSamePos];
                        }
                        else
                        {
                            ret.type = PokeCmbType.ILLEGAL;
                        }
                    }
                    else if(continues && lstPokes.Count >= 6 && 
                        pokeTypeCounts[3]*3 == lstPokes.Count)
                    {
                        ret.type = PokeCmbType.STRIGHT_TRIP;
                        int jokeCount = pokeCounts[Poke.MAX];
                        if(crossA23)
                        {
                            ret.mainPoke = lstPokes[lstPokes.Count - 6 - jokeCount];
                        }
                        else if(cross23)
                        {
                            ret.mainPoke = lstPokes[lstPokes.Count - 3 - jokeCount];
                        }
                        else
                        {
                            ret.mainPoke = lstPokes[maxSamePos];
                        }
                        ret.continuousNum = lstPokes.Count / 3;
                    }
                    else if (continues && lstPokes.Count % 5 == 0 && 
                        pokeTypeCounts[2] == pokeTypeCounts[3] &&
                        pokeTypeCounts[2] * 5 == lstPokes.Count)
                    {
                        ret.type = PokeCmbType.STRIGHT_TRIP_PAIR;
                        if (crossA23)
                        {
                            ret.mainPoke = lstPokes[lstPokes.Count - 6]; // 即使对王存在也可以(三张王怎么办， 加上王的数量)
                        }
                        else
                        {
                            ret.mainPoke = lstPokes[maxSamePos];
                        }
                        ret.continuousNum = lstPokes.Count / 5;         
                    }
                    else
                    {
                        ret.type = PokeCmbType.ILLEGAL;
                    }
                    #endregion
                    break;
                case 4:
                    #region CASE4
                    if (maxSamePos == Poke.MAX)
                    {
                        ret.type = PokeCmbType.JOKER_QUADS;
                    }
                    else if(lstPokes.Count == maxSamePokeNum)
                    {
                        ret.type = PokeCmbType.QUADS;
                        ret.mainPoke = lstPokes[maxSamePos];
                        ret.continuousNum = maxSamePokeNum;
                    }
                    else
                    {
                        ret.type = PokeCmbType.ILLEGAL;
                    }
                    #endregion
                    break;
                case 5:
                case 6:
                case 7:
                case 8:
                    #region CASE5-8
                    if (lstPokes.Count == maxSamePokeNum)
                    {
                        ret.type = PokeCmbType.QUADS;
                        ret.mainPoke = lstPokes[maxSamePos];
                        ret.continuousNum = maxSamePokeNum;
                    }
                    else
                    {
                        ret.type = PokeCmbType.ILLEGAL;
                    }
                    #endregion
                    break;
                default:
                    ret.type = PokeCmbType.ILLEGAL;
                    break;
            }

            return ret;
        }
    }
}