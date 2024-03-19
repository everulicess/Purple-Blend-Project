using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Combat : MonoBehaviour
{
    public AttackTypesScrObj attackType;
    public GameObject child;

    private float damage;
    private float knockback;
    private bool isAttacking;

    private Vector3 point;
    private float lookRotationSpeed = 8f;
    public List<BoxPlaceholderScript> targets = new List<BoxPlaceholderScript>();

    // Start is called before the first frame update
    void Start()
    {
        // Assign values from AttackType Scriptable Object to the script and the attack area.
        damage = attackType.damage;
        knockback = attackType.knockback;
        gameObject.transform.Find("AttackArea").GetComponent<MeshCollider>().sharedMesh = attackType.colliderShape;
        gameObject.transform.Find("AttackArea").GetComponent<MeshFilter>().mesh = attackType.colliderShape;
    }

    private void Update()
    {
        FaceTarget();
        if(Input.GetMouseButtonDown(0) && !isAttacking)
        {
            ClickToAttack();
        }
    }

    void ClickToAttack()
    {
        // Converts click on the screen to a position in the game world.
        RaycastHit hit;
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            point = hit.point;
        }
        Attack();
    }

    void FaceTarget()
    {
        // Turns the player towards the clicked spot.
        if (point != null)
        {
            Vector3 direction = (point - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookRotationSpeed);
        }
    }

    void Attack()
    {
        // Switches isAttacking to true so that the player cannot spam attacks and invokes functions with a small delay.
        isAttacking = true;
        Invoke("TryAttacking", 0.3f);
        Invoke("DisableIsAttacking", 0.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Adds object to potential list of targets.
        if (other.gameObject.GetComponent<BoxPlaceholderScript>() != null)
        {
            targets.Add(other.gameObject.GetComponent<BoxPlaceholderScript>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Removes object from potential list of targets.
        targets.Remove(other.gameObject.GetComponent<BoxPlaceholderScript>());
    }

    void TryAttacking()
    {
        // Enables attack area's MeshRenderer to show the attack happening.
        child.gameObject.GetComponent<MeshRenderer>().enabled = true;
        // Checks through the list of objects within the targets list to damage them all.
        foreach (BoxPlaceholderScript target in targets.ToList())
        {
            Vector3 knockbackVector = target.transform.position * knockback - gameObject.transform.position;
            target.GetComponent<BoxPlaceholderScript>().Damaged(damage);
            target.GetComponent<BoxPlaceholderScript>().ApplyKnockback(knockbackVector);
        }
    }

    void DisableIsAttacking()
    {
        // Allows player to attack again and disables the attack area's MeshRenderer.
        isAttacking = false;
        child.gameObject.GetComponent<MeshRenderer>().enabled = false;
    }
}
