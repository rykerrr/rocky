using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    private Vector3 velocity = Vector3.zero;
    private Vector3 pos;

    private bool damping = false;

    void Start()
    {
        if (!target)
        {
            target = FindObjectOfType<Player>().transform;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (target && !damping)
        {
            pos = new Vector3(target.position.x, target.position.y, transform.position.z);
            transform.position = pos;
        }
        else if (damping)
        {
            transform.position = Vector3.SmoothDamp(transform.position, pos, ref velocity, 0.5f);
        }
    }

    public void SmoothDampToPos(Vector3 _pos, float time)
    {
        damping = true;
        pos = _pos;
        StartCoroutine(ResetVal(time));
    }

    private IEnumerator ResetVal(float time)
    {
        yield return new WaitForSeconds(time);
        damping = false;
    }
}
