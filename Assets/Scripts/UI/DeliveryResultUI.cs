using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryResultUI : MonoBehaviour {

    private const string POP_UP="Popup";
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI messageText;

    [SerializeField] private Color successColor;
    [SerializeField] private Color failColor;
    [SerializeField] private Sprite successSprite;
    [SerializeField] private Sprite failSprite;

    private Animator animator;

    private void Awake(){
        animator = GetComponent<Animator>();
    }

    private void Start(){
        DeliveryManager.Instance.OnRecipeFailed+=DeliveryManager_OnRecipeFailed;
        DeliveryManager.Instance.OnRecipeSuccess+=DeliveryManager_OnRecipeSuccess;

        gameObject.SetActive(false);
    }

    private void DeliveryManager_OnRecipeFailed(object sender,System.EventArgs e){
        gameObject.SetActive(true);
        animator.SetTrigger(POP_UP);
        iconImage.sprite = failSprite;
        backgroundImage.color = failColor;
        messageText.text = "DELIVERY\nFAILED.";
    }

    private void DeliveryManager_OnRecipeSuccess(object sender,System.EventArgs e){
        gameObject.SetActive(true);
        animator.SetTrigger(POP_UP);
        iconImage.sprite = successSprite;
        backgroundImage.color = successColor;
        messageText.text = "DELIVERY\nSUCCESS!";
    }
}
