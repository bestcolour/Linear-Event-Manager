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

    #region FadetoAlpha
   

  

   


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

        public override bool UpdateEffect()
        {
            t += Time.deltaTime / duration;

            //Set target image colour as new colour value
            textMeshProGUI.color = new Color(textMeshProGUI.color.r, textMeshProGUI.color.g, textMeshProGUI.color.b, Mathf.Lerp(initialAlpha, targetAlpha, t));

            //Once the time passed exceeds the assigned duration, end this effect
            if (t > 1.0f)
            {
                //Set the targetimage as the actual targetted colour
                textMeshProGUI.color = new Color(textMeshProGUI.color.r, textMeshProGUI.color.g, textMeshProGUI.color.b, targetAlpha);
                return base.UpdateEffect();
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

        public override bool UpdateEffect()
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
                return base.UpdateEffect();
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

        public override bool UpdateEffect()
        {
            t += Time.deltaTime / duration;

            //Set target image colour as new colour value
            textMeshPro.color = new Color(textMeshPro.color.r, textMeshPro.color.g, textMeshPro.color.b, Mathf.Lerp(initialAlpha, targetAlpha, t));

            //Once the time passed exceeds the assigned duration, end this effect
            if (t > 1.0f)
            {
                //Set the targetimage as the actual targetted colour
                textMeshPro.color = new Color(textMeshPro.color.r, textMeshPro.color.g, textMeshPro.color.b, targetAlpha);
                return base.UpdateEffect();
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

        public override bool UpdateEffect()
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
                return base.UpdateEffect();
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

        public override bool UpdateEffect()
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

        public override bool UpdateEffect()
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

        public override bool UpdateEffect()
        {
            //Retext 
            targetText.text = newText;
            return true;
        }

    } 
    #endregion


}