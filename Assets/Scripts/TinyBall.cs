using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class TinyBall : MonoBehaviour
{
    private Rigidbody rb;
    private AudioSource audioSource;
    public AudioClip rollClip;

    private float speed;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        
    }
    // Start is called before the first frame update
    void Start()
    {
        audioSource.clip = rollClip;
    }
    private void FixedUpdate()
    {
        speed = rb.velocity.magnitude;
        audioSource.pitch = math.clamp(speed / 10,-3,3);
    }

    void OnCollisionStay(Collision collision)
    {
        if (audioSource.isPlaying == false && speed >= 0.1f)
	    {
		    audioSource.Play();
	    }
	    else if (audioSource.isPlaying == true && speed < 0.1f)
	    {
		    audioSource.Pause();
	    }
    }
    void OnCollisionExit(Collision collision)
    {
        if (audioSource.isPlaying == true)
	    {
		    audioSource.Pause();
	    }
    }


    public void OnCheckPointIn(){
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        
    }
    public void OnCheckPointOut(){
        rb.isKinematic = false;
    }
}
