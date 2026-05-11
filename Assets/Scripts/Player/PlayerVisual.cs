using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField] private MeshRenderer _headMeshRenderer;
    [SerializeField] private MeshRenderer _bodyMeshRenderer;

    private Material material;

    private void Awake()
    {
        //创建新材质，让头和身体使用这个材质，不然会修改其他玩家的材质
        material = new Material(_headMeshRenderer.material);
        _headMeshRenderer.material = material;
        _bodyMeshRenderer.material = material;
    }

    public void SetPlayerColor(Color color)
    {
        material.color = color;
    }
}
