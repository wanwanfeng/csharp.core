using System;
namespace FileVersion
{
    public class SvnUpdate : SvnCommon
    {
        public override string Name
        {
            get { return SaveDir + "patch-list"; }
        }

        public override void Run()
        {
            EndCmd();
            UpdatePathList();
        }
    }
}
