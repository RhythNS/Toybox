using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Controlls the camera
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [SerializeField] private float zoomSpeed = 100f;
    [SerializeField] private float zoomTime = 0.1f;
    [SerializeField] private float minZoom = 10f, maxZoom = 60f;
    [SerializeField] private float minDragSpeed = 1f, maxDragSpeed = 6f;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float startingHeight = 30;
    [SerializeField] private int screenBordersInPixel = 10;
    [SerializeField] private float vectorSizeNeededForScrolling = 50;
    [SerializeField] private Vector3 StartingRotation = new Vector3(90, 0, 0);

    public static CameraController Instance { get; private set; }

    /// <summary>
    /// The bounds of the level. This is used to clamp the position with
    /// </summary>
    public Rect LevelRect { get; set; }

    private float zoomTarget = 0f;
    private float zoomVelocity = 0f;

    private Camera attachedCamera;
    private Vector2 screenSize;
    private Vector3 prevMousePos;
    private Vector3 selectMousePos;
    private Vector3 currentMousePos;

    private GameObject previousSelected, newSelected;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        attachedCamera = GetComponent<Camera>();

        Resolution res = Screen.currentResolution;
        screenSize = new Vector2(res.width, res.height);

        zoomTarget = startingHeight;
        Vector3 pos = transform.position;
        pos.y = startingHeight;
        transform.position = pos;
        transform.rotation = Quaternion.Euler(StartingRotation);
    }

    private void Update()
    {
        // Mosue is not over ui
        if (EventSystem.current.IsPointerOverGameObject() == false)
        {
            if (Input.GetMouseButtonDown(0) == true) // Left mouse button was just clicked this frame
            {
                selectMousePos = Input.mousePosition;
                // Save the selectable
                newSelected = GetSelectable();
            }
            else if (Input.GetMouseButtonUp(0) == true) // Left mouse button was let go this frame then try to select someone
            {
                TrySelect();
            }

            if (Input.GetMouseButtonUp(1) == true) // right mouse button up this frame
            {
                // If someone was selected and they can be commanded and raycast found something then command the commandable
                if (previousSelected != null && previousSelected.TryGetComponent(out ICommandable commandable)
                    && GetHitAtMousePos(out RaycastHit hit, int.MaxValue))
                {
                    commandable.Interact(hit);
                }
            }

            // Handle zoom
            float deltaZoom = Input.mouseScrollDelta.y;
            if (deltaZoom != 0)
            {
                zoomTarget -= deltaZoom * zoomSpeed;
                zoomTarget = Mathf.Clamp(zoomTarget, minZoom, maxZoom);
            }
        }
    }

    private void FixedUpdate()
    {
        currentMousePos = Input.mousePosition;
        Vector3 newPos = transform.position;

        if (Input.GetMouseButton(0) == false) // Left mouse button is not pressed 
        {
            newPos += MoveScreen();
        }
        else if (EventSystem.current.IsPointerOverGameObject() == false) // Left mouse button is pressed and mouse not over ui
        {
            // Pan the camera around
            Vector3 dif = prevMousePos - currentMousePos;
            dif.Set(dif.x, 0, dif.y);
            float norm = MathUtil.Normalize(transform.position.y, minZoom, maxZoom);
            newPos += dif * Mathf.Clamp(norm, minDragSpeed, maxDragSpeed);
        }

        // Smooth the zoomn
        newPos.y = Mathf.SmoothDamp(transform.position.y, zoomTarget, ref zoomVelocity, zoomTime);

        prevMousePos = currentMousePos;
        SetPosition(newPos);
    }

    /// <summary>
    /// Outside method to force a gameobject to be selected. Returns wheter it successeded or not
    /// </summary>
    public bool Select(GameObject toSelect)
    {
        if (toSelect.TryGetComponent(out ISelectable selectable) == true)
        {
            if (previousSelected != null && previousSelected.TryGetComponent(out ISelectable prevSelectable) == true)
                DeSelect(prevSelectable);
            selectable.Select();
            previousSelected = toSelect;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Outside method to force a gameobject to be unselected. Returns wheter it successeded or not
    /// </summary>
    public bool DeSelect(ISelectable toUnselect)
    {
        if (previousSelected != null && toUnselect == previousSelected.GetComponent<ISelectable>())
        {
            toUnselect.DeSelect();
            previousSelected = null;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Try and select the newly selectable
    /// </summary>
    private void TrySelect()
    {
        // If we scrolled to far then simply return
        if ((selectMousePos - currentMousePos).sqrMagnitude >= vectorSizeNeededForScrolling)
            return;

        if (previousSelected != null)
        {
            // is the newSelect is null then deselect the previous selected
            if (newSelected == null)
            {
                previousSelected.GetComponent<ISelectable>().DeSelect();
                previousSelected = null;
                return;
            }

            if (previousSelected == newSelected) // previous Select is new select so just ignore it
                return;

            // previous select is not new Select so deselect it
            previousSelected.GetComponent<ISelectable>().DeSelect();
        }

        // lastly select the newSelected and set it to previous selected
        if (newSelected != null)
        {
            newSelected.GetComponent<ISelectable>().Select();
            previousSelected = newSelected;
        }
    }

    /// <summary>
    /// Looks for a selectable from the ray of the mouse position.
    /// </summary>
    private GameObject GetSelectable()
    {
        if (GetHitAtMousePos(out RaycastHit hit, 1 << 8) == false || hit.collider.gameObject.GetComponent<ISelectable>() == null)
            return null;
        return hit.collider.gameObject;
    }

    private bool GetHitAtMousePos(out RaycastHit hit, int layer)
    {
        Ray ray = attachedCamera.ScreenPointToRay(currentMousePos);
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 2);

        return Physics.Raycast(ray, out hit, 100f, layer);
    }

    protected Vector3 MoveScreen()
    {
        float hori = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");

        Vector3 movementVector = Vector3.zero;
        if (currentMousePos.x < screenBordersInPixel)
            hori -= 1;
        else if (currentMousePos.x > screenSize.x - screenBordersInPixel)
            hori += 1;
        movementVector.x = Mathf.Clamp(hori, -1, 1);

        if (currentMousePos.y < screenBordersInPixel)
            vert -= 1;
        else if (currentMousePos.y > screenSize.y - screenBordersInPixel)
            vert += 1;
        movementVector.z = Mathf.Clamp(vert, -1, 1);

        return movementVector * moveSpeed * Time.fixedDeltaTime;
    }


    protected void SetPosition(Vector3 position)
    {
        position.Set(Mathf.Clamp(position.x, LevelRect.x, LevelRect.x + LevelRect.width),
            position.y, Mathf.Clamp(position.z, LevelRect.y, LevelRect.y + LevelRect.height));
        transform.position = position;
    }

}
