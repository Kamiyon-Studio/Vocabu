using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenu.WordDex {
    public class ScrollbarFix : ScrollRect{
        override protected void LateUpdate() {
            base.LateUpdate();
            if (this.verticalScrollbar) {
                this.verticalScrollbar.size = 0.1f;
            }
        }

        override public void Rebuild(CanvasUpdate executing) {
            base.Rebuild(executing);
            if (this.verticalScrollbar) {
                this.verticalScrollbar.size = 0.1f;
            }
        }
    }
}