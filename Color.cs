using System;

public class Color
{
    public Color()
    {
        this.BGR = new byte[3];
        Random.Shared.NextBytes(this.BGR);
    }
    public Color(params byte[] bgr) =>
        this.BGR = bgr;
    public byte[] BGR { get; private set; }

    public int Distance(Color target)
    {
        int result = 0;
        
        for (int i = 0; i < 3; i++)
        {
            int drgb = this.BGR[i] - target.BGR[i];
            result += drgb * drgb;
        }
        
        return result;
    }

    public float Distance(params byte[] target)
    {
        float result = 0f;
        
        for (int i = 0; i < 3; i++)
        {
            float drgb = this.BGR[i] - target[i];
            result += drgb * drgb;
        }
        
        return result;
    }
}