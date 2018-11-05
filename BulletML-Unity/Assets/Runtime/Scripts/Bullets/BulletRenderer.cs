using UnityEngine;

namespace UnityBulletML.Bullets
{
    public class BulletRenderer : MonoBehaviour
    {
        [SerializeField] private Material _material = null;
        [SerializeField] private BulletManager _bulletManager = null;
        [SerializeField] private float _bulletSize = 1f;

        private Mesh _mesh;
        MaterialPropertyBlock _materialPropertyBlock;

        public void Start()
        {
            var size = _bulletSize;
            var pivot = Vector2.one / 2f;

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
            if (_bulletManager.Bullets.Count <= 0)
                return;

            // Draw each batch
            for (int i = 0; i < _bulletManager.BulletTransformMatrices.Count; i++)
            {
                Matrix4x4[] transformMatrixBatch = _bulletManager.BulletTransformMatrices[i];

                _materialPropertyBlock.SetVectorArray("_MainTex_ST", _bulletManager.BulletSpriteOffsetsBatches[i]);
                _materialPropertyBlock.SetVectorArray("_Color", _bulletManager.BulletColorsBatches[i]);

                Graphics.DrawMeshInstanced(_mesh, 0, _material, transformMatrixBatch, transformMatrixBatch.Length, _materialPropertyBlock);
            }
        }
    }
}