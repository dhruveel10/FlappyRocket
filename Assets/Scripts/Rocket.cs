using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust = 65f;
    [SerializeField] float mainThrust = 1000f;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip nextLevel;
    [SerializeField] AudioClip death;

    [SerializeField] ParticleSystem thrustVFX;
    [SerializeField] ParticleSystem successVFX;
    [SerializeField] ParticleSystem deathVFX;

    Rigidbody rigidBody;
    AudioSource audioSource;

    bool collisionsOff = false;

    enum State { Alive, Dying, Transcending}
    State state = State.Alive;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            ProcessInput();
        }
        if (Debug.isDebugBuild)
        {
            RespondToDebugKeys();
        }
    }

    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Invoke("LoadNextScene", 2f);
            return;
        }
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            collisionsOff = !collisionsOff;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(state != State.Alive || collisionsOff) { return; }

        switch(collision.gameObject.tag)
        {
            case "Friendly":
                print("Not Dead");
                break;
            case "Finish":
                state = State.Transcending;
                audioSource.Stop();
                audioSource.PlayOneShot(nextLevel);
                successVFX.Play();
                Invoke("LoadNextScene",2f);
                break;
            case "Fuel":
                print("Fuel");
                break;
            default:
                state = State.Dying;
                audioSource.Stop();
                audioSource.PlayOneShot(death);
                thrustVFX.Stop();    
                deathVFX.Play();
                Invoke("LoadFirstLevel", 2f);
                break;
        }   
    }
    private void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex+1;
        if(nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }
        SceneManager.LoadScene(nextSceneIndex);
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    void ProcessInput()
    {
        float mainSpeed = mainThrust * Time.deltaTime;
        if(state == State.Alive)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                rigidBody.AddRelativeForce(Vector3.up * mainSpeed * Time.deltaTime);
                if (!audioSource.isPlaying)
                {
                    audioSource.PlayOneShot(mainEngine);
                    thrustVFX.Play();
                }
            }
            else
            {
                audioSource.Stop();
                thrustVFX.Stop();
            }

            rigidBody.freezeRotation = true;
            float rotationSpeed = rcsThrust * Time.deltaTime;

            if ((Input.GetKey(KeyCode.LeftArrow)) || (Input.GetKey(KeyCode.A)))
            {
                transform.Rotate(Vector3.forward * rotationSpeed);
            }
            else if ((Input.GetKey(KeyCode.RightArrow)) || (Input.GetKey(KeyCode.D)))
            {
                transform.Rotate(-Vector3.forward * rotationSpeed);
            }

            rigidBody.freezeRotation = false;
        }
      
    }
}
