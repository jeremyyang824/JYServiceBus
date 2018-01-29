using System;
using System.Collections;
using System.Collections.Generic;
using Wind.iSeller.Framework.Core.Dependency;
using Wind.iSeller.NServiceBus.ZeroService.Beans;

namespace Wind.iSeller.NServiceBus.ZeroService.Domain
{
    /// <summary>
    /// 构建类型定义
    /// </summary>
    public class ServiceMsgTypeDefineBuilder : ITransientDependency
    {
        private readonly Dictionary<Type, IServiceMsgTypeDefine> _cache = new Dictionary<Type, IServiceMsgTypeDefine>();

        public IServiceMsgTypeDefine Build(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            //从cache返回
            if (this._cache.ContainsKey(type))
            {
                return this._cache[type];
            }

            return this.build(type, type);
        }

        private IServiceMsgTypeDefine build(Type type, Type rootType)
        {
            if (type.IsArray)
            {
                var elemType = this.build(type.GetElementType(), rootType);
                return new ServiceMsgCollectionTypeDefine(elemType);
            }
            else if (type.GetInterface("IEnumerable") != null && !ServiceMsgBasicTypeDefine.IsBasicType(type))
            {
                if (type.GetInterface("IEnumerable`1") != null)
                {
                    //泛型集合
                    var elemType = this.build(type.GetInterface("IEnumerable`1").GetGenericArguments()[0], rootType);
                    return new ServiceMsgCollectionTypeDefine(elemType);
                }
                else
                {
                    //非泛型集合
                    return new ServiceMsgCollectionTypeDefine(this.build(typeof(object), rootType));
                }
            }
            else if (ServiceMsgBasicTypeDefine.IsBasicType(type))
            {
                return new ServiceMsgBasicTypeDefine(type.Name, false);
            }
            else if (type.IsEnum)
            {
                return new ServiceMsgBasicTypeDefine(type.GetEnumUnderlyingType().Name, false);
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return new ServiceMsgBasicTypeDefine(type.GetGenericArguments()[0].Name, true);
            }
            else
            {
                return buildComplexType(type, rootType);
            }
        }

        private IServiceMsgTypeDefine buildComplexType(Type type, Type rootType)
        {
            var complexTypeDefne = new ServiceMsgComplexTypeDefine();
            foreach (var propInfo in type.GetProperties())
            {
                string propName = propInfo.Name;
                Type propType = propInfo.PropertyType;

                if (propType == rootType)
                {
                    //避免死循环
                    complexTypeDefne.AddProperty(propName, new ServiceMsgBasicTypeDefine(propType.Name));
                }
                else
                {
                    IServiceMsgTypeDefine propTypeDefine = this.build(propType, rootType);
                    complexTypeDefne.AddProperty(propName, propTypeDefine);
                }
            }
            return complexTypeDefne;

        }
    }
}
