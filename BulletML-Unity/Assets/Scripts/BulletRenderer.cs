using UnityEngine;

public class BulletRenderer : MonoBehaviour
{
    [Header("Data")]
    public Material Material;

    [Header("Instances")]
    public BulletManager BulletManager;

    private Mesh _mesh;

    const int MAX_SIZE = 10;
    readonly Vector4[] _spriteOffsets = new Vector4[MAX_SIZE];
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

        //_colors[0] = new Vector4(Random.value, Random.value, Random.value, 1f);

        //_materialPropertyBlock.SetVectorArray("_MainTex_ST", _spriteOffsets); //BulletManager.BulletSpriteOffsets);
        _materialPropertyBlock.SetVectorArray("_Color", BulletManager.BulletColors);

        // Draw each batch
        foreach (var batch in BulletManager.BulletTransformMatrices)
            Graphics.DrawMeshInstanced(_mesh, 0, Material, batch, batch.Length, _materialPropertyBlock);
    }
}