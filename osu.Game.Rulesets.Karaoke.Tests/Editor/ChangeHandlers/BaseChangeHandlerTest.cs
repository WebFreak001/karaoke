// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Testing;
using osu.Game.Rulesets.Karaoke.Beatmaps;
using osu.Game.Rulesets.Karaoke.Beatmaps.Metadatas;
using osu.Game.Rulesets.Karaoke.Configuration;
using osu.Game.Rulesets.Karaoke.Edit.Utils;
using osu.Game.Rulesets.Objects;
using osu.Game.Screens.Edit;
using osu.Game.Tests.Visual;

namespace osu.Game.Rulesets.Karaoke.Tests.Editor.ChangeHandlers
{
    /// <summary>
    /// it's a base class for testing all change handler.
    /// Should inherit <see cref="OsuTestScene"/> because all change handler need the injecting to get the value.
    /// </summary>
    [HeadlessTest]
    public abstract partial class BaseChangeHandlerTest<TChangeHandler> : EditorClockTestScene where TChangeHandler : Component
    {
        private TChangeHandler changeHandler = null!;

        private int transactionCount;

        [BackgroundDependencyLoader]
        private void load()
        {
            var beatmap = new KaraokeBeatmap
            {
                BeatmapInfo =
                {
                    Ruleset = new KaraokeRuleset().RulesetInfo,
                },
            };
            var editorBeatmap = new EditorBeatmap(beatmap);
            var editorChangeHandler = new MockEditorChangeHandler(editorBeatmap);
            Dependencies.Cache(editorBeatmap);
            Dependencies.CacheAs<IEditorChangeHandler>(editorChangeHandler);
            editorChangeHandler.TransactionEnded += () =>
            {
                transactionCount++;
            };

            Children = new Drawable[]
            {
                editorBeatmap,
                changeHandler = CreateChangeHandler()
            };
        }

        protected virtual TChangeHandler CreateChangeHandler()
        {
            if (Activator.CreateInstance(typeof(TChangeHandler)) is not TChangeHandler handler)
                throw new InvalidOperationException("Change handler should have no params in the ctor.");

            return handler;
        }

        [SetUp]
        public virtual void SetUp()
        {
            AddStep("Setup", () =>
            {
                var editorBeatmap = Dependencies.Get<EditorBeatmap>();
                editorBeatmap.Clear();
                editorBeatmap.SelectedHitObjects.Clear();

                var karaokeBeatmap = EditorBeatmapUtils.GetPlayableBeatmap(editorBeatmap);
                karaokeBeatmap.AvailableTranslates.Clear();
                karaokeBeatmap.SingerInfo = new SingerInfo();
                karaokeBeatmap.PageInfo = new PageInfo();
                karaokeBeatmap.StageInfos.Clear();
            });
        }

        protected virtual bool IncludeAutoGenerator => false;

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        {
            if (!IncludeAutoGenerator)
            {
                return base.CreateChildDependencies(parent);
            }

            var baseDependencies = new DependencyContainer(base.CreateChildDependencies(parent));
            baseDependencies.Cache(new KaraokeRulesetEditGeneratorConfigManager());
            return baseDependencies;
        }

        protected void SetUpEditorBeatmap(Action<EditorBeatmap> action)
        {
            AddStep("Prepare testing beatmap", () =>
            {
                var editorBeatmap = Dependencies.Get<EditorBeatmap>();
                action.Invoke(editorBeatmap);
            });
        }

        protected void SetUpKaraokeBeatmap(Action<KaraokeBeatmap> action)
        {
            SetUpEditorBeatmap(editorBeatmap =>
            {
                var karaokeBeatmap = EditorBeatmapUtils.GetPlayableBeatmap(editorBeatmap);
                action.Invoke(karaokeBeatmap);
            });
        }

        protected void TriggerHandlerChanged(Action<TChangeHandler> c)
        {
            AddStep("Trigger change handler", () =>
            {
                // should reset transaction number in here because it will increase if load testing object.
                transactionCount = 0;
                c(changeHandler);
            });
        }

        protected void TriggerHandlerChangedWithException<T>(Action<TChangeHandler> c) where T : Exception
        {
            TriggerHandlerChanged(ch =>
            {
                Assert.Catch<T>(() => c(ch));
            });
        }

        protected void AssertEditorBeatmap(Action<EditorBeatmap> assert)
        {
            AddAssert("Is result matched", () =>
            {
                var editorBeatmap = Dependencies.Get<EditorBeatmap>();
                assert(editorBeatmap);

                // even if there's no property changed in the lyric editor, should still trigger the change handler.
                // because every change handler call should cause one undo step.
                // also, technically should not call the change handler if there's no possible to change the properties.
                return IsTransactionOnlyTriggerOnce();
            });
        }

        protected void AssertKaraokeBeatmap(Action<KaraokeBeatmap> assert)
        {
            AssertEditorBeatmap(editorBeatmap =>
            {
                var karaokeBeatmap = EditorBeatmapUtils.GetPlayableBeatmap(editorBeatmap);
                assert.Invoke(karaokeBeatmap);
            });
        }

        protected void PrepareHitObject(HitObject hitObject, bool selected = true)
            => PrepareHitObjects(new[] { hitObject }, selected);

        protected void PrepareHitObjects(IEnumerable<HitObject> selectedHitObjects, bool selected = true)
        {
            AddStep("Prepare testing hit objects", () =>
            {
                var hitobjects = selectedHitObjects.ToList();
                var editorBeatmap = Dependencies.Get<EditorBeatmap>();

                editorBeatmap.AddRange(hitobjects);

                if (selected)
                {
                    editorBeatmap.SelectedHitObjects.AddRange(hitobjects);
                }
            });
        }

        protected void AssertHitObject<THitObject>(Action<THitObject> assert) where THitObject : HitObject
        {
            AssertHitObjects<THitObject>(hitObjects =>
            {
                foreach (var hitObject in hitObjects)
                {
                    assert(hitObject);
                }
            });
        }

        protected void AssertHitObjects<THitObject>(Action<IEnumerable<THitObject>> assert) where THitObject : HitObject
        {
            AddAssert("Is result matched", () =>
            {
                var editorBeatmap = Dependencies.Get<EditorBeatmap>();
                assert(editorBeatmap.HitObjects.OfType<THitObject>());

                // even if there's no property changed in the lyric editor, should still trigger the change handler.
                // because every change handler call should cause one undo step.
                // also, technically should not call the change handler if there's no possible to change the properties.
                return IsTransactionOnlyTriggerOnce();
            });
        }

        protected bool IsTransactionOnlyTriggerOnce()
        {
            return transactionCount == 1;
        }

        private partial class MockEditorChangeHandler : TransactionalCommitComponent, IEditorChangeHandler
        {
            public event Action? OnStateChange;

            public MockEditorChangeHandler(EditorBeatmap editorBeatmap)
            {
                editorBeatmap.TransactionBegan += BeginChange;
                editorBeatmap.TransactionEnded += EndChange;
                editorBeatmap.SaveStateTriggered += SaveState;
            }

            protected override void UpdateState()
            {
                OnStateChange?.Invoke();
            }
        }
    }
}
