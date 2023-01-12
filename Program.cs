using System;
using System.Drawing;
using System.Numerics;

var image = ImageTranform.Open("sla.png");
image = Hough(image);
// image = ScaleImg(image, 3f, 3f);
// image = Bilinear(image);
ImageTranform.Show(image);

(Bitmap bmp, float[] img) Hough((Bitmap bmp, float[] img) t)
{
    int wid = t.bmp.Width,
        hei = t.bmp.Height,
        N = 1000;
    
    float[] img = t.img,
        newImg = new float[N * N];
    Bitmap newBmp = new Bitmap(N, N);

    for (int j = 0; j < hei; j++)
    {
        for (int i = 0; i < wid; i++)
        {
            int index = i + j * wid;
            if (img[index] == 1f)
                continue;
            
            float a = -i,
                b = j;
            
            for (int x = 0; x < N; x++)
            {
                int y = (int)(a * (x / 50f - 10f) + b);
                if (y < 0 || y >= 1000)
                    continue;

                newImg[x + y * N] += 0.01f;

                if (newImg[x + y * N] > 1f)
                    newImg[x + y * N] = 1f;
            }
        }
    }

    var imgBytes = ImageTranform.DiscretGray(newImg);
    var image = ImageTranform.Img(newBmp, imgBytes);
    return (image as Bitmap, newImg);
}


(Bitmap bmp, float[] img) Bilinear((Bitmap bmp, float[] img) t)
{
    float[] img = t.img,
        result = new float[img.Length];

    int wid = t.bmp.Width,
        hei = t.bmp.Height;

    for (int j = 0; j < hei; j++)
    {
        for (int i = 0; i < wid; i++)
        {
            int index = i + j * wid;
            if (img[index] > 0f || 
                i == 0 || j == 0 || 
                i == wid - 1 || j == hei - 1)
            {
                result[index] = img[index];
                continue;
            }


            

            float[] possibilities = new float[] { checkY(i, j), checkX(i, j), checkXY(i, j) };

            float choice = 0;
            foreach (float possibility in possibilities)
                choice = choice < possibility ? possibility : choice;

            result[index] = choice;
        }
    }

    byte[] imgByte = ImageTranform.DiscretGray(result);
    ImageTranform.Img(t.bmp, imgByte);

    return (t.bmp, result);


    float checkY(int x, int y)
    {
        int upperLeft = x - 1 + (y - 1) * wid,
            upperRight = x + 1 + (y - 1) * wid,
            bottomLeft = x - 1 + (y + 1) * wid,
            bottomRight = x + 1 + (y + 1) * wid;

        while (img[upperLeft] == 0 && (upperLeft - wid) > 0)
            upperLeft -= wid;
        
        while (img[upperRight] == 0 && (upperRight - wid) > 0)
            upperRight -= wid;
        
        while (img[bottomLeft] == 0 && (bottomLeft + wid) < img.Length)
            bottomLeft += wid;
        
        while (img[bottomRight] == 0 && (bottomRight + wid) < img.Length)
            bottomRight += wid;
        
        float upperMiddle = (img[upperLeft] + img[upperRight]) / 2;
        float lowerMiddle = (img[bottomLeft] + img[bottomRight]) / 2;

        return (upperMiddle + lowerMiddle) / 2;
    }

    float checkX(int i, int j)
    {
        int upperLeft = i - 1 + (j - 1) * wid,
            upperRight = i + 1 + (j - 1) * wid,
            bottomLeft = i - 1 + (j + 1) * wid,
            bottomRight = i + 1 + (j + 1) * wid;

        while (img[upperLeft] == 0 && upperLeft % wid > 0)
            upperLeft--;
        
        while (img[upperRight] == 0 && upperRight % wid > 0)
            upperRight--;
        
        while (img[bottomLeft] == 0 && bottomLeft % wid < wid - 1)
            bottomLeft++;
        
        while (img[bottomRight] == 0 && bottomRight % wid < wid - 1)
            bottomRight++;
        
        float upperMiddle = (img[upperLeft] + img[upperRight]) / 2;
        float lowerMiddle = (img[bottomLeft] + img[bottomRight]) / 2;

        return (upperMiddle + lowerMiddle) / 2;
    }

    float checkXY(int x, int y)
    {
        int upperLeft = x - 1 + (y - 1) * wid,
            upperRight = x + 1 + (y - 1) * wid,
            bottomLeft = x - 1 + (y + 1) * wid,
            bottomRight = x + 1 + (y + 1) * wid;

        while (img[upperLeft] == 0 && (upperLeft - (wid - 1)) > 0)
            upperLeft -= wid - 1;
        
        while (img[upperRight] == 0 && (upperRight - (wid + 1)) > 0)
            upperRight -= wid + 1;
        
        while (img[bottomLeft] == 0 && (bottomLeft + (wid - 1)) < img.Length)
            bottomLeft += wid - 1;
        
        while (img[bottomRight] == 0 && (bottomRight + (wid + 1)) < img.Length)
            bottomRight += wid + 1;
        
        float upperMiddle = (img[upperLeft] + img[upperRight]) / 2;
        float lowerMiddle = (img[bottomLeft] + img[bottomRight]) / 2;

        return (upperMiddle + lowerMiddle) / 2;
    }
}

