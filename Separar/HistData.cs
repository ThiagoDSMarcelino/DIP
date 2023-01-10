using System.Linq;
using System.Drawing;

public static class HistData
{
    
    public static void Otsu((Bitmap bmp, float[] img) t, float db = 0.05f)
    {
        var histogram = Hist(t.img, db);
        int threshold = 0;

        float Ex0 = 0;
        float Ex1 = t.img.Average();
        float Dx0 = 0;
        float Dx1 = t.img.Sum(x => x * x);
        int N0 = 0;
        int N1 = t.img.Length;

        float minstddev = float.PositiveInfinity;

        for (int i = 0; i < histogram.Length; i++)
        {
            float value = db * (2 * i + 1) / 2;
            float s = histogram[i] * value;

            if (N0 == 0 && histogram[i] == 0)
                continue;

            Ex0 = (Ex0 * N0 + s) / (N0 + histogram[i]);
            Ex1 = (Ex1 * N1 - s) / (N1 - histogram[i]);

            N0 += histogram[i];
            N1 -= histogram[i];

            Dx0 += value * value * histogram[i];
            Dx1 -= value * value * histogram[i];

            float stddev =
                Dx0 - N0 * Ex0 * Ex0 + 
                Dx1 - N1 * Ex1 * Ex1;
            
            if (float.IsInfinity(stddev) ||
                float.IsNaN(stddev))
                continue;
            
            if (stddev < minstddev)
            {
                minstddev = stddev;
                threshold = i;
            }
        }
        float bestTreshold = db * (2 * threshold + 1) / 2;

        Tresh(t, bestTreshold);
    }

    public static void Tresh((Bitmap bmp, float[] img) t, 
        float threshold = 0.5f)
    {
        for (int i = 0; i < t.img.Length; i++)
            t.img[i] = t.img[i] > threshold ? 1f : 0f;
    }

    public static float[] Equalization(
        (Bitmap bmp, float[] img) t,
        float threshold = 0f,
        float db = 0.05f)
    {
        int[] histogram = Hist(t.img, db);

        int dropCount = (int)(t.img.Length * threshold);
        
        float min = 0;
        int droped = 0;
        for (int i = 0; i < histogram.Length; i++)
        {
            droped += histogram[i];
            if (droped > dropCount)
            {
                min = i * db;
                break;
            }
        }

        float max = 0;
        droped = 0;
        for (int i = histogram.Length - 1; i > -1; i--)
        {
            droped += histogram[i];
            if (droped > dropCount)
            {
                max = i * db;
                break;
            }
        }

        var r = 1 / (max - min);
        
        for (int i = 0; i < t.img.Length; i++)
        {
            float newValue = (t.img[i] - min) * r;
            if (newValue > 1f)
                newValue = 1f;
            else if (newValue < 0f)
                newValue = 0f;
            t.img[i] = newValue;
        }
        
        return t.img;
    }
    public static void ShowHist((Bitmap bmp, float[] img) t, float db = 0.05f)
    {
        var histogram = Hist(t.img, db);
        var histImg = DrawHist(histogram);
        ImageTranform.ShowBmp(histImg);
    }
    public static Image DrawHist(int[] hist)
    {
        var bmp = new Bitmap(512, 256);
        var g = Graphics.FromImage(bmp);
        float margin = 16;

        int max = hist.Max();
        float barlen = (bmp.Width - 2 * margin) / hist.Length;
        float r = (bmp.Height - 2 * margin) / max;

        for (int i = 0; i < hist.Length; i++)
        {
            float bar = hist[i] * r;
            g.FillRectangle(Brushes.Black, 
                margin + i * barlen,
                bmp.Height - margin - bar, 
                barlen,
                bar);
            g.DrawRectangle(Pens.DarkBlue, 
                margin + i * barlen,
                bmp.Height - margin - bar, 
                barlen,
                bar);
        }

        return bmp;
    }
    public static int[] Hist(float[] img, float db = 0.05f)
    {
        int histogramLen = (int)(1 / db) + 1;
        int[] histogram = new int[histogramLen];

        foreach (var pixel in img)
            histogram[(int)(pixel / db)]++;
        
        return histogram;
    }
}