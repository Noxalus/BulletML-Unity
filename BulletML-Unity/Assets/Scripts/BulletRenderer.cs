using UnityEngine;

public class BulletRenderer : MonoBehaviour
{
    [Header("Data")]
    public Material Material;

    [Header("Instances")]
    public BulletManager BulletManager;

    private Mesh _mesh;

    Vector4[] _spriteOffsets = new Vector4[4096];
    Vector4[] _colors = new Vector4[4096];
    MaterialPropertyBlock _materialPropertyBlock;

    public void Awake()
    {
        var size = 1f;
        var pivot = (Vector2.one / 2f) * size;

        _mesh = MeshUtils.GenerateQuad(size, pivot);

        for (int i = 0; i < 4096; i++)
        {
            _spriteOffsets[i] = new Vector4(0.25f, 0.25f, 0.25f, 0.75f);
            _colors[i] = new Vector4(1f, 1f, 0f, 1f);
        }

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

        _materialPropertyBlock.SetVectorArray("_MainTex_ST", _spriteOffsets);//BulletManager.BulletSpriteOffsets);
        _materialPropertyBlock.SetVectorArray("_Color", _colors);

        // Draw each batch
        foreach (var batch in BulletManager.BulletMatrices)
            Graphics.DrawMeshInstanced(_mesh, 0, Material, batch, batch.Length, _materialPropertyBlock);
    }
}