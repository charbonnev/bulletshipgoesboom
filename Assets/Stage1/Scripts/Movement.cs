using UnityEngine;

public class Movement : MonoBehaviour
{
    enum Direction { Up = 2, Down = -2, Left = -1, Right = 1 };
    [SerializeField] ParticleSystem rocketThrustParticle;
    [SerializeField] ParticleSystem leftSideThrustersParticle;
    [SerializeField] ParticleSystem rightSideThrustersParticle;
    [SerializeField] float mainThrust = 20f;
    [SerializeField] float rotationThrust = 5f;
    [SerializeField] bool usePhysicsOnRotation = false;
    
    private Rigidbody rb;
    private AudioSource mainThrusterAudioSource;
    private AudioSource sideThrusterAudioSource;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainThrusterAudioSource = GetComponent<AudioSource>();
        sideThrusterAudioSource = GetComponents<AudioSource>()[1];
    }

    // Update is called once per frame
    void Update()
    {
        ProcessReset();
        ProcessComputerAssistance();
    }

    private void ProcessComputerAssistance()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            usePhysicsOnRotation = !usePhysicsOnRotation;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    void OnDisable()
    {
        rocketThrustParticle.Stop();
        leftSideThrustersParticle.Stop();
        rightSideThrustersParticle.Stop();
        sideThrusterAudioSource.Stop();
    }

    void FixedUpdate()
    {
        ProcessThrust();
        ProcessTurn();
    }

    private void ProcessReset()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            transform.position = new Vector3(-16, 2.13f, 0);
            transform.rotation = Quaternion.identity;
        }
    }

    private void ProcessTurn()
    {
        if (Input.GetKey(KeyCode.A))
        {
            // Debug.Log("A pressed - Rotating left");
            ApplyRotation(Direction.Left);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            // Debug.Log("D pressed - Rotating right");
            ApplyRotation(Direction.Right);
        }
        else {
            leftSideThrustersParticle.Stop();
            rightSideThrustersParticle.Stop();
            sideThrusterAudioSource.Stop();
        }
    }

    private void ApplyRotation(Direction dir)
    {
        if (!sideThrusterAudioSource.isPlaying)
        {
            sideThrusterAudioSource.Play();
        }

        //show particle effects
        if (dir == Direction.Left)
        {   
            rightSideThrustersParticle.Play();
            leftSideThrustersParticle.Stop();
        }
        else if (dir == Direction.Right)
        {
            leftSideThrustersParticle.Play();
            rightSideThrustersParticle.Stop();
        }

        if (!usePhysicsOnRotation)
        {
            var rbc = rb.constraints;
            rb.freezeRotation = true; // Take manual control of rotation
            transform.Rotate(Vector3.back * rotationThrust * 1 / 2 * (int)dir);
            rb.freezeRotation = false; // Resume physics control of rotation
            //freezes the x and y axis of rotation
            rb.constraints = rbc;
        }
        else
        {
            rb.AddRelativeTorque(Vector3.back * rotationThrust * (int)dir);
        }
    }

    void ProcessThrust()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            FireMainThrusters();
        }
        else
        {
            TurnOffMainThrusters();
        }
    }

    private void TurnOffMainThrusters()
    {
        mainThrusterAudioSource.Stop();
        rocketThrustParticle.Stop();
    }

    private void FireMainThrusters()
    {
        // Debug.Log("Space pressed - Thrusting");
        rb.AddRelativeForce(0, mainThrust, 0);
        if (!rocketThrustParticle.isPlaying)
        {
            rocketThrustParticle.Play();
        }

        //if the sound isn't playing
        if (!mainThrusterAudioSource.isPlaying)
        {
            //play the sound
            mainThrusterAudioSource.Play();
        }
    }
}
