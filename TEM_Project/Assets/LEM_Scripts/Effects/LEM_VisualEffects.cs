using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// © 2020 Lee Kee Shen All Rights Reserved
/// Basically this is where i am going to store all the Visual effects related events for TEM
/// Side note, if you are using below unity version 2019.2.12f1, you may need to import unity's textmesh pro package 
/// as for the textmesh pro fade alpha code, i will post it on the page as well
/// </summary>
namespace LEM_Effects
{

    #region Fade to Alpha Types
    public class FadeToAlphaImageComponent : LEM_BaseEffect
    {
        //target
        [Tooltip("The image you want to fade")]
        [SerializeField]Image targetImage = default;

        //Cached alpha values
        float initialAlpha = default;

        //User inputted alpha value
        [Tooltip("The target alpha value you want the image to have at the end of the fade")]
        [SerializeField] float targetAlpha = default;

        //User inputted alpha value
        [Tooltip("How long it takes for the fade to be complete")]
        [SerializeField] float duration = default;

        //How much time passed since this effect was started
        float t = default;

        public override void Initialise()
        {
            //Record initial alpha first
            initialAlpha = targetImage.color.a;

            //Recalculate the target alpha (convert to normalized value)
            targetAlpha = targetAlpha / 255f;
        }

        public override bool ExecuteEffect()
        {
            t += Time.deltaTime/duration;

            //Set target image colour as new colour value
            targetImage.color = new Color(targetImage.color.r,targetImage.color.g,targetImage.color.b, Mathf.Lerp(initialAlpha, targetAlpha, t));

            //Once the time passed exceeds the assigned duration, end this effect
            if(t > 1.0f)
            {
                //Set the targetimage as the actual targetted colour
                targetImage.color = new Color(targetImage.color.r, targetImage.color.g, targetImage.color.b,targetAlpha);
                return base.ExecuteEffect();
            }

            return false;
        }

    }

    public class FadeToAlphaImagesComponent : LEM_BaseEffect
    {
        //target
        [Tooltip("The images you want to fade")]
        [SerializeField] Image[] targetImages = default;

        //Cached alpha values
        float[] initialAlphas = default;

        //User inputted alpha value
        [Tooltip("The target alpha value you want all the images to have at the end of the fade")]
        [SerializeField] float targetAlpha = default;

        //User inputted alpha value
        [Tooltip("How long it takes for all the images' fade to be complete")]
        [SerializeField] float duration = default;

        //How much time passed since this effect was started
        float t = default;

        public override void Initialise()
        {
            for (int i = 0; i < targetImages.Length; i++)
            {
                //Record initial alpha first for each of the targetimages
                initialAlphas[i] = targetImages[i].color.a;
            }

            //Recalculate the target alpha (convert to normalized value)
            targetAlpha = targetAlpha / 255f;
        }

        public override bool ExecuteEffect()
        {
            t += Time.deltaTime / duration;

            //Lerp all the alphas of the images 
            for (int i = 0; i < targetImages.Length; i++)
            {
                //Set target image colour as new colour value
                targetImages[i].color = new Color(targetImages[i].color.r, targetImages[i].color.g, targetImages[i].color.b, Mathf.Lerp(initialAlphas[i], targetAlpha, t));
            }

            //Once the time passed exceeds the assigned duration, end this effect
            if (t > 1.0f)
            {
                for (int i = 0; i < targetImages.Length; i++)
                {
                    //Set the targetimages as the actual targetted colour
                    targetImages[i].color = new Color(targetImages[i].color.r, targetImages[i].color.g, targetImages[i].color.b, targetAlpha);
                }
                return base.ExecuteEffect();
            }

            return false;
        }

    }

    public class FadeToAlphaTextComponent : LEM_BaseEffect
    {
        //target
        [Tooltip("The text you want to fade")]
        [SerializeField] Text targetText = default;

        //Cached alpha values
        float initialAlpha = default;

        //User inputted alpha value
        [Tooltip("The target alpha value you want the text to have at the end of the fade")]
        [SerializeField] float targetAlpha = default;

        //User inputted alpha value
        [Tooltip("How long it takes for the fade to be complete")]
        [SerializeField] float duration = default;

        //How much time passed since this effect was started
        float t = default;

        public override void Initialise()
        {
            //Record initial alpha first
            initialAlpha = targetText.color.a;

            //Recalculate the target alpha (convert to normalized value)
            targetAlpha = targetAlpha / 255f;
        }

