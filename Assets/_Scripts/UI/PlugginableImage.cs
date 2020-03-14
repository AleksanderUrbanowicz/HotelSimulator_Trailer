using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Image))]
    [ExecuteInEditMode]
    public class PlugginableImage : PluggableUIElement
    {
   
        public Image image;
        public int color;
        public UIColorType uIColorType;
        public Image Image { get => image != null ? image : GetComponent<Image>(); }
        public ThemeData ThemeData { get => themeData != null ? themeData : Resources.Load<ThemeData>(""); }
        public override void Awake()
        {
            // themeData = ThemeData;
            base.Awake();

        }
        public override void Update()
        {
            // themeData = ThemeData;
            base.Update();

        }

     

        protected override void OnThemeDraw()
        {
            SetColor();
            base.OnThemeDraw();

           
        }

        public void SetColor()
        {
          Image.color=  ThemeData.GetColor(color);

        }


    }
}