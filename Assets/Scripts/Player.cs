using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//--------Refs
//Rigidbody docs: https://docs.unity3d.com/ScriptReference/Rigidbody.html

[RequireComponent(typeof(Rigidbody))]

public class Player : MonoBehaviour
{
    public float gravity = 20.0f;   
    public float jump_h = 2.5f;     //jump height

    Rigidbody rigid;        //instantiate rigid body
    bool grounded = false;  //true when player touches ground
    Vector3 default_scale;  //xyz scale of the player
    bool crouched = false;  //true if player is crouching

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        rigid.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        rigid.freezeRotation = true; //body rotation is frozen
        rigid.useGravity = false;    //body does not use gravity

        default_scale = transform.localScale;
    }

    void Update()
    {
        // Jump
        if (Input.GetKeyDown(KeyCode.W) && grounded)
        {
            rigid.velocity = new Vector3(rigid.velocity.x, CalculateJumpVerticalSpeed(), rigid.velocity.z);
        }

        //Crouch
        crouched = Input.GetKey(KeyCode.S);
        if (crouched)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(default_scale.x, default_scale.y * 0.4f, default_scale.z), Time.deltaTime * 7);
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, default_scale, Time.deltaTime * 7);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // We apply gravity manually for more tuning control
        rigid.AddForce(new Vector3(0, -gravity * rigid.mass, 0));

        grounded = false;
    }

    void OnCollisionStay()
    {
        grounded = true;
    }

    float CalculateJumpVerticalSpeed()
    {
        //calculate jump speed using height and gravity : https://gamedev.stackexchange.com/questions/29617/how-to-make-a-character-jump
        return Mathf.Sqrt(2 * jump_h * gravity);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Finish")
        {
            GroundGenerator.gg_instance.game_over = true;
        }
    }
}
