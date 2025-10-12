using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class GlitchEffect : MonoBehaviour
{
    [Range(0f, 1f)] public float intensity = 0.2f;
    [Range(0f, 1f)] public float colorSplit = 0.05f;
    [Range(0f, 1f)] public float noiseStrength = 0.3f;

    public Shader glitchShader;
    private Material glitchMaterial;

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (glitchShader == null)
        {
            Graphics.Blit(src, dest);
            return;
        }

        if (glitchMaterial == null)
            glitchMaterial = new Material(glitchShader);

        glitchMaterial.SetFloat("_Intensity", intensity);
        glitchMaterial.SetFloat("_ColorSplit", colorSplit);
        glitchMaterial.SetFloat("_NoiseStrength", noiseStrength);
        glitchMaterial.SetFloat("_TimeFactor", Time.time);

        Graphics.Blit(src, dest, glitchMaterial);
    }
}
