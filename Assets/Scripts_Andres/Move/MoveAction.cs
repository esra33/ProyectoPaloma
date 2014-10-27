using UnityEngine;
using System.Collections;

public class MoveAction : MonoBehaviour {

    public float speed;
    public float angularSpeed;
        
    private Vector3 target;
    private Vector3 forward;
    private float internalTime = 0;
    private bool isSeeking = false;

    private Vector3 pos;
    private Vector3 rot;
    public void Start()
    {
        this.pos = this.transform.position;
        this.rot = this.transform.eulerAngles;
    }

    public Transform _target;
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            this.GoTo(_target.position);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            this.isSeeking = false;
            this.transform.position = this.pos;
            this.transform.eulerAngles = this.rot;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            this.angularSpeed += 5;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            this.angularSpeed -= 5;
        }
    }

    void DrawLine(Vector3 from, Vector3 to)
    {
        Gizmos.DrawLine(transform.position + from, transform.position + to);
    }

    public void GoTo(Vector3 target)
    {
        // Reset the current status
        this.target = target;
        this.forward = this.transform.forward;
        this.internalTime = 0;

        if (!this.isSeeking)
        {
            this.StartCoroutine(Seek());
        }
    }

    private void CalculateOrientation()
    {
        Vector3 desiredDirection = Vector3.Normalize(this.target - this.transform.position);
        float angle = Vector3.Angle(this.transform.forward, desiredDirection);

        // check if situation is complete
        if (angle == 0)
            return;

        // finish rotation
        if (angle < this.angularSpeed * Time.deltaTime)
        {
            this.transform.forward = desiredDirection;
        }

        float timeMultiplicatior = this.angularSpeed / angle;
        Debug.Log(angle + " --- " + timeMultiplicatior);

        // Interpolate between both vectors
        this.transform.forward = Vector3.Slerp(this.forward, desiredDirection, this.internalTime);

        this.internalTime += Time.deltaTime * timeMultiplicatior;
    }

    private IEnumerator Seek()
    {
        this.isSeeking = true;
        while (this.isSeeking && Vector3.Distance(this.transform.position, this.target) > Time.deltaTime * this.speed)
        {
            CalculateOrientation();
            this.transform.position += this.transform.forward * this.speed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }
}
