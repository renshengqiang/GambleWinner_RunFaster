using UnityEngine;
using System.Collections.Generic;
using RunFaster;

public class Testers : MonoBehaviour {

    public bool testCompareFunc = false;

	// Use this for initialization
	void Start () {
        if(testCompareFunc)
        {
            TestPokerCompareFunc();
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
}
