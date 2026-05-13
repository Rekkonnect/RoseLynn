namespace RoseLynn.Writing;

public abstract partial class BaseSeparatableBlockList
{
    private bool _hasPrevious = false;

    protected abstract void AppendSeparator();

    public void BeginNewBlock()
    {
        if (_hasPrevious)
        {
            AppendSeparator();
        }
    }

    public void CommitBlock()
    {
        _hasPrevious = true;
    }

    public BlockScope BeginBlockScope()
    {
        return new(this);
    }

    public readonly struct BlockScope : IDisposable
    {
        private readonly BaseSeparatableBlockList _blockList;

        public BlockScope(BaseSeparatableBlockList blockList)
        {
            _blockList = blockList;
            blockList.BeginNewBlock();
        }

        public void Dispose()
        {
            _blockList.CommitBlock();
        }
    }
}
