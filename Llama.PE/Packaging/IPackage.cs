using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Llama.PE.Tests")]
namespace Llama.PE.Packaging
{
    internal interface IPackage<in TParam, out TResult>
    {
        TResult Package(TParam param);
    }
}