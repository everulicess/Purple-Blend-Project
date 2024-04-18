using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Fusion;

public class ChildColliderScript : NetworkBehaviour
{
    private BaseEnemy enemy;
    private static Collider[] _colliders = new Collider[9];
    [SerializeField] LayerMask LayerMask;
    [SerializeField] float colliderSize;
    private string thisTag;

    private void Start()
    {
        enemy = GetComponentInParent<BaseEnemy>();
        thisTag = this.gameObject.tag;
    }
    public override void FixedUpdateNetwork()
    {
        int collisions = Runner.GetPhysicsScene().OverlapSphere(new Vector3(transform.position.x, 0, transform.position.z), colliderSize, _colliders, LayerMask, QueryTriggerInteraction.Collide);

        switch (thisTag)
        {
            case "HitBox":
                if (collisions > 1) enemy.InRangeSetter(true);
                else enemy.InRangeSetter(false);
                ; break;
            case "DetectionArea":
                for (int i = 0; i < collisions; i++)
                {
                    enemy.TargetSetter(_colliders);
                }
                ; break;

            default:
                ; break;
        }
    }
}
