using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class damageSphere : MonoBehaviour
{
    public bool active = false;
    private MeshRenderer meshRenderer;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer) meshRenderer.enabled = false;
    }
    private void OnTriggerStay(Collider other)
    {
        if (active)
        {
            var health = other.GetComponent<Health>();
            if (health) health.takeDamage(1);
        }
    }

    public void OpenDamageCollider()
    {
        active = true;
        if (meshRenderer) meshRenderer.enabled = true;
    }

    public void CloseDamageCollider()
    {
        active = false;
        if (meshRenderer) meshRenderer.enabled = false;
    }
}
