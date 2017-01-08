﻿using UnityEngine;
using System.Collections;

public class EssenseGain : Value {

    public EssenseGain(float Amount, ObjectController Source, System.Type Type) {
        this.Amount = Amount;
        this.Source = Source;
        this.Type = Type;
    }
}
