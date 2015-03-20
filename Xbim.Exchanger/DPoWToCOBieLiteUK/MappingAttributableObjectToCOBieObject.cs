﻿using System.Collections.Generic;
using System.Linq;
using Xbim.COBieLiteUK;
using Xbim.DPoW;
using Attribute = Xbim.COBieLiteUK.Attribute;

namespace XbimExchanger.DPoWToCOBieLiteUK
{
    abstract class MappingAttributableObjectToCOBieObject<TFrom, TTo> : DPoWToCOBieLiteUKMapping<TFrom, TTo> where TFrom:DPoWAttributableObject where TTo : CobieObject, new()
    {
        protected override TTo Mapping(TFrom source, TTo target)
        {
            if (source.Attributes == null || !source.Attributes.Any())
                return target;

            var tAttrs = source.GetCOBieAttributes();
            if (target.Attributes == null) target.Attributes = new List<Attribute>();
            target.Attributes.AddRange(tAttrs);

            return target;
        }

        
    }
}