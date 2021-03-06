﻿using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace CampFireScene
{
    public class AssetManger
    {
        private string _assetDirectory;

        public AssetManger(string assetDirectory)
        {
            _assetDirectory = assetDirectory;
        }

        public static List<OBJobject> LoadAssets()
        {
            //Load in all assets from the disk here.
            List<string> filesToBeParced = new List<string>();
            List<OBJobject> parcedFiles;
            filesToBeParced.Add(@"Objects\SkyBox.obj");
            filesToBeParced.Add(@"Objects\SkyBoxWater.obj");
            filesToBeParced.Add(@"Objects\SkyBoxIsland.obj");
            filesToBeParced.Add(@"Objects\fire.obj");
            parcedFiles = ParceFiles(filesToBeParced);
            parcedFiles[0].imageTextureHandle = loadImage(@"Images\SkyBox.jpg");
            parcedFiles[1].imageTextureHandle = loadImage(@"Images\water.jpg");
            parcedFiles[2].imageTextureHandle = loadImage(@"Images\BeachSand.jpg");
            parcedFiles[3].imageTextureHandle = loadImage(@"Images\Wood.jpg");
            return parcedFiles;
        }

        private static int loadImage(string filePath)
        {
            Bitmap picture = new Bitmap(Image.FromFile(filePath));
            System.Drawing.Imaging.BitmapData data = picture.LockBits(new System.Drawing.Rectangle(0, 0, picture.Width, picture.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            int textureHandle;
            GL.Enable(EnableCap.Texture2D);
            textureHandle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, textureHandle);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            picture.UnlockBits(data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            return textureHandle;
        }

        private static List<OBJobject> ParceFiles(List<String> files)
        {
            StreamReader sr;
            string line;
            OBJobject OBJ;
            List<OBJobject> listOfOBJS = new List<OBJobject>();
            for (int i = 0; i < files.Count; i++)
            {
                sr = new StreamReader(files[i]);
                OBJ = new OBJobject();
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    string[] str = line.Split(' ');
                    switch (str[0])
                    {
                        case "v":

                            for (int j = 1; j <= 3; j++)
                            {
                                OBJ.Vertices.Add(float.Parse(str[j]));
                            }
                            break;

                        case "vt":

                            OBJ.uvs.Add(float.Parse(str[1]));
                            OBJ.uvs.Add(1 - float.Parse(str[2]));

                            break;

                        case "vn":

                            for (int j = 1; j <= 3; j++)
                            {
                                OBJ.normals.Add(float.Parse(str[j]));
                            }
                            break;

                        case "f":

                            int[] faceIndices = new int[3];
                            int[] faceTexIndices = new int[3];
                            int[] faceNormIndices = new int[3];
                            for (int j = 1; j < str.Length; j++)
                            {
                                string[] subStr = str[j].Split('/');
                                if (subStr.Length >= 1)
                                {
                                    faceIndices[j - 1] = int.Parse(subStr[0]);
                                }
                                if (subStr.Length >= 2)
                                {
                                    OBJ.VTC(true);
                                    faceTexIndices[j - 1] = int.Parse(subStr[1]);
                                }
                                if (subStr.Length >= 3)
                                {
                                    OBJ.VTC(false);
                                    OBJ.VTCN(true);
                                    faceNormIndices[j - 1] = int.Parse(subStr[2]);
                                }
                                //faces face = new faces() { VertexIndex1 = int.Parse(subStr[0]), textureIndex1 = int.Parse(subStr[1]), normalIndex1 = int.Parse(subStr[2]) };
                                //OBJ.faces.Add(face);
                            }
                            faces face = new faces()
                            {
                                VertexIndex1 = faceIndices[0],
                                VertexIndex2 = faceIndices[1],
                                VertexIndex3 = faceIndices[2],
                                textureIndex1 = faceTexIndices[0],
                                textureIndex2 = faceTexIndices[1],
                                textureIndex3 = faceTexIndices[2],
                                normalIndex1 = faceNormIndices[0],
                                normalIndex2 = faceNormIndices[1],
                                normalIndex3 = faceNormIndices[2]
                            };
                            OBJ.faces.Add(face);
                            break;

                        default:
                            break;
                    }
                }
                OBJ.Load();
                listOfOBJS.Add(OBJ);

                //listOfOBJS.Add(OBJ);
            }

            return listOfOBJS;
        }
    }

    public class faces
    {
        public int normalIndex1;
        public int normalIndex2;
        public int normalIndex3;
        public int textureIndex1;
        public int textureIndex2;
        public int textureIndex3;
        public int VertexIndex1;
        public int VertexIndex2;
        public int VertexIndex3;
    }
}