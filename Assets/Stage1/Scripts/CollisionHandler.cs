using UnityEngine;
using UnityEngine.SceneManagement;


public class CollisionHandler : MonoBehaviour
{
    // PARAMETERS - for tuning, possibly public
    // CACHE - references for readability and performance
    // STATE - private variables
    [SerializeField] AudioClip successAudioClip;
    [SerializeField] AudioClip explosionAudioClip;
    [SerializeField] ParticleSystem explosionParticle;
    [SerializeField] ParticleSystem successParticle;

    private AudioSource audioSource;
    private Rigidbody rb;
    private Movement movement;
    private Collider tipCollider;

    bool isTransitioning;
    bool willDie;

    bool is_test;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        movement = GetComponent<Movement>();
        tipCollider = GetComponentInChildren<Collider>();
        is_test = false;
    }

    void Update()
    {
        if (is_test)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                //load level 0
                SceneManager.LoadScene(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                //load level 1
                SceneManager.LoadScene(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                //load level 2
                SceneManager.LoadScene(2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                //load level 3
                SceneManager.LoadScene(3);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                //load level 3
                SceneManager.LoadScene(4);
            }
        }

    }

    void OnTriggerEnter(Collider other)
    {
        willDie = true;
    }

    void OnTriggerExit(Collider other)
    {
        willDie = false;
    }

    void OnCollisionEnter(Collision other)
    {
        handleCollision(other);
    }

    private void OnCollisionStay(Collision other)
    {
        if (willDie && !isTransitioning)
        {
            handleCollision(other);
        }
    }

    private void handleCollision(Collision other)
    {
        if (willDie)
        {
            Debug.Log("Collision with " + other.gameObject.name + " at " + rb.velocity + ", magnitude: " + rb.velocity.magnitude);
        }
        if (!isTransitioning)
        {
            if (rb.velocity.magnitude > 1f || other.gameObject.tag == "Death" || willDie)
            {
                rb.AddExplosionForce(1000f, other.GetContact(0).point, 10f);
                if (other.gameObject.GetComponent<Rigidbody>() != null)
                {
                    other.gameObject.GetComponent<Rigidbody>().AddExplosionForce(1000f, other.GetContact(0).point, 10f);
                }
                StartCrashSequence();
            }
            else
            {
                if (other.gameObject.tag == "Finish")
                {
                    StartSuccessSequence();
                }
            }
        }
    }

    private void StartSuccessSequence()
    {
        audioSource.Stop();
        successParticle.Play();
        audioSource.PlayOneShot(successAudioClip);
        Invoke("NextLevel", 2f);
        movement.enabled = false;
        isTransitioning = true;
    }

    private void StartCrashSequence()
    {
        audioSource.Stop();
        explosionParticle.Play();
        audioSource.PlayOneShot(explosionAudioClip);
        Invoke("ReloadScene", 2f);
        movement.enabled = false;
        isTransitioning = true;
    }

    private void NextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }
        SceneManager.LoadScene(nextSceneIndex);
    }

    void ReloadScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

}
