using UnityEngine;
using System.Collections.Generic;
using RunFaster;
using Common;
using System.Collections;


public class Testers : MonoBehaviour {

    public bool testCompareFunc = false;
    public bool testCardType = false;
    public bool testCardTypeComp = false;
    public bool testTimer = false;
    public bool testCardsView = false;
    public PokeCardsnZoneView cardsZoneView;

	// Use this for initialization
	void Start () {
        if(testCompareFunc)
        {
            TestPokerCompareFunc();
        }
        if(testCardType)
        {
            TestCardType();
        }
        if(testCardTypeComp)
        {
            TestCardTypeCompare();
        }
        if(testTimer)
        {
            TestTimer();
        }
        if(testCardsView)
        {
            TestCardZoneView();
        }
	}

    /// <summary>
    /// 扑克牌大小比较，通过排序来确定
    /// </summary>
    private void TestPokerCompareFunc()
    {
        List<Poke> lstPoker = new List<Poke>();

        for(int i=0; i<16; ++i)
        {
            Poke poke = Core.GenerateOnePoke();
            lstPoker.Add(poke);
        }

        lstPoker.Sort(Poke.CompareForDisplay);

        for(int i=0; i<lstPoker.Count; ++i)
        {
            Debug.Log(lstPoker[i].ToString());
        }
    }

