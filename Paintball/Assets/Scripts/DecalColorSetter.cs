using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DecalColorSetter : MonoBehaviour
{
    public Color splashColor;
    public float durationVisible = 5f;
    public float fadeDuration = 2f;

    private DecalProjector decalProjector;
    private Material materialCopy;

    void Start()
    {
        decalProjector = GetComponent<DecalProjector>();
        materialCopy = new Material(decalProjector.material);
        materialCopy.SetColor("_Color", splashColor);
        decalProjector.material = materialCopy;

        StartCoroutine(FadeDecal());
    }

    IEnumerator FadeDecal()
    {
        yield return new WaitForSeconds(durationVisible);

        float timer = 0f;
        float alpha = 1f;

        while (timer < fadeDuration)
        {
            alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);

            //materialCopy.SetColor("_Color", new Color(splashColor.r, splashColor.g, splashColor.b, alpha));
            decalProjector.fadeFactor = alpha;

            timer += Time.deltaTime;
            yield return null;
        }

        materialCopy.SetColor("_Color", new Color(splashColor.r, splashColor.g, splashColor.b, 0f));

        yield return new WaitForSeconds(0f);

        Destroy(gameObject);
    }
}
