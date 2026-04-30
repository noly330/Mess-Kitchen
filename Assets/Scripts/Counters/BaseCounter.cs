using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCounter : MonoBehaviour,IKitchenObjectParent
{

    [SerializeField] protected Transform _topPoint;
    protected KitchenObject _kitchenObject;  //当前柜台上的物品
    public virtual void Interact(Player player)
    {
        Debug.LogError("子类必须重写这个方法");
    }

    public virtual void InteractAlternate(Player player)
    {
        Debug.Log("如果使用该方法，子类必须重写这个方法");
    }



    #region IKitchenObjectParent接口实现
    public Transform GetKitchenObjectFollowTransform()
    {
        return _topPoint;
    }

    public KitchenObject GetKitchenObject() => _kitchenObject;
    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        _kitchenObject = kitchenObject;
    }
    public void ClearKitchenObject()
    {
        _kitchenObject = null;
    }

    public bool HasKitchenObject() => _kitchenObject != null;
    #endregion
}
