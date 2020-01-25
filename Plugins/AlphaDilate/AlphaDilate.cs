using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// When attached to a camera, this script dilates the colors of opaque pixels to cover partially opaque areas caused by anti aliasing.
public class AlphaDilate : MonoBehaviour
{
    private Camera camera;
    private Material dilateMaterial;
    private RenderTexture temp1;
    private RenderTexture temp2;
    
    public void Start()
    {
        if (camera == null) {
            camera = GetComponent<Camera>();
        }
        if (camera == null)
            return;
        
        dilateMaterial = new Material(Shader.Find("Custom/AlphaDilate"));
    }
    
    public void OnRenderImage(RenderTexture source, RenderTexture destination) {
        if (camera == null)
            return;
        if (temp1 == null)
            temp1 = RenderTexture.GetTemporary(source.descriptor); 
        if (temp2 == null)
            temp2 = RenderTexture.GetTemporary(source.descriptor); 
        Graphics.Blit(source, temp1, dilateMaterial);
        Graphics.Blit(temp1, temp2, dilateMaterial);
        Graphics.Blit(temp2, destination, dilateMaterial);
    }
    
    public void OnDestroy() {
        if (temp1 != null) {
            RenderTexture.ReleaseTemporary(temp1);
            temp1 = null;
        }
        if (temp2 != null) {
            RenderTexture.ReleaseTemporary(temp2);
            temp2 = null;
        }
    }
    
    public void OnApplicationQuit() {
        OnDestroy();
    }
}