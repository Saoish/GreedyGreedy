using UnityEngine;
using System.Collections;
using System;

public class EndlessCourage : PassiveSkill {
    public float EssenseCost_DEC_Percentage = 20;
    string DescriptionTemplate() {
        return "Reduce your Essense cost on auto attack by "+ EssenseCost_DEC_Percentage+"%";
    }

    public override void GenerateDescription() {
        Description = DescriptionTemplate();
    }

    public override void InitSkill(ObjectController OC, int lvl = 0) {
        base.InitSkill(OC, lvl);        
    }

    public override void ApplyPassive() {
        OC.ON_ESSENSE_COST += EndlessCouragePassive;
    }

    private void EndlessCouragePassive(EssenseCost essense_cost) {
        //Value v = new Value(gameObject.GetComponent<EndlessCourage>());
        //OC.GainEssense(essense_cost);

    }
}
