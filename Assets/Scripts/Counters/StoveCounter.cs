using System;
using Unity.Netcode;
using UnityEngine;


public class StoveCounter : BaseCounter, IHasProgress
{
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;

    public class OnStateChangedEventArgs : EventArgs
    {
        public StoveCounterState State;
    }
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public enum StoveCounterState
    {
        Idle, Frying, Fried, Burned
    }
    [SerializeField] private FryingRecipeSO[] _fryingRecipeSOArray;
    [SerializeField] private BurningRecipeSO[] _burningRecipeSOArray;

    private NetworkVariable<StoveCounterState> _state = new NetworkVariable<StoveCounterState>(StoveCounterState.Idle);
    public StoveCounterState GetStoveCounterState() => _state.Value;
    private NetworkVariable<float> _fryingTimer = new NetworkVariable<float>(0);
    private NetworkVariable<float> _burningTimer = new NetworkVariable<float>(0);

    private FryingRecipeSO _fryingRecipeSO;
    private BurningRecipeSO _burningRecipeSO;




    public override void OnNetworkSpawn()
    {
        _fryingTimer.OnValueChanged += FryingTimer_OnValueChanged;
        _burningTimer.OnValueChanged += BurningTimer_OnValueChanged;
        _state.OnValueChanged += State_OnValueChanged;
    }


    private void FryingTimer_OnValueChanged(float previousValue, float newValue)
    {

        float frytingTimerMax = _fryingRecipeSO != null ? _fryingRecipeSO.fryingProgressMax : 1f;
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs()
        {
            progressNormalized = _fryingTimer.Value / frytingTimerMax
        });
    }
    private void BurningTimer_OnValueChanged(float previousValue, float newValue)
    {
        float burningTimerMax = _burningRecipeSO != null ? _burningRecipeSO.burningProgressMax : 1f;
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs()
        {
            progressNormalized = _burningTimer.Value / burningTimerMax
        });
    }
    private void State_OnValueChanged(StoveCounterState previousValue, StoveCounterState newValue)
    {
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { State = _state.Value });

        // 重置进度条
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs()
        {
            progressNormalized = 0f
        });
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        if (HasKitchenObject())
        {

            switch (_state.Value)
            {
                case StoveCounterState.Idle:
                    break;
                case StoveCounterState.Frying:
                    _fryingTimer.Value += Time.deltaTime;

                    if (_fryingRecipeSO != null)
                    {
                        if (_fryingTimer.Value >= _fryingRecipeSO.fryingProgressMax)
                        {
                            KitchenObject.DestoryKitchenObject(GetKitchenObject());
                            KitchenObject.SpawnKitchenObject(_fryingRecipeSO.outputKitchenObjectSO, this);

                            _state.Value = StoveCounterState.Fried;
                            _burningTimer.Value = 0;

                            SetBurningRecipeSOClientRpc(GameMultiplayer.Instance.GetKitchenObjectSOIndex(GetKitchenObject().GetKitchenObjectSO()));

                        }
                    }
                    break;
                case StoveCounterState.Fried:
                    _burningTimer.Value += Time.deltaTime;

                    if (_burningRecipeSO != null)
                    {
                        if (_burningTimer.Value >= _burningRecipeSO.burningProgressMax)
                        {
                            KitchenObject.DestoryKitchenObject(GetKitchenObject());
                            KitchenObject.SpawnKitchenObject(_burningRecipeSO.outputKitchenObjectSO, this);
                            Debug.Log("The kitchen object is burned");
                            _state.Value = StoveCounterState.Burned;



                        }
                    }
                    break;
                case StoveCounterState.Burned:
                    break;
            }
        }
    }

    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            if (player.HasKitchenObject() && HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
            {
                KitchenObject kitchenObject = player.GetKitchenObject();
                kitchenObject.SetKitchenObjectParent(this);

                int index = GameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObject.GetKitchenObjectSO());
                InteractLogicPlaceObjectOnCounterServerRpc(index);

            }
            else
            {
                //TODO: 玩家没拿任何东西
            }
        }
        else  //如果柜台有物品
        {
            if (player.HasKitchenObject())
            {
                //TODO: 玩家有拿东西(比如盘子)
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        KitchenObject.DestoryKitchenObject(GetKitchenObject());

                        SetStoveStateIdleServerRpc();
                    }
                }
            }
            else  //玩家没有拿东西，则把柜台的物品给玩家
            {
                GetKitchenObject().SetKitchenObjectParent(player);
                SetStoveStateIdleServerRpc();
               

            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetStoveStateIdleServerRpc()
    {
        _state.Value = StoveCounterState.Idle;
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicPlaceObjectOnCounterServerRpc(int kitchenObjectIndex)
    {
        _fryingTimer.Value = 0;
        _state.Value = StoveCounterState.Frying;
        SetFryingRecipeSOClientRpc(kitchenObjectIndex);
    }
    [ClientRpc]
    private void SetFryingRecipeSOClientRpc(int kitchenObjectIndex)
    {
        KitchenObjectSO kitchenObjectSO = GameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectIndex);
        _fryingRecipeSO = GetFryingRecipeSOWithInput(kitchenObjectSO);
    }

    [ClientRpc]
    private void SetBurningRecipeSOClientRpc(int kitchenObjectIndex)
    {
        KitchenObjectSO kitchenObjectSO = GameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectIndex);
        _burningRecipeSO = GetBurningRecipeSOWithInput(kitchenObjectSO);
    }

    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        return GetFryingRecipeSOWithInput(inputKitchenObjectSO) != null;
    }

    private KitchenObjectSO GetOutPutForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);
        if (fryingRecipeSO != null)
        {
            return fryingRecipeSO.outputKitchenObjectSO;
        }
        return null;
    }

    private FryingRecipeSO GetFryingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (var fryingRecipeSO in _fryingRecipeSOArray)
        {
            if (fryingRecipeSO.inputKitchenObjectSO == inputKitchenObjectSO)
            {
                return fryingRecipeSO;
            }
        }
        return null;
    }

    private BurningRecipeSO GetBurningRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (var burningRecipeSO in _burningRecipeSOArray)
        {
            if (burningRecipeSO.inputKitchenObjectSO == inputKitchenObjectSO)
            {
                return burningRecipeSO;
            }
        }
        return null;
    }
}
