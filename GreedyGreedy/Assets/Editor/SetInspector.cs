using UnityEngine;
using System.Collections;
using UnityEditor;
using GreedyNameSpace;
using System.Collections.Generic;

[CustomEditor(typeof(Set))]
public class SetInspector : Editor {

    public static bool FoldSL = false;
    public static bool FoldBL = false;

    public List<bool> BL_FoldTrack;

    Set ThisSet;

    void OnEnable() {
        ThisSet = (Set)target;
        BL_FoldTrack = new List<bool>();
    }


    public override void OnInspectorGUI() {
        //base.OnInspectorGUI();
        UpdateEquipments();
        UpdateBounuses();
        if (GUI.changed) {
            EditorUtility.SetDirty(ThisSet);
        }
        //base.OnInspectorGUI();
    }

    private void UpdateEquipments() {
        FoldSL = EditorGUILayout.Foldout(FoldSL, "SetList");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        if (FoldSL) {
            EditorGUILayout.BeginVertical();
            int SL_Size = ThisSet.SetList.Count;
            SL_Size = EditorGUILayout.IntField("Equipments", SL_Size);
            if (SL_Size != ThisSet.SetList.Count) {
                while (SL_Size > ThisSet.SetList.Count) {
                    ThisSet.SetList.Add(null);
                }
                while (SL_Size < ThisSet.SetList.Count) {
                    ThisSet.SetList.RemoveAt(ThisSet.SetList.Count - 1);
                }
            }
            for (int i = 0; i < ThisSet.SetList.Count; i++) {
                ThisSet.SetList[i] = EditorGUILayout.ObjectField("Equip", ThisSet.SetList[i], typeof(EquipmentController), false) as EquipmentController;
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void UpdateBounuses() {
        FoldBL = EditorGUILayout.Foldout(FoldBL, "Bounuses");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        if (FoldBL) {
            EditorGUILayout.BeginVertical();
            int B_Size = ThisSet.Bounuses.Count;
            B_Size = EditorGUILayout.IntField("Size", B_Size);
            if ((B_Size != ThisSet.Bounuses.Count)) {
                while (B_Size > ThisSet.Bounuses.Count) {
                    ThisSet.Bounuses.Add(new Bounus());
                } while (B_Size < ThisSet.Bounuses.Count) {
                    ThisSet.Bounuses.RemoveAt(ThisSet.Bounuses.Count - 1);
                }
            }
            if (BL_FoldTrack.Count != ThisSet.Bounuses.Count) {
                while (BL_FoldTrack.Count < ThisSet.Bounuses.Count)
                    BL_FoldTrack.Add(false);
                while (BL_FoldTrack.Count > ThisSet.Bounuses.Count)
                    BL_FoldTrack.RemoveAt(BL_FoldTrack.Count - 1);
            }
            for (int i = 0; i < ThisSet.Bounuses.Count; i++) {
                BL_FoldTrack[i] = EditorGUILayout.Foldout(BL_FoldTrack[i], "Bounus " + i);
                if (!BL_FoldTrack[i])
                    continue;
                ThisSet.Bounuses[i].bounus_type = (Bounus.BounusType)EditorGUILayout.EnumPopup("Bounus Type", ThisSet.Bounuses[i].bounus_type);
                ThisSet.Bounuses[i].condiction = EditorGUILayout.IntField("Active Condiction", ThisSet.Bounuses[i].condiction);
                switch (ThisSet.Bounuses[i].bounus_type) {
                    case Bounus.BounusType.Stats:                 
                        EditorGUILayout.BeginHorizontal();
                        ThisSet.Bounuses[i].stats_bounus.stats_type = (STATSTYPE)EditorGUILayout.EnumPopup(ThisSet.Bounuses[i].stats_bounus.stats_type);
                        ThisSet.Bounuses[i].stats_bounus.value_type = (SetStatsField.ValueType)EditorGUILayout.EnumPopup(ThisSet.Bounuses[i].stats_bounus.value_type);
                        ThisSet.Bounuses[i].stats_bounus.value = (float)EditorGUILayout.FloatField(ThisSet.Bounuses[i].stats_bounus.value);
                        EditorGUILayout.EndHorizontal();
                        break;
                    case Bounus.BounusType.Passive:
                        ThisSet.Bounuses[i].passive_bounus = EditorGUILayout.ObjectField("Passive", ThisSet.Bounuses[i].passive_bounus, typeof(PassiveSkill), false) as PassiveSkill;
                        break;
                }
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndHorizontal();
    }
}
