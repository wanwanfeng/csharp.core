using System;
namespace FileVersion
{
    public class SvnUpdate : SvnCommon
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
