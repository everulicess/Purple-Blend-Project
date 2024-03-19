using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Player : NetworkBehaviour
{
    [SerializeField] CharacterStatsScrObj Character;

    //[SerializeField]Characters currentCharacter;

    private NetworkCharacterController characterController;

    [SerializeField] private Transform camTarget;

    float turnSpeed = 360f;
    [SerializeField]float speed = 3f;

    [SerializeField] Animator anim;

    private Vector3 point;
    Vector3 direction1;
    Vector3 direction2;

    public override void Spawned()
    {
        if (HasInputAuthority)
        {
            CameraFollow.Singleton.SetTarget(camTarget);
        }
    }
    private void Awake()
    {
        characterController = GetComponent<NetworkCharacterController>();
        anim = GetComponent<Animator>();
        cam = FindObjectOfType<Camera>();
    }
    Camera cam;
    public override void FixedUpdateNetwork()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
            {
                point = hit.point;
                direction2 = point - transform.position;
            }
        }
        float angle = Vector3.Angle(direction2, direction1);
        Debug.Log($" Main Direction: {direction1}\n Looking Direction: {direction2}" +
            $"\n Angle Between those 2 directions: {angle}\n Right Vector {Vector3.right}" +
            $"\n Forward Vector 3: {Vector3.forward} \n Left Vector 3: {Vector3.left}" +
            $"\n Back Vector 3: {Vector3.back}");

        characterController.maxSpeed = Character.MovementStats.MovementSpeed;

        FaceTarget();

        if (GetInput(out NetworkInputData data))
        {
            anim.SetBool("Moving", true);
            var matrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
            //data.direction.Normalize();

            var skewedInput = matrix.MultiplyPoint3x4(data.direction);
            if (data.direction != Vector3.zero)
            {
                var relative = (transform.position + data.direction) - transform.position;
                var rot = Quaternion.LookRotation(relative, Vector3.up);

                transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, turnSpeed * Runner.DeltaTime);
            }
            skewedInput.Normalize();
            characterController.Move(Runner.DeltaTime * speed * skewedInput);
            direction1 = data.direction;
            //Debug.LogWarning($"skewed input is {skewedInput}");
            if (data.direction == Vector3.zero)
                anim.SetBool("Moving", false);
        }
    }

    void FaceTarget()
    {
        if (point != null)
        {
            Vector3 direction = (point - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
        }
    }
}
