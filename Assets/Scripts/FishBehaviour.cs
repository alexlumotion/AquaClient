using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishBehaviour : MonoBehaviour
{

    [SerializeField] private Renderer meshRenderer;
    [SerializeField] private Texture2D originalTexture;             // Оригінал (readonly)
    [SerializeField] private Texture2D cloneTexture;

    public TexturePainter texturePainter;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnEnable()
    {
        texturePainter.SetFish(meshRenderer, originalTexture);
    }

}
