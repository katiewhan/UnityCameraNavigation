using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum NavigationMode
{
    Freeform,
    FixedView
}

public class NavigateCamera : MonoBehaviour
{
    public NavigationMode navigationMode = NavigationMode.Freeform;
    public bool inverseMouseDirection = false;
    public bool clickToMoveForward = true;
    public string collisionTargetTag = "";

    public float keySpeed = 0.1f;
    public float rotateKeySpeed = 0.3f;
    public float rotateMouseSpeed = 2f;
    public float animationStep = 10f;

    private bool isDrag;
    private float xRotation;
    private float yRotation;

    // Used for animation
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float timeCount;

    // Used for bounce on collision
    private int lastDirection; 

    // Start is called before the first frame update
    void Start()
    {
        timeCount = 0f;
        lastDirection = 1;
        startPosition = this.transform.position;
        targetPosition = this.transform.position;
        xRotation = this.transform.localEulerAngles.y;
        yRotation = this.transform.localEulerAngles.x;
    }

    // Update is called once per frame
    void Update()
    {
        // Position animation
        if (timeCount < 1.0f)
        {
            this.transform.position = Vector3.Lerp(startPosition, targetPosition, timeCount);
            timeCount += Time.deltaTime;
        }

        // Mouse click to move forward
        if (Input.GetMouseButtonUp(0) && !isDrag && clickToMoveForward)
        {
            startPosition = this.transform.position;
            Vector3 fwd = this.transform.forward;
            fwd.y = 0f;
            targetPosition = this.transform.position + fwd * animationStep;
            lastDirection = 1;
            timeCount = 0f;
        }

        // Mouse drag to look around
        if (Input.GetMouseButton(0))
        {
            int direction = this.inverseMouseDirection ? -1 : 1;

            float xDelta = Input.GetAxis("Mouse X") * rotateMouseSpeed * direction;
            float yDelta = Input.GetAxis("Mouse Y") * rotateMouseSpeed * direction;

            if (navigationMode == NavigationMode.FixedView || navigationMode == NavigationMode.Freeform)
            {
                xRotation += xDelta;
                xRotation %= 360;
            }

            if (navigationMode == NavigationMode.Freeform)
            {
                yRotation -= yDelta;
                yRotation = Mathf.Clamp(yRotation, -80, 80);
            }

            this.transform.localEulerAngles = new Vector3(yRotation, xRotation, 0f);

            if (Mathf.Abs(xDelta) > 2f || Mathf.Abs(yDelta) > 2f)
            {
                isDrag = true;
            }
        }
        else
        {
            isDrag = false;
        }

        // Keys up/down
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.W))
        {
            int direction = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S) ? -1 : 1;

            if (navigationMode == NavigationMode.FixedView || navigationMode == NavigationMode.Freeform)
            {
                Vector3 fwd = this.transform.forward;
                fwd.y = 0f;
                this.transform.position += fwd * direction * keySpeed;
                lastDirection = direction;
            }
        }

        // Keys left/right
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            int direction = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A) ? -1 : 1;

            if (navigationMode == NavigationMode.Freeform)
            {
                Vector3 fwd = this.transform.right;
                fwd.y = 0f;
                this.transform.position += fwd * direction * keySpeed;
                lastDirection = direction;
            }

            if (navigationMode == NavigationMode.FixedView)
            {
                xRotation += direction * rotateKeySpeed;
                xRotation %= 360;
                this.transform.localEulerAngles = new Vector3(yRotation, xRotation, 0f);
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == collisionTargetTag)
        {
            startPosition = this.transform.position;
            Vector3 fwd = this.transform.forward;
            fwd.y = 0f;
            targetPosition = this.transform.position - fwd * animationStep * lastDirection;
            timeCount = 0f;
        }
    }
}
