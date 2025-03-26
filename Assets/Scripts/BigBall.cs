using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BigBall : MonoBehaviour
{
    private Rigidbody rb;
    public float jumpForce = 10f;
    private AudioSource audioSource;
    public AudioClip rollClip;
    public AudioClip hitClip;
    public AudioClip springClip;
    private float speed;

    private MeshRenderer meshRenderer;

    private string shineProperty = "_FresenelPower";
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        meshRenderer = GetComponent<MeshRenderer>();
        audioSource.clip = rollClip;
    }
    private void FixedUpdate()
    {
        speed = rb.velocity.magnitude;
        audioSource.pitch = math.clamp(speed / 10,-3,3);
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "String")
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            audioSource.clip = springClip;
            audioSource.Play();
        }
        else if(other.gameObject.tag == "Platform")
        {
            audioSource.clip = hitClip;
            audioSource.Play();
        }
    }
    void OnCollisionStay(Collision collision)
    {
        if (audioSource.isPlaying == false && speed >= 0.1f&& collision.gameObject.tag == "Platform")
	    {
            audioSource.clip = rollClip;
		    audioSource.Play();
	    }
	    else if (audioSource.isPlaying == true && speed < 0.1f&& collision.gameObject.tag == "Platform")
	    {
		    audioSource.Pause();
	    }
    }
    void OnCollisionExit(Collision collision)
    {
        if (audioSource.isPlaying == true&& collision.gameObject.tag == "Platform")
	    {
		    audioSource.Pause();
	    }
    }

    public void shineTwice(){
        StartCoroutine(Shine());
    }

    private IEnumerator Shine(){
        var material = meshRenderer.material;
        for (int i = 0; i < 4; i++)
        {
        if(material.HasProperty(shineProperty)){
            float startValue = 4f;
            float targetValue = 1.5f;
            float elapsedTime = 0f;

            while(elapsedTime < 0.5f){
                elapsedTime += Time.deltaTime;
                float currentValue = Mathf.Lerp(startValue, targetValue, elapsedTime / 0.5f);
                material.SetFloat(shineProperty, currentValue);
                yield return null;
            }
            material.SetFloat(shineProperty, targetValue);
            yield return new WaitForSeconds(0.1f);
            startValue = 1.5f;
            targetValue = 4f;
            elapsedTime = 0f;

            while(elapsedTime < 0.5f){
                elapsedTime += Time.deltaTime;
                float currentValue = Mathf.Lerp(startValue, targetValue, elapsedTime / 0.5f);
                material.SetFloat(shineProperty, currentValue);
                yield return null;
            }
            material.SetFloat(shineProperty, targetValue);
        }
        }
    }
}
