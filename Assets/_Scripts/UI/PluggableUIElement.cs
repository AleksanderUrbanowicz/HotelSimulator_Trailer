

using UnityEngine;

namespace UI
{
    [ExecuteInEditMode]
    public class PluggableUIElement : MonoBehaviour
    {
      
        public ThemeData themeData;
        protected virtual void OnThemeDraw()
        {


        }

        public virtual void Awake()
        {
            // themeData = ThemeData;
            OnThemeDraw();
        }
        public virtual void Update()
        {

            if (Application.isEditor)
            {

                OnThemeDraw();
            }
        }

      

     
    }
}