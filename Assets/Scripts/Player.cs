using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IKitchenObjectParent
{
    // Start is called before the first frame update
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask countersLayerMask;
    private bool isWalking;
    private Vector3 lastInteractDir;
    private BaseCounter selectedCounter;
    public static Player Instance{get;private set;}
    private KitchenObject kitchenObject;
    [SerializeField] private Transform kitchenObjectHoldPoint;

    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;

    public class OnSelectedCounterChangedEventArgs : EventArgs {
        public BaseCounter selectedCounter;
    }

    public event EventHandler OnPickedSomething;
    private void Awake(){
        if (Instance != null){
            Debug.LogError("There is more than one player Instance");
        }
        Instance = this;
    }

    private void Start(){
        gameInput.OnInteractAction += GameInput_OnInteractAction; 
        gameInput.OnInteractAlternateAction+=GameInput_OnInteractAlternateAction;
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e){
        if(!KitchenGameManager.Instance.IsGamePlaying()) return;
        if(selectedCounter !=null){
            selectedCounter.Interact(this);
        }
    }

    private void GameInput_OnInteractAlternateAction(object sender, System.EventArgs e){
        if(!KitchenGameManager.Instance.IsGamePlaying()) return;
        if(selectedCounter !=null){
            selectedCounter.InteractAlternate(this);
        }
    }

    // Update is called once per frame
    private void Update() {
        HandleMovement();
        HandleInteractions();
        
    }

    private void HandleInteractions(){
        Vector2 inputVector = gameInput.getMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x,0f,inputVector.y);

        if(moveDir != Vector3.zero){
            lastInteractDir = moveDir;
        }

        float interactDistance = 2f;
        if(Physics.Raycast(transform.position,lastInteractDir,out RaycastHit raycastHit,interactDistance,countersLayerMask)){
            if(raycastHit.transform.TryGetComponent(out BaseCounter baseCounter)){
                if(selectedCounter != baseCounter){
                    SetSelectedCounter(baseCounter);
                }
            }
            else {
                SetSelectedCounter(null);
            }
        } else {
            SetSelectedCounter(null);
        }

    }

    private void SetSelectedCounter(BaseCounter selectedCounter){
        this.selectedCounter = selectedCounter;
        OnSelectedCounterChanged?.Invoke(this,new OnSelectedCounterChangedEventArgs{
            selectedCounter = selectedCounter
        });
    }

    private void HandleMovement(){
        Vector2 inputVector = gameInput.getMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x,0f,inputVector.y);

        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius=.7f;
        float playerHeight = 2f;
        bool canMove = !Physics.CapsuleCast(transform.position,transform.position + Vector3.up * playerHeight, playerRadius,moveDir, moveDistance);
        if(!canMove){
            Vector3 moveDirX = new Vector3(moveDir.x,0,0).normalized;
            canMove = (moveDir.x<-.5f || moveDir.x>+.5f) && !Physics.CapsuleCast(transform.position,transform.position + Vector3.up * playerHeight, playerRadius,moveDirX, moveDistance);

            if(canMove){
                moveDir = moveDirX;
            } else {
                Vector3 moveDirZ = new Vector3(0,0,moveDir.z).normalized;
                canMove = (moveDir.z<-.5f || moveDir.z>+.5f) && !Physics.CapsuleCast(transform.position,transform.position + Vector3.up * playerHeight, playerRadius,moveDirZ, moveDistance);

                if(canMove){
                    moveDir = moveDirZ;
                }
            }
        }
        if(canMove){
            transform.position+=moveDir * moveDistance;
        }

        isWalking = moveDir != Vector3.zero;

        float rotateSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward,moveDir,Time.deltaTime * rotateSpeed);
    }

    public bool IsWalking(){
        return isWalking;
    }

    // IKitchenObjectParent functions
    public Transform GetKitchenObjectFollowTransform(){
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject){
        this.kitchenObject = kitchenObject;

        if(kitchenObject!=null){
            OnPickedSomething?.Invoke(this,System.EventArgs.Empty);
        }
    }

    public KitchenObject GetKitchenObject(){
        return this.kitchenObject;
    }

    public void ClearKitchenObject(){
        this.kitchenObject = null;
    }

    public bool HasKitchenObject(){
        return kitchenObject!=null;
    }


}
