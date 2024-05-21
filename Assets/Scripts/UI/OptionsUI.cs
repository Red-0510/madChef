using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour {

    public static OptionsUI Instance {get;private set;}

    [SerializeField] private Button soundEffectsButton;
    [SerializeField] private Button musicEffectsButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button moveUpButton;
    [SerializeField] private Button moveDownButton;
    [SerializeField] private Button moveLeftButton;
    [SerializeField] private Button moveRightButton;
    [SerializeField] private Button interactButton;
    [SerializeField] private Button interactAlternateButton;
    [SerializeField] private Button pauseButton;


    [SerializeField] private TextMeshProUGUI soundText;
    [SerializeField] private TextMeshProUGUI musicText;
    [SerializeField] private TextMeshProUGUI moveUpText;
    [SerializeField] private TextMeshProUGUI moveDownText;
    [SerializeField] private TextMeshProUGUI moveLeftText;
    [SerializeField] private TextMeshProUGUI moveRightText;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private TextMeshProUGUI interactAlternateText;
    [SerializeField] private TextMeshProUGUI pauseText;

    [SerializeField] private Transform pressToRebindKeyTransform;

    private Action onCloseButtonAction;


    private void Awake(){
        Instance = this;

        soundEffectsButton.onClick.AddListener(()=>{
            SoundManager.Instance.ChangeVolume();
            UpdateVisual();
        });

        musicEffectsButton.onClick.AddListener(()=>{
            MusicManager.Instance.ChangeVolume();
            UpdateVisual();
        });

        closeButton.onClick.AddListener(()=>{
            Hide();
            onCloseButtonAction();
        });

        moveUpButton.onClick.AddListener(()=>{RebindBinding(GameInput.Binding.Move_Up);});
        moveDownButton.onClick.AddListener(()=>{RebindBinding(GameInput.Binding.Move_Down);});
        moveLeftButton.onClick.AddListener(()=>{RebindBinding(GameInput.Binding.Move_Left);});
        moveRightButton.onClick.AddListener(()=>{RebindBinding(GameInput.Binding.Move_Right);});
        interactButton.onClick.AddListener(()=>{RebindBinding(GameInput.Binding.Interact);});
        interactAlternateButton.onClick.AddListener(()=>{RebindBinding(GameInput.Binding.Interact_Alternate);});
        pauseButton.onClick.AddListener(()=>{RebindBinding(GameInput.Binding.Pause);});
    }

    private void Start(){
        KitchenGameManager.Instance.OnGameUnpaused += KitchenGameManager_OnGameUnpaused;
        UpdateVisual();

        Hide();
        HidePressToRebindKey();
    }

    private void KitchenGameManager_OnGameUnpaused(object sender, System.EventArgs e){
        Hide();
    }

    private void UpdateVisual(){
        soundText.text = "Sound Effects : " + Mathf.Round(SoundManager.Instance.GetVolume() * 10f);

        musicText.text = "Music : " + Mathf.Round(MusicManager.Instance.GetVolume() * 10f);

        moveUpText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Up);
        moveDownText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Down);
        moveLeftText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Left);
        moveRightText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Right);
        interactText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact);
        interactAlternateText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact_Alternate);
        pauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Pause);
    }

    private void RebindBinding(GameInput.Binding binding){
        ShowPressToRebindKey();
        GameInput.Instance.RebindBinding(binding,()=>{
            HidePressToRebindKey();
            UpdateVisual();
        });
    }

    public void Show(Action onCloseButtonAction){
        this.onCloseButtonAction =onCloseButtonAction;
        gameObject.SetActive(true);

        soundEffectsButton.Select();
    }

    private void Hide(){
        gameObject.SetActive(false);
    }

    private void ShowPressToRebindKey(){
        pressToRebindKeyTransform.gameObject.SetActive(true);
    }

    private void HidePressToRebindKey(){
        pressToRebindKeyTransform.gameObject.SetActive(false);
    }

}
