var image = ImageTranform.Open("shuregui.png");
image = TranformImage.Affine(image, 0.6f, -0.8f, 0, 0.8f, 0.6f, 0f);
ImageTranform.Show(image);