    /// <summary>
    /// 扑克牌牌型比较
    /// </summary>
    private void TestCardType()
    {
        // 单支
        string[] card1 = new string[] { "A" };
        PokeColor[] colors1 = new PokeColor[] { PokeColor.CLUB };
        List<Poke> lstPoke1 = Core.GeneratePokeList(card1, colors1);
        PokeCardsType type1 = Core.GetPokeCarsdType(lstPoke1);
        if (!type1.ToString().Equals("单只 (A 草花) 0"))
        {
            Debug.LogError("判断单支出错");
        }
        Debug.Log("type1: " + type1.ToString());

        // 对子
        string[] card2 = new string[] { "8", "8"};
        PokeColor[] colors2 = new PokeColor[] { PokeColor.CLUB, PokeColor.HEART};
        List<Poke> lstPoke2 = Core.GeneratePokeList(card2, colors2);
        PokeCardsType type2 = Core.GetPokeCarsdType(lstPoke2);
        Debug.Log("type2: " + type2.ToString());

        // 三条
        string[] card3 = new string[] { "10", "10", "10" };
        PokeColor[] colors3 = new PokeColor[] { PokeColor.CLUB, PokeColor.HEART, PokeColor.DIAMOND};
        List<Poke> lstPoke3 = Core.GeneratePokeList(card3, colors3);
        PokeCardsType type3 = Core.GetPokeCarsdType(lstPoke3);
        Debug.Log("type3: " + type3.ToString());

        // 炸弹
        string[] card4 = new string[] { "Q", "Q", "Q", "Q" };
        PokeColor[] colors4 = new PokeColor[] { PokeColor.CLUB, PokeColor.HEART, PokeColor.DIAMOND, PokeColor.SPADE};
        List<Poke> lstPoke4 = Core.GeneratePokeList(card4, colors4);
        PokeCardsType type4 = Core.GetPokeCarsdType(lstPoke4);
        Debug.Log("type4: " + type4.ToString());

        // 连子
        string[] card5 = new string[] { "3", "4", "7", "6", "5"};
        PokeColor[] colors5 = new PokeColor[] { PokeColor.CLUB, PokeColor.HEART, PokeColor.DIAMOND, PokeColor.SPADE, PokeColor.DIAMOND};
        List<Poke> lstPoke5 = Core.GeneratePokeList(card5, colors5);
        PokeCardsType type5 = Core.GetPokeCarsdType(lstPoke5);
        Debug.Log("type5: " + type5.ToString());

        // 同花顺
        string[] card6 = new string[] { "8", "4", "7", "6", "5", "9"};
        PokeColor[] colors6 = new PokeColor[] { PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB};
        List<Poke> lstPoke6 = Core.GeneratePokeList(card6, colors6);
        PokeCardsType type6 = Core.GetPokeCarsdType(lstPoke6);
        Debug.Log("type6: " + type6.ToString());

        // 连对
        string[] card7 = new string[] { "9", "J", "10", "J", "9", "10" };
        PokeColor[] colors7 = new PokeColor[] { PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB };
        List<Poke> lstPoke7 = Core.GeneratePokeList(card7, colors7);
        PokeCardsType type7 = Core.GetPokeCarsdType(lstPoke7);
        Debug.Log("type7: " + type7.ToString());

        // 三带二
        string[] card8 = new string[] { "3", "4", "4", "3", "3" };
        PokeColor[] colors8 = new PokeColor[] { PokeColor.CLUB, PokeColor.HEART, PokeColor.DIAMOND, PokeColor.SPADE, PokeColor.DIAMOND };
        List<Poke> lstPoke8 = Core.GeneratePokeList(card8, colors8);
        PokeCardsType type8 = Core.GetPokeCarsdType(lstPoke8);
        Debug.Log("type8: " + type8.ToString());

        // 三连
        string[] card9 = new string[] { "9", "10", "10", "10", "9", "9" };
        PokeColor[] colors9 = new PokeColor[] { PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB };
        List<Poke> lstPoke9 = Core.GeneratePokeList(card9, colors9);
        PokeCardsType type9 = Core.GetPokeCarsdType(lstPoke9);
        Debug.Log("type9: " + type9.ToString());

        // 飞机
        string[] card10 = new string[] { "10", "9", "10", "9", "9", "10", "8", "J", "J", "8"};
        PokeColor[] colors10 = new PokeColor[] { PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB };
        List<Poke> lstPoke10 = Core.GeneratePokeList(card10, colors10);
        PokeCardsType type10 = Core.GetPokeCarsdType(lstPoke10);
        Debug.Log("type10: " + type10.ToString());

        // 炸弹
        string[] card11 = new string[] { "3", "3", "3", "3", "3" };
        PokeColor[] colors11 = new PokeColor[] { PokeColor.CLUB, PokeColor.HEART, PokeColor.DIAMOND, PokeColor.SPADE, PokeColor.DIAMOND };
        List<Poke> lstPoke11 = Core.GeneratePokeList(card11, colors11);
        PokeCardsType type11 = Core.GetPokeCarsdType(lstPoke11);
        Debug.Log("type11: " + type11.ToString());

        // 王炸
        string[] card12 = new string[] { "JO", "JO"};
        PokeColor[] colors12 = new PokeColor[] { PokeColor.BLACK, PokeColor.RED };
        List<Poke> lstPoke12 = Core.GeneratePokeList(card12, colors12);
        PokeCardsType type12 = Core.GetPokeCarsdType(lstPoke12);
        Debug.Log("type12: " + type12.ToString());

        // 王炸
        string[] card13 = new string[] { "JO", "JO", "JO", "JO" };
        PokeColor[] colors13 = new PokeColor[] { PokeColor.BLACK, PokeColor.RED, PokeColor.BLACK, PokeColor.RED };
        List<Poke> lstPoke13 = Core.GeneratePokeList(card13, colors13);
        PokeCardsType type13 = Core.GetPokeCarsdType(lstPoke13);
        Debug.Log("type13: " + type13.ToString());

        // 连子(带2)
        string[] card14 = new string[] { "3", "4", "5", "6", "2" };
        PokeColor[] colors14 = new PokeColor[] { PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB };
        List<Poke> lstPoke14 = Core.GeneratePokeList(card14, colors14);
        PokeCardsType type14 = Core.GetPokeCarsdType(lstPoke14);
        Debug.Log("type14: " + type14.ToString());

        // 飞机
        string[] card15 = new string[] { "10", "9", "10", "9", "9", "10", "JO", "JO", "JO", "JO" };
        PokeColor[] colors15 = new PokeColor[] { PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.BLACK, PokeColor.BLACK, PokeColor.RED, PokeColor.RED };
        List<Poke> lstPoke15 = Core.GeneratePokeList(card15, colors15);
        PokeCardsType type15 = Core.GetPokeCarsdType(lstPoke15);
        Debug.Log("type15: " + type15.ToString());

        // 连对
        string[] card22 = new string[] { "3", "2", "A", "A", "2", "3" };
        PokeColor[] colors22 = new PokeColor[] { PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB };
        List<Poke> lstPoke22 = Core.GeneratePokeList(card22, colors22);
        PokeCardsType type22 = Core.GetPokeCarsdType(lstPoke22);
        Debug.Log("type22: " + type22.ToString());

        // 三连对
        string[] card23 = new string[] { "3", "2", "2", "3", "2", "3" };
        PokeColor[] colors23 = new PokeColor[] { PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB };
        List<Poke> lstPoke23 = Core.GeneratePokeList(card23, colors23);
        PokeCardsType type23 = Core.GetPokeCarsdType(lstPoke23);
        Debug.Log("type23: " + type23.ToString());

        // 三连
        string[] card24 = new string[] { "3", "2", "2", "3", "2", "3", "A", "A", "A" };
        PokeColor[] colors24 = new PokeColor[] { PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB };
        List<Poke> lstPoke24 = Core.GeneratePokeList(card24, colors24);
        PokeCardsType type24 = Core.GetPokeCarsdType(lstPoke24);
        Debug.Log("type24: " + type24.ToString());

        // 下面是应该判断错误的情况，如若出现报错说明有bug
        // 三条王
        string[] card16 = new string[] { "JO", "JO", "JO" };
        PokeColor[] colors16 = new PokeColor[] { PokeColor.BLACK, PokeColor.BLACK, PokeColor.RED };
        List<Poke> lstPoke16 = Core.GeneratePokeList(card16, colors16);
        PokeCardsType type16 = Core.GetPokeCarsdType(lstPoke16);
        if(type16.type != PokeCmbType.ILLEGAL)
        {
            Debug.LogError("三条王判断失败");
        }

        // 三带二
        string[] card17 = new string[] { "JO", "4", "4", "JO", "JO" };
        PokeColor[] colors17 = new PokeColor[] { PokeColor.RED, PokeColor.HEART, PokeColor.DIAMOND, PokeColor.BLACK, PokeColor.BLACK };
        List<Poke> lstPoke17 = Core.GeneratePokeList(card17, colors17);
        PokeCardsType type17 = Core.GetPokeCarsdType(lstPoke17);
        if (type17.type != PokeCmbType.ILLEGAL)
        {
            Debug.LogError("三条王判断失败");
        }

        // 单支
        string[] card18 = new string[] { "3", "5"};
        PokeColor[] colors18 = new PokeColor[] { PokeColor.HEART, PokeColor.DIAMOND};
        List<Poke> lstPoke18 = Core.GeneratePokeList(card18, colors18);
        PokeCardsType type18 = Core.GetPokeCarsdType(lstPoke18);
        if (type18.type != PokeCmbType.ILLEGAL)
        {
            Debug.LogError("非单支判断失败");
        }

        // 对子
        string[] card19 = new string[] { "8", "8", "J"};
        PokeColor[] colors19 = new PokeColor[] { PokeColor.CLUB, PokeColor.HEART, PokeColor.HEART };
        List<Poke> lstPoke19 = Core.GeneratePokeList(card19, colors19);
        PokeCardsType type19 = Core.GetPokeCarsdType(lstPoke19);
        if (type19.type != PokeCmbType.ILLEGAL)
        {
            Debug.LogError("非单支判断失败");
        }

        // 三条
        string[] card20 = new string[] { "10", "10", "10" , "J", "Q"};
        PokeColor[] colors20 = new PokeColor[] { PokeColor.CLUB, PokeColor.HEART, PokeColor.DIAMOND, PokeColor.HEART, PokeColor.DIAMOND };
        List<Poke> lstPoke20 = Core.GeneratePokeList(card20, colors20);
        PokeCardsType type20 = Core.GetPokeCarsdType(lstPoke20);
        if (type20.type != PokeCmbType.ILLEGAL)
        {
            Debug.LogError("非三条判断失败");
        }

        // 连子(带2)
        string[] card21 = new string[] { "3", "4", "K", "A", "2" };
        PokeColor[] colors21 = new PokeColor[] { PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB };
        List<Poke> lstPoke21 = Core.GeneratePokeList(card21, colors21);
        PokeCardsType type21 = Core.GetPokeCarsdType(lstPoke21);
        if (type21.type != PokeCmbType.ILLEGAL)
        {
            Debug.LogError("非连子判断失败");
        }

        // 连对
        string[] card25 = new string[] { "3", "2", "A", "A", "2", "3", "K", "K"};
        PokeColor[] colors25 = new PokeColor[] { PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB };
        List<Poke> lstPoke25 = Core.GeneratePokeList(card25, colors25);
        PokeCardsType type25 = Core.GetPokeCarsdType(lstPoke25);
        if (type25.type != PokeCmbType.ILLEGAL)
        {
            Debug.LogError("非连对判断失败");
        }

        // 三连
        string[] card26 = new string[] { "3", "2", "2", "3", "2", "3", "A", "A", "A", "K", "K", "K" };
        PokeColor[] colors26 = new PokeColor[] { PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB };
        List<Poke> lstPoke26 = Core.GeneratePokeList(card26, colors26);
        PokeCardsType type26 = Core.GetPokeCarsdType(lstPoke26);
        if (type26.type != PokeCmbType.ILLEGAL)
        {
            Debug.LogError("非三连判断失败");
        }

        // 炸弹
        string[] card27 = new string[] { "Q", "Q", "Q", "Q", "K"};
        PokeColor[] colors27 = new PokeColor[] { PokeColor.CLUB, PokeColor.HEART, PokeColor.DIAMOND, PokeColor.SPADE, PokeColor.SPADE };
        List<Poke> lstPoke27 = Core.GeneratePokeList(card27, colors27);
        PokeCardsType type27 = Core.GetPokeCarsdType(lstPoke27);
        if (type27.type != PokeCmbType.ILLEGAL)
        {
            Debug.LogError("非炸弹判断失败");
        }

        // 王炸
        string[] card28 = new string[] { "JO", "JO", "K", "K"};
        PokeColor[] colors28 = new PokeColor[] { PokeColor.BLACK, PokeColor.RED, PokeColor.CLUB, PokeColor.CLUB };
        List<Poke> lstPoke28 = Core.GeneratePokeList(card28, colors28);
        PokeCardsType type28 = Core.GetPokeCarsdType(lstPoke28);
        if (type28.type != PokeCmbType.ILLEGAL)
        {
            Debug.LogError("非王炸判断失败");
        }

        // 王炸
        string[] card29 = new string[] { "JO", "JO", "JO", "JO", "K", "K" };
        PokeColor[] colors29 = new PokeColor[] { PokeColor.BLACK, PokeColor.RED, PokeColor.BLACK, PokeColor.RED, PokeColor.CLUB, PokeColor.CLUB };
        List<Poke> lstPoke29 = Core.GeneratePokeList(card29, colors29);
        PokeCardsType type29 = Core.GetPokeCarsdType(lstPoke29);
        if (type29.type != PokeCmbType.ILLEGAL)
        {
            Debug.LogError("非王炸判断失败");
        }

        // 飞机
        string[] card30 = new string[] { "J", "9", "J", "9", "9", "J", "JO", "JO", "JO", "JO" };
        PokeColor[] colors30 = new PokeColor[] { PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.BLACK, PokeColor.BLACK, PokeColor.RED, PokeColor.RED };
        List<Poke> lstPoke30 = Core.GeneratePokeList(card30, colors30);
        PokeCardsType type30 = Core.GetPokeCarsdType(lstPoke30);
        if (type30.type != PokeCmbType.ILLEGAL)
        {
            Debug.LogError("非飞机判断失败");
        }
    }

