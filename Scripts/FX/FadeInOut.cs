using UnityEngine;
using System.Collections;

public class FadeInOut : MonoBehaviour
{

    public delegate void OnComplete();

    public float fadeTime = 2.0f;
    public bool autoFade = true;
    private bool isFaded = false;

    private Material fadeMaterial = null;

    void Awake() {
        fadeMaterial = new Material(Shader.Find("Transparent/Diffuse"));
    }

    void Start() {
        if (autoFade) FadeIn(null);
    }

    public void FadeOut(OnComplete onComplete) {
        StartCoroutine(FadeOutC(onComplete));
    }

    public void FadeIn(OnComplete onComplete) {
        StartCoroutine(FadeInC(onComplete));
    }

    IEnumerator FadeOutC(OnComplete onComplete) {
        float elapsedTime = 0.0f;
        Color color = fadeMaterial.color;
        color.a = 0.0f;
        isFaded = true;
        while (elapsedTime < fadeTime) {
            yield return new WaitForEndOfFrame();
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / fadeTime);
            fadeMaterial.color = color;
        }
        if (onComplete != null) onComplete();
    }

    IEnumerator FadeInC(OnComplete onComplete) {
        float elapsedTime = 0.0f;
        Color color = fadeMaterial.color;
        color.a = 1.0f;
        isFaded = true;
        while (elapsedTime < fadeTime) {
            yield return new WaitForEndOfFrame();
            elapsedTime += Time.deltaTime;
            color.a = 1.0f - Mathf.Clamp01(elapsedTime / fadeTime);
            fadeMaterial.color = color;
        }
        isFaded = false;
        if (onComplete != null) onComplete();
    }

    void OnPostRender() {
        if (isFaded) {
            fadeMaterial.SetPass(0);
            GL.PushMatrix();
            GL.LoadOrtho();
            GL.Begin(GL.QUADS);
            GL.Color(fadeMaterial.color);
            GL.Vertex3(0, 0, -1);
            GL.Vertex3(0, 1, -1);
            GL.Vertex3(1, 1, -1);
            GL.Vertex3(1, 0, -1);
            GL.End();
            GL.PopMatrix();
        }
    }

    void OnDestroy() {
        if (fadeMaterial != null) {
            Destroy(fadeMaterial);
        }
    }
}