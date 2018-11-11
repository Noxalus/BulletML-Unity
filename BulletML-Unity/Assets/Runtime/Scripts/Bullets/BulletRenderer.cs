using UnityEngine;

namespace UnityBulletML.Bullets
{
    public class BulletRenderer : MonoBehaviour
    {
        [SerializeField] private Material _material = null;
        [SerializeField] private BulletManager _bulletManager = null;
        [SerializeField] private int _layer = 0;
        [SerializeField] private Camera _camera= null;

        private Mesh _mesh;
        MaterialPropertyBlock _materialPropertyBlock;

        public void Start()
        {
            var size = _bulletManager.BulletInitialSize;
            var pivot = Vector2.one * (size / 2f);

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

                Graphics.DrawMeshInstanced(
                    _mesh, 0, _material, transformMatrixBatch, transformMatrixBatch.Length, _materialPropertyBlock,
                    UnityEngine.Rendering.ShadowCastingMode.Off, false, _layer, _camera
                );
            }
        }
    }
}