﻿using System;
using System.Linq;
using Delpin.Application.Interfaces;
using Delpin.Domain.Entities;

namespace Delpin.Application.Contracts.v1.ProductGroups
{
    public class ProductGroupOrderBy : IOrderBy<ProductGroup>
    {
        public Func<IQueryable<ProductGroup>, IOrderedQueryable<ProductGroup>> Sorting(string orderBy)
        {
            switch (orderBy)
            {
                case "name":
                    return x => x.OrderBy(p => p.Name);
                case "nameDesc":
                    return x => x.OrderByDescending(p => p.Name);
                default:
                    return null;
            }
        }
    }
}
