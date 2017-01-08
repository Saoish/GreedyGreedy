using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using InControl;

public class ControllerManager : MonoBehaviour {

    public static ControlActions Actions;    

    //public static string J_LB 
    [HideInInspector]
    public static Vector2 MoveVector;
    [HideInInspector]
    public static Vector2 AttackVector;
    [HideInInspector]
    public static int Direction; //0,1,2,3 -> Down,Left,Right,Up

    [HideInInspector]
    public static bool SyncActions = true;


    public static ControllerManager instance;
    public static ControllerManager Instance { get { return instance; } }    
         

    void Awake() {        
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
        } else {
            instance = this;
            DontDestroyOnLoad(this);
        }
        Actions = new ControlActions();
        MoveVector = new Vector2(0, 0);
        AttackVector = new Vector2(0, 0);
    }

    // Use this for initialization
    void Start () {

    }

    // Update is called once per frame
    void Update() {
        if (SyncActions) {
            UpdateMoveVector();
            UpdateAttackVector();
            UpdateDirection();
        } else {
            AttackVector = Vector2.zero;
            MoveVector = Vector2.zero;
        }
    }
   
    //Moving Update
    void UpdateMoveVector() {
        if (CacheManager.MP != null) {
            Vector2 PureActionVector = Vector3.Normalize(Actions.Move - Vector2.zero);
            if (CacheManager.MP.Casting)
                MoveVector = PureActionVector;
            else {
                float x = PureActionVector.x;
                float y = PureActionVector.y;
                if (CacheManager.MP.ContactDetector.HasContacted) {
                    if (CacheManager.MP.ContactDetector.ContactLeft && x < 0 || CacheManager.MP.ContactDetector.ContactRight && x > 0)
                        x = 0;
                    if (CacheManager.MP.ContactDetector.ContactTop && y > 0 || CacheManager.MP.ContactDetector.ContactDown && y < 0)
                        y = 0;                    
                }
                MoveVector = new Vector2(x, y);
            }
        }
    }

    //Attacking Update
    void UpdateAttackVector() {
        AttackVector = Vector3.Normalize(Actions.Attack - Vector2.zero);
    }
    
    void UpdateDirection() {
        if (AttackVector == Vector2.zero) {
            if (Mathf.Abs(Actions.Move.X) > Mathf.Abs(Actions.Move.Y)) {
                if (Actions.Move.X < 0)
                    Direction = 1;
                else if (Actions.Move.X > 0)
                    Direction = 2;
            }
            if (Mathf.Abs(Actions.Move.X) < Mathf.Abs(Actions.Move.Y)) {
                if (Actions.Move.Y < 0)
                    Direction = 0;
                else if (Actions.Move.Y > 0)
                    Direction = 3;
            }
        }

        if (Mathf.Abs(Actions.Attack.X) > Mathf.Abs(Actions.Attack.Y)) {
            if (Actions.Attack.X < 0)
                Direction = 1;
            else if (Actions.Attack.X > 0)
                Direction = 2;
        }
        if (Mathf.Abs(Actions.Attack.X) < Mathf.Abs(Actions.Attack.Y)) {
            if (Actions.Attack.Y < 0)
                Direction = 0;
            else if (Actions.Attack.Y > 0)
                Direction = 3;
        }
    }

}
