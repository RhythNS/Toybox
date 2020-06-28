using cakeslice;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for placing a building
/// </summary>
public class BuildingPlacer : SimpleSelectable, ICommandable
{
    [SerializeField] private Transform[] toLowerWhenPlaced;
    private Building building;

    public Supplies Cost { get; set; }

    private List<Collider> colliders;
    private InnerState state;
    private bool canPlace;

    private Vector2 rotateAroundMosPos;

    private IBuildingsSelectable buildingsSelectable;

    protected override void InnerAwake()
    {
        // Deactivate all colliders
        colliders = new List<Collider>();
        transform.GetComponentsInChildren(false, colliders);
        for (int i = 0; i < colliders.Count; i++)
            colliders[i].enabled = false;

        building = GetComponent<Building>();
        state = InnerState.Placing;
        canPlace = false;
    }

    private enum InnerState
    {
        Placing, Rotating, Placed
    }

    private void Update()
    {
        // switch case on the current innerState
        switch (state)
        {
            case InnerState.Placing: // if in placing mode

                // Get the position of the mouse on the world and place the building onto this position
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit[] hits = Physics.RaycastAll(ray, 100);
                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].collider.gameObject == MapValuesDict.Instance.Map)
                    {
                        Vector3 newPos = hits[i].point - building.MovePoint.position;
                        transform.position += newPos;
                        break;
                    }
                }
                CheckIfCanPlace();
                break;

            case InnerState.Rotating: // if in rotating mode

                // Rotate the building around based on the position of where the mouse was when the building was placed
                // and where the mouse is right now
                Vector2 newMousePos = Input.mousePosition;
                newMousePos -= rotateAroundMosPos;

                float rotation = (float)((Mathf.Atan2(newMousePos.x, newMousePos.y) / System.Math.PI) * 180f);
                if (rotation < 0)
                    rotation += 360f;

                transform.rotation = Quaternion.Euler(0, rotation, 0);

                CheckIfCanPlace();
                break;
        }
    }

    /// <summary>
    /// Sets the canPlace bool and changes the color based on wheter the building can be placed
    /// </summary>
    private void CheckIfCanPlace()
    {
        // Get all colliders around the building
        Collider[] colls = Physics.OverlapBox(transform.position + building.BoxCenter, building.BoxSize / 2, transform.rotation);

        bool prevCan = canPlace;
        canPlace = true;

        // iterate through all colliders
        for (int i = 0; i < colls.Length; i++)
        {
            // If the collider is a decor or the map then continue
            if (colls[i].CompareTag("decor") == true || colls[i].gameObject == MapValuesDict.Instance.Map)
                continue;
            // otherwise set the canPlace to false
            canPlace = false;
            break;
        }

        // If the previous can place was not the same value as the current can place value then change the outline color
        if (canPlace != prevCan)
            SetOutlineColor(canPlace == true ? 1 : 2);
    }

    protected override void ModifyStartingOutline(Outline outline)
    {
        outline.color = 1;
    }

    protected override void InnerSelect()
    {
        if (state == InnerState.Placed && buildingsSelectable != null)
            buildingsSelectable.Select();
    }

    protected override void InnerDeSelect()
    {
        if (state != InnerState.Placed)
            Destroy(gameObject);
    }

    public void Interact(RaycastHit hit)
    {
        switch (state)
        {
            case InnerState.Placing:

                // If it can not be placed simply break
                if (canPlace == false)
                    break;

                // otherwise set the state to the next state and save the MousePosition for the rotation
                state = InnerState.Rotating;
                rotateAroundMosPos = Input.mousePosition;

                break;
            case InnerState.Rotating:

                // If the building can not be placed then simply break
                if (canPlace == false)
                    break;

                // Add it to the town. If there are not enough resources then delete the building
                Town town = TownDict.Instance.Towns[0]; // TODO: Replace if multiple towns are allowed
                if (town.Supplies.AdjustAmount(Cost) == false)
                {
                    UIManager.Instance.DisplayEvent(ToyEvent.NotEnoughResources);
                    DeSelect();
                    return;
                }
                town.AddBuilding(building);

                // Set the state to the placed state
                state = InnerState.Placed;

                // Deselect the building from the camera controller
                if (CameraController.Instance.DeSelect(this) == false)
                    DeSelect();

                // Reenable all colliders
                for (int i = 0; i < colliders.Count; i++)
                    colliders[i].enabled = true;

                buildingsSelectable = GetComponent<IBuildingsSelectable>();
                SetOutlineColor(0);

                building.OnBuildingPlaced();

                break;
        }
    }
}
