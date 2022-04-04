using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedTexture : MonoBehaviour
{
    [SerializeField] private Texture[] textures;
    private int currentTextureID = 0;
    private Renderer meshRenderer;

    [SerializeField]
    private float frameTime = 0.2f;

    private void OnEnable()
    {
        StartCoroutine(Animate());
    }

    private void OnDisable()
    {
        StopCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        while (true)
        {
            yield return new WaitForSeconds(frameTime);

            currentTextureID++;
            if (currentTextureID >= textures.Length)
            {
                currentTextureID = 0;
            }
            meshRenderer.material.SetTexture("_BaseMap", textures[currentTextureID]);
            meshRenderer.material.SetTexture("_EmissionMap", textures[currentTextureID]);
        }
    }
}