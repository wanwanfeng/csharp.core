namespace FileVersion
{
    public class GitUpdate : GitCommon
    {
        public override string Name
        {
            get { return SaveDir + "patch-list"; }
        }

        public override void Run()
        {
            UpdatePathList();
        }
    }
}
