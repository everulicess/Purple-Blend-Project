using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Combat : MonoBehaviour
{

    public float attackRange;
    public int damage;
    public bool isAttacking;

    private Vector3 point;
    private float lookRotationSpeed = 8f;
    public List<BoxPlaceholderScript> targets = new List<BoxPlaceholderScript>();

    // Start is called before the first frame update
    void Start()
    {
        
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
        RaycastHit hit;
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            point = hit.point;
        }
        Attack();
    }

    void FaceTarget()
    {
        if (point != null)
        {
            Vector3 direction = (point - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookRotationSpeed);
        }
    }

    void Attack()
    {
        isAttacking = true;
        Invoke("TryAttacking", 0.5f);
        Invoke("DisableIsAttacking", 0.8f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<BoxPlaceholderScript>() != null)
        {
            targets.Add(other.gameObject.GetComponent<BoxPlaceholderScript>());
        }
    }

    public void EnterTargetList(BoxPlaceholderScript targetSelected)
    {
        targets.Add(targetSelected);
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log(other);
        targets.Remove(other.gameObject.GetComponent<BoxPlaceholderScript>());
    }

    void TryAttacking()
    {
        foreach (BoxPlaceholderScript target in targets.ToList())
        {
                target.GetComponent<BoxPlaceholderScript>().Damaged(damage);
        }
    }

    void DisableIsAttacking()
    {
        isAttacking = false;
    }
}
