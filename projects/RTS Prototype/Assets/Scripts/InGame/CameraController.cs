using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class CameraController : MonoBehaviour
{

    public float PanSpeed = 1.0f;
    public float ZoomSpeed = 1.0f;
    public int MousePanBorderPercentage = 5;
    public float ZoomSafetyDistance = 1.0f;
    public bool enableMousePan;

    private Transform camTarget;

    /* TODO: Need to figure out how to disable Panning if the mouse is no longer inside the window.
    */

    // Use this for initialization
    void Start()
    {
        // For now as working with the UI makes it easier when the camera is not rotate when editing
        transform.rotation = Quaternion.Euler(60.0f, 0.0f, 0.0f);

        if(transform.GetChild(0).name == "CameraTarget")
        {
            camTarget = transform.GetChild(0).transform;
        }
        else
        {
            Debug.LogError("The Camera does not have a child object which is the "
                + "reference point 'CameraTarget'.");
        }

        Ray CamRay = Camera.main.ScreenPointToRay(new Vector3((float)Screen.width / 2.0f, 
                             (float)Screen.height / 2.0f, 0.0f));
        RaycastHit hit;
        //Camera.main.transform.LookAt(new Vector3(0.0f, 0.0f, 0.0f));

        if(Physics.Raycast(CamRay, out hit, 100.0f, LayerMask.GetMask("Ground")))
        {
            //Debug.Log("Camera Transform local" + transform.InverseTransformPoint(hit.point));
            //Debug.Log("Camera Ground Point:" + hit.point);
            //Debug.Log("Camera Target Pos: " + camTarget.position);
        }
        
    }
	
    // Update is called once per frame
    void Update()
    {

        if(FindObjectOfType<GameController>().state == GameController.GameState.InGame)
        {
            // Panning Camera Movement:
            // Note: Pan defaults to panning to the right
            Vector3 Pan = new Vector3(0.0f, 0.0f, 0.0f);
            if(Input.GetKey(KeyCode.RightArrow) || MousePanRight()) Pan += Vector3.right;
            if(Input.GetKey(KeyCode.LeftArrow) || MousePanLeft()) Pan += -Vector3.right;
            if(Input.GetKey(KeyCode.UpArrow) || MousePanUp()) Pan += Vector3.forward;
            if(Input.GetKey(KeyCode.DownArrow) || MousePanDown()) Pan += -Vector3.forward;

            float CameraZoomMultiplicator = ((camTarget.position - transform.position).magnitude / 2.82842684f);
            transform.Translate(Pan.normalized * PanSpeed * Time.deltaTime * CameraZoomMultiplicator, Space.World);

            // Zooming:
            float ZoomInput = 0;
            //Debug.Log(Input.GetAxis("Mouse Scrollwheel"));
            if(Input.GetAxis("Mouse Scrollwheel") != 0) ZoomInput = Input.GetAxis("Mouse Scrollwheel");

            if(Input.GetAxis("Zoom") != 0) ZoomInput = Input.GetAxis("Zoom");
            

            Vector3 ZoomVec = (camTarget.position - transform.position).normalized * ZoomInput * ZoomSpeed * Time.deltaTime;
            if((camTarget.position - (transform.position + ZoomVec)).magnitude > ZoomSafetyDistance
               || ZoomInput < 0)
            {
                transform.Translate(ZoomVec, Space.World);
                camTarget.Translate(-ZoomVec, Space.World);

            }
        }

    }

    private bool MousePanRight()
    {
        if(Input.mousePosition.x >= ((1.0f - ((float)MousePanBorderPercentage / 100.0f)) * Screen.width) && enableMousePan)
        {
            return true;
        }
        return false;
    }

    private bool MousePanLeft()
    {
        if(Input.mousePosition.x <= (((float)MousePanBorderPercentage / 100.0f) * Screen.width) && enableMousePan)
        {
            return true;
        }
        return false;
    }

    private bool MousePanUp()
    {
        if(Input.mousePosition.y >= ((1.0f - ((float)MousePanBorderPercentage / 100.0f)) * Screen.height) && enableMousePan)
        {
            return true;
        }
        return false;
    }

    private bool MousePanDown()
    {
        if(Input.mousePosition.y <= (((float)MousePanBorderPercentage / 100.0f) * Screen.height) && enableMousePan)
        {
            return true;
        }
        return false;
    }
}
