using UnityEngine;
using System.Collections;
using InControl;

public class ControlActions : PlayerActionSet {    
    //Keyboard
    Key submit = Key.F;
    Key cancel = Key.Escape;

    Key menu = Key.Escape;
    Key inventory = Key.C;
    
    Key moveup = Key.W;
    Key moveleft = Key.A;
    Key movedown = Key.S;
    Key moveright = Key.D;
    
    Key attackup = Key.UpArrow;
    Key attackleft = Key.LeftArrow;
    Key attackdown = Key.DownArrow;
    Key attackright = Key.RightArrow;

    Key flip = Key.Tab;
    Key toggleName = Key.LeftShift;
    
    Key skill_0 = Key.Key1;
    Key skill_1 = Key.Key2;
    Key skill_2 = Key.Key3;
    Key skill_3 = Key.Key4;

    public PlayerAction Submit;
    public PlayerAction Cancel;

    public PlayerAction Menu;
    public PlayerAction Inventory;

    public PlayerAction MoveUp;
    public PlayerAction MoveLeft;
    public PlayerAction MoveDown;
    public PlayerAction MoveRight;

    public PlayerTwoAxisAction Move;

    public PlayerAction AttackUp;
    public PlayerAction AttackLeft;
    public PlayerAction AttackDown;
    public PlayerAction AttackRight;

    public PlayerTwoAxisAction Attack;

    public PlayerAction Flip;
    public PlayerAction NextPage;
    public PlayerAction PreviousPage;

    public PlayerAction ToggleName;

    public PlayerAction Skill_0;
    public PlayerAction Skill_1;
    public PlayerAction Skill_2;
    public PlayerAction Skill_3;

    public ControlActions() {
        Submit = CreatePlayerAction("Submit");
        Cancel = CreatePlayerAction("Cancel");

        Menu = CreatePlayerAction("Menu");
        Inventory = CreatePlayerAction("Inventory");

        MoveUp = CreatePlayerAction("MoveUp");
        MoveLeft = CreatePlayerAction("MoveLeft");
        MoveDown = CreatePlayerAction("MoveDown");
        MoveRight = CreatePlayerAction("MoveRight");
        Move = CreateTwoAxisPlayerAction(MoveLeft, MoveRight, MoveDown, MoveUp);     

        AttackUp = CreatePlayerAction("AttackUp");
        AttackLeft = CreatePlayerAction("AttackLeft");
        AttackDown = CreatePlayerAction("AttackDown");
        AttackRight = CreatePlayerAction("AttackRight");
        Attack = CreateTwoAxisPlayerAction(AttackLeft, AttackRight, AttackDown, AttackUp);

        Flip = CreatePlayerAction("Flip");
        NextPage = CreatePlayerAction("NextPage");
        PreviousPage = CreatePlayerAction("PreviousPage");

        ToggleName = CreatePlayerAction("ToggleName");

        Skill_0 = CreatePlayerAction("Skill_0");
        Skill_1 = CreatePlayerAction("Skill_1");
        Skill_2 = CreatePlayerAction("Skill_2");
        Skill_3 = CreatePlayerAction("Skill_3");

        MapKeyboard();
        MapJoystick();
    }

    void MapJoystick() {
        Submit.AddDefaultBinding(InputControlType.Action1);
        Cancel.AddDefaultBinding(InputControlType.Action2);

        Menu.AddDefaultBinding(InputControlType.Options);//Dual Shock type controller
        Menu.AddDefaultBinding(InputControlType.Menu);//Xbox type controller

        Inventory.AddDefaultBinding(InputControlType.Action3);

        MoveUp.AddDefaultBinding(InputControlType.LeftStickUp);
        MoveLeft.AddDefaultBinding(InputControlType.LeftStickLeft);
        MoveDown.AddDefaultBinding(InputControlType.LeftStickDown);
        MoveRight.AddDefaultBinding(InputControlType.LeftStickRight);

        AttackUp.AddDefaultBinding(InputControlType.RightStickUp);
        AttackLeft.AddDefaultBinding(InputControlType.RightStickLeft);
        AttackDown.AddDefaultBinding(InputControlType.RightStickDown);
        AttackRight.AddDefaultBinding(InputControlType.RightStickRight);

        NextPage.AddDefaultBinding(InputControlType.RightBumper);
        PreviousPage.AddDefaultBinding(InputControlType.LeftBumper);

        ToggleName.AddDefaultBinding(InputControlType.DPadLeft);

        Skill_0.AddDefaultBinding(InputControlType.LeftBumper);
        Skill_1.AddDefaultBinding(InputControlType.RightBumper);
        Skill_2.AddDefaultBinding(InputControlType.LeftTrigger);
        Skill_3.AddDefaultBinding(InputControlType.RightTrigger);
    }

    void MapKeyboard() {
        Submit.AddDefaultBinding(submit);        
        Cancel.AddDefaultBinding(cancel);

        Menu.AddDefaultBinding(menu);
        Inventory.AddDefaultBinding(inventory);

        MoveUp.AddDefaultBinding(moveup);
        MoveLeft.AddDefaultBinding(moveleft);
        MoveDown.AddDefaultBinding(movedown);
        MoveRight.AddDefaultBinding(moveright);

        AttackUp.AddDefaultBinding(attackup);
        AttackLeft.AddDefaultBinding(attackleft);
        AttackDown.AddDefaultBinding(attackdown);
        AttackRight.AddDefaultBinding(attackright);

        Flip.AddDefaultBinding(flip);
        ToggleName.AddDefaultBinding(toggleName);

        Skill_0.AddDefaultBinding(skill_0);
        Skill_1.AddDefaultBinding(skill_1);
        Skill_2.AddDefaultBinding(skill_2);
        Skill_3.AddDefaultBinding(skill_3);

    }

}


