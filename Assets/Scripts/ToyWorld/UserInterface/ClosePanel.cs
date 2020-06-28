using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Closes a parent panel
/// </summary>
public class ClosePanel : MonoBehaviour
{
    private void Start()
    {
        // Add the close funtion on the button on this gameobject and then destroy the instance of this script
        GetComponent<Button>().onClick.AddListener(transform.parent.GetComponent<IClosable>().Close);
        Destroy(this);
    }
}

