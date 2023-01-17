using System;
using System.Linq;
using System.Drawing;
using System.Numerics;

public static class TranformImage
{
    public static (Bitmap bmp, float[] img) Affine(
        (Bitmap bmp, float[] img) t,
        Matrix4x4 mat)
    {
        float[] p = new float[]
        {
            mat.M11, mat.M12, mat.M13,
            mat.M21, mat.M22, mat.M23,
            mat.M31, mat.M32, mat.M33,
        };
        
        if (p.Length == 6)
            p = p.Concat(new float[] { 0f, 0f, 1f }).ToArray();
        else if (p.Length != 9)
            throw new Exception("Erro"); // TODO: Create error
        
        int wid = t.bmp.Width,
            hei = t.bmp.Height;

        float[] _img = t.img,
            result = new float[_img.Length];

        for (int j = 0; j < hei; j++)
        {
            for (int i = 0; i < wid; i++)
            {
                int X = (int)(p[0] * i + p[1] * j + p[2]),
                    Y = (int)(p[3] * i + p[4] * j + p[5]),
                    index = X + Y * wid;
                
                if (X < 0 || Y < 0 || X >= wid || Y >= hei)
                    continue;

                result[index] = _img[i + j * wid];
            }
        }

        var imgBytes = ImageTranform.DiscretGray(result);
        ImageTranform.Img(t.bmp, imgBytes);

        return (t.bmp, result);
    }
}