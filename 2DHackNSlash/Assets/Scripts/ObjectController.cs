using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GreedyNameSpace;

public abstract class ObjectController : MonoBehaviour {
    protected Rigidbody2D rb;

    [HideInInspector]
    public Vector2 MoveVector = Vector2.zero;
    [HideInInspector]
    public Vector2 AttackVector = Vector2.zero;
    [HideInInspector]
    public int Direction = 0;

    [HideInInspector]
    public bool Stunned = false;
    [HideInInspector]
    public bool Attacking = false;
    [HideInInspector]
    public bool Alive = true;

    [HideInInspector]
    public Transform T_Actives;
    [HideInInspector]
    public Transform T_Passives;
    [HideInInspector]
    public Transform Buffs;
    [HideInInspector]
    public Transform Debuffs;

    public delegate void on_dmg_deal(ObjectController target = null);
    public delegate void on_health_update(Value health_change);
    public delegate void on_mana_update(Value mana_change);
    public delegate void on_dealth_update();

    public on_dmg_deal ON_DMG_DEAL;
    public on_health_update ON_HEALTH_UPDATE;
    public on_mana_update ON_MANA_UPDATE;
    public on_dealth_update ON_DEATH_UPDATE;

    public float movement_animation_interval = 1f;
    public float attack_animation_interval = 1f;

    protected float RegenTimer = 0f;
    protected float RegenInterval = 0.1f;

    private Transform VFX_Transform;
    protected Transform VisualHolder;
    protected Transform MeleeAttackCollider;

    protected Collider2D RootCollider;

    protected IndicationController IC;

    protected Stats MaxStats;
    protected Stats CurrStats;

