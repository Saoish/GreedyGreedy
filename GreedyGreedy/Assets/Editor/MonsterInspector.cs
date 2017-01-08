using UnityEngine;
using System.Collections;
using GreedyNameSpace;
using System.Collections.Generic;
using UnityEditor;
[CustomEditor(typeof(Monster), true)]
public class MonsterInspector : Editor {
    Monster EC;
    public static bool FoldStats = false;
    void OnEnable() {
        EC = (Monster)target;
    }

    public override void OnInspectorGUI() {
        if (EC.MaxStats == null || EC.MaxStats.stats.Length < Stats.Size - 1) {
            EC.MaxStats = new Stats();
        }
        base.OnInspectorGUI();
        FoldStats = EditorGUILayout.Foldout(FoldStats, "MaxStats");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        if (FoldStats) {
            EditorGUILayout.BeginVertical();
            for (int i = 0; i < EC.MaxStats.stats.Length; i++) {
                EC.MaxStats.stats[i] = EditorGUILayout.FloatField(((STATSTYPE)i).ToString(), EC.MaxStats.stats[i]);
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndHorizontal();
        if (GUI.changed) {
            EditorUtility.SetDirty(EC);
        }
    }
}
