using System;
using System.Collections.Generic;
using AutoMapper;

namespace Wind.iSeller.Framework.AutoMapper
{
    public interface IWindAutoMapperConfiguration
    {
        List<Action<IConfiguration>> Configurators { get; }
    }
}
