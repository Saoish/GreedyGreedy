using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GreedyNameSpace;

public abstract class ObjectController : MonoBehaviour {
    protected Rigidbody2D rb;

    public GameObject DieVFX;
    public AudioClip DieSFX;

    [HideInInspector]
    public Vector2 MoveVector = Vector2.zero;
    [HideInInspector]
    public Vector2 AttackVector = Vector2.zero;
    [HideInInspector]
    public int Direction = 0;

    [HideInInspector]
    public bool Casting = false;
    [HideInInspector]
    public bool Stunned = false;
    [HideInInspector]
    public bool Attacking = false;
    [HideInInspector]
    public bool Alive = true;

    [HideInInspector]
    public Transform T_Buffs;
    [HideInInspector]
    public Transform T_Debuffs;
    [HideInInspector]
    public List<ActiveSkill> Actives;
    [HideInInspector]
    public List<PassiveSkill> Passives;

    protected List<Skill> CachedTreeSkills;



    public delegate void on_dmg_deal(ObjectController target = null);
    public delegate void on_dmg_taken(Damage dmg);
    public delegate void on_health_gain(HealHP heal);
    public delegate void on_essense_cost(EssenseCost essense_cost);
    public delegate void on_essense_gain(EssenseGain essense_gain);    
    public delegate void on_dealth_update();

    public on_dmg_deal ON_DMG_DEAL;
    public on_dmg_taken ON_DMG_TAKEN;
    public on_health_gain ON_HEALTH_GAIN;
    public on_essense_cost ON_ESSENSE_COST;
    public on_essense_gain ON_ESSENSE_GAIN;
    public on_dealth_update ON_DEATH_UPDATE;

    public float movement_animation_interval = 1f;
    public float attack_animation_interval = 1f;

    protected float RegenTimer = 0f;
    protected float RegenInterval = 0.1f;

    private Transform VFX_Transform;
    protected Transform VisualHolder;
    protected Transform MeleeAttackCollider_T;

    protected Collider2D RootCollider;

    protected IndicationController IC;

    [HideInInspector]
    public Stats MaxStats;

    protected Stats CurrStats;

    virtual protected void Awake() {
        transform.Find("Root").gameObject.layer = LayerMask.NameToLayer(CollisionLayer.KillingGround);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(CollisionLayer.KillingGround), LayerMask.NameToLayer(CollisionLayer.Loot));
        rb = transform.GetComponent<Rigidbody2D>();
        IC = GetComponentInChildren<IndicationController>();
        VisualHolder = transform.Find("Root/VisualHolder");
        MeleeAttackCollider_T = transform.Find("Root/MeleeAttackCollider");
        VFX_Transform = transform.Find("Root/VFX");
        T_Buffs = transform.Find("Root/Buffs");
        T_Debuffs = transform.Find("Root/Debuffs");
        RootCollider = transform.Find("Root").GetComponent<Collider2D>();

