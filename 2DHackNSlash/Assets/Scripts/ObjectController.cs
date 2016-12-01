using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
            rb.MovePosition(rb.position + MoveVector * (GetCurrMoveSpd() / 100) * Time.deltaTime);
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
    public void ActiveOutsideVFXPartical(string VFX,int layer) {
        float scale = VFX_Transform.GetComponent<VFXScaler>().scale;
        GameObject VFX_OJ = Instantiate(Resources.Load("VFXPrefabs/" + VFX), rb.position, transform.rotation) as GameObject;
        VFX_OJ.transform.GetComponent<ParticleSystem>().startSize *= scale;
        VFX_OJ.transform.GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingOrder = layer;
        VFX_OJ.name = VFX;
        Destroy(VFX_OJ,VFX_OJ.transform.GetComponent<ParticleSystem>().duration);
    }

    public void ActiveVFXParticalWithStayTime(string VFX, float StayTime,int layer) {
        float scale = VFX_Transform.GetComponent<VFXScaler>().scale;
        GameObject VFX_OJ = Instantiate(Resources.Load("VFXPrefabs/" + VFX), VFX_Transform) as GameObject;
        VFX_OJ.transform.position = VFX_Transform.position + VFX_OJ.transform.position * scale;
        VFX_OJ.transform.GetComponent<ParticleSystem>().startSize *= scale;
        VFX_OJ.transform.GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingOrder = layer;
        VFX_OJ.name = VFX;
        Destroy(VFX_OJ, StayTime);
    }
    public void ActiveVFXParticle(string VFX,int layer) {
        float scale = VFX_Transform.GetComponent<VFXScaler>().scale;
        GameObject VFX_OJ = Instantiate(Resources.Load("VFXPrefabs/" + VFX), VFX_Transform) as GameObject;
        VFX_OJ.transform.position = VFX_Transform.position + VFX_OJ.transform.position*scale;
        VFX_OJ.transform.GetComponent<ParticleSystem>().startSize *= scale;
        VFX_OJ.transform.GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingOrder = layer;
        VFX_OJ.name = VFX;
    }

    public void DeactiveVFXParticle(string VFX) {
        Destroy(VFX_Transform.Find(VFX).gameObject);
    }

    public void ActiveOneShotVFXParticle(string VFX,int layer) {
        float scale = VFX_Transform.GetComponent<VFXScaler>().scale;
        GameObject VFX_OJ = Instantiate(Resources.Load("VFXPrefabs/" + VFX), VFX_Transform) as GameObject;
        VFX_OJ.transform.position = VFX_Transform.position + VFX_OJ.transform.position * scale;
        VFX_OJ.transform.GetComponent<ParticleSystem>().startSize *= scale;
        VFX_OJ.transform.GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingOrder = layer;
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

    abstract public string GetName();


    //Combat
    public void HealHP(Value heal_hp) {
        if (GetCurrHealth() < GetMaxHealth() && GetCurrHealth() + heal_hp.Amount <= GetMaxHealth()) {
            SetCurrHealth(GetCurrHealth()+heal_hp.Amount);
            IC.PopUpText(heal_hp);
        } else if (GetCurrHealth() < GetMaxHealth() && GetCurrHealth() + heal_hp.Amount > GetMaxHealth()) {
            heal_hp.Amount = GetMaxHealth() - GetCurrHealth();
            SetCurrHealth(GetCurrHealth() + heal_hp.Amount);
            IC.PopUpText(heal_hp);
        }
    }
    public void HealMana(Value heal_mana) {
        if (GetCurrMana() < GetMaxMana() && GetCurrMana() + heal_mana.Amount <= GetMaxMana()) {
            SetCurrMana(GetCurrMana()+heal_mana.Amount);
        } else if (GetCurrMana() < GetMaxMana() && GetCurrMana() + heal_mana.Amount > GetMaxMana()) {
            heal_mana.Amount = GetMaxMana() - GetCurrMana();
            SetCurrMana(GetCurrMana()+heal_mana.Amount);
        }
    }

    public Value AutoAttackDamageDeal(float TargetDefense) {
        Value dmg = Value.CreateValue(0, 0, false, GetComponent<ObjectController>());
        if (Random.value < (GetCurrCritChance() / 100)) {
            dmg.Amount += GetCurrAD() * (GetCurrCritDmgBounus() / 100);
            dmg.Amount += GetCurrMD() * (GetCurrCritDmgBounus() / 100);
            dmg.IsCrit = true;
        } else {
            dmg.Amount = GetCurrAD() + GetCurrMD();
            dmg.IsCrit = false;
        }
        float reduced_dmg = dmg.Amount * (TargetDefense / 100);
        dmg.Amount = dmg.Amount - reduced_dmg;
        return dmg;
    }

    abstract public void DeductHealth(Value dmg);

    protected virtual void Die() {
        SetCurrHealth(0);
        Alive = false;
        RootCollider.enabled = false;
        VisualHolder.gameObject.SetActive(false);
    }

    public void DeductMana(Value mana_cost) {
        if (GetCurrMana() - mana_cost.Amount >= 0)//Double check
            SetCurrMana(GetCurrMana() - mana_cost.Amount);
    }

    protected void ManaRegen() {
        if (RegenTimer >= RegenInterval) {
            ON_MANA_UPDATE += HealMana;
            ON_MANA_UPDATE(Value.CreateValue(GetCurrManaRegen() / 10, 1));
            ON_MANA_UPDATE -= HealMana;
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
        return (GetCurrMoveSpd() / 100) / (movement_animation_interval);
    }
    public float GetAttackAnimSpeed() {
        return (GetCurrAttkSpd() / 100) / (attack_animation_interval);
    }
    public float GetPhysicsSpeedFactor() {
        if (!Attacking) {
            if (GetCurrMoveSpd() < 100)
                return 1 + GetCurrMoveSpd() / 100;
            else if (GetCurrMoveSpd() > 100)
                return 1 - GetCurrMoveSpd() / 100;
            else
                return 1;
        } else {
            if (GetCurrAttkSpd() < 100)
                return 1 + GetCurrAttkSpd() / 100;
            else if (GetCurrMoveSpd() > 100)
                return 1 - GetCurrAttkSpd() / 100;
            else
                return 1;
        }
    }

    //Stats
    abstract public float GetMaxHealth();
    abstract public float GetMaxMana();
    abstract public float GetMaxAD();
    abstract public float GetMaxMD();
    abstract public float GetMaxAttkSpd();
    abstract public float GetMaxMoveSpd();
    abstract public float GetMaxDefense();
    abstract public float GetMaxCritChance();
    abstract public float GetMaxCritDmgBounus();
    abstract public float GetMaxLPH();
    abstract public float GetMaxManaRegen();

    abstract public float GetCurrHealth();
    abstract public float GetCurrMana();
    abstract public float GetCurrAD();
    abstract public float GetCurrMD();
    abstract public float GetCurrAttkSpd();
    abstract public float GetCurrMoveSpd();
    abstract public float GetCurrDefense();
    abstract public float GetCurrCritChance();
    abstract public float GetCurrCritDmgBounus();
    abstract public float GetCurrLPH();
    abstract public float GetCurrManaRegen();

    abstract public void SetMaxHealth(float health);
    abstract public void SetMaxMana(float mana);
    abstract public void SetMaxAD(float ad);
    abstract public void SetMaxMD(float md);
    abstract public void SetMaxAttkSpd(float attkspd);
    abstract public void SetMaxMoveSpd(float movespd);
    abstract public void SetMaxDefense(float defense);
    abstract public void SetMaxCritChance(float critchance);
    abstract public void SetMaxCritDmgBounus(float critdmg);
    abstract public void SetMaxLPH(float lph);
    abstract public void SetMaxManaRegen(float mph);

    abstract public void SetCurrHealth(float health);
    abstract public void SetCurrMana(float mana);
    abstract public void SetCurrAD(float ad);
    abstract public void SetCurrMD(float md);
    abstract public void SetCurrAttkSpd(float attkspd);
    abstract public void SetCurrMoveSpd(float movespd);
    abstract public void SetCurrDefense(float defense);
    abstract public void SetCurrCritChance(float critchance);
    abstract public void SetCurrCritDmgBounus(float critdmg);
    abstract public void SetCurrLPH(float lph);
    abstract public void SetCurrManaRegen(float mph);
}
