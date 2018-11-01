using UnityEngine;

public class BulletRenderer : MonoBehaviour
{
    [Header("Data")]
    public Material Material;

    [Header("Instances")]
    public BulletManager BulletManager;

    private Mesh _mesh;
    MaterialPropertyBlock _materialPropertyBlock;

    public void Awake()
    {
        var size = 1f;
        var pivot = (Vector2.one / 2f) * size;

        _mesh = MeshUtils.GenerateQuad(size, pivot);
        _materialPropertyBlock = new MaterialPropertyBlock();
    }

    private void Update()
    {
        BatchAndRender();
    }

    private void BatchAndRender()
    {
        // If we dont have projectiles to render then just get out
        if (BulletManager.Bullets.Count <= 0)
            return;

        // Draw each batch
        for (int i = 0; i < BulletManager.BulletTransformMatrices.Count; i++)
        {
            Matrix4x4[] transformMatrixBatch = BulletManager.BulletTransformMatrices[i];

            _materialPropertyBlock.SetVectorArray("_MainTex_ST", BulletManager.BulletSpriteOffsetsBatches[i]);
            _materialPropertyBlock.SetVectorArray("_Color", BulletManager.BulletColorsBatches[i]);

            Graphics.DrawMeshInstanced(_mesh, 0, Material, transformMatrixBatch, transformMatrixBatch.Length, _materialPropertyBlock);
        }
    }
}