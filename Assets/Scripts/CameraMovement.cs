using System.Collections;
using Oculus.Interaction.Body.Input;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    [SerializeField] private Transform mazeBall;
    [SerializeField] private Transform bigBall;
    [SerializeField] private float lerpSpeed = 5f; // Speed for switching targets
    [SerializeField] private float followSpeed = 3f; // Speed for following the target
    [SerializeField] private Vector3 offset;// Offset from the target
    [SerializeField] private GameObject MazeCameraRig; 
    [SerializeField] private GameObject RoomCube;
    private Transform currentTarget;

    private bool isTransitioning = false;

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        currentTarget = mazeBall; // Start at the first target
        MazeCameraRig.transform.position = currentTarget.position + offset;
    }

    public void SwitchTarget()
    {
        if (currentTarget == mazeBall)
        {
            currentTarget = bigBall;
            mazeBall.gameObject.SetActive(false);
            MazeCameraRig.transform.position = new Vector3(0,4.5f,0) + offset;
            
            
        }
        else
        {
            
            currentTarget = mazeBall;
            mazeBall.gameObject.SetActive(true);
            MazeCameraRig.transform.position = currentTarget.position + offset;
        }
    }

    private IEnumerator LerpPosition (Vector3 targetPosition)
    {
        float time = 0;
        Vector3 startPosition = MazeCameraRig.transform.position;

        while (time < lerpSpeed)
        {
            MazeCameraRig.transform.position = Vector3.Lerp(startPosition, targetPosition, time);
            time += Time.deltaTime * lerpSpeed;
            yield return null;
        }
    }

    public void SetOffset(Vector3 newOffset)
    {
        offset = newOffset;
    }
}
