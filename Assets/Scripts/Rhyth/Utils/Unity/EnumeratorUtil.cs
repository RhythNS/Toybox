using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class EnumeratorUtil
{
    public static IEnumerator Multiple(params IEnumerator[] enumerators)
    {
        foreach (IEnumerator enumerator in enumerators)
            yield return enumerator;
    }

    public static IEnumerator MoveTo(Transform transform, Vector3 from, Vector3 to, float timeToFinish)
    {
        float percentage = 0, timer = 0;
        while (percentage < 1)
        {
            timer += Time.deltaTime;
            percentage = timer / timeToFinish;
            transform.position = Vector3.Lerp(from, to, percentage);
            yield return null;
        }
    }

    public static IEnumerator RotateTo(Transform transform, Quaternion from, Quaternion to, float timeToFinish)
    {
        float percentage = 0, timer = 0;
        while (percentage < 1)
        {
            timer += Time.deltaTime;
            percentage = timer / timeToFinish;
            transform.rotation = Quaternion.Lerp(from, to, percentage);
            yield return null;
        }
    }

    public static IEnumerator FadeCharacters(Color fadeTo, float timeToFinish, params SpriteRenderer[] renderers)
    {
        float percentage = 0, timer = 0;
        Color[] prevColors = new Color[renderers.Length];
        for (int i = 0; i < prevColors.Length; i++)
            prevColors[i] = renderers[i].color;
        while (percentage < 1)
        {
            timer += Time.deltaTime;
            percentage = timer / timeToFinish;
            for (int i = 0; i < renderers.Length; i++)
                renderers[i].color = Color.Lerp(prevColors[i], fadeTo, percentage);
            yield return null;
        }
    }

    public static IEnumerator FadeImage(Color fadeTo, float timeToFinish, params Image[] images)
    {
        float percentage = 0, timer = 0;
        Color[] prevColors = new Color[images.Length];
        for (int i = 0; i < prevColors.Length; i++)
            prevColors[i] = images[i].color;
        while (percentage < 1)
        {
            timer += Time.deltaTime;
            percentage = timer / timeToFinish;
            for (int i = 0; i < images.Length; i++)
                images[i].color = Color.Lerp(prevColors[i], fadeTo, percentage);
            yield return null;
        }
    }

    public struct Movement
    {
        public Transform toMove;
        public Vector3 from, to;
    }

    public static IEnumerator MoveToMultiple(float timeToFinish, params Movement[] movements)
    {
        float percentage = 0, timer = 0;
        while (percentage < 1)
        {
            timer += Time.deltaTime;
            percentage = timer / timeToFinish;
            foreach (Movement movement in movements)
                movement.toMove.position = Vector3.Lerp(movement.from, movement.to, percentage);

            yield return null;
        }
    }

    public static IEnumerator ChangeRuntimeAnimatorController(float timeToFinish, Animator animator, RuntimeAnimatorController runtimeAnimatorController)
    {
        yield return new WaitForSeconds(timeToFinish);
        animator.runtimeAnimatorController = runtimeAnimatorController;
    }

    public static IEnumerator EnableAfterSeconds(float timeToFinish, GameObject gameObject, bool shouldEnable)
    {
        yield return new WaitForSeconds(timeToFinish);
        gameObject.SetActive(shouldEnable);
    }
}
