using UnityEngine;

public static class RuntimeSpriteFactory
{
    private static Sprite _square;
    private static Sprite _circle;

    public static Sprite Square()
    {
        if (_square == null)
        {
            Texture2D tex = Texture2D.whiteTexture;
            _square = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
            _square.name = "RuntimeSquare";
        }
        return _square;
    }

    public static Sprite Circle()
    {
        if (_circle == null)
        {
            int size = 32;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp,
                hideFlags = HideFlags.HideAndDontSave
            };
            float r = size * 0.5f;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = x + 0.5f - r;
                    float dy = y + 0.5f - r;
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);
                    float alpha = Mathf.Clamp01((r - dist) / 1.5f);
                    tex.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
            }
            tex.Apply();
            _circle = Sprite.Create(tex, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), 100f);
            _circle.name = "RuntimeCircle";
        }
        return _circle;
    }
}
