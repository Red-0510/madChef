using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameStartCountdownUI : MonoBehaviour{

    [SerializeField] private TextMeshProUGUI countDownText;

    private void Start(){
        KitchenGameManager.Instance.OnStateChanged+=KitchenGameManager_OnStateChanged;

        Hide();
    }

    private void Update(){
        countDownText.text = Mathf.Ceil(KitchenGameManager.Instance.GetCountdownToStartTimer()).ToString();
    }

    private void KitchenGameManager_OnStateChanged(object sender, System.EventArgs e){
        if(KitchenGameManager.Instance.IsCountdownToStart()){
            Show();
        } else {
            Hide();
        }
    }

    private void Show(){
        gameObject.SetActive(true);
    }

    private void Hide(){
        gameObject.SetActive(false);
    }
}
