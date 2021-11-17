using UnityEngine;
using UnityEngine.UI;

public class BirdMover : MonoBehaviour
{
    // Public members to be set in the inspector
    [Header("Switch between real and virtual joystick below")]
    public bool useVirtualJoystick;

    [Header("References to other game objects - do not change")]
    public VariableJoystick virtualJoystick;
    public Text transferFunctionLabel;
    public Text speedLabel;
    public Text timeLabel;

    // Private members used for the internal logic
    public enum TransferFunction { IsotonicPosition, IsotonicRate, IsotonicAcceleration, ElasticPosition, ElasticRate, ElasticAcceleration };
    private TransferFunction currentTransferFunction = TransferFunction.IsotonicPosition; // currently activated transfer function
    private Vector3 startingPosition; // initial position of the bird
    private Vector3 lastFramePosition; // position of the bird in the last frame
    private float lastResetTime; // last time when the function Reset() was called

    /* Begin Exercise 2.x */
    // For all exercises, you may introduce additional variables here.
    // Do not forget to reset them in the function Reset() to a default value.
    private Vector3 birdVelocity;
    private Vector3 birdAcceleration;
    /* End Exercise 2.x */

    // Start is called before the first frame update
    void Start()
    {
        startingPosition = transform.position;
        virtualJoystick.gameObject.SetActive(false);
        Reset();
    }

    // Reset is called whenever a number key is pressed to activate
    // a certain combination of input device and transfer function
    void Reset()
    {
        transform.position = startingPosition;
        lastFramePosition = startingPosition;
        lastResetTime = Time.time;
        TargetSphere.ResetAllTaskSpheres();

        /* Begin Exercise 2.x */
        // Reset your additional variables introduced above here.
        birdVelocity = Vector3.zero;
        birdAcceleration = Vector3.zero;
        /* End Exercise 2.x */

    }

    // Update is called once per frame
    void Update()
    {
        CheckModeChange();
        ApplyCurrentTransferFunctionOnInput();
        ClampBirdPosition();
        UpdateSpeedLabel();
        UpdateTimeLabel();
        lastFramePosition = transform.position;
    }

