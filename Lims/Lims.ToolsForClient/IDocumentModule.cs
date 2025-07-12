namespace Lims.ToolsForClient
{
    public interface IDocumentModule
    {
        string Caption
        {
            get;
        }

        bool IsActive
        {
            get; set;
        }
    }
}