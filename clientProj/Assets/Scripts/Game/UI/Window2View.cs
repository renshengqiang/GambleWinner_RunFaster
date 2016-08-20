using Common;
using UnityEngine.UI;

namespace UI
{
    public class Window2View : WndView
    {
        public Text txtContent;

        private Window2Context windowContext;

        public override void Init(WndContext context)
        {
            windowContext = (Window2Context)context;
        }

        public override void Open(WndContext context)
        {
            if(windowContext != null)
            {
                txtContent.text = windowContext.GetValue().ToString();
            }
        }

        public override void Close(WndContext context)
        {
            //todo: add your code for close a window here
            base.Close(context);
        }

        public override void Destory(WndContext context)
        {
            //todo: add your code for destroy a window here
            base.Destory(context);
        }

        public void OnBtnRtn()
        {
            UIManager.GetInstance().CloseTop();
        }
    }
}