using UnityEngine;

public class HoverGlow : MonoBehaviour
{
    private Renderer rend;
    private Material material;
    public float defaultPower = 10f;  // Ĭ��״̬�������⣩
    public float hoverPower = 5f;     // �����ͣʱ����΢���⣩
    public float selectedPower = 0f;  // ѡ��ʱ����ȫ���⣩
    private string glowProperty = "_FresnelPower"; // ȷ��ƥ�� Shader Graph ������
    private bool isSelected = false; // ��¼�Ƿ�ѡ��

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend != null)
        {
            material = rend.material;
        }
    }

    void OnMouseEnter()
    {
        if (!isSelected && material != null && material.HasProperty(glowProperty))
        {
            material.SetFloat(glowProperty, hoverPower); // �����ͣʱ���� FresnelPower�����������
        }
    }

    void OnMouseExit()
    {
        if (!isSelected && material != null && material.HasProperty(glowProperty))
        {
            material.SetFloat(glowProperty, defaultPower); // ����뿪���ָ�Ĭ�ϣ������⣩
        }
    }

    void OnMouseDown()
    {
        if (material != null && material.HasProperty(glowProperty))
        {
            isSelected = !isSelected; // ������л�ѡ��״̬
            material.SetFloat(glowProperty, isSelected ? selectedPower : defaultPower);
        }
    }
}
