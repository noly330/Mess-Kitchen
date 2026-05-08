using System;
using UnityEngine;
using Unity.Netcode;

public class Player : NetworkBehaviour, IKitchenObjectParent
{
    public static event EventHandler OnAnyPlayerSpawned;
    public static event EventHandler OnAnyPickedSomething;
    public static void ResetStaticData()
    {
        OnAnyPlayerSpawned = null;
        OnAnyPickedSomething = null;
    }
    public static Player LocalInstance { get; private set; }

    public event EventHandler OnPickedSomething;
    public event EventHandler<SelectedCounterChangedEventArgs> OnSelectedCounterChanged;  //泛型事件
    public class SelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }

    [Header("玩家移动参数")]
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private float _rotationSpeed = 15f;
    [SerializeField] private float _interactionDistance = 2f;

    [Header("玩家交互参数")]
    [SerializeField] private LayerMask _contersLayerMask;
    [SerializeField] private LayerMask _blockingLayerMask;
    [SerializeField] private Transform _kitchenObjectHoldPoint;
    private KitchenObject _kitchenObject;

    //
    private Vector2 _inputDirection;
    private bool _isWalking;
    public bool IsWalking => _isWalking;
    private BaseCounter _selectedCounter;

    private void Awake()
    {

        // if (Instance != null)
        // {
        //     Debug.LogError("超过一个玩家实例！！！");
        // }
        // Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
                return;
        }
        LocalInstance = this;
        OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);
    }
    private void Start()
    {
        GameInput.Instance.OnInteractAction += OnInteractAction;
        GameInput.Instance.OnInteractAlternateAction += OnInteractAlternateAction;
    }

    private void Update()
    {

        if(!IsOwner)
        {
            return;
        }
        // 在Update中处理输入
        _inputDirection = GameInput.Instance.GetMovementDirectionNormalized();
        _isWalking = _inputDirection != Vector2.zero;

        HandleInteractions();
        HandleMovement();
    }

    private void FixedUpdate()
    {
    }

    /// <summary>
    /// 处理玩家的移动和旋转
    /// </summary>

    private void HandleMovement()
    {
        Vector2 inputVcetor = GameInput.Instance.GetMovementDirectionNormalized();
        Vector3 moveDirection = new Vector3(inputVcetor.x, 0, inputVcetor.y);

        float moveDistance = _moveSpeed * Time.deltaTime;
        float playerRadius = 0.7f;
        float playerHeight = 2f;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirection, moveDistance, _blockingLayerMask);
        if (canMove)
        {
            transform.position += moveDirection * moveDistance;
        }
        else
        {
            Vector3 moveDirX = new Vector3(moveDirection.x, 0, 0).normalized;

            canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance, _blockingLayerMask);
            if (canMove)
            {
                transform.position += moveDirX * moveDistance;
            }
            else
            {
                Vector3 moveDirZ = new Vector3(0, 0, moveDirection.z).normalized;

                canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance, _blockingLayerMask);
                if (canMove)
                {
                    transform.position += moveDirZ * moveDistance;
                }
            }
        }

        _isWalking = moveDirection != Vector3.zero;

        transform.forward = Vector3.Slerp(transform.forward, moveDirection, _rotationSpeed * Time.deltaTime);
    }

    /// <summary>
    /// 处理与交互对象的交互
    /// </summary>
    private void HandleInteractions()
    {

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit raycastHit, _interactionDistance, _contersLayerMask))
        {
            if (raycastHit.transform.TryGetComponent(out BaseCounter counter))
            {
                if (counter != _selectedCounter)
                {
                    SetSelectedCounter(counter);
                }
            }
            else
            {
                SetSelectedCounter(null);
            }
        }
        else
        {
            SetSelectedCounter(null);
        }
    }

    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        if (this._selectedCounter == selectedCounter) return;

        this._selectedCounter = selectedCounter;
        OnSelectedCounterChanged?.Invoke(this, new SelectedCounterChangedEventArgs { selectedCounter = selectedCounter });
    }

    private void OnInteractAction(object sender, EventArgs e)
    {
        if (!GameManager.Instance.IsGamePlaying()) return;
        if (_selectedCounter != null)
        {
            _selectedCounter.Interact(this);
        }
    }
    private void OnInteractAlternateAction(object sender, EventArgs e)
    {
        if (!GameManager.Instance.IsGamePlaying()) return;
        if(_selectedCounter != null)
        {
            _selectedCounter.InteractAlternate(this);
        }
    }

    #region IKitchenObjectParent接口实现
    public Transform GetKitchenObjectFollowTransform()
    {
        return _kitchenObjectHoldPoint;
    }

    public KitchenObject GetKitchenObject() => _kitchenObject;
    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        _kitchenObject = kitchenObject;
        if (_kitchenObject != null)
        {
            OnPickedSomething?.Invoke(this, EventArgs.Empty);
            OnAnyPickedSomething?.Invoke(this, EventArgs.Empty);
        }
    }
    public void ClearKitchenObject()
    {
        _kitchenObject = null;
    }

    public bool HasKitchenObject() => _kitchenObject != null;
    #endregion
}