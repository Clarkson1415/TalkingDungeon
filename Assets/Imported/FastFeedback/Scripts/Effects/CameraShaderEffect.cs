using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FastFeedback
{
    public class CameraShaderEffect : FastFeedbackEffect
    {
        private List<ApplyCameraShader> activeShaders = new List<ApplyCameraShader>();
        public Dictionary<ApplyCameraShader, float> timeAddedDict = new Dictionary<ApplyCameraShader, float>();
        public Dictionary<ApplyCameraShader, Material> materialDict = new Dictionary<ApplyCameraShader, Material>();

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            RenderTexture tempSrc = RenderTexture.GetTemporary(source.width, source.height, source.depth, source.format);
            RenderTexture tempDst = RenderTexture.GetTemporary(source.width, source.height, source.depth, source.format);
            Graphics.Blit(source, tempSrc);

            for (int i = 0; i < activeShaders.Count; i++)
            {
                float ratio = (Time.time - timeAddedDict[activeShaders[i]]) / activeShaders[i].durationTime;
                float strength = activeShaders[i].effectStrength * activeShaders[i].customCurve.Evaluate(ratio);
                materialDict[activeShaders[i]].SetFloat("_Strength", strength);
                if (i % 2 == 0)
                    Graphics.Blit(tempSrc, tempDst, materialDict[activeShaders[i]]);
                else
                    Graphics.Blit(tempDst, tempSrc, materialDict[activeShaders[i]]);
            }

            if (activeShaders.Count % 2 == 0)
                Graphics.Blit(tempSrc, destination);
            else
                Graphics.Blit(tempDst, destination);

            RenderTexture.ReleaseTemporary(tempSrc);
            RenderTexture.ReleaseTemporary(tempDst);
        }

        public override void Apply(FeedbackItem item, GameObject target = null, GameObject origin = null)
        {
            base.Apply(item, target, origin);
            ApplyCameraShader o = (ApplyCameraShader)item;
            if (!activeShaders.Contains(o))
            {
                bool added = false;
                for (int i = 0; i < activeShaders.Count; i++)
                {
                    if (o.priority >= activeShaders[i].priority)
                    {
                        activeShaders.Insert(i, o);
                        added = true;
                        break;
                    }
                }
                if (!added) activeShaders.Add(o);
            }
            timeAddedDict[o] = Time.time;
            
            if (o.cameraShaderSettings == ApplyCameraShader.CameraShaderSettings.CustomShader)
            {
                materialDict[o] = new Material(o.customCameraMaterial);
            }
            else
            {
                materialDict[o] = new Material(o.customCameraShader);
            }
        }

        void Update()
        {
            List<ApplyCameraShader> toDelete = new List<ApplyCameraShader>();
            foreach (ApplyCameraShader activeShader in activeShaders)
            {
                if (activeShader.durationSettings == ApplyCameraShader.DurationSettings.ForXSeconds && Time.time > timeAddedDict[activeShader] + activeShader.durationTime)
                {
                    toDelete.Add(activeShader);
                }
            }
            foreach (ApplyCameraShader activeShader in toDelete)
            {
                activeShaders.Remove(activeShader);
                timeAddedDict.Remove(activeShader);
                materialDict.Remove(activeShader);
            }
        }

        public void RemoveOutlinesWithTag(string tag)
        {
            for (int i = activeShaders.Count - 1; i >= 0; i--)
            {
                if (activeShaders[i].effectTag == tag)
                {
                    timeAddedDict.Remove(activeShaders[i]);
                    materialDict.Remove(activeShaders[i]);
                    activeShaders.RemoveAt(i);
                }
            }
        }

        public void RemoveOutlinesWithPriority(int priority)
        {
            for (int i = activeShaders.Count - 1; i >= 0; i--)
            {
                if (activeShaders[i].priority == priority)
                {
                    timeAddedDict.Remove(activeShaders[i]);
                    materialDict.Remove(activeShaders[i]);
                    activeShaders.RemoveAt(i);
                }
            }
        }

        public void RemoveOutlinesLessThanPriority(int priority)
        {
            for (int i = activeShaders.Count - 1; i >= 0; i--)
            {
                if (activeShaders[i].priority < priority)
                {
                    timeAddedDict.Remove(activeShaders[i]);
                    materialDict.Remove(activeShaders[i]);
                    activeShaders.RemoveAt(i);
                }
            }
        }

        public void RemoveOutlinesGreaterThanPriority(int priority)
        {
            for (int i = activeShaders.Count - 1; i >= 0; i--)
            {
                if (activeShaders[i].priority > priority)
                {
                    timeAddedDict.Remove(activeShaders[i]);
                    materialDict.Remove(activeShaders[i]);
                    activeShaders.RemoveAt(i);
                }
            }
        }

        public void RemoveAllOutlines()
        {
            timeAddedDict = new Dictionary<ApplyCameraShader, float>();
            materialDict = new Dictionary<ApplyCameraShader, Material>();
            activeShaders = new List<ApplyCameraShader>();
        }
    }
}
