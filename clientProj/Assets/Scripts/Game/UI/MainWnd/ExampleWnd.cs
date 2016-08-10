using Common;

namespace UI
{
    public class ExampleWnd : WndContext
    {
        public override WndType Type()
        {
            //todo: comment below line and return your window type
            throw new System.NotImplementedException();
        }
        public override string Name()
        {
            //todo: comment below line and return your window name
            throw new System.NotImplementedException();
        }

        public override string PrefabPath()
        {
            //todo:comment the below line and return your prefab path
            throw new System.NotImplementedException();
        }

        public override void Init()
        {
            //todo: add your Init work here
            base.Init();
        }
        public override void PrepareData(params object[] args)
        {
            //todo: add your data process here
            base.PrepareData(args);
        }
    }
}