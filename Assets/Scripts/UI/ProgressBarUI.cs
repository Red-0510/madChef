using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressbarUI : MonoBehaviour{

    [SerializeField] private GameObject hasProgressGameObject;
    [SerializeField] private Image barImage;
    private IHasProgress hasProgress;


    private void Start(){
        Debug.Log(hasProgressGameObject);
        hasProgress = hasProgressGameObject.GetComponent<IHasProgress>();
        if(hasProgress==null){
            Debug.LogError("GameObject "+hasProgressGameObject+" Does not have a component with IHasProgress");
        }

        hasProgress.OnProgressChanged +=IHasProgress_OnProgressChanged;
        barImage.fillAmount = 0f;

        Hide();
    }

    private void IHasProgress_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e){
        barImage.fillAmount = e.progressNormalized;
        if(e.progressNormalized==0f || e.progressNormalized==1f){
            Hide();
        } else {
            Show();
        }
    }

    private void Show(){
        gameObject.SetActive(true);
    }
    private void Hide(){
        gameObject.SetActive(false);
    }

}