        CurrStats = new Stats();
        Actives = new List<ActiveSkill>();
        Passives = new List<PassiveSkill>();
    }

    virtual protected void Start() {
    }

    virtual protected void Update() {
    }

    virtual protected void FixedUpdate() {
        MoveUpdate();
    }

    protected void MoveUpdate() {
        if (MoveVector != Vector2.zero) {            
            rb.MovePosition(rb.position + MoveVector * (GetCurrStats(STATSTYPE.MOVE_SPEED) / 100) * Time.deltaTime);
        }
    }

    //Transform
    public Transform Skills_T() {
        return transform.Find("Root/Skills");
    }

    public Collider2D GetRootCollider() {
        return RootCollider;
    }

    public Transform Debuffs_T() {
        return T_Debuffs;
    }
    
    public Transform Buffs_T() {
        return T_Buffs;
    }

    public Transform GetVisualHolderTransform() {
        return VisualHolder;
    }

    public MeleeAttackCollider Melee_AC{
        get { return MeleeAttackCollider_T.GetComponent<MeleeAttackCollider>(); }
    }

    public void SwapMeleeAttackCollider(Transform MeleeAttackCollider) {
        Destroy(this.MeleeAttackCollider_T.gameObject);
        this.MeleeAttackCollider_T = null;
        MeleeAttackCollider.parent = transform.Find("Root");
        this.MeleeAttackCollider_T = MeleeAttackCollider;
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
    public void ClearVFX() {
        foreach (Transform VFX in VFX_Transform)
            Destroy(VFX.gameObject);
    }
    public void ActiveOutsideVFXPartical(GameObject VFX) {
        string cached_name = VFX.name;
        float scale = VFX_Transform.GetComponent<VFXScaler>().scale;
        VFX = Instantiate(VFX, rb.position, transform.rotation) as GameObject;
        VFX.transform.GetComponent<ParticleSystem>().startSize *= scale;
        VFX.name = cached_name;
        Destroy(VFX, VFX.transform.GetComponent<ParticleSystem>().duration);
    }
    public void ActiveVFXParticalWithStayTime(GameObject VFX, float StayTime) {
        string cached_name = VFX.name;
        float scale = VFX_Transform.GetComponent<VFXScaler>().scale;
        VFX = Instantiate(VFX, VFX_Transform) as GameObject;
        VFX.transform.position = VFX_Transform.position + VFX.transform.position * scale;
        VFX.transform.GetComponent<ParticleSystem>().startSize *= scale;
        VFX.name = cached_name;
        Destroy(VFX, StayTime);
    }
    public void ActiveVFXParticle(GameObject VFX) {
        string cached_name = VFX.name;
        float scale = VFX_Transform.GetComponent<VFXScaler>().scale;
        VFX = Instantiate(VFX, VFX_Transform) as GameObject;
        VFX.transform.position = VFX_Transform.position + VFX.transform.position * scale;
        VFX.transform.GetComponent<ParticleSystem>().startSize *= scale;
        VFX.name = cached_name;
    }
    public void DeactiveVFXParticle(GameObject VFX) {
        Destroy(VFX_Transform.Find(VFX.name).gameObject);
    }

    public void ActiveOneShotVFXParticle(GameObject VFX) {
        string cached_name = VFX.name;
        float scale = VFX_Transform.GetComponent<VFXScaler>().scale;
        VFX = Instantiate(VFX,VFX_Transform) as GameObject;
        VFX.transform.position = VFX_Transform.position + VFX.transform.position * scale;
        VFX.transform.GetComponent<ParticleSystem>().startSize *= scale;
        VFX.name = cached_name;
        float length = VFX.transform.GetComponent<ParticleSystem>().duration;
        Destroy(VFX, length);
    }

    //Combat
    public void HealHP(HealHP heal_hp) {
        if (GetCurrStats(STATSTYPE.HEALTH) < GetMaxStats(STATSTYPE.HEALTH) && GetCurrStats(STATSTYPE.HEALTH) + heal_hp.Amount <= GetMaxStats(STATSTYPE.HEALTH)) {
            AddCurrStats(STATSTYPE.HEALTH, heal_hp.Amount);
            IC.PopUpText(heal_hp);
        } else if (GetCurrStats(STATSTYPE.HEALTH) < GetMaxStats(STATSTYPE.HEALTH) && GetCurrStats(STATSTYPE.HEALTH) + heal_hp.Amount > GetMaxStats(STATSTYPE.HEALTH)) {
            heal_hp.Amount = GetMaxStats(STATSTYPE.HEALTH) - GetCurrStats(STATSTYPE.HEALTH);
            AddCurrStats(STATSTYPE.HEALTH,heal_hp.Amount);
            IC.PopUpText(heal_hp);
        }
    }
    public void GainEssense(EssenseGain essense_gain) {
        if (GetCurrStats(STATSTYPE.ESSENSE) < GetMaxStats(STATSTYPE.ESSENSE) && GetCurrStats(STATSTYPE.ESSENSE) + essense_gain.Amount <= GetMaxStats(STATSTYPE.ESSENSE)) {
            AddCurrStats(STATSTYPE.ESSENSE, essense_gain.Amount);
        } else if (GetCurrStats(STATSTYPE.ESSENSE) < GetMaxStats(STATSTYPE.ESSENSE) && GetCurrStats(STATSTYPE.ESSENSE) + essense_gain.Amount > GetMaxStats(STATSTYPE.ESSENSE)) {
            essense_gain.Amount = GetMaxStats(STATSTYPE.ESSENSE) - GetCurrStats(STATSTYPE.ESSENSE);
            AddCurrStats(STATSTYPE.ESSENSE, essense_gain.Amount);
        }
    }

    abstract public void DeductHealth(Damage dmg);

    protected virtual void Die() {
        SetCurrStats(STATSTYPE.HEALTH,0);
        Alive = false;
        RootCollider.enabled = false;
        VisualHolder.gameObject.SetActive(false);
        ClearBuffs();
        ClearDebuffs();
        //ClearVFX();
        ////VFX_Transform.gameObject.SetActive(false);
        IC.DisableHealthBar();
        if(DieSFX!=null)
            AudioSource.PlayClipAtPoint(DieSFX, transform.position, GameManager.SFX_Volume);
    }

    public void DeductEssense(EssenseCost essense_cost) {        
        if (GetCurrStats(STATSTYPE.ESSENSE) - essense_cost.Amount >= 0)//Double check
            DecCurrStats(STATSTYPE.ESSENSE,essense_cost.Amount);
    }

    protected void Regen() {//No composition on events
        if (RegenTimer >= RegenInterval) {
            if (GetCurrStats(STATSTYPE.ESSENSE_REGEN) > 0f) {
                if (GetCurrStats(STATSTYPE.ESSENSE) + GetCurrStats(STATSTYPE.ESSENSE_REGEN) / 10 > GetMaxStats(STATSTYPE.ESSENSE))
                    SetCurrStats(STATSTYPE.ESSENSE, GetMaxStats(STATSTYPE.ESSENSE));
                else
                    AddCurrStats(STATSTYPE.ESSENSE, GetCurrStats(STATSTYPE.ESSENSE_REGEN) / 10);
            }
            if (GetCurrStats(STATSTYPE.HEALTH_REGEN) > 0f) {
                if (GetCurrStats(STATSTYPE.HEALTH) + GetCurrStats(STATSTYPE.HEALTH_REGEN) / 10 > GetMaxStats(STATSTYPE.HEALTH))
                    SetCurrStats(STATSTYPE.HEALTH, GetMaxStats(STATSTYPE.HEALTH));
                else
                    AddCurrStats(STATSTYPE.HEALTH, GetCurrStats(STATSTYPE.HEALTH_REGEN) / 10);
            }
            RegenTimer = 0;
        } else {
            RegenTimer += Time.deltaTime;
        }
    }

    public void ClearBuffs() {
        Buff[] buffs = T_Buffs.GetComponentsInChildren<Buff>();        
        foreach (Buff _buff in buffs)
            _buff.RemoveBuff();
    }

    public void ClearDebuffs() {
        Debuff[] debuffs = T_Debuffs.GetComponentsInChildren<Debuff>();        
        foreach (Debuff _debuff in debuffs) {
            _debuff.RemoveDebuff();
        }
    }

    public bool HasBuff(System.Type buff) {
        Buff[] buffs = T_Buffs.GetComponentsInChildren<Buff>();
        if (buffs.Length == 0)
            return false;
        foreach (Buff _buff in buffs)
            if (_buff.GetType() == buff)
                return true;
        return false;
    }

    public Buff GetBuff(System.Type buff) {
        Buff[] buffs = T_Buffs.GetComponentsInChildren<Buff>();
        foreach (Buff _buff in buffs)
            if (_buff.GetType() == buff)
                return _buff;
        return null;
    }

    public Debuff GetDebuff(System.Type debuff) {
        Debuff[] debuffs = T_Debuffs.GetComponentsInChildren<Debuff>();
        foreach (Debuff _debuff in debuffs)
            if (_debuff.GetType() == debuff)
                return _debuff;
        return null;
    }

    public bool HasDebuff(System.Type debuff) {
        Debuff[] debuffs = T_Debuffs.GetComponentsInChildren<Debuff>();
        if (debuffs.Length == 0)
            return false;
        foreach (Debuff _debuff in debuffs)
            if (_debuff.GetType() == debuff)
                return true;
        return false;
    }

    public int DebuffStack(System.Type debuff) {
        Debuff[] debuffs = T_Debuffs.GetComponentsInChildren<Debuff>();
        if (debuffs.Length == 0)
            return 0;
        int stack = 0;
        foreach (Debuff _debuff in debuffs)
            if (_debuff.GetType() == debuff)
                stack++;
        return stack;
    }

    public ActiveSkill GetActive(System.Type active_skill_type) {
        foreach (ActiveSkill active in Actives) {
            if (active.GetType() == active_skill_type)
                return active;
        }
        return null;
    }

    public PassiveSkill GetPassive(System.Type passive_skill_type) {
        foreach (PassiveSkill passive in Passives) {
            if (passive.GetType() == passive_skill_type)
                return passive;
        }
        return null;
    }

    //Animation
    public float GetMovementAnimSpeed() {
        return (GetCurrStats(STATSTYPE.MOVE_SPEED) / 100) / (movement_animation_interval);
    }
    public float GetAttackAnimSpeed() {
        return (GetCurrStats(STATSTYPE.ATTACK_SPEED) / 100) / (attack_animation_interval);
    }
    //public float GetPhysicsSpeedFactor() {
    //    if (!Attacking) {
    //        if (GetCurrStats(STATSTYPE.MOVE_SPEED) < 100)
    //            return 1 + GetCurrStats(STATSTYPE.MOVE_SPEED) / 100;
    //        else if (GetCurrStats(STATSTYPE.MOVE_SPEED) > 100)
    //            return 1 - GetCurrStats(STATSTYPE.MOVE_SPEED) / 100;
    //        else
    //            return 1;
    //    } else {
    //        if (GetCurrStats(STATSTYPE.ATTACK_SPEED) < 100)
    //            return 1 + GetCurrStats(STATSTYPE.ATTACK_SPEED) / 100;
    //        else if (GetCurrStats(STATSTYPE.ATTACK_SPEED) > 100)
    //            return 1 - GetCurrStats(STATSTYPE.ATTACK_SPEED) / 100;
    //        else
    //            return 1;
    //    }
    //}

    //Stats

    //public float MaxHealth {
    //    get { return _MaxStats.Get(STATSTYPE.HEALTH); }
    //    set { _MaxStats.Set(STATSTYPE.HEALTH, value); }
    //}

    //General Stats handling
    public float GetMaxStats(STATSTYPE type) {
        return MaxStats.Get(type);
    }

    public void SetMaxStats(STATSTYPE type, float value) {
        MaxStats.Set(type, value);
    }

    public void AddMaxStats(STATSTYPE type, float value) {
        MaxStats.Add(type, value);
    }

    public void DecMaxStats(STATSTYPE type, float value) {
        MaxStats.Dec(type, value);
    }

    public float GetCurrStats(STATSTYPE type) {
        return CurrStats.Get(type);
    }

    public void SetCurrStats(STATSTYPE type, float value) {
        CurrStats.Set(type, value);
    }

    public void AddCurrStats(STATSTYPE type, float value) {
        CurrStats.Add(type, value);
    }

    public void DecCurrStats(STATSTYPE type, float value) {
        CurrStats.Dec(type, value);
    }

    //Specific Stats Handling
    public float CurrHealth {
        get { return CurrStats.Get(STATSTYPE.HEALTH); }
        set { CurrStats.Set(STATSTYPE.HEALTH, value); }            
    }
    public float CurrEssense {
        get { return CurrStats.Get(STATSTYPE.ESSENSE); }
        set { CurrStats.Set(STATSTYPE.ESSENSE, value); }
    }
    public float CurrDamage {
        get { return CurrStats.Get(STATSTYPE.DAMAGE); }
        set { CurrStats.Set(STATSTYPE.DAMAGE, value); }
    }
    public float CurrAttackSpeed {
        get { return CurrStats.Get(STATSTYPE.ATTACK_SPEED); }
        set { CurrStats.Set(STATSTYPE.ATTACK_SPEED, value); }
    }
    public float CurrMoveSpeed {
        get { return CurrStats.Get(STATSTYPE.MOVE_SPEED); }
        set { CurrStats.Set(STATSTYPE.MOVE_SPEED, value); }
    }
    public float CurrDefense {
        get { return CurrStats.Get(STATSTYPE.DEFENSE); }
        set { CurrStats.Set(STATSTYPE.DEFENSE, value); }
    }
    public float CurrPenetration {
        get { return CurrStats.Get(STATSTYPE.PENETRATION); }
        set { CurrStats.Set(STATSTYPE.PENETRATION, value); }
    }
    public float CurrCritChance {
        get { return CurrStats.Get(STATSTYPE.CRIT_CHANCE); }
        set { CurrStats.Set(STATSTYPE.CRIT_CHANCE, value); }
    }
    public float CurrCritDmg {
        get { return CurrStats.Get(STATSTYPE.CRIT_DMG); }
        set { CurrStats.Set(STATSTYPE.CRIT_DMG, value); }
    }
    public float CurrLPH {
        get { return CurrStats.Get(STATSTYPE.LPH); }
        set { CurrStats.Set(STATSTYPE.LPH, value); }
    }
    public float CurrHaste {
        get { return CurrStats.Get(STATSTYPE.HASTE); }
        set { CurrStats.Set(STATSTYPE.HASTE, value); }
    }
    public float CurrHealthRegen {
        get { return CurrStats.Get(STATSTYPE.HEALTH_REGEN); }
        set { CurrStats.Set(STATSTYPE.HEALTH_REGEN, value); }
    }
    public float CurrEssenseRegen {
        get { return CurrStats.Get(STATSTYPE.ESSENSE_REGEN); }
        set { CurrStats.Set(STATSTYPE.ESSENSE_REGEN, value); }
    }

    public float MaxHealth {
        get { return MaxStats.Get(STATSTYPE.HEALTH); }
        set { MaxStats.Set(STATSTYPE.HEALTH, value); }
    }
    public float MaxEssense {
        get { return MaxStats.Get(STATSTYPE.ESSENSE); }
        set { MaxStats.Set(STATSTYPE.ESSENSE, value); }
    }
    public float MaxDamage {
        get { return MaxStats.Get(STATSTYPE.DAMAGE); }
        set { MaxStats.Set(STATSTYPE.DAMAGE, value); }
    }
    public float MaxAttackSpeed {
        get { return MaxStats.Get(STATSTYPE.ATTACK_SPEED); }
        set { MaxStats.Set(STATSTYPE.ATTACK_SPEED, value); }
    }
    public float MaxMoveSpeed {
        get { return MaxStats.Get(STATSTYPE.MOVE_SPEED); }
        set { MaxStats.Set(STATSTYPE.MOVE_SPEED, value); }
    }
    public float MaxDefense {
        get { return MaxStats.Get(STATSTYPE.DEFENSE); }
        set { MaxStats.Set(STATSTYPE.DEFENSE, value); }
    }
    public float MaxPenetration {
        get { return MaxStats.Get(STATSTYPE.PENETRATION); }
        set { MaxStats.Set(STATSTYPE.PENETRATION, value); }
    }
    public float MaxCritChance {
        get { return MaxStats.Get(STATSTYPE.CRIT_CHANCE); }
        set { MaxStats.Set(STATSTYPE.CRIT_CHANCE, value); }
    }
    public float MaxCritDmg {
        get { return MaxStats.Get(STATSTYPE.CRIT_DMG); }
        set { MaxStats.Set(STATSTYPE.CRIT_DMG, value); }
    }
    public float MaxLPH {
        get { return MaxStats.Get(STATSTYPE.LPH); }
        set { MaxStats.Set(STATSTYPE.LPH, value); }
    }
    public float MaxHaste {
        get { return MaxStats.Get(STATSTYPE.HASTE); }
        set { MaxStats.Set(STATSTYPE.HASTE, value); }
    }
    public float MaxHealthRegen {
        get { return MaxStats.Get(STATSTYPE.HEALTH_REGEN); }
        set { MaxStats.Set(STATSTYPE.HEALTH_REGEN, value); }
    }
    public float MaxEssenseRegen {
        get { return MaxStats.Get(STATSTYPE.ESSENSE_REGEN); }
        set { MaxStats.Set(STATSTYPE.ESSENSE_REGEN, value); }
    }

    public abstract string GetName();

    public Vector2 Position {
        get { return rb.position; }
        set { rb.position = value; }
    }

}
