namespace GitHubNugetPackageManager.DragDrop
{
    public interface IFileDragDropTarget
    {
        void OnFileDrop(string[] filepaths);
    }
}