        public override bool ExecuteEffect()
        {
            t += Time.deltaTime / duration;

            //Set target image colour as new colour value
            targetText.color = new Color(targetText.color.r, targetText.color.g, targetText.color.b, Mathf.Lerp(initialAlpha, targetAlpha, t));

            //Once the time passed exceeds the assigned duration, end this effect
            if (t > 1.0f)
            {
                //Set the targetimage as the actual targetted colour
                targetText.color = new Color(targetText.color.r, targetText.color.g, targetText.color.b, targetAlpha);
                return base.ExecuteEffect();
            }

            return false;
        }

    }

    public class FadeToAlphaTextsComponent : LEM_BaseEffect
    {
        //target
        [Tooltip("The texts you want to fade")]
        [SerializeField] Text[] targetTexts = default;

        //Cached alpha values
        float[] initialAlphas = default;

        //User inputted alpha value
        [Tooltip("The target alpha value you want the texts to have at the end of the fade")]
        [SerializeField] float targetAlpha = default;

        //User inputted alpha value
        [Tooltip("How long it takes for the fade to be complete")]
        [SerializeField] float duration = default;

        //How much time passed since this effect was started
        float t = default;

        public override void Initialise()
        {
            for (int i = 0; i < targetTexts.Length; i++)
            {
                //Record initial alpha first for each of the targetimages
                initialAlphas[i] = targetTexts[i].color.a;
            }

            //Recalculate the target alpha (convert to normalized value)
            targetAlpha = targetAlpha / 255f;
        }

        public override bool ExecuteEffect()
        {
            t += Time.deltaTime / duration;

            //Lerp all the alphas of the images 
            for (int i = 0; i < targetTexts.Length; i++)
            {
                //Set target image colour as new colour value
                targetTexts[i].color = new Color(targetTexts[i].color.r, targetTexts[i].color.g, targetTexts[i].color.b, Mathf.Lerp(initialAlphas[i], targetAlpha, t));
            }

            //Once the time passed exceeds the assigned duration, end this effect
            if (t > 1.0f)
            {
                for (int i = 0; i < targetTexts.Length; i++)
                {
                    //Set the targetimages as the actual targetted colour
                    targetTexts[i].color = new Color(targetTexts[i].color.r, targetTexts[i].color.g, targetTexts[i].color.b, targetAlpha);
                }
                return base.ExecuteEffect();
            }

            return false;
        }

    }

    public class FadeToAlphaTextMeshProGUIComponent : LEM_BaseEffect
    {
        //target
        [Tooltip("The text mesh pro gui you want to fade")]
        [SerializeField] TextMeshProUGUI textMeshProGUI = default;

        //Cached alpha values
        float initialAlpha = default;

        //User inputted alpha value
        [Tooltip("The target alpha value you want the text mesh pro gui to have at the end of the fade")]
        [SerializeField] float targetAlpha = default;

        //User inputted alpha value
        [Tooltip("How long it takes for the fade to be complete")]
        [SerializeField] float duration = default;

        //How much time passed since this effect was started
        float t = default;

        public override void Initialise()
        {
            //Record initial alpha first
            initialAlpha = textMeshProGUI.color.a;

            //Recalculate the target alpha (convert to normalized value)
            targetAlpha = targetAlpha / 255f;
        }

        public override bool ExecuteEffect()
        {
            t += Time.deltaTime / duration;

            //Set target image colour as new colour value
            textMeshProGUI.color = new Color(textMeshProGUI.color.r, textMeshProGUI.color.g, textMeshProGUI.color.b, Mathf.Lerp(initialAlpha, targetAlpha, t));

            //Once the time passed exceeds the assigned duration, end this effect
            if (t > 1.0f)
            {
                //Set the targetimage as the actual targetted colour
                textMeshProGUI.color = new Color(textMeshProGUI.color.r, textMeshProGUI.color.g, textMeshProGUI.color.b, targetAlpha);
                return base.ExecuteEffect();
            }

            return false;
        }

    }

    public class FadeToAlphaTextMeshProGUIsComponent : LEM_BaseEffect
    {
        //target
        [Tooltip("The text mesh pro guis you want to fade")]
        [SerializeField] Text[] textMeshProGUIs = default;

        //Cached alpha values
        float[] initialAlphas = default;

        //User inputted alpha value
        [Tooltip("The target alpha value you want the text mesh pro guis to have at the end of the fade")]
        [SerializeField] float targetAlpha = default;

        //User inputted alpha value
        [Tooltip("How long it takes for the fade to be complete")]
        [SerializeField] float duration = default;

