using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace simpleImageProcessing
{
    class ImageProcessor
    {
        private float[,] imageData;

        private static float[,] edgeDetectionMask = new float[3, 3] { 
                                { -1, -1, -1 },
                                { -1,  8, -1 },
                                { -1, -1, -1 }
        }; 

        public ImageProcessor(string imageUrl)
        {
            LoadImage(imageUrl);
        }

        private void LoadImage(string imageUrl)
        {
            Bitmap image = new Bitmap(imageUrl);
            imageData = BitmapToArray(image);
        }

        private float[,] ConvolveWithoutPadding(float[,] imageData, float[,] kernel)
        {
            float[,] convolvedImage = new float[imageData.GetLength(0), imageData.GetLength(1)];

            for(int imageDataX = 1; imageDataX<imageData.GetLength(0)-1; imageDataX++)
            {
                for(int imageDataY = 1; imageDataY < imageData.GetLength(1)-1; imageDataY++)
                {

                    for (int kernelX = -1; kernelX <=1; kernelX++)
                    {
                        for(int kernelY = -1; kernelY <=1; kernelY++)
                        {
                            convolvedImage[imageDataX, imageDataY] += imageData[imageDataX + kernelX, imageDataY + kernelY]*kernel[1+kernelX, 1+kernelY];  
                        }
                    }
                }
            }

            return convolvedImage;
        }

        private float[,] BitmapToArray(Bitmap image)
        {
            float[,] brightnessArray = new float[image.Width, image.Height]; 

            for(int i=0; i< image.Width; i++)
            {
                for(int j=0; j< image.Height; j++)
                {
                    Color pixel = image.GetPixel(i, j);
                    float rPart = pixel.R;
                    float gPart = pixel.G;
                    float bPart = pixel.B;
                    brightnessArray[i, j] = 0.3F* rPart + 0.3F*gPart + 0.3F*bPart;

                }
            }

            return brightnessArray;
        }

        public void GenerateEdges()
        {
            imageData = ConvolveWithoutPadding(imageData, edgeDetectionMask);
        }

        private float findMin(float[,] imageData)
        {
            float minValue = float.MaxValue;
            for (int imageDataX = 0; imageDataX < imageData.GetLength(0); imageDataX++)
            {
                for (int imageDataY = 0; imageDataY < imageData.GetLength(1); imageDataY++)
                {
                    if(minValue > imageData[imageDataX, imageDataY])
                    {
                        minValue = imageData[imageDataX, imageDataY];
                    }
                    
                }
            }
            return minValue;
        }

        private float findMax(float[,] imageData)
        {
            float maxValue = float.MinValue;
            for (int imageDataX = 0; imageDataX < imageData.GetLength(0); imageDataX++)
            {
                for (int imageDataY = 0; imageDataY < imageData.GetLength(1); imageDataY++)
                {
                    if (maxValue < imageData[imageDataX, imageDataY])
                    {
                        maxValue = imageData[imageDataX, imageDataY];
                    }

                }
            }
            return maxValue;
        }

        private float[,] AbsOnArray(float [,] imageArray)
        {
            for (int imageArrayX = 0; imageArrayX < imageArray.GetLength(0); imageArrayX++)
            {
                for (int imageArrayY = 0; imageArrayY < imageArray.GetLength(1); imageArrayY++)
                {

                    imageArray[imageArrayX, imageArrayY] = Math.Abs(imageArray[imageArrayX, imageArrayY]);

                }
            }
            return imageArray;
        }

        private int[,] NormalizeArrayValues(float [,] imageArray)
        {

            float[,] imageAbsArray =AbsOnArray(imageArray);
            int[,] normalizedArrayValues = new int[imageArray.GetLength(0), imageArray.GetLength(1)];
        
            float minBrightnessValue = findMin(imageArray);
   
            for (int imageArrayX = 0; imageArrayX < imageArray.GetLength(0); imageArrayX++)
            {
                for (int imageArrayY = 0; imageArrayY < imageArray.GetLength(1); imageArrayY++)
                {

                    imageAbsArray[imageArrayX, imageArrayY] -= minBrightnessValue;

                }
            }

            float maxBrightnessValue = findMax(imageArray);

            for (int imageArrayX = 0; imageArrayX < imageArray.GetLength(0); imageArrayX++)
            {
                for (int imageArrayY = 0; imageArrayY < imageArray.GetLength(1); imageArrayY++)
                {

                    imageAbsArray[imageArrayX, imageArrayY] /= maxBrightnessValue;
                    imageAbsArray[imageArrayX, imageArrayY] *= 255;
                    normalizedArrayValues[imageArrayX, imageArrayY] = (int)imageAbsArray[imageArrayX, imageArrayY];

                }
            }

            return normalizedArrayValues;
        }

        private Bitmap ArrayToBitmap(float [,] imageArray)
        {
            Bitmap brightnessBitmap = new Bitmap(imageArray.GetLength(0), imageArray.GetLength(1));
            int[,] normalizedImageArray = NormalizeArrayValues(imageArray);
            for (int imageArrayX = 0; imageArrayX < imageData.GetLength(0); imageArrayX++)
            {
                for (int imageArrayY = 0; imageArrayY < imageArray.GetLength(1); imageArrayY++)
                {
                    Color newCol = Color.FromArgb(normalizedImageArray[imageArrayX, imageArrayY], normalizedImageArray[imageArrayX, imageArrayY], normalizedImageArray[imageArrayX, imageArrayY]);

                    brightnessBitmap.SetPixel(imageArrayX, imageArrayY, newCol);
                }
            }

            return brightnessBitmap;
        }

        public void SaveImage(string name, ImageFormat format)
        {
            Bitmap outputImage = ArrayToBitmap(imageData);
            outputImage.Save(name, format);
        }
    }
}
