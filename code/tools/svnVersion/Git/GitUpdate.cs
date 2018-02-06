namespace FileVersion
{
    public class GitUpdate : GitCommon
    {
        public override string Name
        {
            get { return "patch-list"; }
        }

        public override void Run()
        {
            EndCmd();
            UpdatePathList();
        }
    }
}
