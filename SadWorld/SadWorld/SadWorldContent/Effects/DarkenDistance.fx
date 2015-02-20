texture TileTexture;

float2 LightPosition : VPOS;
float2 TilePosition : VPOS;

sampler TextureSampler = sampler_state
{
    Texture = <TileTexture>;
};

float4 PixelShaderFunction(float2 TextureCoordinate : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(TextureSampler, TextureCoordinate);
    float x_dist;
    float y_dist;
    float distance;
    // Calculate distance formula (pythagorean - a2 = b2 + c2)
    x_dist = abs(LightPosition.x - ((TextureCoordinate.x * 40) + TilePosition.x));
	y_dist = abs(LightPosition.y - ((TextureCoordinate.y * 30) + TilePosition.y));
    distance = pow(x_dist, 2) + pow(y_dist, 2);
    distance = sqrt(distance);
    color.r = color.r - distance * 0.01;
    color.g = color.g - distance * 0.01;
    color.b = color.b - distance * 0.01;
	                                 
    return color;
}


technique Lighting
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}