using System;
using System.Drawing;
using System.Numerics;
public static class FilterImage
{
    public static (Bitmap bmp, float[] img) Sobel(
        (Bitmap bmp, float[] img) t)
    {
        int wid = t.bmp.Width,
            hei = t.bmp.Height;

        float[] _img = t.img,
            result = new float[_img.Length];

        float sum = 2 * _img[wid] + _img[wid + 1];
            
        for (int j = 1; j < hei - 1; j++)
        {
            for (int i = 1; i < wid - 1; i++)
            {
                int index = i + j * wid;
                sum += _img[index + 1];
                sum -= _img[index - 1];

                result[index] = sum + _img[index];
            }
        }

        float flag = _img[wid] + _img[wid + 1];

        for (int j = 1; j < hei - 1; j++)
        {
            for (int i = 1; i < wid - 1; i++)
            {
                int index = i + j * wid;
                float value = result[index] + result[index + 1];

                var sla = value - flag;
                if (sla > 1f)
                    sla = 1f;
                else if (sla < 0f)
                    sla = 0;

                result[index] = sla;

                flag = value;
            }
        }

        var imgBytes = ImageTranform.DiscretGray(result);
        ImageTranform.Img(t.bmp, imgBytes);

        return (t.bmp, result);
    }

    public static (Bitmap bmp, float[] img) Conv(
        (Bitmap bmp, float[] img) t, float[] kernel)
    {
        var N = (int)Math.Sqrt(kernel.Length);
        var wid = t.bmp.Width;
        var hei = t.bmp.Height;
        var _img = t.img;
        float[] result = new float[_img.Length];

        for (int j = N / 2; j < hei - N / 2; j++)
        {
            for (int i = N /2; i < wid - N / 2; i++)
            {
                float sum = 0;

                for (int k = 0; k < N; k++)
                {
                    for (int l = 0; l < N; l++)
                    {
                        sum += _img[i + k +(j + l) * wid] *
                            kernel[k + l * N];
                    }
                }

                result[i + j * wid] = sum;
            }
        }

        var imgBytes = ImageTranform.DiscretGray(result);
        ImageTranform.Img(t.bmp, imgBytes);

        return (t.bmp, result);
    }

    // public static float[] Rotation(float degree)
    // {
    //     float radian = degree / 180 * MathF.PI,
    //           cos = MathF.Cos(radian),
    //           sin = MathF.Sin(radian);
        
    // }
}