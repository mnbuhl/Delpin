using Delpin.Application.Interfaces;
using Delpin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delpin.API.Services
{
    public class GenericRepositoryProcessingService : IScopedProcessingService
    {
        private readonly IGenericRepository<ProductItem> _productItemRepository;
        private readonly IGenericRepository<Rental> _rentalRepository;
        private readonly ILogger<GenericRepositoryProcessingService> _logger;
        private static int _itemsMadeAvailable = 0;

        public GenericRepositoryProcessingService(IGenericRepository<ProductItem> productItemRepository,
            IGenericRepository<Rental> rentalRepository, ILogger<GenericRepositoryProcessingService> logger)
        {
            _productItemRepository = productItemRepository;
            _rentalRepository = rentalRepository;
            _logger = logger;
        }

        // Finding rental end date to check for expired, and through rental lines finding product item to update availability 
        public async Task DoWork()
        {
            _logger.LogInformation("Scanning for rentals that have expired to make items available");

            var rentalsPastEndDate = await _rentalRepository.GetAllAsync(x =>
                    x.EndDate <= DateTime.UtcNow && x.EndDate > DateTime.UtcNow.AddDays(-5),
                x => x.Include(r => r.RentalLines).ThenInclude(rl => rl.ProductItem));

            if (rentalsPastEndDate == null || rentalsPastEndDate.Count == 0)
            {
                _logger.LogInformation("No items to be made available were found");
                return;
            }

            var productItemsToUpdate = new List<ProductItem>();

            foreach (var rental in rentalsPastEndDate)
            {
                if (rental.RentalLines == null || rental.RentalLines.Count <= 0)
                    continue;

                foreach (var rentalLine in rental.RentalLines)
                {
                    if (productItemsToUpdate.Any(x => x.Id == rentalLine.ProductItem.Id))
                        continue;

                    if (rentalLine.ProductItem.IsAvailable == false)
                        productItemsToUpdate.Add(rentalLine.ProductItem);
                }
            }

            if (productItemsToUpdate.Count <= 0)
            {
                _logger.LogInformation("No items to be made available were found");
                return;
            }


            foreach (var productItem in productItemsToUpdate)
            {
                productItem.IsAvailable = true;
                await _productItemRepository.UpdateAsync(productItem);
                _itemsMadeAvailable++;
            }

            _logger.LogInformation("Items made available by service: " + _itemsMadeAvailable);
        }
    }
}
