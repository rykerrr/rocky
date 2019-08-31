using UnityEngine;

#pragma warning disable 0649
public class MouseFollow : MonoBehaviour
{
    [SerializeField] private LayerMask whatCanBeLockedOnTo;
    [SerializeField] private float lockOnRadius;
    [SerializeField] private float rotOffset = 90f;

    public Joystick movJoystick;
    public Joystick rotJoystick;

    private Vector3 difference;
    private Camera cam;

    private float angleZ;
    private bool lockedOn = false;

    #region LockOn variables
    Transform enem; // the transform of the enemy that it's locked on to
    Vector3 pos; // position for RotateTowardsPosition (maybe make it so it get's passed in? might be a bit confusing but less variables taking up space here)
    Color32 normCol; // color of the enemy as the lock on changes it to another color
    SpriteRenderer enSr; // cached enemy sr so that it only does GetComponent once in LockOn()
    Vector3 posBuffer; // buffer for the position in case the player isn't trying to change rotation
    #endregion

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        InputChecker();
    }

    private void InputChecker()
    {
        if (GameMaster.isTesting)
        {
            if (!lockedOn)
            {
                pos = cam.ScreenToWorldPoint(Input.mousePosition);

                if (Input.GetMouseButtonDown(2))
                {
                    enem = LockOn(pos);

                    if (enem != null)
                    {
                        lockedOn = true;
                    }
                }
            }
            else
            {
                if (!enem.gameObject.activeSelf)
                {
                    enSr.color = normCol;
                    lockedOn = false;
                }

                pos = enem.position;
            }
        } // ignore, pc controls for testing
        else
        {
            if (!lockedOn) // locked on is the bool used to check whether it's locked on or not, i guess an enum might be better for this
            { // but i don't want to overcomplicate this script more than it already is
                if (rotJoystick.Horizontal != 0 || rotJoystick.Vertical != 0)
                {
                    if (Input.touchCount > 0)
                    {
                        Touch inpTouch = Input.GetTouch(Input.touchCount - 1); // i don't know if it would be better to get the second touch at id 1, since the player can move using a joystick

                        if (!GameMaster.Instance.hud.TouchOverUI(inpTouch)) // this just checks if it's over ui, the method will be provied down
                        {
                            foreach (Touch touch in Input.touches)
                            {
                                if (touch.phase == TouchPhase.Began)
                                {
                                    enem = LockOn(cam.ScreenToWorldPoint(touch.position)); // enem returns a transform, or null
                                    if (enem != null)
                                    {
                                        lockedOn = true;
                                        return;
                                    }
                                }

                            } // check if enemy was pressed
                        }

                        transform.eulerAngles = new Vector3(0f, 0f, Mathf.Atan2(rotJoystick.Vertical, rotJoystick.Horizontal) * 180 / Mathf.PI + rotOffset);
                    }

                }
                else
                {
                    pos = posBuffer;// + transform.position; // the buffer here is used to add a vector to the last position the mouse hit, so if the player moves up, the position would stay the same and he wouldn't rotate towards it
                } // e.g, if he was moving up, but pos was at Vector3.zero, he would rotate to face Vector3.zero
            }
            else
            {
                if (!enem.gameObject.activeSelf) // checking .activeSelf instead of whether it's null because the object it's checking for is using object pooling
                {
                    lockedOn = false;
                    return;
                }
                else
                {
                    pos = enem.position;
                    RotateTowardsPosition();
                }
            }
        }
    }

    //private void FixedUpdate()
    //{

    //}

    private void RotateTowardsPosition() // rotation logic
    {
        var newRot = Quaternion.LookRotation((transform.position - pos), Vector3.forward);
        newRot.x = 0;
        newRot.y = 0;
        transform.rotation = newRot;
        transform.localRotation = newRot;
        //difference = (pos - transform.position).normalized;
        //angleZ = Mathf.Atan2(difference.x, difference.y) * Mathf.Rad2Deg;
        //transform.localRotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, (angleZ + rotOffset) * -1);
    }

    private Transform LockOn(Vector3 origin)
    {
        Collider2D en = Physics2D.OverlapCircle(origin, lockOnRadius, whatCanBeLockedOnTo); // the lock on radius is pretty small, checks if an enemy is inside the radius then locks on to it and moves around it

        if (en != null)
        {
            enSr = en.GetComponent<SpriteRenderer>();
            normCol = enSr.color;
            enSr.color = Color.green;

            return en.transform;
        }
        else return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, lockOnRadius);
        Gizmos.color = Color.gray;
    }
}
#pragma warning restore 0649