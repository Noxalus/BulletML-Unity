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
    public List<GameObject> splashPool = new List<GameObject>();

    // Working values
    private RaycastHit[] rayHitBuffer = new RaycastHit[1];
    private Vector3 worldPoint;
    private Vector3 transPoint;
    private List<Matrix4x4[]> bufferedData = new List<Matrix4x4[]>();

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

    //    private void UpdateProjectiles(float tick)
    //    {
    //        foreach (var projectile in projectiles)
    //        {
    //            projectile.experation -= tick;

    //            if (projectile.experation > 0)
    //            {
    //                //Sort out the projectiles 'forward' direction
    //                transPoint = projectile.rot * Vector3.forward;
    //                //See if its going to hit something and if so handle that
    //                if (Physics.RaycastNonAlloc(projectile.pos, transPoint, rayHitBuffer, speed * tick, Config.ColliderLayers) > 0)
    //                {
    //                    projectile.experation = -1;
    //                    worldPoint = rayHitBuffer[0].point;
    //                    SpawnSplash(worldPoint);
    //                    ConquestShipCombatController target = rayHitBuffer[0].rigidbody.GetComponent<ConquestShipCombatController>();
    //                    if (target.teamId != projectile.team)
    //                    {
    //                        target.ApplyDamage(projectile.damage * projectile.damageScale, worldPoint);
    //                    }
    //                }
    //                else
    //                {
    //                    //This project wont be hitting anything this tick so just move it forward
    //                    projectile.pos += transPoint * (speed * tick);
    //                }
    //            }
    //        }
    //        //Remove all the projectiles that have hit there experation, can happen due to time or impact
    //        BulletManager.Bullets.RemoveAll(p => p.experation <= 0);
    //    }

    //    private void SpawnSplash(Vector3 worlPoint)
    //    {
    //        //TODO: implament spawning of your splash effect e.g. the visual effect of a projectile hitting something
    //    }
}