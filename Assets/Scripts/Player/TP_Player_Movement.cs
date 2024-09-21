
using UnityEngine;
using UnityEngine.InputSystem;

public class TP_Player_Movement : MonoBehaviour
{ 
    public enum PlayerState
    {
        Idle,
        Walking,
        Sprinting,
        Jump,
        Crouch_Idle,
        Crouch_Walk
    }
    //[HideInInspector]public PlayerState playerState;
    public PlayerState playerState;

    //Player Input - new Input system//
    private PlayerInput playerInput;
    private InputAction Movement;
    private InputAction Running;
    private bool Jump_Key;
    private bool Crouch_Key;
     
    // references // 
    private Rigidbody Player_RB;
    private Transform CameraTransform;
    private CapsuleCollider Collider;

    [Header("Movement")]
    [SerializeField] private bool Can_Walk;
    [SerializeField] private float Walk_Speed;
    [SerializeField] private bool Can_Run;
    [SerializeField] private float Running_Speed;
    [SerializeField] private float Rotate_Smooth_Time;
    private Vector3 Input_Axis;
    private Vector3 Direction; 
    private float Turn_velocity;
    private float Move_Speed;

    [Header("Ground_Check")]
    [SerializeField] private Transform GroundCheck_object;
    [SerializeField] private float GroundCheck_Sphere_radius = 0.4f;
    [SerializeField] private LayerMask GroundMask;
    [SerializeField] private bool isGrounded;         //
    [SerializeField] private float Ground_Drag;
    [SerializeField] private float Air_Drag; 

    [Header("Jump")]
    [SerializeField] private  bool canJump = true;
    [SerializeField] private float jump_force;
    [SerializeField] private float Air_Move_Multipyler;
    [SerializeField] private bool isJumping;     // 
    [SerializeField] private float gravity;
    [SerializeField] private float Jump_CoolDownTime;

    [Header("Collider_Jump_Settings")]
    [SerializeField] private float Stand_hight;
    [SerializeField] private float jump_hight;
    [SerializeField] private float Crouch_hight;
    [SerializeField] private Vector3 stand_Center;
    [SerializeField] private Vector3 Jump_Center;
    [SerializeField] private Vector3 Crouch_Center;
    [SerializeField] private float Stand_Radius;
    [SerializeField] private float Jump_Radius;
    [SerializeField] private float Crouch_Radius;
    [SerializeField] private float Reset_Time;
    
