# ifndef MYHLSLINCLUDE_INCLUDED
# define MYHLSLINCLUDE_INCLUDED

// RGB 转 HSV 函数
float3 RGBToHSV(float3 rgb) {
    float Cmax = max(rgb.r, max(rgb.g, rgb.b));
    float Cmin = min(rgb.r, min(rgb.g, rgb.b));
    float delta = Cmax - Cmin;

    float hue = 0;
    if (delta != 0) {
        if (Cmax == rgb.r) {
            hue = fmod((rgb.g - rgb.b) / delta, 6);
        } else if (Cmax == rgb.g) {
            hue = ((rgb.b - rgb.r) / delta) + 2;
        } else {
            hue = ((rgb.r - rgb.g) / delta) + 4;
        }
        hue = hue / 6;
    }

    float saturation = (Cmax != 0) ? (delta / Cmax) : 0;
    float value = Cmax;

    return float3(hue, saturation, value);
}

// HSV 转 RGB 函数
float3 HSVToRGB(float3 hsv) {
    float r = 0, g = 0, b = 0;
    float c = hsv.z * hsv.y;
    float x = c * (1 - abs(fmod(hsv.x * 6, 2) - 1));
    float m = hsv.z - c;

    if (hsv.x < 1.0 / 6.0) {
        r = c;
        g = x;
        b = 0;
    } else if (hsv.x < 2.0 / 6.0) {
        r = x;
        g = c;
        b = 0;
    } else if (hsv.x < 3.0 / 6.0) {
        r = 0;
        g = c;
        b = x;
    } else if (hsv.x < 4.0 / 6.0) {
        r = 0;
        g = x;
        b = c;
    } else if (hsv.x < 5.0 / 6.0) {
        r = x;
        g = 0;
        b = c;
    } else {
        r = c;
        g = 0;
        b = x;
    }

    r += m;
    g += m;
    b += m;

    return float3(r, g, b);
}

// 调整色相、饱和度、明度的函数
void AdjustHSV_float(float4 in_color, float hue, float saturation, float value, out float4 out_color) {
    float3 hsv = RGBToHSV(in_color.rgb);
    // 计算实际的色相偏移量，满足 0(-180°) 到 1(+180°) 且 0.5 不偏移
    float hueOffset = hue - 0.5; 
    hsv.x = fmod(hsv.x + hueOffset, 1);
    if (hsv.x < 0) {
        hsv.x += 1;
    }
    // 处理饱和度
    if (saturation == 0) {
        // 饱和度为 0 时，转换为灰度图像
        float gray = dot(in_color.rgb, float3(0.299, 0.587, 0.114));
        hsv.y = 0;
        hsv.z = gray;
    } else {
        hsv.y = saturate(hsv.y * saturation);
    }
    // 处理明度
    hsv.z = saturate(hsv.z * value);

    out_color.rgb = HSVToRGB(hsv);
    out_color.a = in_color.a;
}


# endif //MYHLSLINCLUDE_INCLUDED