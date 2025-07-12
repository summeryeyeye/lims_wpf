namespace Lims.ToolsForClient
{
    public interface IProgressModule
    {
        protected TestProgress RelativeProgress
        {
            get; set;
        }
    }
}