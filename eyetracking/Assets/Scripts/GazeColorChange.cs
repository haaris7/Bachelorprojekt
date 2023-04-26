using UnityEngine;

public class GazeColorChange : MonoBehaviour
{
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color gazeColor = Color.red;

    private Material material;

    private void Awake()
    {
        material = GetComponent<Renderer>().material;
    }

    public void OnGazeEnter()
    {
        Debug.Log("OnGazeEnter called. Changing color to: " + gazeColor);
        material.color = gazeColor;
    }

    public void OnGazeExit()
    {
        Debug.Log("OnGazeExit called. Changing color to: " + normalColor);
        material.color = normalColor;
    }
<<<<<<< HEAD
}
=======
}
>>>>>>> parent of fba6a12 (Data logger(adjusted))
