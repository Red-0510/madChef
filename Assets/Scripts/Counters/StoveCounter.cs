using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounter : BaseCounter,IHasProgress {

    public enum State{
        Idle,
        Frying,
        Fried,
        Burned,
    }

    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs:EventArgs{
        public State state;
    }
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
    [SerializeField] private BurningRecipeSO[] burningRecipeSOArray;
    private FryingRecipeSO fryingRecipeSO;
    private BurningRecipeSO burningRecipeSO;

    private float fryingTimer=0f;
    private float burningTimer=0f;
    private State state;

    private void Start(){
        state = State.Idle;
    }
    private void Update(){
        if(HasKitchenObject()){
            switch(state){
                case State.Idle:
                    break;
                case State.Frying:
                    fryingTimer+=Time.deltaTime;
                    
                    OnProgressChanged?.Invoke(this,
                        new IHasProgress.OnProgressChangedEventArgs{
                            progressNormalized = fryingTimer/fryingRecipeSO.fryingTimerMax
                        }
                    );
                    
                    if(fryingTimer >= fryingRecipeSO.fryingTimerMax){
                        fryingTimer=0f;
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(fryingRecipeSO.output,this);
                        state = State.Fried;

                        OnStateChanged?.Invoke(this,new OnStateChangedEventArgs{
                            state=state
                        });

                        burningTimer=0f;
                        burningRecipeSO = GetBurningRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
                    }
                    break;
                case State.Fried:
                    burningTimer+=Time.deltaTime;

                    OnProgressChanged?.Invoke(this,
                        new IHasProgress.OnProgressChangedEventArgs{
                            progressNormalized = burningTimer/burningRecipeSO.burningTimerMax
                        }
                    );

                    if(burningTimer >= burningRecipeSO.burningTimerMax){
                        burningTimer=0f;
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(burningRecipeSO.output,this);
                        state = State.Burned;
                        OnStateChanged?.Invoke(this,new OnStateChangedEventArgs{
                            state=state
                        });

                        OnProgressChanged?.Invoke(this,
                            new IHasProgress.OnProgressChangedEventArgs{
                                progressNormalized = 0f
                            }
                        );
                    }
                    break;
                case State.Burned:
                    break;
            }
        }
    }
    public override void Interact(Player player){
        if(HasKitchenObject()){
            if(!player.HasKitchenObject()){
                GetKitchenObject().SetKitchenObjectParent(player);
                state = State.Idle;
                OnStateChanged?.Invoke(this,new OnStateChangedEventArgs{
                    state=state
                });

                OnProgressChanged?.Invoke(this,
                    new IHasProgress.OnProgressChangedEventArgs{
                        progressNormalized = 0f
                    }
                );
            } else if(player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)){
                if(plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())){
                    GetKitchenObject().DestroySelf();

                    state = State.Idle;
                    OnStateChanged?.Invoke(this,new OnStateChangedEventArgs{
                        state=state
                    });

                    OnProgressChanged?.Invoke(this,
                        new IHasProgress.OnProgressChangedEventArgs{
                            progressNormalized = 0f
                        }
                    );
                }
            }
        } else {
            if(player.HasKitchenObject() && HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO())){
                player.GetKitchenObject().SetKitchenObjectParent(this);
                fryingRecipeSO = GetFryingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                state = State.Frying;
                OnStateChanged?.Invoke(this,new OnStateChangedEventArgs{
                    state=state
                });
                OnProgressChanged?.Invoke(this,
                    new IHasProgress.OnProgressChangedEventArgs{
                        progressNormalized = fryingTimer/fryingRecipeSO.fryingTimerMax
                    }
                );
                fryingTimer=0f;
            }
        }  
    }

    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO){
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);
        return fryingRecipeSO!=null;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO){
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);
        if(fryingRecipeSO !=null) return fryingRecipeSO.output;
        else return null;
    }

    private FryingRecipeSO GetFryingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO){
        foreach(FryingRecipeSO fryingRecipeSO in fryingRecipeSOArray){
            if(fryingRecipeSO.input == inputKitchenObjectSO){
                return fryingRecipeSO;
            }
        }
        return null;
    }

    private BurningRecipeSO GetBurningRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO){
        foreach(BurningRecipeSO burningRecipeSO in burningRecipeSOArray){
            if(burningRecipeSO.input == inputKitchenObjectSO){
                return burningRecipeSO;
            }
        }
        return null;
    }
}
