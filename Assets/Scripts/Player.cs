using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Player : NetworkBehaviour
{
    [SerializeField] CharacterStatsScrObj Character;

    //[SerializeField]Characters currentCharacter;

    public static Vector3 m_MousePosition;
    private NetworkCharacterController characterController;

    [SerializeField] private Transform camTarget;
    Camera cam;

    float turnSpeed = 360f;
    [SerializeField]float speed = 3f;

    [SerializeField] Animator anim;

    private Vector3 point;
    Vector3 direction1;
    Vector3 direction2;

    [Header("Camera Reference")]
    [SerializeField] GameObject localCameraObject;
    Camera localCamera;

    [SerializeField] NetworkObject debugObject;

    public override void Spawned()
    {
        //Cursor.lockState = CursorLockMode.Confined;
        if (HasInputAuthority)
        {
            this.gameObject.name = $"{Runner.ActivePlayers}";
            var cam = Instantiate(localCameraObject);
            cam.GetComponent<LocalCamera>().SetTarget(camTarget);
            localCamera = cam.GetComponentInChildren<Camera>();
            //    CameraFollow.Singleton.SetTarget(camTarget);
            //    //cam = CameraFollow.Singleton.GetComponent<Camera>();

        }
    }
    private void Awake()
    {
        characterController = GetComponent<NetworkCharacterController>();
        anim = GetComponent<Animator>();
        //cam = FindObjectOfType<Camera>();
    }
    //NetworkInputData data_;
    public override void FixedUpdateNetwork()
    {
        //data_.MousePosition = Input.mousePosition;
        //if (Input.GetMouseButtonDown(0))
        //{
        //    RaycastHit hit;
        //    if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
        //    {
        //        point = hit.point;
        //        direction2 = point - transform.position;
        //    }
        //}
        float angle = Vector3.Angle(direction2, direction1);
        //Debug.Log($" Main Direction: {direction1}\n Looking Direction: {direction2}" +
        //    $"\n Angle Between those 2 directions: {angle}\n Right Vector {Vector3.right}" +
        //    $"\n Forward Vector 3: {Vector3.forward} \n Left Vector 3: {Vector3.left}" +
        //    $"\n Back Vector 3: {Vector3.back}");

        characterController.maxSpeed = Character.MovementStats.MovementSpeed;

        //FaceTarget();
        m_MousePosition = Input.mousePosition;
        if (GetInput(out NetworkInputData data))
        {
            anim.SetBool("Moving", true);
            var matrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
            //data.direction.Normalize();
            if ((data.buttons.IsSet(MyButtons.PingsButton) || Input.GetKeyDown(KeyCode.V) )&& HasInputAuthority)
            {
                //RPC_SendMessage(MousePosition.PingPosition);
                //Runner.Spawn(GameObject.CreatePrimitive(PrimitiveType.Cube), data.PingPosition, Quaternion.identity);

                //Debug.LogError(" V was pressed");
                //Ray ray = localCamera.ScreenPointToRay(Input.mousePosition);
                //if (Physics.Raycast(ray, out RaycastHit hit))
                //{
                //    data.PingPosition = new(hit.point.x, 1f, hit.point.z);
                //}
                //FindObjectOfType<CommunicationManager>().PlacePing_RPC(Pings.LocationPing);
            }
            var skewedInput = matrix.MultiplyPoint3x4(data.direction);
            if (data.direction != Vector3.zero)
            {
                var relative = (transform.position + data.direction) - transform.position;
                var rot = Quaternion.LookRotation(relative, Vector3.up);
                //m_MousePosition = data.MousePosition;
                //Debug.LogWarning($"PLAYER MOUSE POSITION IS: {data.MousePosition}---------------------------------------------------------------");
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
    //[Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    //public void RPC_SendMessage(Vector3 pVector, RpcInfo info = default)
    //{
    //    RPC_RelayMessage(pVector, info.Source);
    //}

    //[Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    //public void RPC_RelayMessage(Vector3 pVector, PlayerRef messageSource)
    //{
    //    string message = "";
    //    if (messageSource == Runner.LocalPlayer)
    //    {
    //        message = $"You said: {pVector}\n";
    //    }
    //    else
    //    {
    //        message = $"Some other player said: {pVector}\n";
    //    }
    //    Debug.LogWarning(message);
    //    Runner.Spawn(debugObject, pVector, Quaternion.identity);

    //}
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






//public enum Pings
//{
//    None,
//    MissingPing,
//    LocationPing,
//    NewPing,
//    AnotherPing
//}
//[Serializable]
//public class PingInfo
//{
//    [Tooltip("3D visual for the ping")] public GameObject Prefab;
//    [Tooltip("It will be played when the ping is placed")] public AudioClip Sound;
//    [Tooltip("Icon that will be displayed in the Menu")] public Texture Icon;
//}
//[Serializable]
//public struct CommunicationLibrary
//{
//    [Header("Pings")]
//    public PingInfo Ping1;
//    public PingInfo Ping2;
//    public PingInfo Ping3;
//    public PingInfo Ping4;
//    public PingInfo Ping5;
//}
