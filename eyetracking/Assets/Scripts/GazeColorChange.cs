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
        material.color = gazeColor;
    }

    public void OnGazeExit()
    {
        material.color = normalColor;
    }
}
