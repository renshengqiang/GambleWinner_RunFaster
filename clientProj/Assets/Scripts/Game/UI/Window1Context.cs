using Common;

namespace UI
{
    public class Window1Context : WndContext
    {
        private string content;

        public override WndType Type()
        {
            return WndType.NORMAL;
        }
        public override string Name()
        {
            return WndNames.WND1_NAME;
        }

        public override string PrefabPath()
        {
            return WndNames.WND1_PATH;
        }

        public override void Init()
        {
            content = string.Empty;
        }
        public override void PrepareData(params object[] args)
        {
            if(args != null && args.Length > 0)
            {
                content = (string)args[0];
            }
        }

        public string GetContent()
        {
            return content;
        }
    }
}