        //How much time passed since this effect was started
        float t = default;

        public override void Initialise()
        {
            for (int i = 0; i < textMeshProGUIs.Length; i++)
            {
                //Record initial alpha first for each of the targetimages
                initialAlphas[i] = textMeshProGUIs[i].color.a;
            }

            //Recalculate the target alpha (convert to normalized value)
            targetAlpha = targetAlpha / 255f;
        }

        public override bool ExecuteEffect()
        {
            t += Time.deltaTime / duration;

            //Lerp all the alphas of the images 
            for (int i = 0; i < textMeshProGUIs.Length; i++)
            {
                //Set target image colour as new colour value
                textMeshProGUIs[i].color = new Color(textMeshProGUIs[i].color.r, textMeshProGUIs[i].color.g, textMeshProGUIs[i].color.b, Mathf.Lerp(initialAlphas[i], targetAlpha, t));
            }

            //Once the time passed exceeds the assigned duration, end this effect
            if (t > 1.0f)
            {
                for (int i = 0; i < textMeshProGUIs.Length; i++)
                {
                    //Set the targetimages as the actual targetted colour
                    textMeshProGUIs[i].color = new Color(textMeshProGUIs[i].color.r, textMeshProGUIs[i].color.g, textMeshProGUIs[i].color.b, targetAlpha);
                }
                return base.ExecuteEffect();
            }

            return false;
        }

    }

    public class FadeToAlphaTextMeshProComponent : LEM_BaseEffect
    {
        //target
        [Tooltip("The text mesh pro you want to fade")]
        [SerializeField] TextMeshPro textMeshPro = default;

        //Cached alpha values
        float initialAlpha = default;

        //User inputted alpha value
        [Tooltip("The target alpha value you want the text mesh pro to have at the end of the fade")]
        [SerializeField] float targetAlpha = default;

        //User inputted alpha value
        [Tooltip("How long it takes for the fade to be complete")]
        [SerializeField] float duration = default;

        //How much time passed since this effect was started
        float t = default;

        public override void Initialise()
        {
            //Record initial alpha first
            initialAlpha = textMeshPro.color.a;

            //Recalculate the target alpha (convert to normalized value)
            targetAlpha = targetAlpha / 255f;
        }

        public override bool ExecuteEffect()
        {
            t += Time.deltaTime / duration;

            //Set target image colour as new colour value
            textMeshPro.color = new Color(textMeshPro.color.r, textMeshPro.color.g, textMeshPro.color.b, Mathf.Lerp(initialAlpha, targetAlpha, t));

            //Once the time passed exceeds the assigned duration, end this effect
            if (t > 1.0f)
            {
                //Set the targetimage as the actual targetted colour
                textMeshPro.color = new Color(textMeshPro.color.r, textMeshPro.color.g, textMeshPro.color.b, targetAlpha);
                return base.ExecuteEffect();
            }

            return false;
        }

    }

    public class FadeToAlphaTextMeshProsComponent : LEM_BaseEffect
    {
        //target
        [Tooltip("The text mesh pros you want to fade")]
        [SerializeField] Text[] textMeshPros = default;

        //Cached alpha values
        float[] initialAlphas = default;

        //User inputted alpha value
        [Tooltip("The target alpha value you want all the text mesh pros to have at the end of the fade")]
        [SerializeField] float targetAlpha = default;

        //User inputted alpha value
        [Tooltip("How long it takes for the fade to be complete")]
        [SerializeField] float duration = default;

        //How much time passed since this effect was started
        float t = default;

        public override void Initialise()
        {
            for (int i = 0; i < textMeshPros.Length; i++)
            {
                //Record initial alpha first for each of the targetimages
                initialAlphas[i] = textMeshPros[i].color.a;
            }

            //Recalculate the target alpha (convert to normalized value)
            targetAlpha = targetAlpha / 255f;
        }

        public override bool ExecuteEffect()
        {
            t += Time.deltaTime / duration;

            //Lerp all the alphas of the images 
            for (int i = 0; i < textMeshPros.Length; i++)
            {
                //Set target image colour as new colour value
                textMeshPros[i].color = new Color(textMeshPros[i].color.r, textMeshPros[i].color.g, textMeshPros[i].color.b, Mathf.Lerp(initialAlphas[i], targetAlpha, t));
            }

            //Once the time passed exceeds the assigned duration, end this effect
            if (t > 1.0f)
            {
                for (int i = 0; i < textMeshPros.Length; i++)
                {
                    //Set the targetimages as the actual targetted colour
                    textMeshPros[i].color = new Color(textMeshPros[i].color.r, textMeshPros[i].color.g, textMeshPros[i].color.b, targetAlpha);
                }
                return base.ExecuteEffect();
            }

            return false;
        }

    }

