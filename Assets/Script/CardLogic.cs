using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardLogic : MonoBehaviour
{
    public Material[] cardMaterial;
    public MeshRenderer MeshRenderer;
    public void Start()
    {
        var startPosition = MeshRenderer.transform.localPosition;

        MeshRenderer.transform.localPosition = startPosition + (Vector3.up *Random.Range(0.001f, 0.002f));

        var randomCardMat = cardMaterial[Random.Range(0, cardMaterial.Length)];
        MeshRenderer.material = randomCardMat;
    }
    public void Update()
    {
        
    }
}
