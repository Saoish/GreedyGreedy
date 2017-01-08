using UnityEngine;
using System.Collections;
using GreedyNameSpace;
using System.Collections.Generic;

public class ClassSelectionButton : MonoBehaviour {
    public CLASS ClassToReg;
    public CharacterCreationController CCC;

    List<ClassSelectionButton> OtherCSBs;

    void Awake() {
        OtherCSBs = new List<ClassSelectionButton>();
        foreach(ClassSelectionButton CSB in transform.parent.GetComponentsInChildren<ClassSelectionButton>(true)) {
            if (CSB != GetComponent<ClassSelectionButton>())
                OtherCSBs.Add(CSB);
        }
    }


    public void ActiveSelected() {
        foreach (ClassSelectionButton CSB in OtherCSBs) {
            CSB.DeactiveSelected();
        }
        GetComponent<Animator>().SetBool("Selected", true);
    }

    public void DeactiveSelected()         {
        GetComponent<Animator>().SetBool("Selected", false);
    }

    public void RegisterClass() {
        CCC.RegisteredClass = ClassToReg;
        CCC.ClassRegistered = true;
        ActiveSelected();
    }

}
