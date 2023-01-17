using System.Collections.Generic;
using System.Drawing;
using System.Linq;

public class K_means
{
    public K_means(int N = 255)
    {
        Color[] colors = new Color[N];
        
        for (int i = 0; i < N; i++)
            colors[i] = new Color();

        this.colorPalette = colors;
    }
    public K_means(IEnumerable<Color> colors) =>
        this.colorPalette = colors.ToArray();

    public Color[] colorPalette { get; private set; }

    public void Fit(Color[] target, int threshold = 5)
    {
        int change = int.MaxValue;
        while (change > 5)
            change = epoch(target);
    }
    private int epoch(Color[] target)
    {
        long[] counts = new long[this.colorPalette.Length],
               blues = new long[this.colorPalette.Length],
               greens = new long[this.colorPalette.Length],
               reds = new long[this.colorPalette.Length];
        
        var paletteIndex = this.colorPalette.Select((color, index) => (color, index));


        foreach (Color pixel in target)
        {
            var newColor = paletteIndex.MinBy(color => pixel.Distance(color.color));
            int i = newColor.index;
            byte[] bgr = pixel.BGR;

            counts[i]++;
            blues[i] += bgr[0];
            greens[i] += bgr[1];
            reds[i] += bgr[2];
        }
        
        int maxDisplacement = int.MinValue;

        for (int i = 0; i < this.colorPalette.Length; i++)
        {
            if (counts[i] == 0)
                continue;

            var temp = this.colorPalette[i];
            byte b = (byte)(blues[i] / counts[i]),
                g = (byte)(greens[i] / counts[i]),
                r = (byte)(reds[i] / counts[i]);
            this.colorPalette[i] = new Color(b, g, r);
            
            int displacement = temp.Distance(this.colorPalette[i]);
            maxDisplacement = maxDisplacement < displacement ? displacement : maxDisplacement;
        }

        return maxDisplacement;
    }

    public byte[] CompressImage(Bitmap bmp)
    {
        byte[] byteArray = ImageTranform.Bytes(bmp),
               file = new byte[
                    3 + 1 + byteArray.Length
                ];

        file[0] = (byte)'S';
        file[1] = (byte)'L';
        file[2] = (byte)'A';
        file[3] = (byte)byteArray.Length;

        var sla = this.colorPalette.Select((color, index) => (color, index));

        for (int i = 0; i < byteArray.Length; i+=3)
        {
            Color color = new Color(byteArray[i], byteArray[i+1], byteArray[i+2]);
            var indxColor = sla.MinBy(t => t.color.Distance(color));
            file[i+4] = (byte)indxColor.index;
        }

        return file;
    }
}