    [Header("Crouch")]
    [SerializeField] private bool can_Crouch;
    [SerializeField] private bool isCrouching;
    [SerializeField] private float Crouch_Speed;
    
    
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        Movement = playerInput.actions.FindAction("Movement");
        //Running = playerInput.actions.FindAction("Running");
        Player_RB = GetComponent<Rigidbody>();
        CameraTransform = Camera.main.transform;
        Physics.gravity = new Vector3(0,gravity,0);
        Collider = GetComponent<CapsuleCollider>();
    }
    
    private void Update() 
    {
        Player_Input();
        Player_State();
        Ground_Check();
        Speed_Control();
        Crouching();

    }

    private void FixedUpdate() 
    {
        Player_Move();
        Player_jump();
    }
    
    private void Player_Input()
    {
        Input_Axis = Movement.ReadValue<Vector2>();
        Crouch_Key = Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.C);
        Jump_Key = Input.GetKeyDown(KeyCode.Space);
        if(Jump_Key){isJumping = true;}
    }

    private void Player_Move()
    {
        Direction = new Vector3(Input_Axis.x,0,Input_Axis.y).normalized;

        if(Direction.magnitude >= 0.1f)
        {
           
            float TargetAngle = Mathf.Atan2(Direction.x,Direction.z) * Mathf.Rad2Deg + CameraTransform.eulerAngles.y;
            float Angle = Mathf.SmoothDampAngle(transform.eulerAngles.y,TargetAngle,ref Turn_velocity,Rotate_Smooth_Time);
            transform.rotation = Quaternion.Euler(0,Angle,0);
        
            //Vector3 Move_Direction = Quaternion.Euler(0,TargetAngle,0)*Vector3.forward;
            Vector3 Move_Direction = transform.forward * Direction.magnitude;
        
            if(isGrounded)
            {
               Player_RB.AddForce(Move_Direction.normalized* Move_Speed * 100 * Time.deltaTime ,ForceMode.Force);
            }else
            {
               Player_RB.AddForce(Move_Direction.normalized* Move_Speed * 100 * Air_Move_Multipyler * Time.deltaTime,ForceMode.Force);
            }

        } 
    }

    private void Speed_Control()
    {
        Vector3 Flat_Velocity = new Vector3(Player_RB.velocity.x,0,Player_RB.velocity.z);
        if(Flat_Velocity.magnitude > Move_Speed)
        {
            Vector3 limit_Velocity = Flat_Velocity.normalized * Move_Speed;
            Player_RB.velocity = new Vector3(limit_Velocity.x,Player_RB.velocity.y,limit_Velocity.z);
        }

    }

    private void Player_State()
    {
        if(can_Crouch && isCrouching)
        {
            if(Direction.magnitude >= 0.1f)
            {
                playerState = PlayerState.Crouch_Walk;
                Move_Speed = Crouch_Speed;
            }else
            {
                playerState = PlayerState. Crouch_Idle;
            }
        }
        else if(canJump && isJumping)
        {
            playerState = PlayerState.Jump;
        }
        else if(Direction.magnitude == 0)
        {
            playerState = PlayerState.Idle;
            Move_Speed = 0;
        }
        else if(Direction.magnitude >= 0.1f)
        {
            if(Input.GetKey(KeyCode.LeftShift) && Can_Run)
            {
                playerState = PlayerState.Sprinting;
                Move_Speed = Running_Speed;
            }
            else if(Can_Walk)
            {
                playerState = PlayerState.Walking; 
                Move_Speed = Walk_Speed;
            }
        }
    }
    
    private void Ground_Check()
    {
        isGrounded = Physics.CheckSphere(GroundCheck_object.position,GroundCheck_Sphere_radius,GroundMask);
        Player_RB.drag = isGrounded? Ground_Drag : Air_Drag;
    }
    
    private void Player_jump()
    {
        if(canJump)
        {
            if(isJumping == true && isGrounded)
            {
                Player_RB.velocity = new Vector3(Player_RB.velocity.x , 0 ,Player_RB.velocity.z);
                Player_RB.AddForce(transform.up * jump_force * 100 * Time.deltaTime , ForceMode.Impulse);
                
                Collider_Setting(isJumping);

                Invoke("Jump_Reset",Jump_CoolDownTime);
               
            }
        }
    }

    private void Jump_Reset()
    {
        isJumping = false;
        Collider_Setting(isJumping);
    }

    private void Collider_Setting(bool Condition)
    {

        float Condition_hight = 0;
        Vector3 Condition_Center = Vector3.zero;
        float Condition_Radius = 0;

        if(Condition == isJumping)
        {
            Condition_hight = jump_hight;
            Condition_Center = Jump_Center;
            Condition_Radius = Jump_Radius;
        }
        else if(Condition == isCrouching)
        {
            Condition_hight = Crouch_hight;
            Condition_Center = Crouch_Center;
            Condition_Radius = Crouch_Radius;
        }


        float timeTaken = 0;
        float TargetHight = Condition? Condition_hight : Stand_hight;
        float Current_Collider_Hight = Collider.height;

        Vector3 TargetCenter = Condition? Condition_Center : stand_Center;
        Vector3 Current_Collider_Center = Collider.center;

        float TargetRadius = Condition? Condition_Radius : Stand_Radius;
        float Current_Collider_Radius = Collider.radius;

        while(timeTaken < Reset_Time)
        {
            Collider.height = Mathf.Lerp(Current_Collider_Hight,TargetHight,timeTaken/Reset_Time);
            Collider.center = Vector3.Lerp(Current_Collider_Center,TargetCenter,timeTaken/Reset_Time);
            Collider.radius = Mathf.Lerp(Current_Collider_Radius,TargetRadius,timeTaken/Reset_Time);
            timeTaken += Time.deltaTime;
        }

        Collider.height = TargetHight;
        Collider.center = TargetCenter;
        Collider.radius = TargetRadius;
    }

    private void Crouching()
    {
       if(Crouch_Key && isGrounded)
       {
            isCrouching = !isCrouching;
            Collider_Setting(isCrouching);

            if(isCrouching)
            {
                canJump = false;
                Can_Run = false;
                Can_Walk = false;
            }
            else
            {
                canJump = true;
                Can_Run = true;
                Can_Walk = true;  
            }
        }
    }



}
