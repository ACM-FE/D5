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

    // component refs
    private Rigidbody rb;
    private CinemachineBasicMultiChannelPerlin ns;
    private CinemachineRecomposer rc;
    public Animator currentWeaponAnim;
    public Weapon currentWeapon;

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

        // assign component refs 
        rb = GetComponent<Rigidbody>();
        rc = GetComponentInChildren<CinemachineRecomposer>();
        ns = GetComponentInChildren<CinemachineBasicMultiChannelPerlin>();

        rc.m_Tilt=0;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Fire(InputAction.CallbackContext cc) {
        if (!isAttacking) {
            //isAttacking = true;
            currentWeaponAnim.SetTrigger("fire");
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
        ns.m_AmplitudeGain = Move();
        
        Look();
    }
}