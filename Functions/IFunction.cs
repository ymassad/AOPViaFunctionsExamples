namespace Functions
{
    public interface IFunction<in TInput, out TOutput>
    {
        TOutput Invoke(TInput input);
    }
}
