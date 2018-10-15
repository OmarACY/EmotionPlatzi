using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using EmtionPlatzi.Web.Models;
using Microsoft.ProjectOxford.Common.Contract;
using Microsoft.ProjectOxford.Emotion;

namespace EmtionPlatzi.Web.Util
{
    public class EmotionHelper
    {
        public EmotionServiceClient EmoClient { get; set; }

        public EmotionHelper(string key)
        {
            EmoClient = new EmotionServiceClient(key);
        }

        public async Task<EmoPicture> DetectAndExtractFacesAsync(Stream imageStream)
        {
            var emotions = await EmoClient.RecognizeAsync(imageStream);

            var emoPicture = new EmoPicture();

            emoPicture.Faces = ExtractFaces(emotions, emoPicture);

            return emoPicture;
        }

        private ObservableCollection<EmoFace> ExtractFaces(Emotion[] emotions, EmoPicture emoPicture)
        {
            var facesList = new ObservableCollection<EmoFace>();

            foreach (var emotion in emotions)
            {
                var emoFace = new EmoFace()
                {
                    X = emotion.FaceRectangle.Left,
                    Y = emotion.FaceRectangle.Top,
                    Width = emotion.FaceRectangle.Width,
                    Height = emotion.FaceRectangle.Height,
                    Picture = emoPicture
                };

                emoFace.Emotions = ProcessEmotions(emotion.Scores, emoFace);
                emoPicture.Faces.Add(emoFace);
            }

            return facesList;
        }

        private ObservableCollection<EmoEmotion> ProcessEmotions(EmotionScores emotionScores, EmoFace emoFace)
        {
            var emotionList = new ObservableCollection<EmoEmotion>();

            var properties = emotionScores.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var filterProperty = properties.Where(p => p.PropertyType == typeof(float));
            /*var filterProperty = from p in properties
                                 where p.PropertyType == typeof(float)
                                 select p;*/

            foreach (var prop in filterProperty)
            {
                if (Enum.TryParse<EmoEmotionEnum>(prop.Name, out var emoType))
                {
                    emoType = EmoEmotionEnum.Undetermined;
                }
                
                var emoEmotion = new EmoEmotion();
                emoEmotion.Score = (float) prop.GetValue(emotionScores);
                emoEmotion.EmotionType = emoType;
                emoEmotion.Face = emoFace;

                emotionList.Add(emoEmotion);
            }

            return emotionList;
        }
    }
}