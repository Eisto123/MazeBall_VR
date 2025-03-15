Shader "Unlit/OilPaintingShaderOutline"
{
    Properties
    {
        _BaseColor ("主颜色", Color) = (1,1,1,1)
        _MainTex ("颜色贴图", 2D) = "white" {}
        _BrushTex ("笔刷贴图", 2D) = "white" {}
        _AlphaTex ("笔刷透明度贴图", 2D) = "white" {}
        _StrokeHSV ("笔刷HSV", Vector) = (0.48,2.0,1.3)
        
        _RandomSeed ("笔刷分布随机种子",Range(-1000,1000)) = 0
        _StrokeSizeVariation ("笔刷大小",float) = 0.3
        _NoiseScale ("噪声大小",float) = 6.0
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
        LOD 100

        Pass
        {

            // 关闭深度写入
            ZWrite Off
            // 开启混合模式，并设置混合因子为SrcAlpha和OneMinusSrcAlpha
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _BrushTex;
            sampler2D _AlphaTex;
            float4 _BaseColor;
            float3 _StrokeHSV;
            int _RandomSeed;
            float _StrokeSizeVariation;
            float _NoiseScale;


            // 伪随机数生成函数
            float rand(float2 co){
                return frac(sin(dot(co.xy ,float2(12.9898,78.233))) * 43758.5453);
            }

            // 2D Voronoi函数，返回最近的特征点的位置和距离
            float4 voronoi(float2 uv, float cellCount) {
                uv*= cellCount;
                float2 i_st = floor(uv);
                float2 f_st = frac(uv);

                float min_dist = 1.0;
                float second_min_dist = 1.0;
                float2 nearest_point = float2(0, 0);

                // 遍历周围的9个格子
                for (int y = -1; y <= 1; y++) {
                    for (int x = -1; x <= 1; x++) {
                        float2 neighbor = float2(float(x), float(y));
                        float2 point1 = rand(i_st + neighbor) + neighbor;
                        float dist = distance(f_st, point1);

                        if (dist < min_dist) {
                            second_min_dist = min_dist;
                            min_dist = dist;
                            nearest_point = point1 + i_st;
                        } else if (dist < second_min_dist) {
                            second_min_dist = dist;
                        }
                    }
                }

                // 计算到边缘的距离
                float dist_to_edge = (second_min_dist - min_dist) * 0.5;

                return float4(nearest_point/cellCount, min_dist, dist_to_edge);
            }

            // 2D Voronoi函数，返回颜色
            float3 voronoiColor(float2 uv, float cellCount) {
                uv*= cellCount;
                float2 i_st = floor(uv);
                float2 f_st = frac(uv);

                float min_dist = 1.0;
                float2 nearest_point = float2(0, 0);

                // 遍历周围的9个格子
                for (int y = -1; y <= 1; y++) {
                    for (int x = -1; x <= 1; x++) {
                        float2 neighbor = float2(float(x), float(y));
                        float2 point1 = rand(i_st + neighbor) + neighbor;
                        float dist = distance(f_st, point1);

                        if (dist < min_dist) {
                            min_dist = dist;
                            nearest_point = point1 + i_st;
                        }
                    }
                }

                // 为每个特征点生成随机颜色
                float3 color = float3(rand(nearest_point), rand(nearest_point + 1.0), rand(nearest_point + 2.0));

                return color;
            }


            // 构建绕X轴旋转的矩阵
            float3x3 RotationMatrixX(float angle) {
                float rad = radians(angle);
                float c = cos(rad);
                float s = sin(rad);
                return float3x3(
                    1, 0, 0,
                    0, c, -s,
                    0, s, c
                );
            }

            // 构建绕Y轴旋转的矩阵
            float3x3 RotationMatrixY(float angle) {
                float rad = radians(angle);
                float c = cos(rad);
                float s = sin(rad);
                return float3x3(
                    c, 0, s,
                    0, 1, 0,
                    -s, 0, c
                );
            }

            // 构建绕Z轴旋转的矩阵
            float3x3 RotationMatrixZ(float angle) {
                float rad = radians(angle);
                float c = cos(rad);
                float s = sin(rad);
                return float3x3(
                    c, -s, 0,
                    s, c, 0,
                    0, 0, 1
                );
            }

            // 对输入的float3依次进行缩放、旋转、移动的函数
            float3 transform3d(float3 pos,float3 offset,float3 rotation,float3 scale) {
                // 缩放操作
                float3 scaled = pos * scale;

                // 旋转操作，按X、Y、Z轴顺序旋转
                float3x3 rotationX = RotationMatrixX(rotation.x);
                float3x3 rotationY = RotationMatrixY(rotation.y);
                float3x3 rotationZ = RotationMatrixZ(rotation.z);
                float3x3 rotationMatrix = mul(rotationZ, mul(rotationY, rotationX));
                float3 rotated = mul(rotationMatrix, scaled);

                // 移动操作
                float3 translated = rotated + offset;

                return translated;
            }

            
            // 映射范围函数
            float Mapping(float input, float fromMin, float fromMax, float toMin, float toMax) {
                // 先将输入值从原范围归一化到 [0, 1] 范围
                float normalizedInput = (input - fromMin) / (fromMax - fromMin);
                // 进行线性插值
                return lerp(toMin, toMax, normalizedInput);
            }

            // 阶梯线性插值函数
            float SteppedLinearInterpolation(float input, float fromMin, float fromMax, float toMin, float toMax, float stepCount) {
                // 先将输入值从原范围归一化到 [0, 1] 范围
                float normalizedInput = (input - fromMin) / (fromMax - fromMin);
                // 计算每个阶梯的宽度
                float stepWidth = 1.0 / stepCount;
                // 确定输入值所在的阶梯
                float stepIndex = floor(normalizedInput / stepWidth);
                // 计算当前阶梯的起始值
                float stepStart = stepIndex * stepWidth;
                // 进行线性插值
                return lerp(toMin, toMax, stepStart);
            }

            // 笔刷贴图采样
            float sample_brush(float2 brush_alpha,float2 brush_uv){
                // brush_alpha/36
                float alpha_stepped = SteppedLinearInterpolation(brush_alpha,0.0,36.0,0.0,36.0,36.0);
                float offset_x = alpha_stepped*0.167;
                float offset_y = SteppedLinearInterpolation(alpha_stepped,0.0,36.0,0.0,0.835,5.0);
                
                float colorA = tex2D(_AlphaTex, brush_uv);
                float colorB = tex2D(_BrushTex, brush_uv*0.167+float2(offset_x,offset_y));

                return colorA*colorB;
            }

            // 笔画生成
            void flood_fill_stroke(float2 uv,float scale, float random_seed, float stroke_size_variation,
                out float2 texture_vector, out float stroke_aplha){
                // 随机移动UV坐标
                uv += random_seed;
                // 缩放UV坐标
                float2 uv_scaled = uv*scale;
                float3 voronoi_color1 = voronoiColor(uv, scale);
                float4 voronoi1 = voronoi(uv, scale);

                texture_vector = voronoi1.xy;

                float brush_alpha = voronoi_color1.r * 36.0;
                float3 cell_rotation =  float3(0.0,0.0,voronoi_color1.g * 360.0);

                float cell_scale_max = stroke_size_variation*2.5+0.5;
                float3 cell_scale = voronoi_color1.b*(cell_scale_max-0.5)+0.5;
                

                float4 voronoi2 = voronoi(uv_scaled, 1.0);
                float2 cell_uv = voronoi2.xy-uv_scaled;
                
                float2 brush_uv = transform3d(float3(cell_uv,0.0),float3(0.5,0.5,0.0),cell_rotation,cell_scale);

                float brush_color = sample_brush(brush_alpha,brush_uv);

                if(voronoi2.w < 0.059){
                    brush_color *= voronoi2.w * 0.059;
                }
                stroke_aplha = brush_color;

            }


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
            float3 AdjustHSV(float3 rgb, float hue, float saturation, float value) {
                float3 hsv = RGBToHSV(rgb);
                // 计算实际的色相偏移量，满足 0(-180°) 到 1(+180°) 且 0.5 不偏移
                float hueOffset = hue - 0.5; 
                hsv.x = fmod(hsv.x + hueOffset, 1);
                if (hsv.x < 0) {
                    hsv.x += 1;
                }
                // 处理饱和度
                if (saturation == 0) {
                    // 饱和度为 0 时，转换为灰度图像
                    float gray = dot(rgb, float3(0.299, 0.587, 0.114));
                    hsv.y = 0;
                    hsv.z = gray;
                } else {
                    hsv.y = saturate(hsv.y * saturation);
                }
                // 处理明度
                hsv.z = saturate(hsv.z * value);

                return HSVToRGB(hsv);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float3 color_screen(float3 colorA,float3 colorB){
                return 1.0-(1.0-colorA)*(1.0-colorB);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 texture_vector1;
                float stroke_aplha1;
                flood_fill_stroke(i.uv,_NoiseScale,_RandomSeed,_StrokeSizeVariation,texture_vector1,stroke_aplha1);
                float4 colorA = float4(1.0,1.0,1.0,0.0);
                float3 colorB = tex2D(_MainTex,texture_vector1)*_BaseColor.rgb;
                colorB = AdjustHSV(colorB,_StrokeHSV.x,_StrokeHSV.y,_StrokeHSV.z);
                fixed4 col = lerp(colorA,float4(colorB,1.0),stroke_aplha1);
                return col;
            }
            ENDCG
        }
    }
}
