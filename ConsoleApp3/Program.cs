using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using System.IO;
using System.Collections;

namespace ConsoleApp3
{


    class Program
    {
        public class Pair
        {
            public String tag;
            public Double confidence;

            public Pair(String tag, Double confidence)
            {
                this.tag = tag;
                this.confidence = confidence;
            }


            override
            public String ToString()
            {
                String result = "Tag " + this.tag + "/nConfidence " + this.confidence;
                return result;
            }
        }
        private static string SubscriptionKey = "2f5ec80f14a4448ab2056cc0d4e5eb36";

        private static async Task<AnalysisResult> UploadAndAnalyzeImage(string imageFilePath)
        {
            VisionServiceClient VisionServiceClient = new VisionServiceClient(SubscriptionKey, "https://westcentralus.api.cognitive.microsoft.com/vision/v1.0");
            Console.WriteLine("VisionServiceClient is created");

            using (Stream imageFileStream = File.OpenRead(imageFilePath))
            {
                Console.WriteLine("Calling VisionServiceClient.AnalyzeImageAsync()...");
                VisualFeature[] visualFeatures = new VisualFeature[] { VisualFeature.Adult, VisualFeature.Categories, VisualFeature.Color, VisualFeature.Description, VisualFeature.Faces, VisualFeature.ImageType, VisualFeature.Tags };
                AnalysisResult analysisResult = await VisionServiceClient.GetTagsAsync(imageFileStream);
                return analysisResult;
            }
        }

        static void Main(string[] args)
        {
            // task 3
            Task.Run(async () =>
            {
                AnalysisResult task = await UploadAndAnalyzeImage("download.jpg");
                for (int i = 0; i < task.Tags.Length; ++i)
                    Console.WriteLine("Name " + task.Tags[i].Name + "\nConfidence " + task.Tags[i].Confidence);
            }).GetAwaiter().GetResult();

            // task 4
            string[] fileEntries = Directory.GetFiles("Images");
            String target = "Tagless";
            Directory.CreateDirectory(target);
            //File.Copy("download.jpg", target + "\\lel.jpg", true);
            foreach (string fileName in fileEntries)
            {
                Console.WriteLine(fileName);
                Task.Run(async () =>
                {
                    AnalysisResult task = await UploadAndAnalyzeImage(fileName);
                    if (task.Tags.Length == 0)
                        Console.WriteLine("No Tags");
                    else
                        Console.WriteLine("It has tags");
                    if (task.Tags.Length == 0)
                    {
                        String copy = fileName;
                        String[] spliting = copy.Split(new char[] { '\\' });
                        File.Copy(fileName, target + "\\" + spliting[1], true);
                    }
                    else
                    {
                        Double maximum = task.Tags[0].Confidence;
                        int index = 0;
                        for (int i = 0; i < task.Tags.Length; ++i)
                            if (maximum.CompareTo(task.Tags[i].Confidence) >= 0)
                            {
                                maximum = task.Tags[i].Confidence;
                                index = i;
                            }
                        Console.WriteLine(task.Tags[index].Name + " " + index + " " + task.Tags[index].Confidence);
                        String folder_target = task.Tags[index].Name;
                        String copy = fileName;
                        Directory.CreateDirectory(folder_target);
                        String[] spliting = copy.Split(new char[] { '\\' });
                        File.Copy(fileName, folder_target + "\\" + spliting[1], true);

                    }
                }).GetAwaiter().GetResult(); 
            } 
        }
    }
}
