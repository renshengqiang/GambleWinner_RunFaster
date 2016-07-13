using UnityEngine;
using System.Collections.Generic;
using RunFaster;

public class Testers : MonoBehaviour {

    public bool testCompareFunc = false;
    public bool testCardType = false;
    public bool testCardTypeComp = false;

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
	}

    private void TestPokerCompareFunc()
    {
        List<Poke> lstPoker = new List<Poke>();

        for(int i=0; i<16; ++i)
        {
            Poke poke = Utils.GenerateOnePoke();
            lstPoker.Add(poke);
        }

        lstPoker.Sort(Poke.CompareForDisplay);

        for(int i=0; i<lstPoker.Count; ++i)
        {
            Debug.Log(lstPoker[i].ToString());
        }
    }

    private void TestCardType()
    {
        // 单支
        string[] card1 = new string[] { "A" };
        PokeColor[] colors1 = new PokeColor[] { PokeColor.CLUB };
        List<Poke> lstPoke1 = Utils.GeneratePokeList(card1, colors1);
        PokeCardsType type1 = Utils.GetPokeCarsdType(lstPoke1);
        if (!type1.ToString().Equals("单只 (A 草花) 0"))
        {
            Debug.LogError("判断单支出错");
        }
        Debug.Log("type1: " + type1.ToString());

        // 对子
        string[] card2 = new string[] { "8", "8"};
        PokeColor[] colors2 = new PokeColor[] { PokeColor.CLUB, PokeColor.HEART};
        List<Poke> lstPoke2 = Utils.GeneratePokeList(card2, colors2);
        PokeCardsType type2 = Utils.GetPokeCarsdType(lstPoke2);
        Debug.Log("type2: " + type2.ToString());

        // 三条
        string[] card3 = new string[] { "10", "10", "10" };
        PokeColor[] colors3 = new PokeColor[] { PokeColor.CLUB, PokeColor.HEART, PokeColor.DIAMOND};
        List<Poke> lstPoke3 = Utils.GeneratePokeList(card3, colors3);
        PokeCardsType type3 = Utils.GetPokeCarsdType(lstPoke3);
        Debug.Log("type3: " + type3.ToString());

        // 炸弹
        string[] card4 = new string[] { "Q", "Q", "Q", "Q" };
        PokeColor[] colors4 = new PokeColor[] { PokeColor.CLUB, PokeColor.HEART, PokeColor.DIAMOND, PokeColor.SPADE};
        List<Poke> lstPoke4 = Utils.GeneratePokeList(card4, colors4);
        PokeCardsType type4 = Utils.GetPokeCarsdType(lstPoke4);
        Debug.Log("type4: " + type4.ToString());

        // 连子
        string[] card5 = new string[] { "3", "4", "7", "6", "5"};
        PokeColor[] colors5 = new PokeColor[] { PokeColor.CLUB, PokeColor.HEART, PokeColor.DIAMOND, PokeColor.SPADE, PokeColor.DIAMOND};
        List<Poke> lstPoke5 = Utils.GeneratePokeList(card5, colors5);
        PokeCardsType type5 = Utils.GetPokeCarsdType(lstPoke5);
        Debug.Log("type5: " + type5.ToString());

        // 同花顺
        string[] card6 = new string[] { "8", "4", "7", "6", "5", "9"};
        PokeColor[] colors6 = new PokeColor[] { PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB};
        List<Poke> lstPoke6 = Utils.GeneratePokeList(card6, colors6);
        PokeCardsType type6 = Utils.GetPokeCarsdType(lstPoke6);
        Debug.Log("type6: " + type6.ToString());

        // 连对
        string[] card7 = new string[] { "9", "J", "10", "J", "9", "10" };
        PokeColor[] colors7 = new PokeColor[] { PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB };
        List<Poke> lstPoke7 = Utils.GeneratePokeList(card7, colors7);
        PokeCardsType type7 = Utils.GetPokeCarsdType(lstPoke7);
        Debug.Log("type7: " + type7.ToString());

        // 三带二
        string[] card8 = new string[] { "3", "4", "4", "3", "3" };
        PokeColor[] colors8 = new PokeColor[] { PokeColor.CLUB, PokeColor.HEART, PokeColor.DIAMOND, PokeColor.SPADE, PokeColor.DIAMOND };
        List<Poke> lstPoke8 = Utils.GeneratePokeList(card8, colors8);
        PokeCardsType type8 = Utils.GetPokeCarsdType(lstPoke8);
        Debug.Log("type8: " + type8.ToString());

        // 三连
        string[] card9 = new string[] { "9", "10", "10", "10", "9", "9" };
        PokeColor[] colors9 = new PokeColor[] { PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB };
        List<Poke> lstPoke9 = Utils.GeneratePokeList(card9, colors9);
        PokeCardsType type9 = Utils.GetPokeCarsdType(lstPoke9);
        Debug.Log("type9: " + type9.ToString());

        // 飞机
        string[] card10 = new string[] { "10", "9", "10", "9", "9", "10", "8", "J", "J", "8"};
        PokeColor[] colors10 = new PokeColor[] { PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB };
        List<Poke> lstPoke10 = Utils.GeneratePokeList(card10, colors10);
        PokeCardsType type10 = Utils.GetPokeCarsdType(lstPoke10);
        Debug.Log("type10: " + type10.ToString());

        // 炸弹
        string[] card11 = new string[] { "3", "3", "3", "3", "3" };
        PokeColor[] colors11 = new PokeColor[] { PokeColor.CLUB, PokeColor.HEART, PokeColor.DIAMOND, PokeColor.SPADE, PokeColor.DIAMOND };
        List<Poke> lstPoke11 = Utils.GeneratePokeList(card11, colors11);
        PokeCardsType type11 = Utils.GetPokeCarsdType(lstPoke11);
        Debug.Log("type11: " + type11.ToString());

        // 王炸
        string[] card12 = new string[] { "JO", "JO"};
        PokeColor[] colors12 = new PokeColor[] { PokeColor.BLACK, PokeColor.RED };
        List<Poke> lstPoke12 = Utils.GeneratePokeList(card12, colors12);
        PokeCardsType type12 = Utils.GetPokeCarsdType(lstPoke12);
        Debug.Log("type12: " + type12.ToString());

        // 王炸
        string[] card13 = new string[] { "JO", "JO", "JO", "JO" };
        PokeColor[] colors13 = new PokeColor[] { PokeColor.BLACK, PokeColor.RED, PokeColor.BLACK, PokeColor.RED };
        List<Poke> lstPoke13 = Utils.GeneratePokeList(card13, colors13);
        PokeCardsType type13 = Utils.GetPokeCarsdType(lstPoke13);
        Debug.Log("type13: " + type13.ToString());

        // 连子(带2)
        string[] card14 = new string[] { "3", "4", "5", "6", "2" };
        PokeColor[] colors14 = new PokeColor[] { PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB };
        List<Poke> lstPoke14 = Utils.GeneratePokeList(card14, colors14);
        PokeCardsType type14 = Utils.GetPokeCarsdType(lstPoke14);
        Debug.Log("type14: " + type14.ToString());

        // 飞机
        string[] card15 = new string[] { "10", "9", "10", "9", "9", "10", "JO", "JO", "JO", "JO" };
        PokeColor[] colors15 = new PokeColor[] { PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.BLACK, PokeColor.BLACK, PokeColor.RED, PokeColor.RED };
        List<Poke> lstPoke15 = Utils.GeneratePokeList(card15, colors15);
        PokeCardsType type15 = Utils.GetPokeCarsdType(lstPoke15);
        Debug.Log("type15: " + type15.ToString());

        // 连对
        string[] card22 = new string[] { "3", "2", "A", "A", "2", "3" };
        PokeColor[] colors22 = new PokeColor[] { PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB };
        List<Poke> lstPoke22 = Utils.GeneratePokeList(card22, colors22);
        PokeCardsType type22 = Utils.GetPokeCarsdType(lstPoke22);
        Debug.Log("type22: " + type22.ToString());

        // 三连对
        string[] card23 = new string[] { "3", "2", "2", "3", "2", "3" };
        PokeColor[] colors23 = new PokeColor[] { PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB };
        List<Poke> lstPoke23 = Utils.GeneratePokeList(card23, colors23);
        PokeCardsType type23 = Utils.GetPokeCarsdType(lstPoke23);
        Debug.Log("type23: " + type23.ToString());

        // 三连
        string[] card24 = new string[] { "3", "2", "2", "3", "2", "3", "A", "A", "A" };
        PokeColor[] colors24 = new PokeColor[] { PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB };
        List<Poke> lstPoke24 = Utils.GeneratePokeList(card24, colors24);
        PokeCardsType type24 = Utils.GetPokeCarsdType(lstPoke24);
        Debug.Log("type24: " + type24.ToString());

        // 下面是应该判断错误的情况，如若出现报错说明有bug
        // 三条王
        string[] card16 = new string[] { "JO", "JO", "JO" };
        PokeColor[] colors16 = new PokeColor[] { PokeColor.BLACK, PokeColor.BLACK, PokeColor.RED };
        List<Poke> lstPoke16 = Utils.GeneratePokeList(card16, colors16);
        PokeCardsType type16 = Utils.GetPokeCarsdType(lstPoke16);
        if(type16.type != PokeCmbType.ILLEGAL)
        {
            Debug.LogError("三条王判断失败");
        }

        // 三带二
        string[] card17 = new string[] { "JO", "4", "4", "JO", "JO" };
        PokeColor[] colors17 = new PokeColor[] { PokeColor.RED, PokeColor.HEART, PokeColor.DIAMOND, PokeColor.BLACK, PokeColor.BLACK };
        List<Poke> lstPoke17 = Utils.GeneratePokeList(card17, colors17);
        PokeCardsType type17 = Utils.GetPokeCarsdType(lstPoke17);
        if (type17.type != PokeCmbType.ILLEGAL)
        {
            Debug.LogError("三条王判断失败");
        }

        // 单支
        string[] card18 = new string[] { "3", "5"};
        PokeColor[] colors18 = new PokeColor[] { PokeColor.HEART, PokeColor.DIAMOND};
        List<Poke> lstPoke18 = Utils.GeneratePokeList(card18, colors18);
        PokeCardsType type18 = Utils.GetPokeCarsdType(lstPoke18);
        if (type18.type != PokeCmbType.ILLEGAL)
        {
            Debug.LogError("非单支判断失败");
        }

        // 对子
        string[] card19 = new string[] { "8", "8", "J"};
        PokeColor[] colors19 = new PokeColor[] { PokeColor.CLUB, PokeColor.HEART, PokeColor.HEART };
        List<Poke> lstPoke19 = Utils.GeneratePokeList(card19, colors19);
        PokeCardsType type19 = Utils.GetPokeCarsdType(lstPoke19);
        if (type19.type != PokeCmbType.ILLEGAL)
        {
            Debug.LogError("非单支判断失败");
        }

        // 三条
        string[] card20 = new string[] { "10", "10", "10" , "J", "Q"};
        PokeColor[] colors20 = new PokeColor[] { PokeColor.CLUB, PokeColor.HEART, PokeColor.DIAMOND, PokeColor.HEART, PokeColor.DIAMOND };
        List<Poke> lstPoke20 = Utils.GeneratePokeList(card20, colors20);
        PokeCardsType type20 = Utils.GetPokeCarsdType(lstPoke20);
        if (type20.type != PokeCmbType.ILLEGAL)
        {
            Debug.LogError("非三条判断失败");
        }

        // 连子(带2)
        string[] card21 = new string[] { "3", "4", "K", "A", "2" };
        PokeColor[] colors21 = new PokeColor[] { PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB };
        List<Poke> lstPoke21 = Utils.GeneratePokeList(card21, colors21);
        PokeCardsType type21 = Utils.GetPokeCarsdType(lstPoke21);
        if (type21.type != PokeCmbType.ILLEGAL)
        {
            Debug.LogError("非连子判断失败");
        }

        // 连对
        string[] card25 = new string[] { "3", "2", "A", "A", "2", "3", "K", "K"};
        PokeColor[] colors25 = new PokeColor[] { PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB };
        List<Poke> lstPoke25 = Utils.GeneratePokeList(card25, colors25);
        PokeCardsType type25 = Utils.GetPokeCarsdType(lstPoke25);
        if (type25.type != PokeCmbType.ILLEGAL)
        {
            Debug.LogError("非连对判断失败");
        }

        // 三连
        string[] card26 = new string[] { "3", "2", "2", "3", "2", "3", "A", "A", "A", "K", "K", "K" };
        PokeColor[] colors26 = new PokeColor[] { PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB };
        List<Poke> lstPoke26 = Utils.GeneratePokeList(card26, colors26);
        PokeCardsType type26 = Utils.GetPokeCarsdType(lstPoke26);
        if (type26.type != PokeCmbType.ILLEGAL)
        {
            Debug.LogError("非三连判断失败");
        }

        // 炸弹
        string[] card27 = new string[] { "Q", "Q", "Q", "Q", "K"};
        PokeColor[] colors27 = new PokeColor[] { PokeColor.CLUB, PokeColor.HEART, PokeColor.DIAMOND, PokeColor.SPADE, PokeColor.SPADE };
        List<Poke> lstPoke27 = Utils.GeneratePokeList(card27, colors27);
        PokeCardsType type27 = Utils.GetPokeCarsdType(lstPoke27);
        if (type27.type != PokeCmbType.ILLEGAL)
        {
            Debug.LogError("非炸弹判断失败");
        }

        // 王炸
        string[] card28 = new string[] { "JO", "JO", "K", "K"};
        PokeColor[] colors28 = new PokeColor[] { PokeColor.BLACK, PokeColor.RED, PokeColor.CLUB, PokeColor.CLUB };
        List<Poke> lstPoke28 = Utils.GeneratePokeList(card28, colors28);
        PokeCardsType type28 = Utils.GetPokeCarsdType(lstPoke28);
        if (type28.type != PokeCmbType.ILLEGAL)
        {
            Debug.LogError("非王炸判断失败");
        }

        // 王炸
        string[] card29 = new string[] { "JO", "JO", "JO", "JO", "K", "K" };
        PokeColor[] colors29 = new PokeColor[] { PokeColor.BLACK, PokeColor.RED, PokeColor.BLACK, PokeColor.RED, PokeColor.CLUB, PokeColor.CLUB };
        List<Poke> lstPoke29 = Utils.GeneratePokeList(card29, colors29);
        PokeCardsType type29 = Utils.GetPokeCarsdType(lstPoke29);
        if (type29.type != PokeCmbType.ILLEGAL)
        {
            Debug.LogError("非王炸判断失败");
        }

        // 飞机
        string[] card30 = new string[] { "J", "9", "J", "9", "9", "J", "JO", "JO", "JO", "JO" };
        PokeColor[] colors30 = new PokeColor[] { PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.CLUB, PokeColor.BLACK, PokeColor.BLACK, PokeColor.RED, PokeColor.RED };
        List<Poke> lstPoke30 = Utils.GeneratePokeList(card30, colors30);
        PokeCardsType type30 = Utils.GetPokeCarsdType(lstPoke30);
        if (type30.type != PokeCmbType.ILLEGAL)
        {
            Debug.LogError("非飞机判断失败");
        }
    }

    private void TestCardTypeCompare()
    {

    }
}
