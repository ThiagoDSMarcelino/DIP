using System.Linq;
using System.Drawing;
using System.Collections.Generic;

public static class RectsangleData
{
    public static List<Rectangle> Segmentation((Bitmap bmp, float[] img) t)
    {
        var rects = SegmentationT(t, 0);
        var areas = rects.Select(r => r.Width * r.Height);
        var average = areas.Average();
        
        return rects
            .Where(r => r.Width * r.Height > average)
            .ToList();
    }

    public static List<Rectangle> SegmentationT((Bitmap bmp, float[] img) t, int threshold)
    {
        List<Rectangle> list = new List<Rectangle>();
        Stack<int> stack = new Stack<int>();

        float[] img = t.img;
        int wid = t.bmp.Width;
        float crr = 0.01f;

        int minx, maxx, miny, maxy;
        int count = 0;

        for (int i = 0; i < img.Length; i++)
        {
            if (img[i] > 0f)
                continue;
            
            minx = int.MaxValue;
            miny = int.MaxValue;
            maxx = int.MinValue;
            maxy = int.MinValue;
            count = 0;
            stack.Push(i);

            while (stack.Count > 0)
            {
                int j = stack.Pop();

                if (j < 0 || j >= img.Length)
                    continue;
                
                if (img[j] > 0f)
                    continue;

                int x = j % wid,
                    y = j / wid;
                
                if (x < minx)
                    minx = x;
                if (x > maxx)
                    maxx = x;
                
                if (y < miny)
                    miny = y;
                if (y > maxy)
                    maxy = y;
                
                img[j] = crr;
                count++;

                stack.Push(j - 1);
                stack.Push(j + 1);
                stack.Push(j + wid);
                stack.Push(j - wid);
            }

            crr += 0.01f;
            if (count < threshold)
                continue;

            Rectangle rect = new Rectangle(
                minx, miny, maxx - minx, maxy - miny
            );
            list.Add(rect);
        }

        return list;
    }
    public static void ShowRects((Bitmap bmp, float[] img) t, List<Rectangle> list)
    {
        var g = Graphics.FromImage(t.bmp);

        foreach (var rect in list)
            g.DrawRectangle(Pens.Red, rect);
        
        ImageTranform.ShowBmp(t.bmp);
    }
}