    // Waits for presses of the number buttons to switch between the combinations
    // of input device and transfer function
    void CheckModeChange()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentTransferFunction = TransferFunction.IsotonicPosition;
            transferFunctionLabel.text = "Isotonic Input - Position Control";
            virtualJoystick.gameObject.SetActive(false);
            Reset();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentTransferFunction = TransferFunction.IsotonicRate;
            transferFunctionLabel.text = "Isotonic Input - Rate Control";
            virtualJoystick.gameObject.SetActive(false);
            Reset();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentTransferFunction = TransferFunction.IsotonicAcceleration;
            transferFunctionLabel.text = "Isotonic Input - Acceleration Control";
            virtualJoystick.gameObject.SetActive(false);
            Reset();
        } 
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            currentTransferFunction = TransferFunction.ElasticPosition;
            transferFunctionLabel.text = "Elastic Input - Position Control";
            virtualJoystick.gameObject.SetActive(useVirtualJoystick);
            Reset();
        } 
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            currentTransferFunction = TransferFunction.ElasticRate;
            transferFunctionLabel.text = "Elastic Input - Rate Control";
            virtualJoystick.gameObject.SetActive(useVirtualJoystick);
            Reset();
        } 
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            currentTransferFunction = TransferFunction.ElasticAcceleration;
            transferFunctionLabel.text = "Elastic Input - Acceleration Control";
            virtualJoystick.gameObject.SetActive(useVirtualJoystick);
            Reset();
        }
    }

    // Retrieves the inputs of both devices and forwards them to the currently
    // activated transfer function
    void ApplyCurrentTransferFunctionOnInput()
    {
        float isotonicInputX = Input.GetAxis("Mouse X");
        float isotonicInputY = Input.GetAxis("Mouse Y");

        float elasticInputX, elasticInputY;

        if (useVirtualJoystick)
        {
            elasticInputX = virtualJoystick.Horizontal;
            elasticInputY = virtualJoystick.Vertical;
        }
        else
        {
            elasticInputX = Input.GetAxis("Horizontal");
            elasticInputY = Input.GetAxis("Vertical");
        }

        switch (currentTransferFunction)
        {
            case TransferFunction.IsotonicPosition:
                MapInputIsotonicPosition(isotonicInputX, isotonicInputY); break;
            case TransferFunction.IsotonicRate:
                MapInputIsotonicRate(isotonicInputX, isotonicInputY); break;
            case TransferFunction.IsotonicAcceleration:
                MapInputIsotonicAcceleration(isotonicInputX, isotonicInputY); break;
            case TransferFunction.ElasticPosition:
                MapInputElasticPosition(elasticInputX, elasticInputY); break;
            case TransferFunction.ElasticRate:
                MapInputElasticRate(elasticInputX, elasticInputY); break;
            case TransferFunction.ElasticAcceleration:
                MapInputElasticAcceleration(elasticInputX, elasticInputY); break;
        }
    }

    // Takes the isotonic input values and maps them using a position-control transfer function
    void MapInputIsotonicPosition(float isotonicInputX, float isotonicInputY)
    {
        var reachFactor = .1f;
        transform.Translate(isotonicInputX * reachFactor, isotonicInputY * reachFactor, 0.0f);
    }

    // Takes the isotonic input values and maps them using a rate-control transfer function
    void MapInputIsotonicRate(float isotonicInputX, float isotonicInputY)
    {
        // todo-moch: delete comment
        // 1.Moving the mouse in a certain direction results in a change of the bird’s velocity,
        // --> which is in turn applied to a change of the bird’s position every frame.

        // 2.When the mouse stops moving, the bird keeps moving with the previously defined velocity.

        // 3.To stop the bird, the mouse needs to be moved back to its start position.

        // NOTE
        // (done) add a further variable to the class to store the bird’s velocity over frames;
        // (done) reset it to a default value when the Reset() func
        // (done) apply a suitable scaling factor to the input values
        // (done) compensate for varying frame rates such that the bird’s movements are not affected by the current frame rate. (* Time.deltaTime)


        /* Begin Exercise 2.1 */
        if (isotonicInputX != 0f && isotonicInputY != 0f) 
        { 
            birdVelocity += new Vector3(isotonicInputX, isotonicInputY, 0f);
        }

        var scalingFactor = .1f;
        transform.Translate(birdVelocity * Time.deltaTime * scalingFactor);
        /* End Exercise 2.1 */
    }

    // Takes the isotonic input values and maps them using an acceleration-control transfer function
    void MapInputIsotonicAcceleration(float isotonicInputX, float isotonicInputY)
    {
        // todo-moch: delete comment
        // 1. Moving the mouse in a certain direction results in a change of the bird’s acceleration
        // --> which is applied to a change of the bird’s velocity every frame
        // --> which is in turn applied to a change of the bird’s position every frame

        // 2. When the mouse stops moving, the bird keeps getting faster with respect to the previously defined acceleration

        // 3. To stop the bird, the mouse needs to be moved in the inverse direction for the same amount of time

        // NOTE
        // (done) you should introduce additional variables to store the bird’s acceleration and velocity over frames;
        // (done) make sure to reset them to a default value when the Reset() function is called.Please
        // (done) also make sure to apply a suitable scaling factor to the input values.

        /* Begin Exercise 2.2 */
        if (isotonicInputX != 0f && isotonicInputY != 0f)
        {
            birdAcceleration += new Vector3(isotonicInputX, isotonicInputY, 0);
        }

        birdVelocity = birdVelocity + birdAcceleration * Time.deltaTime;

        var scalingFactor = .005f;
        transform.Translate(birdVelocity * scalingFactor);
        /* End Exercise 2.2 */
    }

    // Takes the elastic input values and maps them using a position-control transfer function
    void MapInputElasticPosition(float elasticInputX, float elasticInputY)
    {
        /* Begin Exercise 2.3 */
        var scalingFactor = 10;
        transform.position = new Vector3(elasticInputX * scalingFactor, (elasticInputY * scalingFactor) + startingPosition.y, 0f);
        /* Begin Exercise 2.3 */
    }

    // Takes the elastic input values and maps them using a rate-control transfer function
    void MapInputElasticRate(float elasticInputX, float elasticInputY)
    {
        /* Begin Exercise 2.4 */
        if (elasticInputX == 0f && elasticInputY == 0f) 
        { 
            birdVelocity = Vector3.zero;
            return;
        }

        var scalingFactor = .05f;
        birdVelocity += new Vector3(elasticInputX, elasticInputY, 0f);
        transform.Translate(birdVelocity * Time.deltaTime * scalingFactor);
        /* End Exercise 2.4 */
    }

    // Takes the elastic input values and maps them using an acceleration-control transfer function
    void MapInputElasticAcceleration(float elasticInputX, float elasticInputY)
    {
        /* Begin Exercise 2.5 */
        // ...
        /* End Exercise 2.5 */
    }

    // Makes sure the bird does not continue moving when outside of the frustum boundaries
    void ClampBirdPosition()
    {
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        bool visible = GeometryUtility.TestPlanesAABB(frustumPlanes, GetComponentInChildren<MeshRenderer>().bounds);

        if (!visible)
        {
            transform.position = lastFramePosition;
        }
    }

    // Computes the current speed of the bird and updates the corresponding label
    void UpdateSpeedLabel()
    {
        float distanceTraveled = Vector3.Distance(lastFramePosition, transform.position);
        float speed = distanceTraveled / Time.deltaTime;
        speedLabel.text = "Speed: " + speed.ToString("F2") + " m/s";
    }

    // Computes the time since the technique was activated and updates the corresponding label
    void UpdateTimeLabel()
    {
        if (TargetSphere.numCollected < GameObject.FindObjectsOfType<TargetSphere>().Length)
        {
            float elapsed = Time.time - lastResetTime;
            timeLabel.text = "Time: " + elapsed.ToString("F1") + " s";
        }
    }
}

/* Begin Exercise 2.6
todo-moch: delete
Complete the sphere collection task and report on your task completion times. 
Which combinations were suitable for this task
Which combinations were less suitable? Why? 
For the less-suitable combinations, what are use cases where the respective combinations are more appropriate?

***IsotonicPosition    =>    =>

IsotonicRate           =>   =>
IsotonicAcceleration   =>   =>
ElasticPosition        =>   =>
ElasticRate            =>   =>
ElasticAcceleration    =>   => 

/* End Exercise 2.6 */
