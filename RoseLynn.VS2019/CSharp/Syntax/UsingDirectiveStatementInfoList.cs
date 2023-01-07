using System.Collections.Generic;
using System.Text;

namespace RoseLynn.CSharp.Syntax;

/// <summary>Includes some helper methods to quickly add <seealso cref="UsingDirectiveStatementInfo"/> instances.</summary>
public class UsingDirectiveStatementInfoList : List<UsingDirectiveStatementInfo>
{
    public UsingDirectiveStatementInfoList() { }
    /// <summary>
    /// Creates a new instance of the <seealso cref="UsingDirectiveStatementInfoList"/> out of an existing collection of
    /// <seealso cref="UsingDirectiveStatementInfo"/> instances, and optionally sorts the list.
    /// </summary>
    /// <param name="usingDirectives">The collection of <seealso cref="UsingDirectiveStatementInfo"/> instances to add to the list.</param>
    /// <param name="sort"><see langword="true"/> will cause sorting the list, otherwise <see langword="false"/>.</param>
    public UsingDirectiveStatementInfoList(IEnumerable<UsingDirectiveStatementInfo> usingDirectives, bool sort = false)
        : base(usingDirectives)
    {
        ConditionallySort(sort);
    }

    /// <summary>Adds a range of <seealso cref="UsingDirectiveStatementInfo"/> instances of the specified <seealso cref="UsingDirectiveKind"/>.</summary>
    /// <param name="kind">The <seealso cref="UsingDirectiveKind"/> of all the <seealso cref="UsingDirectiveStatementInfo"/> instances.</param>
    /// <param name="qualifiedNames">The qualified names of the using directives.</param>
    /// <returns>This instance.</returns>
    /// <remarks>
    /// The added instances are created through
    /// <seealso cref="UsingDirectiveStatementInfo.DirectivesOfKind(UsingDirectiveKind, IEnumerable{string})"/>.
    /// </remarks>
    public UsingDirectiveStatementInfoList AddRange(UsingDirectiveKind kind, IEnumerable<string> qualifiedNames)
    {
        AddRange(UsingDirectiveStatementInfo.DirectivesOfKind(kind, qualifiedNames));
        return this;
    }
    /// <remarks>
    /// The added instances are created through
    /// <seealso cref="UsingDirectiveStatementInfo.DirectivesOfKind(UsingDirectiveKind, string[])"/>.
    /// </remarks>
    /// <inheritdoc cref="AddRange(UsingDirectiveKind, IEnumerable{string})"/>
    public UsingDirectiveStatementInfoList AddRange(UsingDirectiveKind kind, params string[] qualifiedNames)
    {
        return AddRange(kind, (IEnumerable<string>)qualifiedNames);
    }

    /// <summary>Conditionally sorts the list using <seealso cref="Sort"/>.</summary>
    /// <param name="sort"><see langword="true"/> will cause the list to be sorted, <see langword="false"/> causes the method to do nothing.</param>
    /// <returns>This instance.</returns>
    public UsingDirectiveStatementInfoList ConditionallySort(bool sort)
    {
        if (sort)
            Sort();
        return this;
    }
    /// <summary>Sorts the <seealso cref="UsingDirectiveStatementInfo"/> list based on the <seealso cref="UsingDirectiveStatementInfo.SortingComparer"/>.</summary>
    /// <returns>This instance.</returns>
    public new UsingDirectiveStatementInfoList Sort()
    {
        Sort(UsingDirectiveStatementInfo.SortingComparer.Instance);
        return this;
    }

    /// <summary>Stringifies the current list of <seealso cref="UsingDirectiveStatementInfo"/> instances.</summary>
    /// <returns>A string containing all the using directives in their current order, one per line.</returns>
    public override string ToString()
    {
        var result = new StringBuilder();
        foreach (var statement in this)
            result.AppendLine(statement.ToString());

        return result.ToString().TrimEnd();
    }

    /// <summary>
    /// Creates a new <seealso cref="UsingDirectiveStatementInfoList"/> out of
    /// the given collection of <seealso cref="UsingDirectiveStatementInfo"/> instances,
    /// or returns the existing <seealso cref="UsingDirectiveStatementInfoList"/> instance.
    /// </summary>
    /// <param name="usingDirectives">
    /// The collection of <seealso cref="UsingDirectiveStatementInfo"/> instances whose contents will
    /// be contained in the resulting <seealso cref="UsingDirectiveStatementInfoList"/>.
    /// </param>
    /// <param name="sort">
    /// Determines whether the using directives should be sorted using <seealso cref="Sort"/>.
    /// Applies to the existing <seealso cref="UsingDirectiveStatementInfoList"/> instance too.
    /// </param>
    /// <returns>A <seealso cref="UsingDirectiveStatementInfoList"/> containing the given <seealso cref="UsingDirectiveStatementInfo"/> instances.</returns>
    public static UsingDirectiveStatementInfoList CreateOrCurrent(IEnumerable<UsingDirectiveStatementInfo> usingDirectives, bool sort = false)
    {
        if (usingDirectives is UsingDirectiveStatementInfoList list)
            return list.ConditionallySort(sort);

        return new(usingDirectives, sort);
    }
}
