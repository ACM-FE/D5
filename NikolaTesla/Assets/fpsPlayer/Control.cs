using System.Collections;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class Control : MonoBehaviour {
    public InputActionAsset PlayerBindings;
    public float speed;
    public float sens;
    public float jumpHeight;
    public float fallSpeed;

    // actions
    private InputAction move;
    private InputAction sprint;
    private InputAction look;
    private InputAction fire;
    private InputAction weapon;
    private InputAction jump;

    // component refs
    private Rigidbody rb;
    private CinemachineBasicMultiChannelPerlin ns;
    private CinemachineRecomposer rc;
    private Animator armAnimator;
    public Weapon currentWeapon;
    public Weapon[] weapons;
    public Animation hitEffect;
    private int weaponIndex = 1;

    // transients
    private bool isAttacking = false;
    [HideInInspector]
    public bool canJump = true;
    private bool canMove = true;


    void Awake()
    {
        // assign bindings to actions
        move = PlayerBindings.FindActionMap("Player").FindAction("Move"); 
        sprint = PlayerBindings.FindActionMap("Player").FindAction("Sprint");
        fire = PlayerBindings.FindActionMap("Player").FindAction("Fire");
        fire.performed += Fire;
        look = PlayerBindings.FindActionMap("Player").FindAction("Look");
        weapon = PlayerBindings.FindActionMap("Player").FindAction("Weapon");
        weapon.performed += SwitchWeapon;
        jump = PlayerBindings.FindActionMap("Player").FindAction("Jump");
        jump.performed += Jump;
        //look.performed += Look;

        // assign component refs 
        rb = GetComponent<Rigidbody>();
        rc = GetComponentInChildren<CinemachineRecomposer>();
        ns = GetComponentInChildren<CinemachineBasicMultiChannelPerlin>();
        armAnimator = GetComponentInChildren<Animator>();

        rc.m_Tilt=0;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void AddWeapon(Weapon weapon) {
        Array.Resize(ref weapons,weapons.Length+1);
        weapons[weapons.Length-1] = weapon;
        //Equip(weapons.Length);
    }

    // this makes me want to vomit...
    private void SwitchWeapon(InputAction.CallbackContext cc) {
        if (weapon.activeControl.displayName == "Scroll Down" || weapon.activeControl.displayName == "D-Pad Down") {
            weaponIndex = Mathf.Clamp(weaponIndex+(int)Mathf.Sign(weapon.ReadValue<float>()),1, weapons.Length);
        } else {
            weaponIndex = int.Parse(weapon.activeControl.displayName);
        }
        isAttacking = false;
        try {
            currentWeapon = weapons[weaponIndex-1];
            armAnimator.SetInteger("weaponIndex",weaponIndex);
        } finally {}
    }

    public void Equip(int weapon) {
        weaponIndex=weapon;
        isAttacking = false;
        armAnimator.SetInteger("weaponIndex",weaponIndex);
        currentWeapon = weapons[weaponIndex-1];
    }

    private void Fire(InputAction.CallbackContext cc) {
        if (!isAttacking && canMove && !currentWeapon.reload.isPlaying) {
            isAttacking = true;
            armAnimator.SetTrigger("fire");
            Attack attack = currentWeapon.makeAttack(); 

            IEnumerator finishAttack() {
                yield return new WaitForSeconds(attack.anim.length);
                isAttacking = false;
            }   
            StartCoroutine(finishAttack());
            
        }
        
    }

    private void Jump(InputAction.CallbackContext cc) {
        IEnumerator fall() {

            while(!canJump) {
                rb.velocity = new Vector3(rb.velocity.x,(Mathf.MoveTowards(rb.velocity.y,-jumpHeight,fallSpeed)),rb.velocity.z);
                yield return new WaitForEndOfFrame();
            }
        }
        if (canJump&&canMove) { 
            rb.velocity += Vector3.up * jumpHeight;
            GetComponentInChildren<Grounding>().stayTime=0;
            canJump = false;
            
            StartCoroutine(fall());
        }
    }

    private int Move() {
        // ugly but fine for now, try remember to fix
        // edit: YOU MADE IT WORSE?? HOW????? WHY ARE YOU USING TERNARIES?
        // you should lose your fucking hands for this
        Vector2 direction = move.ReadValue<Vector2>()*(sprint.IsPressed() ? speed*2: speed);
        
        rb.velocity = transform.TransformDirection(new Vector3(direction.x,rb.velocity.y,direction.y));

        if (direction.magnitude>0 && !sprint.IsPressed()) {
            return 1;
        } 
        else if (direction.magnitude>0 && sprint.IsPressed()) {
            return 2;
        }
        else {
            return 0;
        }
    }

    private void Look() {
        Vector2 delta = look.ReadValue<Vector2>()*sens;
        transform.Rotate(new Vector3(0,delta.x,0));
        rc.m_Tilt = Mathf.Clamp(rc.m_Tilt-delta.y,-70,70);   
    }

    void Update() {
        if (canMove) {
            Look();
            ns.m_AmplitudeGain = Move();
        } else {
            ns.m_AmplitudeGain = 0;
            rb.velocity = Vector3.zero;
        }
    }

    public void Hurt(float damage) {
        hitEffect.Play("hurt");
    }

    public void Freeze() {
        canMove = false;
    }
    public void Go() {
        canMove = true;
    }
}