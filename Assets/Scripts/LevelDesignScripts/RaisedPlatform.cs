using UnityEngine;

public class RaisedPlatform : MonoBehaviour
{

    [Header("Pillar")]
    [SerializeField] private float pillarHeight = 5f;
    [SerializeField] private float pillarWidth = 1f;
    [Header("Platform")]
    [SerializeField] private float PlatformSizeX = 5f;
    [SerializeField] private float PlatformSizeY = 1f;
    [SerializeField] private float PlatformSizeZ = 5f;

    private Transform pillar;
    private Transform platform;

    


    private void OnValidate()
    {
        CacheChildren();
        ApplyChanges();
    }

    private void CacheChildren()
    {
        if (transform.childCount < 2)
        {
            return;
        }

        pillar = transform.GetChild(0);
        platform = transform.GetChild(1);
    }


    private void ApplyChanges()
    {
        if (!pillar || !platform)
        {
            return;
        }

        pillar.localScale = new Vector3(pillarWidth, pillarHeight, pillarWidth);
        pillar.localPosition = new Vector3(0f, pillarHeight, 0f);

        platform.localScale = new Vector3(PlatformSizeX, PlatformSizeY, PlatformSizeZ);

        platform.localPosition = new Vector3(0f, pillarHeight * 2f, 0f);
    }
}
