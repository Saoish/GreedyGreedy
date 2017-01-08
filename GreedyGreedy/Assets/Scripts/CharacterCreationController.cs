using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using GreedyNameSpace;
using Networking;
using Networking.Data;

public class CharacterCreationController : MonoBehaviour {
    public InputField Name;

    public Slider R;
    public Slider G;
    public Slider B;

    public SpriteRenderer BaseModelSpriteRenderer;

    [HideInInspector]
    public bool ClassRegistered = false;
    [HideInInspector]
    public CLASS RegisteredClass;

    //UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject
    // Use this for initialization
    void Start () {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(R.gameObject);
    }
	
	// Update is called once per frame
	void Update () {
        BaseModelSpriteRenderer.color = new Color(R.value, G.value, B.value);
    }

    public void CreatCharacter() {
        if(Name.text == "") {
            PopUpNotification.Push("Name can not be empty.", PopUpNotification.Type.Confirm);
        } else if(!ClassRegistered) {
            PopUpNotification.Push("Please select a class.", PopUpNotification.Type.Confirm);
        } else {
            PopUpNotification.Push("Waiting for server...", PopUpNotification.Type.Pending);
            CreationData CreationData = new CreationData(CacheManager.CachedPlayerSlotIndex, new RGB(R.value, G.value, B.value), Name.text, RegisteredClass);
            Client.Send(Protocols.CreateCharacter, CreationData);
        }
    }
}
