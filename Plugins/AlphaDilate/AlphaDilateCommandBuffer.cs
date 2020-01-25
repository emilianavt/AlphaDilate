using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// When attached to a camera, this script dilates the colors of opaque pixels to cover partially opaque areas caused by anti aliasing.
[ExecuteInEditMode()]
public class AlphaDilateCommandBuffer : MonoBehaviour
{
    private Camera camera;
    private Material dilateMaterial;
    static private CommandBuffer commandBuffer = null;
    
    public void Start()
    {
        if (camera == null) {
            camera = GetComponent<Camera>();
        }
        if (camera == null)
            return;
        if (commandBuffer != null)
            return;
        
        dilateMaterial = new Material(Shader.Find("Custom/AlphaDilate"));
        
        commandBuffer = new CommandBuffer();
        commandBuffer.name = "AlphaDilate";
        
        int source = Shader.PropertyToID("_AlphaDilateSource");
        int temp1 = Shader.PropertyToID("_AlphaDilateTemp1");
        int temp2 = Shader.PropertyToID("_AlphaDilateTemp2");
        
        commandBuffer.GetTemporaryRT(source, -1, -1, 0, FilterMode.Bilinear);
        commandBuffer.GetTemporaryRT(temp1, -1, -1, 0, FilterMode.Bilinear);
        commandBuffer.GetTemporaryRT(temp2, -1, -1, 0, FilterMode.Bilinear);
        
        commandBuffer.Blit(BuiltinRenderTextureType.CurrentActive, source);
        
        commandBuffer.Blit(source, temp1, dilateMaterial);
        commandBuffer.Blit(temp1, temp2, dilateMaterial);
        commandBuffer.Blit(temp2, BuiltinRenderTextureType.CameraTarget);//, dilateMaterial);
        
        commandBuffer.ReleaseTemporaryRT(source);
        commandBuffer.ReleaseTemporaryRT(temp1);
        commandBuffer.ReleaseTemporaryRT(temp2);
    }
    
    public void OnEnable() {
        if (camera == null)
            return;
        if (commandBuffer == null)
            Start();
        CommandBuffer[] buffers = camera.GetCommandBuffers(CameraEvent.AfterImageEffects);
        bool found = false;
        foreach (CommandBuffer buffer in buffers) {
            if (buffer.name == commandBuffer.name)
                found = true;
        }
        if (!found)
            camera.AddCommandBuffer(CameraEvent.AfterImageEffects, commandBuffer);
    }
    
    public void OnDisable() {
        if (camera == null)
            return;
        if (commandBuffer == null)
            Start();
        CommandBuffer[] buffers = camera.GetCommandBuffers(CameraEvent.AfterImageEffects);
        bool found = false;
        foreach (CommandBuffer buffer in buffers) {
            if (buffer.name == commandBuffer.name)
                found = true;
        }
        if (found)
            camera.RemoveCommandBuffer(CameraEvent.AfterImageEffects, commandBuffer);
    }
    
    public void Update() {
        if (camera == null)
            return;
        if (commandBuffer == null)
            Start();
        CommandBuffer[] buffers = camera.GetCommandBuffers(CameraEvent.AfterImageEffects);
        if (buffers.Length >= 1) {
            if (buffers[buffers.Length - 1].name != commandBuffer.name) {
                OnDisable();
                OnEnable();
            }
        }
    }
        
    public void OnDestroy() {
        OnDisable();
    }
    
    public void OnApplicationQuit() {
        OnDestroy();
    }
}