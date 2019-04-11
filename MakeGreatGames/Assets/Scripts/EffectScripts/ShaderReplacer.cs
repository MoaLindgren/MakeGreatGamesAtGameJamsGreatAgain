using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

[ExecuteInEditMode]
public class ShaderReplacer : MonoBehaviour
{
    [SerializeField]
    Shader colorblindShader;

    [SerializeField]
    Material mat;
    
    void OnEnable()
    {
        print("eyy");
        ReplaceShader(colorblindShader, null);
    }

    void ReplaceShader(Shader shader, string tag)
    {
        //Camera.main.SetReplacementShader(shader, tag);
        DrawRendererSettings dRS = new DrawRendererSettings(Camera.main, new ShaderPassName());
        dRS.SetOverrideMaterial(mat, 0);
    }

    private void OnDisable()
    {
        GetComponent<Camera>().ResetReplacementShader();
        print("yyooooo");
    }

    private void Awake()
    {
        mat = new Material(colorblindShader);
    }

    /*
    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, mat);
        print("??");
    }
    */
}
