using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public abstract class Debuff : MonoBehaviour {
    [HideInInspector]
    public float Duration = 0;

    protected ObjectController target;


    protected virtual void Update() {
        if (target == null)
            return;
        else {
            if (Duration > 0)
                Duration -= Time.deltaTime;
            else if (Duration <= 0)
                RemoveDebuff();
        }
    }

    virtual public void ApplyDebuff(ObjectController target) {
        this.target = target;
        gameObject.transform.SetParent(target.Debuffs_T());
        gameObject.transform.localPosition = Vector3.zero;
    }

    abstract protected void RemoveDebuff();
}
