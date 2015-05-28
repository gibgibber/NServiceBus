﻿namespace NServiceBus.Core.Tests.Pipeline
{
    using System;
    using System.Collections.Generic;
    using NServiceBus.Pipeline;
    using NUnit.Framework;

    public class PipelineModelBuilderTests
    {

        [Test]
        public void ShouldDetectConflictingStageConnectors()
        {
            var builder = new PipelineModelBuilder(typeof(RootContext), new List<RegisterStep>
            {
                RegisterStep.Create("Root1", typeof(RootBehavior), "desc"),
                RegisterStep.Create("RootToChildConnector", typeof(RootToChildConnector), "desc"),
                RegisterStep.Create("RootToChild2Connector", typeof(RootToChild2Connector), "desc")
        
            }, new List<RemoveStep>(), new List<ReplaceBehavior>());


            var ex = Assert.Throws<Exception>(() => builder.Build());

            Assert.True(ex.Message.Contains("Multiple stage connectors found"));
        }

        [Test]
        public void ShouldDetectRegistrationsWithContextsReachableFromTheRootContext()
        {
            var builder = new PipelineModelBuilder(typeof(RootContext), new List<RegisterStep>
            {
                RegisterStep.Create("Root", typeof(RootBehavior), "desc"),
                RegisterStep.Create("RootToChildConnector", typeof(RootToChild2Connector), "desc"),
                RegisterStep.Create("Child", typeof(NonReachableChildBehavior), "desc")
            }, new List<RemoveStep>(), new List<ReplaceBehavior>());


            var model = builder.Build();

            Assert.AreEqual(3, model.Count);
        }

        [Test]
        public void ShouldHandleTheTerminator()
        {
            var builder = new PipelineModelBuilder(typeof(RootContext), new List<RegisterStep>
            {
                RegisterStep.Create("Root1", typeof(RootBehavior), "desc"),
                RegisterStep.Create("RootToChildConnector", typeof(RootToChildConnector), "desc"),
                RegisterStep.Create("Terminator", typeof(Terminator), "desc")
        
            }, new List<RemoveStep>(), new List<ReplaceBehavior>());


            var model = builder.Build();

            Assert.AreEqual(3, model.Count);
        }



        public class RootContext : BehaviorContext
        {
            public RootContext(BehaviorContext parentContext)
                : base(parentContext)
            {
            }
        }

        public class ChildContext : RootContext
        {
            public ChildContext(BehaviorContext parentContext)
                : base(parentContext)
            {
            }
        }
        public class ChildContextReachableButNotInheritingFromRootContext : BehaviorContext
        {
            public ChildContextReachableButNotInheritingFromRootContext(BehaviorContext parentContext)
                : base(parentContext)
            {
            }
        }

        public class RootToChildConnector : StageConnector<RootContext, ChildContext>
        {
            public override void Invoke(RootContext context, Action<ChildContext> next)
            {
                throw new NotImplementedException();
            }
        }

        public class Terminator : PipelineTerminator<ChildContext>
        {
            public override void Terminate(ChildContext context)
            {
                throw new NotImplementedException();
            }
        }
        public class RootToChild2Connector : StageConnector<RootContext, ChildContextReachableButNotInheritingFromRootContext>
        {
            public override void Invoke(RootContext context, Action<ChildContextReachableButNotInheritingFromRootContext> next)
            {
                throw new NotImplementedException();
            }
        }

        public class RootBehavior : Behavior<RootContext>
        {
            public override void Invoke(RootContext context, Action next)
            {
                throw new NotImplementedException();
            }
        }

        public class ChildBehavior : Behavior<ChildContext>
        {
            public override void Invoke(ChildContext context, Action next)
            {
                throw new NotImplementedException();
            }
        }
        public class NonReachableChildBehavior : Behavior<ChildContextReachableButNotInheritingFromRootContext>
        {
            public override void Invoke(ChildContextReachableButNotInheritingFromRootContext context, Action next)
            {
                throw new NotImplementedException();
            }
        }
    }

}