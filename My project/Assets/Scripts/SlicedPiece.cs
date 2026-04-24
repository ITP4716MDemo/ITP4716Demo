using UnityEngine;

public class SlicedPiece : MonoBehaviour
{
    public float fadeTime = 2f;
    public float destroyTime = 5f;

    private void Start()
    {
        Destroy(gameObject, destroyTime);
    }
}