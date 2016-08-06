using UnityEngine;
using UnityEditor;
using System.Collections;
using RunFaster;

[CustomEditor(typeof(Testers))]
public class TesterEditor : Editor 
{
    public override void OnInspectorGUI()
    {
        Testers tester = (Testers)target;
        EditorGUILayout.BeginVertical();
        tester.testCompareFunc = EditorGUILayout.Toggle("扑克牌大小比较", tester.testCompareFunc);
        tester.testCardType = EditorGUILayout.Toggle("牌型测试", tester.testCardType);
        tester.testCardTypeComp = EditorGUILayout.Toggle("牌型比较", tester.testCardTypeComp);
        tester.testTimer = EditorGUILayout.Toggle("Timer测试", tester.testTimer);
        if (tester.testCardsView = EditorGUILayout.Toggle("测试牌区视图", tester.testCardsView))
        {
            tester.cardsZoneView = (PokeCardsnZoneView)EditorGUILayout.ObjectField("发牌区", tester.cardsZoneView, typeof(GameObject));
        }
        EditorGUILayout.EndVertical();
    }
}
