using Common;
using UnityEngine.UI;

namespace UI
{
    public class Window1View : WndView
    {
        public Text txtContent;

        private Window1Context wndContext;

        public override void Init(WndContext context)
        {
            wndContext = (Window1Context)context;
        }

        public override void Open(WndContext context)
        {
            if(wndContext != null)
            {
                txtContent.text = wndContext.GetContent();
            }
        }

        public override void Close(WndContext context)
        {
            base.Close(context);
        }

        public override void Destory(WndContext context)
        {
            base.Destory(context);
        }

        public void OnBtnClick()
        {
            UIManager.GetInstance().Open(WndNames.WND2_NAME, 66666);
        }
    }
}