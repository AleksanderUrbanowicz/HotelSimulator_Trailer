using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Text))]
    [ExecuteInEditMode]
    public class PlugginableText : PluggableUIElement
    {

        public Text text;
        public int color;
        private UIColorType uIColorType;
        public Text Text { get => text != null ? text : GetComponent<Text>(); }
        public ThemeData ThemeData { get => themeData != null ? themeData : Resources.Load<ThemeData>(""); }
        public override void Awake()
        {
            
            base.Awake();

        }
        public override void Update()
        {
           
            base.Update();

        }



        protected override void OnThemeDraw()
        {
            SetColor();
            base.OnThemeDraw();


        }

        public void SetColor()
        {
            Text.color = ThemeData != null ? ThemeData.GetColor(color) : Color.magenta;

        }


    }
}