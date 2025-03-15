using System.Collections;
using UnityEngine;

public class SequentialAppear : MonoBehaviour
{
    public GameObject[] objectsToAppear; // ��Ҫ������ʾ����������
    public float delayBetweenAppearance = 0.5f; // ÿ��������ֵ�ʱ����
    public float dissolveDuration = 1f; // �ܽ����ʱ��
    public float timeBeforeDissolve = 2f; // ������ȫ���ֺ�ͣ����ʱ��

    private string dissolveProperty = "_Distance";

    void Start()
    {
        foreach (GameObject obj in objectsToAppear)
        {
            obj.SetActive(false);
        }

    }

    public void AppearFootprints(){
        StartCoroutine(ManageObjects());
    }

    IEnumerator ManageObjects()
    {
        foreach (GameObject obj in objectsToAppear)
        {
            obj.SetActive(true);


            StartCoroutine(DissolveEffect(obj, false));


            yield return new WaitForSeconds(delayBetweenAppearance);



            StartCoroutine(DissolveAndHide(obj));
        }
    }

    IEnumerator DissolveEffect(GameObject obj, bool isDissolving)
    {
        Renderer rend = obj.GetComponent<Renderer>();
        if (rend != null)
        {
            Material mat = rend.material;
            if (mat.HasProperty(dissolveProperty))
            {
                float startValue = isDissolving ? 20f : 0f;
                float targetValue = isDissolving ? 0f : 20f;
                float elapsedTime = 0f;

                while (elapsedTime < dissolveDuration)
                {
                    elapsedTime += Time.deltaTime;
                    float currentValue = Mathf.Lerp(startValue, targetValue, elapsedTime / dissolveDuration);
                    mat.SetFloat(dissolveProperty, currentValue);
                    yield return null;
                }

                mat.SetFloat(dissolveProperty, targetValue);
            }
        }
    }

    IEnumerator DissolveAndHide(GameObject obj)
    {
        yield return new WaitForSeconds(timeBeforeDissolve);
        StartCoroutine(DissolveEffect(obj, true));
        yield return new WaitForSeconds(dissolveDuration);
        obj.SetActive(false);
    }
}
