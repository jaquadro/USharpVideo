
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class VideoActionController : UdonSharpBehaviour
{
    public Material giMaterial;

    void Start()
    {
        giMaterial.SetFloat("_WallLightingMode", 1);
    }

    public void PlayerStart()
    {
        giMaterial.SetFloat("_WallLightingMode", 0);
    }

    public void PlayerStop()
    {
        giMaterial.SetFloat("_WallLightingMode", 1);
    }

    public void PlayerFault()
    {
        giMaterial.SetFloat("_WallLightingMode", 2);
    }
}
