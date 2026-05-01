using System;

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

    private StoveCounterState _state = StoveCounterState.Idle;
    private float _fryingTimer;
    private float _burningTimer;

    private FryingRecipeSO _fryingRecipeSO;
    private BurningRecipeSO _burningRecipeSO;


    private void Start()
    {
        _fryingTimer = 0;
        _state = StoveCounterState.Idle;
    }

    private void Update()
    {
        if (HasKitchenObject())
        {

            switch (_state)
            {
                case StoveCounterState.Idle:
                    break;
                case StoveCounterState.Frying:
                    _fryingTimer += Time.deltaTime;

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs()
                    {
                        progressNormalized = _fryingTimer / _fryingRecipeSO.fryingProgressMax
                    });

                    if (_fryingRecipeSO != null)
                    {
                        if (_fryingTimer >= _fryingRecipeSO.fryingProgressMax)
                        {
                            GetKitchenObject().DestroySelf();
                            KitchenObject.SpawnKitchenObject(_fryingRecipeSO.outputKitchenObjectSO, this);
                            Debug.Log("已经烹饪完成");
                            _state = StoveCounterState.Fried;
                            _burningTimer = 0;

                            _burningRecipeSO = GetBurningRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
                            OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { State = _state });
                        }
                    }
                    break;
                case StoveCounterState.Fried:
                    _burningTimer += Time.deltaTime;

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs()
                    {
                        progressNormalized = _burningTimer / _burningRecipeSO.burningProgressMax
                    });
                    if (_burningRecipeSO != null)
                    {
                        if (_burningTimer >= _burningRecipeSO.burningProgressMax)
                        {
                            GetKitchenObject().DestroySelf();
                            KitchenObject.SpawnKitchenObject(_burningRecipeSO.outputKitchenObjectSO, this);
                            Debug.Log("烤焦了");
                            _state = StoveCounterState.Burned;

                            OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { State = _state });

                            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs()
                            {
                                progressNormalized = 0f
                            });
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
                player.GetKitchenObject().SetKitchenObjectParent(this);
                _fryingRecipeSO = GetFryingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
                _state = StoveCounterState.Frying;
                _fryingTimer = 0;

                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { State = _state });

                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs()
                {
                    progressNormalized = _fryingTimer / _fryingRecipeSO.fryingProgressMax
                });
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
                        GetKitchenObject().DestroySelf();

                        _state = StoveCounterState.Idle;

                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { State = _state });

                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs()
                        {
                            progressNormalized = 0f
                        });
                    }
                }
            }
            else  //玩家没有拿东西，则把柜台的物品给玩家
            {
                GetKitchenObject().SetKitchenObjectParent(player);
                _state = StoveCounterState.Idle;

                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { State = _state });

                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs()
                {
                    progressNormalized = 0f
                });
            }
        }
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
