using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericSpaceObject : MonoBehaviour
{
    protected Renderer render;
    protected bool WrapFromScreenOnX = true;

    private void Awake()
    {
        render = GetComponent<Renderer>();
        AwakenCall();
    }

    protected virtual void AwakenCall()
    {

    }

    private void Update()
    {
        GenericUtilities.WrapFromScreenEdge(transform, GenericUtilities.GetScreenWrapOffset(render), WrapFromScreenOnX);
        UpdateCall();
    }

    protected virtual void UpdateCall()
    {

    }
}
