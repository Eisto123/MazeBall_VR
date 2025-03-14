using UnityEngine;

public class HoverGlow : MonoBehaviour
{
    private Renderer rend;
    private Material material;
    public float defaultPower = 10f;  // 默认状态（不发光）
    public float hoverPower = 5f;     // 鼠标悬停时（轻微发光）
    public float selectedPower = 0f;  // 选中时（完全发光）
    private string glowProperty = "_FresnelPower"; // 确保匹配 Shader Graph 参数名
    private bool isSelected = false; // 记录是否被选中

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
            material.SetFloat(glowProperty, hoverPower); // 鼠标悬停时降低 FresnelPower，让物体变亮
        }
    }

    void OnMouseExit()
    {
        if (!isSelected && material != null && material.HasProperty(glowProperty))
        {
            material.SetFloat(glowProperty, defaultPower); // 鼠标离开，恢复默认（不发光）
        }
    }

    void OnMouseDown()
    {
        if (material != null && material.HasProperty(glowProperty))
        {
            isSelected = !isSelected; // 点击后切换选中状态
            material.SetFloat(glowProperty, isSelected ? selectedPower : defaultPower);
        }
    }
}
