using UnityEngine;
using System.Collections;
using UnityEditor;
using GreedyNameSpace;
[CustomEditor(typeof(EquipmentController))]
public class EquipmentControllerInspector : Editor {
    EquipmentController EC;

    void OnEnable() {
        EC = (EquipmentController)target;
    }

    public override void OnInspectorGUI() {
        if (EC.E == null)
            EC.E = new Equipment();
        EC.E.Name = EditorGUILayout.TextField("Name", EC.E.Name);
        EC.E.Class = (CLASS)EditorGUILayout.EnumPopup("Class", EC.E.Class);
        EC.E.EquipType = (EQUIPTYPE)EditorGUILayout.EnumPopup("Type", EC.E.EquipType);
        EC.E.Set = (EQUIPSET)EditorGUILayout.EnumPopup("Set", EC.E.Set);
        EC.E.Description = EditorGUILayout.TextField("Description", EC.E.Description);
        if (GUILayout.Button("Preset Solid Fields")) {
            if (EC.E.EquipType == EQUIPTYPE.Trinket)
                Debug.Log("No preset for Trinket");
            else {
                PresetFields PF = ((GameObject)Resources.Load("PresetFields/" + EC.E.Class.ToString() + "/" + EC.E.EquipType.ToString())).GetComponent<PresetFields>();
                EC.SolidFields = PF.SolidFields;
            }
        }        
        base.OnInspectorGUI();
        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }
}
