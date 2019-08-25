namespace Llama.PE.Packaging
{
    internal interface IPackage<out TResult, in TParam> where TResult : IPackagingResult
    {
        TResult Package(TParam param);
    }
}