    public class FadeToAlphaSpriteRendererComponent : LEM_BaseEffect
    {
        //target
        [Tooltip("The sprite you want to fade")]
        [SerializeField] SpriteRenderer targetSprite = default;

        //Cached alpha values
        float initialAlpha = default;

        //User inputted alpha value
        [Tooltip("The target alpha value you want the sprite to have at the end of the fade")]
        [SerializeField] float targetAlpha = default;

        //User inputted alpha value
        [Tooltip("How long it takes for the fade to be complete")]
        [SerializeField] float duration = default;

        //How much time passed since this effect was started
        float t = default;

        public override void Initialise()
        {
            //Record initial alpha first
            initialAlpha = targetSprite.color.a;

            //Recalculate the target alpha (convert to normalized value)
            targetAlpha = targetAlpha / 255f;
        }

        public override bool ExecuteEffect()
        {
            t += Time.deltaTime / duration;

            //Set target image colour as new colour value
            targetSprite.color = new Color(targetSprite.color.r, targetSprite.color.g, targetSprite.color.b, Mathf.Lerp(initialAlpha, targetAlpha, t));

            //Once the time passed exceeds the assigned duration, end this effect
            if (t > 1.0f)
            {
                //Set the targetimage as the actual targetted colour
                targetSprite.color = new Color(targetSprite.color.r, targetSprite.color.g, targetSprite.color.b, targetAlpha);
                return base.ExecuteEffect();
            }

            return false;
        }

    }

    public class FadeToAlphaSpriteRenderersComponent : LEM_BaseEffect
    {
        //target
        [Tooltip("The sprites you want to fade")]
        [SerializeField] Image[] targetSprites = default;

        //Cached alpha values
        float[] initialAlphas = default;

        //User inputted alpha value
        [Tooltip("The target alpha value you want all the sprites to have at the end of the fade")]
        [SerializeField] float targetAlpha = default;

        //User inputted alpha value
        [Tooltip("How long it takes for all the sprites' fade to be complete")]
        [SerializeField] float duration = default;

        //How much time passed since this effect was started
        float t = default;

        public override void Initialise()
        {
            for (int i = 0; i < targetSprites.Length; i++)
            {
                //Record initial alpha first for each of the targetimages
                initialAlphas[i] = targetSprites[i].color.a;
            }

            //Recalculate the target alpha (convert to normalized value)
            targetAlpha = targetAlpha / 255f;
        }

        public override bool ExecuteEffect()
        {
            t += Time.deltaTime / duration;

            //Lerp all the alphas of the images 
            for (int i = 0; i < targetSprites.Length; i++)
            {
                //Set target image colour as new colour value
                targetSprites[i].color = new Color(targetSprites[i].color.r, targetSprites[i].color.g, targetSprites[i].color.b, Mathf.Lerp(initialAlphas[i], targetAlpha, t));
            }

            //Once the time passed exceeds the assigned duration, end this effect
            if (t > 1.0f)
            {
                for (int i = 0; i < targetSprites.Length; i++)
                {
                    //Set the targetimages as the actual targetted colour
                    targetSprites[i].color = new Color(targetSprites[i].color.r, targetSprites[i].color.g, targetSprites[i].color.b, targetAlpha);
                }
                return base.ExecuteEffect();
            }

            return false;
        }

    }

    public class FadeToAlphaRendererComponent : LEM_BaseEffect
    {
        //target
        [Tooltip("The renderer you want to fade")]
        [SerializeField] Renderer targetRenderer = default;

        //Cached alpha values
        float [] initialAlpha = default;

        //User inputted alpha value
        [Tooltip("The target alpha value you want the renderer to have at the end of the fade")]
        [SerializeField] float targetAlpha = default;

        //User inputted alpha value
        [Tooltip("How long it takes for the fade to be complete")]
        [SerializeField] float duration = default;

        //How much time passed since this effect was started
        float t = default;

        public override void Initialise()
        {
            //Record initial alpha first
            for (int i = 0; i < targetRenderer.materials.Length; i++)
            {
                initialAlpha[i] = targetRenderer.materials[i].color.a;
            }

            //Recalculate the target alpha (convert to normalized value)
            targetAlpha = targetAlpha / 255f;
        }