    virtual protected void Awake() {
        transform.Find("Root").gameObject.layer = LayerMask.NameToLayer("KillingGround");
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("KillingGround"), LayerMask.NameToLayer("Loot"));
        rb = transform.GetComponent<Rigidbody2D>();
        IC = GetComponentInChildren<IndicationController>();
        VisualHolder = transform.Find("Root/VisualHolder");
        MeleeAttackCollider = transform.Find("Root/MeleeAttackCollider");
        VFX_Transform = transform.Find("Root/VFX");
        T_Actives = transform.Find("Root/Actives");
        T_Passives = transform.Find("Root/Passives");
        Buffs = transform.Find("Root/Buffs");
        Debuffs = transform.Find("Root/Debuffs");
        RootCollider = transform.Find("Root").GetComponent<Collider2D>();
        MaxStats = new Stats();
        CurrStats = new Stats();
    }

    virtual protected void Start() {
    }

    virtual protected void Update() {
    }

    protected void FixedUpdate() {
        MoveUpdate();
    }

    void MoveUpdate() {
        if (MoveVector != Vector2.zero) {
            rb.MovePosition(rb.position + MoveVector * (GetCurrStats(StatsType.MOVE_SPEED) / 100) * Time.deltaTime);
        }
    }

    //Transform
    public Collider2D GetRootCollider() {
        return RootCollider;
    }

    public Transform Debuffs_T() {
        return Debuffs;
    }
    
    public Transform Buffs_T() {
        return Buffs;
    }

    public Transform GetVisualHolderTransform() {
        return VisualHolder;
    }

    public Transform GetMeleeAttackColliderTransform() {
        return MeleeAttackCollider;
    }

    public void SwapMeleeAttackCollider(Transform MeleeAttackCollider) {
        Destroy(this.MeleeAttackCollider.gameObject);
        this.MeleeAttackCollider = null;
        MeleeAttackCollider.parent = transform.Find("Root");
        this.MeleeAttackCollider = MeleeAttackCollider;
    }

    //Physics
    public bool HasForce() {
        return rb.velocity.magnitude>=0.1f;
    }

    public void MountainlizeRigibody() {
        //rb.mass = 1000;
        rb.isKinematic = true;
    }

    public void NormalizeRigibody() {
        //rb.mass = 1;
        rb.isKinematic = false;
    }

    public void ZerolizeForce() {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = 0;
    }

    public void ZerolizeDrag() {
        rb.drag = 0;
    }

    public void NormalizeDrag() {
        rb.drag = 10;
    }

    public void AddForce(Vector2 Direction, float Magnitude, ForceMode2D ForceMode) {
        rb.AddForce(Direction * Magnitude, ForceMode);
    }
    
    public float GetVFXScale() {
        return VFX_Transform.GetComponent<VFXScaler>().scale;
    }

    //Particle VFX
    public void ActiveOutsideVFXPartical(string VFX) {
        float scale = VFX_Transform.GetComponent<VFXScaler>().scale;
        GameObject VFX_OJ = Instantiate(Resources.Load("VFXPrefabs/" + VFX), rb.position, transform.rotation) as GameObject;
        VFX_OJ.transform.GetComponent<ParticleSystem>().startSize *= scale;
        VFX_OJ.name = VFX;
        Destroy(VFX_OJ,VFX_OJ.transform.GetComponent<ParticleSystem>().duration);
    }

    public void ActiveVFXParticalWithStayTime(string VFX, float StayTime) {
        float scale = VFX_Transform.GetComponent<VFXScaler>().scale;
        GameObject VFX_OJ = Instantiate(Resources.Load("VFXPrefabs/" + VFX), VFX_Transform) as GameObject;
        VFX_OJ.transform.position = VFX_Transform.position + VFX_OJ.transform.position * scale;
        VFX_OJ.transform.GetComponent<ParticleSystem>().startSize *= scale;
        VFX_OJ.name = VFX;
        Destroy(VFX_OJ, StayTime);
    }
    public void ActiveVFXParticle(string VFX) {
        float scale = VFX_Transform.GetComponent<VFXScaler>().scale;
        GameObject VFX_OJ = Instantiate(Resources.Load("VFXPrefabs/" + VFX), VFX_Transform) as GameObject;
        VFX_OJ.transform.position = VFX_Transform.position + VFX_OJ.transform.position*scale;
        VFX_OJ.transform.GetComponent<ParticleSystem>().startSize *= scale;
        VFX_OJ.name = VFX;
    }

    public void DeactiveVFXParticle(string VFX) {
        Destroy(VFX_Transform.Find(VFX).gameObject);
    }

    public void ActiveOneShotVFXParticle(string VFX) {
        float scale = VFX_Transform.GetComponent<VFXScaler>().scale;
        GameObject VFX_OJ = Instantiate(Resources.Load("VFXPrefabs/" + VFX), VFX_Transform) as GameObject;
        VFX_OJ.transform.position = VFX_Transform.position + VFX_OJ.transform.position * scale;
        VFX_OJ.transform.GetComponent<ParticleSystem>().startSize *= scale;
        VFX_OJ.name = VFX;
        float length = VFX_OJ.transform.GetComponent<ParticleSystem>().duration;
        Destroy(VFX_OJ,length);
    }

    //Anim VFX
    public void ActiveOneShotVFXAnim(string VFX,int layer) {
        float scale = VFX_Transform.GetComponent<VFXScaler>().scale;
        GameObject VFX_OJ = Instantiate(Resources.Load("VFXPrefabs/" + VFX), VFX_Transform) as GameObject;
        VFX_OJ.transform.position += VFX_Transform.position;
        VFX_OJ.transform.localScale *= scale;
        VFX_OJ.transform.GetComponent<SpriteRenderer>().sortingOrder = layer;
        VFX_OJ.name = VFX;
        float length = VFX_OJ.transform.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
        Destroy(VFX_OJ, length);
    }


    //Combat
    public void HealHP(Value heal_hp) {
        if (GetCurrStats(StatsType.HEALTH) < GetMaxStats(StatsType.HEALTH) && GetCurrStats(StatsType.HEALTH) + heal_hp.Amount <= GetMaxStats(StatsType.HEALTH)) {
            AddCurrStats(StatsType.HEALTH, heal_hp.Amount);
            if(heal_hp.Pop_Update)
                IC.PopUpText(heal_hp);
        } else if (GetCurrStats(StatsType.HEALTH) < GetMaxStats(StatsType.HEALTH) && GetCurrStats(StatsType.HEALTH) + heal_hp.Amount > GetMaxStats(StatsType.HEALTH)) {
            heal_hp.Amount = GetMaxStats(StatsType.HEALTH) - GetCurrStats(StatsType.HEALTH);
            AddCurrStats(StatsType.HEALTH,heal_hp.Amount);
            if (heal_hp.Pop_Update)
                IC.PopUpText(heal_hp);
        }
    }
    public void HealMana(Value heal_mana) {
        if (GetCurrStats(StatsType.MANA) < GetMaxStats(StatsType.MANA) && GetCurrStats(StatsType.MANA) + heal_mana.Amount <= GetMaxStats(StatsType.MANA)) {
            AddCurrStats(StatsType.MANA,heal_mana.Amount);
        } else if (GetCurrStats(StatsType.MANA) < GetMaxStats(StatsType.MANA) && GetCurrStats(StatsType.MANA) + heal_mana.Amount > GetMaxStats(StatsType.MANA)) {
            heal_mana.Amount = GetMaxStats(StatsType.MANA) - GetCurrStats(StatsType.MANA);
            AddCurrStats(StatsType.MANA,heal_mana.Amount);
        }
    }

    public Value AutoAttackDamageDeal(float TargetDefense) {
        Value dmg = new Value(0, 0, false, GetComponent<ObjectController>());
        if (Random.value < (GetCurrStats(StatsType.CRIT_CHANCE) / 100)) {
            dmg.Amount += GetCurrStats(StatsType.AD) * (GetCurrStats(StatsType.CRIT_DMG) / 100);
            dmg.Amount += GetCurrStats(StatsType.MD) * (GetCurrStats(StatsType.CRIT_DMG) / 100);
            dmg.IsCrit = true;
        } else {
            dmg.Amount = GetCurrStats(StatsType.AD) + GetCurrStats(StatsType.MD);
            dmg.IsCrit = false;
        }
        float reduced_dmg = dmg.Amount * (TargetDefense / 100);
        dmg.Amount = dmg.Amount - reduced_dmg;
        return dmg;
    }

    abstract public void DeductHealth(Value dmg);

    protected virtual void Die() {
        SetCurrStats(StatsType.HEALTH,0);
        Alive = false;
        RootCollider.enabled = false;
        VisualHolder.gameObject.SetActive(false);
    }

    public void DeductMana(Value mana_cost) {
        if (GetCurrStats(StatsType.MANA) - mana_cost.Amount >= 0)//Double check
            DecCurrStats(StatsType.MANA,mana_cost.Amount);
    }

    protected void Regen() {
        if (RegenTimer >= RegenInterval) {
            ON_MANA_UPDATE += HealMana;
            ON_MANA_UPDATE(new Value(GetCurrStats(StatsType.MANA_REGEN) / 10, 1,false,null,false,false));
            ON_MANA_UPDATE -= HealMana;
            if (GetCurrStats(StatsType.HEALTH_REGEN) > 0f) {
                ON_HEALTH_UPDATE += HealHP;
                ON_HEALTH_UPDATE(new Value(GetCurrStats(StatsType.HEALTH_REGEN) / 10, 1, false, null, false, false));
                ON_HEALTH_UPDATE -= HealHP;
            }
            RegenTimer = 0;
        } else {
            RegenTimer += Time.deltaTime;
        }
    }

    public bool HasBuff(System.Type buff) {
        Buff[] buffs = Buffs.GetComponentsInChildren<Buff>();
        if (buffs.Length == 0)
            return false;
        foreach (Buff _buff in buffs)
            if (_buff.GetType() == buff)
                return true;
        return false;
    }

    public Buff GetBuff(System.Type buff) {
        Buff[] buffs = Buffs.GetComponentsInChildren<Buff>();
        foreach (Buff _buff in buffs)
            if (_buff.GetType() == buff)
                return _buff;
        return null;
    }

    public Debuff GetDebuff(System.Type debuff) {
        Debuff[] debuffs = Debuffs.GetComponentsInChildren<Debuff>();
        foreach (Debuff _debuff in debuffs)
            if (_debuff.GetType() == debuff)
                return _debuff;
        return null;
    }

    public bool HasDebuff(System.Type debuff) {
        Debuff[] debuffs = Debuffs.GetComponentsInChildren<Debuff>();
        if (debuffs.Length == 0)
            return false;
        foreach (Debuff _debuff in debuffs)
            if (_debuff.GetType() == debuff)
                return true;
        return false;
    }

    public int DebuffStack(System.Type debuff) {
        Debuff[] debuffs = Debuffs.GetComponentsInChildren<Debuff>();
        if (debuffs.Length == 0)
            return 0;
        int stack = 0;
        foreach (Debuff _debuff in debuffs)
            if (_debuff.GetType() == debuff)
                stack++;
        return stack;
    }

    //Animation
    public float GetMovementAnimSpeed() {
        return (GetCurrStats(StatsType.MOVE_SPEED) / 100) / (movement_animation_interval);
    }
    public float GetAttackAnimSpeed() {
        return (GetCurrStats(StatsType.ATTACK_SPEED) / 100) / (attack_animation_interval);
    }
    public float GetPhysicsSpeedFactor() {
        if (!Attacking) {
            if (GetCurrStats(StatsType.MOVE_SPEED) < 100)
                return 1 + GetCurrStats(StatsType.MOVE_SPEED) / 100;
            else if (GetCurrStats(StatsType.MOVE_SPEED) > 100)
                return 1 - GetCurrStats(StatsType.MOVE_SPEED) / 100;
            else
                return 1;
        } else {
            if (GetCurrStats(StatsType.ATTACK_SPEED) < 100)
                return 1 + GetCurrStats(StatsType.ATTACK_SPEED) / 100;
            else if (GetCurrStats(StatsType.ATTACK_SPEED) > 100)
                return 1 - GetCurrStats(StatsType.ATTACK_SPEED) / 100;
            else
                return 1;
        }
    }

    //Stats
    public float GetMaxStats(StatsType type) {
        return MaxStats.Get(type);
    }

    public void SetMaxStats(StatsType type, float value) {
        MaxStats.Set(type, value);
    }

    public void AddMaxStats(StatsType type, float value) {
        MaxStats.Add(type, value);
    }

    public void DecMaxStats(StatsType type, float value) {
        MaxStats.Dec(type, value);
    }

    public float GetCurrStats(StatsType type) {
        return CurrStats.Get(type);
    }

    public void SetCurrStats(StatsType type, float value) {
        CurrStats.Set(type, value);
    }

    public void AddCurrStats(StatsType type, float value) {
        CurrStats.Add(type, value);
    }

    public void DecCurrStats(StatsType type, float value) {
        CurrStats.Dec(type, value);
    }

    public abstract string GetName();

}
