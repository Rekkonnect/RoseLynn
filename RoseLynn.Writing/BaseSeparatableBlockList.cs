namespace RoseLynn.Writing;

/// <summary>Provides shared state for block lists that insert separators between committed blocks.</summary>
public abstract partial class BaseSeparatableBlockList
{
    private bool _hasPrevious = false;

    /// <summary>Appends the separator that should appear before the next block.</summary>
    protected abstract void AppendSeparator();

    /// <summary>Begins a new block, inserting a separator when a previous block was already committed.</summary>
    public void BeginNewBlock()
    {
        if (_hasPrevious)
        {
            AppendSeparator();
        }
    }

    /// <summary>Marks the current block as committed so future blocks can be separated from it.</summary>
    public void CommitBlock()
    {
        _hasPrevious = true;
    }

    /// <summary>Begins a disposable block scope that commits automatically when disposed.</summary>
    /// <returns>A scope that commits the block on disposal.</returns>
    public BlockScope BeginBlockScope()
    {
        return new(this);
    }

    /// <summary>Represents a disposable scope for a block in a <see cref="BaseSeparatableBlockList"/>.</summary>
    public readonly struct BlockScope : IDisposable
    {
        private readonly BaseSeparatableBlockList _blockList;

        /// <summary>Initializes the scope and begins a new block on the target list.</summary>
        /// <param name="blockList">The block list whose scope is being entered.</param>
        public BlockScope(BaseSeparatableBlockList blockList)
        {
            _blockList = blockList;
            blockList.BeginNewBlock();
        }

        /// <summary>Commits the current block.</summary>
        public void Dispose()
        {
            _blockList.CommitBlock();
        }
    }
}
