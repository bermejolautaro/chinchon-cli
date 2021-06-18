namespace Chinchon
{
    public class HandlerResponse
    {
        public IAction Action { get; set; } = new NothingAction();
    }
}
