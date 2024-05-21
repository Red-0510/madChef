using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{   
    public static DeliveryManager Instance{get;private set;}
    [SerializeField] private RecipeListSO recipeListSO;
    private List<RecipeSO> waitingRecipeSOList;
    private float spawnRecipeTimer;
    private float spawnRecipeTimerMax=4f;
    private int waitingRecipesMax=4;
    private int successfulRecipesAmount;

    public event EventHandler OnRecipeSpawned,OnRecipeCompleted,OnRecipeSuccess,OnRecipeFailed;
    private void Awake(){
        Instance = this;
        waitingRecipeSOList = new List<RecipeSO>();
    }

    private void Update(){
        spawnRecipeTimer-=Time.deltaTime;
        if(spawnRecipeTimer <= 0f){
            spawnRecipeTimer = spawnRecipeTimerMax;

            if( KitchenGameManager.Instance.IsGamePlaying() && waitingRecipeSOList.Count < waitingRecipesMax){
                RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[UnityEngine.Random.Range(0,recipeListSO.recipeSOList.Count)];

                Debug.Log(waitingRecipeSO.recipeName);
                waitingRecipeSOList.Add(waitingRecipeSO);

                OnRecipeSpawned?.Invoke(this,System.EventArgs.Empty);

            }
        }
    }

    public void DeliverRecipe(PlateKitchenObject plateKitchenObject){
        foreach (RecipeSO waitingRecipeSO in waitingRecipeSOList){
            if(waitingRecipeSO.kitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count){
                bool recipeMatched=true;
                foreach (KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList){
                    bool ingredientFound=false;
                    foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList()){
                        if(plateKitchenObjectSO == recipeKitchenObjectSO){
                            ingredientFound=true;
                            break;
                        }
                    }
                    if(!ingredientFound){
                        recipeMatched=false;
                        break;
                    }
                }

                if(recipeMatched){
                    Debug.Log("ATTA Boy! " + waitingRecipeSO.recipeName + " Delivered");
                    waitingRecipeSOList.Remove(waitingRecipeSO);
                    successfulRecipesAmount++;
                    OnRecipeCompleted?.Invoke(this,System.EventArgs.Empty);

                    OnRecipeSuccess?.Invoke(this,System.EventArgs.Empty);
                    return;
                }
            }
        }
        Debug.Log("I cant eat this shit!");
        OnRecipeFailed?.Invoke(this,System.EventArgs.Empty);
    }

    public List<RecipeSO> GetWaitingRecipeSOList(){
        return waitingRecipeSOList;
    }

    public int GetSuccessfulRecipesAmount(){
        return successfulRecipesAmount;
    }


}
