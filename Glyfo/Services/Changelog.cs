using System;
using System.Collections.Generic;
using System.Linq;

namespace Glyfo.Services;

/// <summary>One release and the points worth telling a user about it.</summary>
/// <param name="Keys">
/// String-table keys, not text. The notes are shown in whatever language the interface is in, so
/// they have to go through <see cref="Loc"/> like everything else the user reads.
/// </param>
internal sealed record Release(Version Version, string[] Keys);

/// <summary>
/// What changed in each release, for the notes shown the first time a new version is opened.
/// </summary>
internal static class Changelog
{
    /// <summary>Newest first. Add a release at the top; nothing else needs touching.</summary>
    public static readonly IReadOnlyList<Release> Releases = new[]
    {
        new Release(new Version(1, 2, 0), new[] { "News_120_1", "News_120_2", "News_120_3" }),
        new Release(new Version(1, 1, 0), new[] { "News_110_1", "News_110_2", "News_110_3", "News_110_4" }),
    };

    /// <summary>
    /// The releases a user who last saw <paramref name="lastSeen"/> has not read about yet.
    /// </summary>
    /// <remarks>
    /// Every release newer than theirs, not merely the current one: Store updates are free to skip
    /// versions, and someone coming from two releases back should be told about both rather than
    /// having the middle one silently disappear.
    ///
    /// Capped at the running version so that notes written ahead of a release — while the version in
    /// the manifest is still the old one — stay invisible until that version actually ships.
    /// </remarks>
    public static IReadOnlyList<Release> Since(Version? lastSeen)
    {
        var current = ProductInfo.Current;

        return Releases
            .Where(r => r.Version <= current && (lastSeen is null || r.Version > lastSeen))
            .OrderByDescending(r => r.Version)
            .ToList();
    }
}
