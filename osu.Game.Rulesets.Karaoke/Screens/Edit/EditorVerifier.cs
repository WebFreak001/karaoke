// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Edit.Checks.Components;
using osu.Game.Screens.Edit;

namespace osu.Game.Rulesets.Karaoke.Screens.Edit;

/// <summary>
/// This class is focus on mange the list of <see cref="ICheck"/> and save/load list of <see cref="Issue"/>.
/// </summary>
/// <typeparam name="TEnum"></typeparam>
public abstract partial class EditorVerifier<TEnum> : Component, IEditorVerifier<TEnum> where TEnum : struct, Enum
{
    [Resolved, AllowNull]
    private EditorBeatmap beatmap { get; set; }

    [Resolved, AllowNull]
    private IBindable<WorkingBeatmap> workingBeatmap { get; set; }

    private readonly IDictionary<TEnum, ICheck[]> checkMappings = new Dictionary<TEnum, ICheck[]>();
    private readonly IDictionary<TEnum, BindableList<Issue>> issues = new Dictionary<TEnum, BindableList<Issue>>();

    protected EditorVerifier()
    {
        initializeCheckMappings();
        initializeIssues();
    }

    private void initializeCheckMappings()
    {
        foreach (var mode in Enum.GetValues<TEnum>())
        {
            checkMappings.Add(mode, CreateChecks(mode).ToArray());
        }
    }

    private void initializeIssues()
    {
        foreach (var mode in Enum.GetValues<TEnum>())
        {
            issues.Add(mode, new BindableList<Issue>());
        }
    }

    public IBindableList<Issue> GetIssueByType(TEnum type)
        => issues[type];

    public abstract void Refresh();

    #region Checks

    protected abstract IEnumerable<ICheck> CreateChecks(TEnum type);

    protected void ClearChecks(TEnum type)
    {
        issues[type].Clear();
    }

    protected void AddChecks(TEnum type, IEnumerable<Issue> newIssues)
    {
        issues[type].AddRange(newIssues);
    }

    protected virtual TEnum ClassifyIssue(Issue issue)
    {
        foreach (var (type, checks) in checkMappings)
        {
            if (checks.Contains(issue.Check))
                return type;
        }

        throw new ArgumentOutOfRangeException();
    }

    protected virtual BeatmapVerifierContext CreateBeatmapVerifierContext(IBeatmap beatmap, WorkingBeatmap workingBeatmap) => new(beatmap, workingBeatmap);

    protected IEnumerable<Issue> CreateIssues(Action<BeatmapVerifierContext>? action = null)
    {
        var context = CreateBeatmapVerifierContext(beatmap, workingBeatmap.Value);
        action?.Invoke(context);
        return new EditorBeatmapVerifier(checkMappings.Values.SelectMany(x => x)).Run(context);
    }

    protected IEnumerable<Issue> CreateIssuesByType(TEnum type, BeatmapVerifierContext context)
        => new EditorBeatmapVerifier(checkMappings[type]).Run(context);

    #endregion

    private class EditorBeatmapVerifier : IBeatmapVerifier
    {
        private readonly IEnumerable<ICheck> checks;

        public EditorBeatmapVerifier(IEnumerable<ICheck> checks)
        {
            this.checks = checks;
        }

        public IEnumerable<Issue> Run(BeatmapVerifierContext context)
        {
            return checks.SelectMany(check => check.Run(context));
        }
    }
}

/// <summary>
/// This class is focus on mange the list of <see cref="ICheck"/> and save/load list of <see cref="Issue"/>.
/// </summary>
public abstract partial class EditorVerifier : Component, IEditorVerifier
{
    [Resolved, AllowNull]
    private EditorBeatmap beatmap { get; set; }

    [Resolved, AllowNull]
    private IBindable<WorkingBeatmap> workingBeatmap { get; set; }

    private readonly List<ICheck> checks = new();
    private readonly BindableList<Issue> issues = new();

    public IBindableList<Issue> Issues => issues;

    protected EditorVerifier()
    {
        checks.AddRange(CreateChecks().ToList());
    }

    public abstract void Refresh();

    #region Checks

    protected abstract IEnumerable<ICheck> CreateChecks();

    protected IBindableList<Issue> GetIssues()
        => issues;

    protected void ClearChecks()
    {
        issues.Clear();
    }

    protected void AddChecks(IEnumerable<Issue> newIssues)
    {
        issues.AddRange(newIssues);
    }

    protected virtual BeatmapVerifierContext CreateBeatmapVerifierContext(IBeatmap beatmap, WorkingBeatmap workingBeatmap) => new(beatmap, workingBeatmap);

    protected IEnumerable<Issue> CreateIssues(Action<BeatmapVerifierContext>? action = null)
    {
        var context = CreateBeatmapVerifierContext(beatmap, workingBeatmap.Value);
        action?.Invoke(context);
        return new EditorBeatmapVerifier(checks).Run(context);
    }

    #endregion

    private class EditorBeatmapVerifier : IBeatmapVerifier
    {
        private readonly IEnumerable<ICheck> checks;

        public EditorBeatmapVerifier(IEnumerable<ICheck> checks)
        {
            this.checks = checks;
        }

        public IEnumerable<Issue> Run(BeatmapVerifierContext context)
        {
            return checks.SelectMany(check => check.Run(context));
        }
    }
}
