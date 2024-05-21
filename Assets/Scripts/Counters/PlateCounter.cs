using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateCounter : BaseCounter{

    [SerializeField] KitchenObjectSO plateKitchenObjectSO;
    public event EventHandler OnPlateSpawned;
    public event EventHandler OnPlateRemoved;
    private float spawnPlateTimer;
    private float spawnPlateTimerMax=2f;
    private int platesSpawnedAmount;
    private int platesSpawnedAmountMax=4;
    
    private void Update(){
        spawnPlateTimer+=Time.deltaTime;
        if(spawnPlateTimer >=spawnPlateTimerMax){
            if(KitchenGameManager.Instance.IsGamePlaying() && platesSpawnedAmount <platesSpawnedAmountMax){
                platesSpawnedAmount++;
                spawnPlateTimer=0f;
                OnPlateSpawned?.Invoke(this,EventArgs.Empty);
            }
        }
    }
    
    public override void Interact(Player player){
        if(!player.HasKitchenObject() && platesSpawnedAmount>0){
            platesSpawnedAmount--;
            KitchenObject.SpawnKitchenObject(plateKitchenObjectSO,player);
            OnPlateRemoved?.Invoke(this,EventArgs.Empty);
            spawnPlateTimer=0f;
        }
    }
}