        public override bool ExecuteEffect()
        {
            t += Time.deltaTime / duration;

            for (int i = 0; i < targetRenderer.materials.Length; i++)
            {
                //Set target image colour as new colour value
                targetRenderer.materials[i].color = new Color(targetRenderer.materials[i].color.r, targetRenderer.materials[i].color.g,
                    targetRenderer.materials[i].color.b, Mathf.Lerp(initialAlpha[i], targetAlpha, t));
            }

            //Once the time passed exceeds the assigned duration, end this effect
            if (t > 1.0f)
            {
                //Set the targetimage as the actual targetted colour
                for (int i = 0; i < targetRenderer.materials.Length; i++)
                {
                    targetRenderer.materials[i].color = new Color(targetRenderer.materials[i].color.r,
                       targetRenderer.materials[i].color.g, targetRenderer.materials[i].color.b, targetAlpha); 
                }

                return base.ExecuteEffect();
            }

            return false;
        }

    }

    public class FadeToAlphaRenderersComponent : LEM_BaseEffect
    {
        //target
        [Tooltip("The renderers you want to fade")]
        [SerializeField] Renderer[] targetRenderers = default;

        //Cached alpha values
        float[,] initialAlphas = default;

        //User inputted alpha value
        [Tooltip("The target alpha value you want all the renderers' materials to have at the end of the fade")]
        [SerializeField] float targetAlpha = default;

        //User inputted alpha value
        [Tooltip("How long it takes for all the sprites' fade to be complete")]
        [SerializeField] float duration = default;

        //How much time passed since this effect was started
        float t = default;

        public override void Initialise()
        {
            for (int i = 0; i < targetRenderers.Length; i++)
            {
                for (int r = 0; r < targetRenderers[i].materials.Length; r++)
                {
                    //Record initial alpha first for each of the targetimages
                    initialAlphas[i,r] = targetRenderers[i].materials[r].color.a;
                }
              
            }

            //Recalculate the target alpha (convert to normalized value)
            targetAlpha = targetAlpha / 255f;
        }

        public override bool ExecuteEffect()
        {
            t += Time.deltaTime / duration;

            //Lerp all the alphas of the images 
            for (int i = 0; i < targetRenderers.Length; i++)
            {
                for (int r = 0; r < targetRenderers[i].materials.Length; r++)
                {
                    //Set target image colour as new colour value
                    targetRenderers[i].materials[r].color = new Color(targetRenderers[i].materials[r].color.r, targetRenderers[i].materials[r].color.g, 
                        targetRenderers[i].materials[r].color.b, Mathf.Lerp(initialAlphas[i,r], targetAlpha, t));
                }
            }

            //Once the time passed exceeds the assigned duration, end this effect
            if (t > 1.0f)
            {
                //Lerp all the alphas of the images 
                for (int i = 0; i < targetRenderers.Length; i++)
                {
                    for (int r = 0; r < targetRenderers[i].materials.Length; r++)
                    {
                        //Set target renderer colour as new colour value
                        targetRenderers[i].materials[r].color = new Color(targetRenderers[i].materials[r].color.r, targetRenderers[i].materials[r].color.g,
                            targetRenderers[i].materials[r].color.b,targetAlpha);
                    }
                }

                return base.ExecuteEffect();
            }

            return false;
        }

    }

    #endregion

    #region ReWord Types
    public class ReWordTextComponent : LEM_BaseEffect
    {
        //target
        [Tooltip("The text you want to reword")]
        [SerializeField] Text targetText = default;

        [Tooltip("The text you want to insert into the target text")]
        [SerializeField] string newText = default;

        public override bool ExecuteEffect()
        {
            //Retext 
            targetText.text = newText;
            return true;
        }

    }

    public class ReWordTextMeshProGUIComponent : LEM_BaseEffect
    {
        //target
        [Tooltip("The text mesh pro gui you want to reword")]
        [SerializeField] TextMeshProUGUI targetText = default;

        [Tooltip("The text you want to insert into the target text")]
        [SerializeField] string newText = default;

        public override bool ExecuteEffect()
        {
            //Retext 
            targetText.text = newText;
            return true;
        }

    }

    public class ReWordTextMeshProComponent : LEM_BaseEffect
    {
        //target
        [Tooltip("The text mesh pro you want to reword")]
        [SerializeField] TextMeshPro targetText = default;

        [Tooltip("The text you want to insert into the target text")]
        [SerializeField] string newText = default;

        public override bool ExecuteEffect()
        {
            //Retext 
            targetText.text = newText;
            return true;
        }

    } 
    #endregion


}