(Bitmap bmp, float[] img) ScaleImg((Bitmap bmp, float[] img) t, 
    float scaleX, float scaleY)
{
    int wid = t.bmp.Width,
        hei = t.bmp.Height;
    
    Bitmap scaledBitmap = new Bitmap(
        (int)(scaleX * wid),
        (int)(scaleY * hei)
    );
    
    int widResult = scaledBitmap.Width,
        heiResult = scaledBitmap.Height;
    
    float[] img = t.img,
        scaledArray = new float[widResult * heiResult];
    
    for (int i = 0; i < wid; i++)
    {
        for (int j = 0; j < hei; j++)
        {
            scaledArray[i + j * widResult]
                = img[i + j * wid];
        }
    }
    byte[] imgByte = ImageTranform.DiscretGray(scaledArray);
    Bitmap newImg = ImageTranform.Img(scaledBitmap, imgByte) as Bitmap;
    (Bitmap, float[]) result = (newImg, scaledArray);

    result = 
        affine(result,
            scale(scaleX, scaleY));
    
    return result;
}

Matrix4x4 mat(params float[] arr)
{
    return new Matrix4x4(
        arr[0], arr[1], arr[2], 0,
        arr[3], arr[4], arr[5], 0,
        arr[6], arr[7], arr[8], 0,
             0,      0,      0, 1
    );
}

Matrix4x4 rotation(float degree)
{
    float radian = degree / 180 * MathF.PI;
    float cos = MathF.Cos(radian);
    float sin = MathF.Sin(radian);
    return mat(
        cos, -sin, 0,
        sin,  cos, 0,
          0,    0, 1
    );
}

Matrix4x4 translate(float dx, float dy)
{
    return mat(
        1, 0, dx,
        0, 1, dy,
        0, 0, 1
    );
}

Matrix4x4 translateFromSize(float dx, float dy,
    (Bitmap bmp, float[] img) t)
{
    return mat(
        1, 0, dx * t.bmp.Width,
        0, 1, dy * t.bmp.Height,
        0, 0, 1
    );
}

Matrix4x4 scale(float dx, float dy)
{
    return mat(
        dx, 0, 0,
        0, dy, 0,
        0, 0, 1
    );
}

Matrix4x4 shear(float cx, float cy)
{
    return mat(
        1, cx, 0,
        cy, 1, 0,
        0, 0, 1
    );
}

(Bitmap bmp, float[] img) affine((Bitmap bmp, float[] img) t,
    Matrix4x4 mat)
{
    float[] p = new float[]
    {
        mat.M11, mat.M12, mat.M13,
        mat.M21, mat.M22, mat.M23,
        mat.M31, mat.M32, mat.M33,
    };
    var _img = t.img;
    float[] nova = new float[_img.Length];
    int wid = t.bmp.Width;
    int hei = t.bmp.Height;
    int x = 0;
    int y = 0;
    int index = 0;

    for (int i = 0; i < wid; i++)
    {
        for (int j = 0; j < hei; j++)
        {
            x = (int)(p[0] * i + p[1] * j + p[2]);
            y = (int)(p[3] * i + p[4] * j + p[5]);

            if(x < 0 || x >= wid || y < 0 || y >= wid)
                continue;
            else
            {
                index = (int)(x + y * wid);
                nova[index] = _img[i+j * wid];
            }
        }
    }

    var Imgbytes = ImageTranform.DiscretGray(nova);
    ImageTranform.Img(t.bmp, Imgbytes);

    return (t.bmp, nova);
}