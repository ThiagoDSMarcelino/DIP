using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

public static class ImageTranform
{
    public static (Bitmap bmp, float[] img) Morfology((Bitmap bmp, float[] img) t, float[] kernel, bool erosion = true)
    {
        int N = (int)MathF.Sqrt(kernel.Length),
            mid = N / 2;
        
        if (N % 1 != 0 || N % 2 == 0)
            throw new InvalidParameterSizeException();
        
        float[] originalImg = t.img,
                result = new float[t.img.Length];

        for (int i = 0; i < result.Length; i++)
        {
            bool match = erosion;
            int x = i % t.bmp.Width,
                y = i / t.bmp.Width;
            
            for (int j = 0; j < kernel.Length; j++)
            {
                if (kernel[j] == 0f)
                    continue;
                
                int kX = j % N,
                    kY = j / N;

                int target = (x + kX - mid) + (y + kY - mid) * t.bmp.Width;
                
                if (target < 0 || target >= t.img.Length)
                    continue;

                if (erosion) match &= originalImg[target] == 0f;
                else match |= originalImg[target] == 0f;
            }

            result[i] = match ? 0f : 1f;
        }

        var imgBytes = DiscretGray(result);
        Img(t.bmp, imgBytes);

        return (t.bmp, result);
    }
    public static (Bitmap bmp, float[] img) Open(string path)
    {
        var bmp = Bitmap.FromFile("images/" + path) as Bitmap;
        var byteArray = Bytes(bmp);
        var dataCont = Continuous(byteArray);
        var gray = GrayScale(dataCont);
        return (bmp, gray);
    }
    public static void Inverse((Bitmap bmp, float[] img) t)
    {
        for (int i = 0; i < t.img.Length; i++)
            t.img[i] = 1f - t.img[i];
    }
    public static void Show((Bitmap bmp, float[] gray) t)
    {
        var bytes = DiscretGray(t.gray);
        var image = Img(t.bmp, bytes);
        ShowBmp(image);
    }
    public static float[] GrayScale(float[] img)
    {
        float[] result = new float[img.Length / 3];
        
        for (int i = 0, j = 0; i < img.Length; i += 3, j++)
        {
            result[j] = 0.1f * img[i] + 
                0.59f * img[i + 1] +
                0.3f * img[i + 2];
        }

        return result;
    }
    public static float[] Continuous(byte[] img)
    {
        var result = new float[img.Length];
        
        for (int i = 0; i < img.Length; i++)
            result[i] = img[i] / 255f;

        return result;
    }
    public static byte[] Discret(float[] img)
    {
        var result = new byte[img.Length];
        
        for (int i = 0; i < img.Length; i++)
            result[i] = (byte)(255 * img[i]);

        return result;
    }
    public static byte[] DiscretGray(float[] img)
    {
        var result = new byte[3 * img.Length];
        
        for (int i = 0; i < img.Length; i++)
        {
            var value = (byte)(255 * img[i]);
            result[3 * i] = value;
            result[3 * i + 1] = value;
            result[3 * i + 2] = value;
        }

        return result;
    }
    public static byte[] Bytes(Image img)
    {
        var bmp = img as Bitmap;
        var data = bmp.LockBits(
            new Rectangle(0, 0, img.Width, img.Height),
            ImageLockMode.ReadOnly,
            PixelFormat.Format24bppRgb);
        
        byte[] byteArray = new byte[3 * data.Width * data.Height];

        byte[] temp = new byte[data.Stride * data.Height];
        Marshal.Copy(data.Scan0, temp, 0, temp.Length);

        for (int j = 0; j < data.Height; j++)
        {
            for (int i = 0; i < 3 * data.Width; i++)
            {
                byteArray[i + j * 3 * data.Width] =
                    temp[i + j * data.Stride];
            }
        }

        bmp.UnlockBits(data);

        return byteArray;
    }
    public static Image Img(Image img, byte[] bytes)
    {
        var bmp = img as Bitmap;
        var data = bmp.LockBits(
            new Rectangle(0, 0, img.Width, img.Height),
            ImageLockMode.ReadWrite,
            PixelFormat.Format24bppRgb);
        
        byte[] temp = new byte[data.Stride * data.Height];
        
        for (int j = 0; j < data.Height; j++)
        {
            for (int i = 0; i < 3 * data.Width; i++)
            {
                temp[i + j * data.Stride] =
                    bytes[i + j * 3 * data.Width];
            }
        }
        
        Marshal.Copy(temp, 0, data.Scan0, temp.Length);

        bmp.UnlockBits(data);
        return img;
    }
    public static void ShowBmp(Image img)
    {
        ApplicationConfiguration.Initialize();

        Form form = new Form();

        PictureBox pb = new PictureBox();
        pb.Dock = DockStyle.Fill;
        pb.SizeMode = PictureBoxSizeMode.Zoom;
        form.Controls.Add(pb);

        form.WindowState = FormWindowState.Maximized;
        form.FormBorderStyle = FormBorderStyle.None;

        form.Load += delegate
        {
            pb.Image = img;
        };

        form.KeyDown += (o, e) =>
        {
            if (e.KeyCode == Keys.Escape)
            {
                Application.Exit();
            }
        };

        Application.Run(form);
    }
}