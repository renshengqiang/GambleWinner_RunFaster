using UnityEngine;
using System.Collections.Generic;
using RunFaster;

public class Testers : MonoBehaviour {

    public bool testCompareFunc = false;
    public bool testCardType = false;

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
    }
}
