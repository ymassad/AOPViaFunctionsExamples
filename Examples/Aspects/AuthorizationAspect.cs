using System;
using Examples.FileSystem.Authorizers;
using Functions;

namespace Examples.Aspects
{
    public static class AuthorizationAspect
    {
        public static IFunction<TInput, TOutput> ApplyAuthorizationAspect<TInput, TOutput, TResourceId>(
            this IFunction<TInput, TOutput> decorated,
            IResourceAuthorize<TResourceId> authorizer,
            Func<TInput, TResourceId> resourceIdSelector)
        {
            return new Aspect<TInput,TOutput,TResourceId>(decorated, authorizer, resourceIdSelector);
        }

        private class Aspect<TInput, TOutput, TResourceId> : IFunction<TInput, TOutput>
        {
            private readonly IFunction<TInput, TOutput> decorated;
            private readonly IResourceAuthorize<TResourceId> authorizer;
            private readonly Func<TInput, TResourceId> resourceIdSelector;

            public Aspect(
                IFunction<TInput, TOutput> decorated,
                IResourceAuthorize<TResourceId> authorizer,
                Func<TInput, TResourceId> resourceIdSelector)
            {
                this.decorated = decorated;
                this.authorizer = authorizer;
                this.resourceIdSelector = resourceIdSelector;
            }

            public TOutput Invoke(TInput input)
            {
                var resourceId = resourceIdSelector(input);

                if(!authorizer.HasAccess(resourceId))
                    throw new Exception("Access denied");

                return decorated.Invoke(input);
            }
        }
    }
}