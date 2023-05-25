using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class Control : MonoBehaviour {
    public InputActionAsset PlayerBindings;
    public float speed;
    public float sens;

    // actions
    private InputAction move;
    private InputAction sprint;
    private InputAction look;
    private InputAction fire;
    private InputAction weapon;

    // component refs
    private Rigidbody rb;
    private CinemachineBasicMultiChannelPerlin ns;
    private CinemachineRecomposer rc;
    private Animator armAnimator;
    public Weapon currentWeapon;
    public Weapon[] weapons;
    private int weaponIndex = 1;

    // transients
    private bool isAttacking = false;


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
        //look.performed += Look;

        // assign component refs 
        rb = GetComponent<Rigidbody>();
        rc = GetComponentInChildren<CinemachineRecomposer>();
        ns = GetComponentInChildren<CinemachineBasicMultiChannelPerlin>();
        armAnimator = GetComponentInChildren<Animator>();
        weapons = GetComponentsInChildren<Weapon>();

        rc.m_Tilt=0;
        Cursor.lockState = CursorLockMode.Locked;
    }


    // this makes me want to vomit...
    private void SwitchWeapon(InputAction.CallbackContext cc) {
        if (weapon.activeControl.displayName == "Scroll Down" || weapon.activeControl.displayName == "D-Pad Down") {
            weaponIndex = Mathf.Clamp(weaponIndex+(int)Mathf.Sign(weapon.ReadValue<float>()),1, weapons.Length);
        } else {
            weaponIndex = int.Parse(weapon.activeControl.displayName);
        }
        isAttacking = false;
        armAnimator.SetInteger("weaponIndex",weaponIndex);
        currentWeapon = weapons[weaponIndex-1];
    }

    private void Fire(InputAction.CallbackContext cc) {
        if (!isAttacking) {
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
        Look();
        ns.m_AmplitudeGain = Move();
    }
}