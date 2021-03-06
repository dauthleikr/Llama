﻿namespace Llama.PE.Packaging.PE32Plus.MZHeader
{
    using Structures.Header;

    internal class MZHeaderPackager : IPackage<IMZInfo, IMZResult>
    {
        public unsafe IMZResult Package(IMZInfo param) => new MZResult(MZHeader.CreateWithDefaultStub((uint)sizeof(MZHeader)));
    }
}