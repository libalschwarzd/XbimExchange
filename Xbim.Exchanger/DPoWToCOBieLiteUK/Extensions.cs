﻿using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.COBieLiteUK;
using Xbim.DPoW;
using CAttribute = Xbim.COBieLiteUK.Attribute;
using DAttribute = Xbim.DPoW.Attribute;
namespace XbimExchanger.DPoWToCOBieLiteUK
{
    public static class Extensions
    {

        public static string GetEncodedClassification(this PlanOfWork pow, IEnumerable<Guid> classificationReferenceIds, string suffix = "")
        {
            var ids = classificationReferenceIds != null ? classificationReferenceIds.ToArray() : new Guid[]{};
            var idsNum = ids.Length;
            if (idsNum == 0)
                return null;
            
            var result = "";
            var processedCount = 0;

            //itterate over ids and get it all together
            foreach (var classification in pow.ClassificationSystems)
            {
                foreach (var reference in classification.ClassificationReferences)
                {
                    if (!ids.Contains(reference.Id)) continue;

                    result += String.Format("{0}:{1}:{2}|", classification.Name, reference.ClassificationCode,
                        suffix);

                    processedCount++;
                    if (processedCount == ids.Length) break;
                }
            }
            return result.Trim('|');
        }

        public static void Add(this List<CAttribute> act, string name, string description, string value, string pset = null)
        {
            act.Add(new CAttribute()
                    {
                        CreatedOn = DateTime.Now,
                        Name = name,
                        Description = description,
                        ExternalEntity = pset,
                        Value = new StringAttributeValue { Value = value}
                    });
        }

        public static void Add(this List<CAttribute> act, string name, string description, int value, string pset = null)
        {
            act.Add(new CAttribute()
            {
                CreatedOn = DateTime.Now,
                Name = name,
                Description = description,
                ExternalEntity = pset,
                Value = new IntegerAttributeValue { Value = value }
            });
        }

        public static void Add(this List<CAttribute> act, string name, string description, double value, string pset = null)
        {
            act.Add(new CAttribute()
            {
                CreatedOn = DateTime.Now,
                Name = name,
                Description = description,
                ExternalEntity = pset,
                Value = new DecimalAttributeValue { Value = value }
            });
        }

        public static void Add(this List<CAttribute> act, string name, string description, bool value, string pset = null)
        {
            act.Add(new CAttribute()
            {
                CreatedOn = DateTime.Now,
                Name = name,
                Description = description,
                ExternalEntity = pset,
                Value = new BooleanAttributeValue { Value = value }
            });
        }

        public static void Add(this List<CAttribute> act, string name, string description)
        {
            act.Add(new CAttribute()
            {
                CreatedOn = DateTime.Now,
                Name = name,
                Description = description,
            });
        }

        public static IEnumerable<CAttribute> GetCOBieAttributes(this DPoWAttributableObject obj)
        {
            IEnumerable<Xbim.DPoW.Attribute> sAttributes = obj.Attributes;
            var sAttrs = sAttributes as DAttribute[] ?? sAttributes.ToArray();
            if (sAttributes == null || !sAttrs.Any())
                yield break;

            foreach (var sAttr in sAttrs)
            {
                //create attribute in target
                var tAttr = new CAttribute { Name = sAttr.Name, Description = sAttr.Definition };
                switch (sAttr.ValueType)
                {
                    case ValueTypeEnum.NotDefined:
                        tAttr.Value = new StringAttributeValue { Value = sAttr.Value };
                        break;
                    case ValueTypeEnum.Boolean:
                        bool bVal;
                        if (bool.TryParse(sAttr.Value, out bVal))
                            tAttr.Value = new BooleanAttributeValue { Value = bVal };
                        break;
                    case ValueTypeEnum.DateTime:
                        DateTime dtVal;
                        if (DateTime.TryParse(sAttr.Value, out dtVal))
                            tAttr.Value = new DateTimeAttributeValue {Value = dtVal};
                        break;
                    case ValueTypeEnum.Decimal:
                        float fVal;
                        if (float.TryParse(sAttr.Value, out fVal))
                            tAttr.Value = new DecimalAttributeValue {Value = fVal};
                        break;
                    case ValueTypeEnum.Integer:
                        int iVal;
                        if (int.TryParse(sAttr.Value, out iVal))
                            tAttr.Value = new IntegerAttributeValue { Value = iVal };
                        break;
                    case ValueTypeEnum.String:
                        tAttr.Value = new StringAttributeValue { Value = sAttr.Value };
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                //default action is to create string
                if (tAttr.Value == null)
                    tAttr.Value = new StringAttributeValue { Value = sAttr.Value };
                yield return tAttr;
            }
        }
    }
}