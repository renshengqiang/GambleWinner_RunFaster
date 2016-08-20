using Common;

namespace UI
{
    public class Window2Context : WndContext
    {
        private int intValue;

        public override WndType Type()
        {
            return WndType.NORMAL;
        }
        public override string Name()
        {
            return WndNames.WND2_NAME;
        }

        public override string PrefabPath()
        {
            return WndNames.WND2_PATH;
        }

        public override void Init()
        {
            intValue = 0;
        }
        public override void PrepareData(params object[] args)
        {
            if(args != null && args.Length > 0)
            {
                intValue = (int)args[0];
            }
        }

        public int GetValue()
        {
            return intValue;
        }
    }
}