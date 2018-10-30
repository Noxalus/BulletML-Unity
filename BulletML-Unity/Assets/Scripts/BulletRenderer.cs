using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BulletRenderer : MonoBehaviour
{
    [Header("Data")]
    public Mesh mesh;
    public Material material;

    [Header("Instances")]
    public BulletManager BulletManager;

    public void Awake()
    {
        var size = 1f;
        var pivot = (Vector2.one / 2f) * size;
        mesh = MeshUtils.GenerateQuad(size, pivot);
    }

    private void Update()
    {
        BatchAndRender();
    }

    private void BatchAndRender()
    {
        //If we dont have projectiles to render then just get out
        if (BulletManager.Bullets.Count <= 0)
            return;

        //Draw each batch
        foreach (var batch in BulletManager.BulletMatrices)
            Graphics.DrawMeshInstanced(mesh, 0, material, batch, batch.Length);
    }
}