    /// <summary>
    /// 扑克牌牌型比较
    /// </summary>
    private void TestCardTypeCompare()
    {
        // 相同类型的比较大小
        string[] cardOne1 = new string[] { "A" };
        PokeColor[] colorsOne1 = new PokeColor[] { PokeColor.CLUB };
        List<Poke> lstPokeOne1 = Core.GeneratePokeList(cardOne1, colorsOne1);
        PokeCardsType typeOne1 = Core.GetPokeCarsdType(lstPokeOne1);

        string[] cardTwo1 = new string[] { "8" };
        PokeColor[] colorsTwo1 = new PokeColor[] { PokeColor.CLUB };
        List<Poke> lstPokeTwo1 = Core.GeneratePokeList(cardTwo1, colorsTwo1);
        PokeCardsType typeTwo1 = Core.GetPokeCarsdType(lstPokeTwo1);

        if(typeOne1.type == PokeCmbType.ILLEGAL || typeTwo1.type == PokeCmbType.ILLEGAL ||
            typeOne1 <= typeTwo1)
        {
            Debug.LogError("相同类型比较失败");
        }

        // 相同类型的顺子比较
        string[] cardOne2 = new string[] { "3", "5", "6", "4", "7" };
        PokeColor[] colorsOne2 = new PokeColor[] { PokeColor.CLUB, PokeColor.DIAMOND, PokeColor.HEART, PokeColor.SPADE, PokeColor.CLUB };
        List<Poke> lstPokeOne2 = Core.GeneratePokeList(cardOne2, colorsOne2);
        PokeCardsType typeOne2 = Core.GetPokeCarsdType(lstPokeOne2);

        string[] cardTwo2 = new string[] { "3", "5", "6", "2", "A" };
        PokeColor[] colorsTwo2 = new PokeColor[] { PokeColor.CLUB, PokeColor.DIAMOND, PokeColor.HEART, PokeColor.SPADE, PokeColor.CLUB };
        List<Poke> lstPokeTwo2 = Core.GeneratePokeList(cardTwo2, colorsTwo2);
        PokeCardsType typeTwo2 = Core.GetPokeCarsdType(lstPokeTwo2);

        if (typeOne2.type == PokeCmbType.ILLEGAL || typeTwo2.type == PokeCmbType.ILLEGAL ||
            typeOne2 <= typeTwo2)
        {
            Debug.LogError("相同类型比较失败");
        }

        // 不同类型的顺子比较
        string[] cardOne3 = new string[] { "3", "5", "6", "4", "7", "8" };
        PokeColor[] colorsOne3 = new PokeColor[] { PokeColor.CLUB, PokeColor.DIAMOND, PokeColor.HEART, PokeColor.SPADE, PokeColor.CLUB, PokeColor.CLUB };
        List<Poke> lstPokeOne3 = Core.GeneratePokeList(cardOne3, colorsOne3);
        PokeCardsType typeOne3 = Core.GetPokeCarsdType(lstPokeOne3);

        string[] cardTwo3 = new string[] { "3", "5", "6", "2", "A" };
        PokeColor[] colorsTwo3 = new PokeColor[] { PokeColor.CLUB, PokeColor.DIAMOND, PokeColor.HEART, PokeColor.SPADE, PokeColor.CLUB };
        List<Poke> lstPokeTwo3 = Core.GeneratePokeList(cardTwo3, colorsTwo3);
        PokeCardsType typeTwo3 = Core.GetPokeCarsdType(lstPokeTwo3);

        if (typeOne3.type == PokeCmbType.ILLEGAL || typeTwo3.type == PokeCmbType.ILLEGAL ||
            typeOne3 != typeTwo3)
        {
            Debug.LogError("相同类型比较失败");
        }

        // 同花顺比较
        string[] cardOne4 = new string[] { "3", "5", "6", "4", "7"};
        PokeColor[] colorsOne4 = new PokeColor[] { PokeColor.CLUB, PokeColor.DIAMOND, PokeColor.HEART, PokeColor.SPADE, PokeColor.CLUB };
        List<Poke> lstPokeOne4 = Core.GeneratePokeList(cardOne4, colorsOne4);
        PokeCardsType typeOne4 = Core.GetPokeCarsdType(lstPokeOne4);

        string[] cardTwo4 = new string[] { "4", "5", "6", "2", "A" };
        PokeColor[] colorsTwo4 = new PokeColor[] { PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB };
        List<Poke> lstPokeTwo4 = Core.GeneratePokeList(cardTwo4, colorsTwo4);
        PokeCardsType typeTwo4 = Core.GetPokeCarsdType(lstPokeTwo4);

        if (typeOne4.type == PokeCmbType.ILLEGAL || typeTwo4.type == PokeCmbType.ILLEGAL ||
            typeOne4 >= typeTwo4)
        {
            Debug.LogError("相同类型比较失败");
        }

        // 和炸弹比较
        string[] cardOne5 = new string[] { "3", "4", "6", "5", "7" };
        PokeColor[] colorsOne5 = new PokeColor[] { PokeColor.CLUB, PokeColor.DIAMOND, PokeColor.HEART, PokeColor.SPADE, PokeColor.CLUB };
        List<Poke> lstPokeOne5 = Core.GeneratePokeList(cardOne5, colorsOne5);
        PokeCardsType typeOne5 = Core.GetPokeCarsdType(lstPokeOne5);

        string[] cardTwo5 = new string[] { "5", "5", "5", "5"};
        PokeColor[] colorsTwo5 = new PokeColor[] { PokeColor.CLUB, PokeColor.DIAMOND, PokeColor.HEART, PokeColor.SPADE };
        List<Poke> lstPokeTwo5 = Core.GeneratePokeList(cardTwo5, colorsTwo5);
        PokeCardsType typeTwo5 = Core.GetPokeCarsdType(lstPokeTwo5);

        if (typeOne5.type == PokeCmbType.ILLEGAL || typeTwo5.type == PokeCmbType.ILLEGAL ||
            typeOne5 >= typeTwo5)
        {
            Debug.LogError("和炸弹比较失败");
        }

        // 和王炸比较
        string[] cardOne6 = new string[] { "3", "4", "6", "5", "7" };
        PokeColor[] colorsOne6 = new PokeColor[] { PokeColor.CLUB, PokeColor.DIAMOND, PokeColor.HEART, PokeColor.SPADE, PokeColor.CLUB };
        List<Poke> lstPokeOne6 = Core.GeneratePokeList(cardOne6, colorsOne6);
        PokeCardsType typeOne6 = Core.GetPokeCarsdType(lstPokeOne6);

        string[] cardTwo6 = new string[] { "JO", "JO"};
        PokeColor[] colorsTwo6 = new PokeColor[] { PokeColor.BLACK, PokeColor.RED};
        List<Poke> lstPokeTwo6 = Core.GeneratePokeList(cardTwo6, colorsTwo6);
        PokeCardsType typeTwo6 = Core.GetPokeCarsdType(lstPokeTwo6);

        if (typeOne6.type == PokeCmbType.ILLEGAL || typeTwo6.type == PokeCmbType.ILLEGAL ||
            typeOne6 >= typeTwo6)
        {
            Debug.LogError("和炸弹比较失败");
        }

        // 和王炸比较
        string[] cardOne7 = new string[] { "3", "4", "7", "5", "6" };
        PokeColor[] colorsOne7 = new PokeColor[] { PokeColor.CLUB, PokeColor.DIAMOND, PokeColor.HEART, PokeColor.SPADE, PokeColor.CLUB };
        List<Poke> lstPokeOne7 = Core.GeneratePokeList(cardOne7, colorsOne7);
        PokeCardsType typeOne7 = Core.GetPokeCarsdType(lstPokeOne7);

        string[] cardTwo7 = new string[] { "JO", "JO" };
        PokeColor[] colorsTwo7 = new PokeColor[] { PokeColor.BLACK, PokeColor.RED };
        List<Poke> lstPokeTwo7 = Core.GeneratePokeList(cardTwo7, colorsTwo7);
        PokeCardsType typeTwo7 = Core.GetPokeCarsdType(lstPokeTwo7, false);

        if (typeOne7.type == PokeCmbType.ILLEGAL || typeTwo7.type != PokeCmbType.ILLEGAL)
        {
            Debug.LogError("和炸弹比较失败");
        }

        Debug.Log("所有测试用例都用过");
    }

    private void TestTimer()
    {
        Timer.GetInstance().AddTimer(2.0f, (param) =>
        {
            Debug.Log("Timer1 worked OKKK");
        });

        Timer.GetInstance().AddTimer(2.0f, (param) =>
        {
            Debug.Log("Timer2 worked OKKK");
        }, 4.0f);

        Timer.GetInstance().AddTimer(2.0f, 2, (param) =>
        {
            Debug.Log("Timer3 worked OKKK");
        }, 2.0f);

        Timer.GetInstance().AddTimer(2.0f, 6.0f, (param) =>
        {
            Debug.Log("Timer4 worked OKKK");
        }, 6.0f);
    }

    private void TestCardZoneView()
    {
        string[] cards = new string[] { "8", "4", "7", "6", "5", "9" };
        PokeColor[] colors = new PokeColor[] { PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB };
        List<Poke> lstPoke6 = Core.GeneratePokeList(cards, colors);
        cardsZoneView.SetData(lstPoke6);
        //StartCoroutine(ClearPokesCoroutine());
    }

    private IEnumerator ClearPokesCoroutine()
    {
        yield return new WaitForSeconds(5);
        cardsZoneView.Clear();
    }
}