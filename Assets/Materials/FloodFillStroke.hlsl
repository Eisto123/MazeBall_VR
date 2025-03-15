# ifndef MYHLSLINCLUDE_INCLUDED
# define MYHLSLINCLUDE_INCLUDED



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
float sample_brush(UnityTexture2D brushTex,UnityTexture2D alphaTex, float2 brush_alpha,float2 brush_uv){
    // brush_alpha/36
    float alpha_stepped = SteppedLinearInterpolation(brush_alpha,0.0,36.0,0.0,36.0,36.0);
    float offset_x = alpha_stepped*0.167;
    float offset_y = SteppedLinearInterpolation(alpha_stepped,0.0,36.0,0.0,0.835,5.0);
    
    float colorA = SAMPLE_TEXTURE2D_LOD(alphaTex.tex, alphaTex.samplerstate, brush_uv, 0);
    float colorB = SAMPLE_TEXTURE2D_LOD(brushTex.tex, brushTex.samplerstate, brush_uv*0.167+float2(offset_x,offset_y), 0);

    return colorA*colorB;
}

// 笔画生成
void FloodFillStroke_float(UnityTexture2D brushTex,UnityTexture2D alphaTex, float2 uv,float scale, float random_seed, float stroke_size_variation,
    out float2 texture_vector, out float stroke_alpha){
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

    float brush_color = sample_brush(brushTex,alphaTex, brush_alpha,brush_uv);

    if(voronoi2.w < 0.059){
        brush_color *= voronoi2.w * 0.059;
    }
    stroke_alpha = brush_color;

    
}

# endif //MYHLSLINCLUDE_INCLUDED