using System;
using System.Collections.Generic;
using AutoMapper;

namespace Wind.iSeller.Framework.AutoMapper
{
    public class WindAutoMapperConfiguration : IWindAutoMapperConfiguration
    {
        public List<Action<IConfiguration>> Configurators { get; private set; }

        public bool UseStaticMapper { get; set; }

        public WindAutoMapperConfiguration()
        {
            UseStaticMapper = true;
            Configurators = new List<Action<IConfiguration>>();
        }
    }
}
